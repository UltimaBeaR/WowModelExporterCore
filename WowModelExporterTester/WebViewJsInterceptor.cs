using EO.WebBrowser;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Linq;
using WowModelExporterTester.Properties;

namespace WowModelExporterTester
{
    public class WebViewJsInjection
    {
        public WebViewJsInjection(string urlMatchPattern, string searchString, string dataToIntercept, bool isCircularData = false)
        {
            UrlMatchPattern = urlMatchPattern;
            SearchString = searchString;
            DataToIntercept = dataToIntercept;
            IsCircularData = isCircularData;
        }

        /// <summary>
        /// Паттерн совпадения файла по url (regex). Инжектор вызывается только для файлов, по url которых было совпадение с этим паттерном
        /// Содержимое Url по этому паттерну должно возвращать javascript код
        /// </summary>
        public string UrlMatchPattern { get; private set; }

        /// <summary>
        /// Строка, в исходном файле, сразу после которой будет блок инжектора, позволяющий вызвать событие Intercepted.
        /// Блок инжектора вставится только после первого вхождения этой строки, то есть стркоа должна уникально идентифицировать место в оригинальном коде.
        /// ToDo: можно сделать разные опции типа вставить до этой строки, после нее вместо нее, сделать regex вместо строки и тд. Можно сделать еще чтоб задавать не строкой а индексом символа в коде
        /// </summary>
        public string SearchString { get; private set; }

        /// <summary>
        /// javascript выражение возвращающее значение одной переменной (объект, строка, число и т.д.).
        /// Это значение будет преобразовано в json и передано как параметр в Intercepted событии
        /// </summary>
        public string DataToIntercept { get; private set; }

        /// <summary>
        /// true, если DataToIntercept содержит circular references
        /// </summary>
        public bool IsCircularData { get; private set; }

        /// <summary>
        /// Происходит при вызове заинжектеного кода
        /// </summary>
        public event InterceptedEventHandler Intercepted;

        /// <summary>
        /// Используется внутренне в WebViewJsInterceptor. Вызывать это не нужно.
        /// </summary>
        /// <param name="interceptedDataJson"></param>
        public void InvokeEvent(Func<string> getInterceptedDataJson)
        {
            Intercepted?.Invoke(this, getInterceptedDataJson);
        }

        public delegate void InterceptedEventHandler(WebViewJsInjection sender, Func<string> getInterceptedDataJson);
    }

    public class WebViewJsInterceptor
    {
        public WebViewJsInterceptor(WebView webView, IEnumerable<WebViewJsInjection> injections)
        {
            _injectionsPerUrlMatchPattern = new Dictionary<string, List<WebViewJsInjection>>();
            foreach (var injection in injections)
            {
                if (injection == null || (injection.UrlMatchPattern == null || injection.SearchString == null))
                    continue;

                List<WebViewJsInjection> injectionsForCurrentUrlMatchPattern;
                if (!_injectionsPerUrlMatchPattern.TryGetValue(injection.UrlMatchPattern, out injectionsForCurrentUrlMatchPattern))
                {
                    injectionsForCurrentUrlMatchPattern = new List<WebViewJsInjection>();
                    _injectionsPerUrlMatchPattern.Add(injection.UrlMatchPattern, injectionsForCurrentUrlMatchPattern);
                }

                injectionsForCurrentUrlMatchPattern.Add(injection);
            }

            _injectedList = new List<WebViewJsInjection>();

            webView.BeforeNavigate += (o, e) =>
            {
                if (e.IsMainFrame)
                    _injectedList.Clear();
            };

            webView.RegisterResourceHandler(new InjectionResourceHandler(this));

            webView.JSExtInvoke += (o, e) =>
            {
                if (e.FunctionName != null && e.FunctionName.StartsWith(extInvokeFunctionNamePrefix))
                {
                    var injectedListIdx = Convert.ToInt32(e.FunctionName.Remove(0, extInvokeFunctionNamePrefix.Length));

                    if (injectedListIdx < _injectedList.Count)
                    {
                        var getDataFunction = (JSFunction)e.Arguments[0];
                        var thisObj = (JSObject)e.Arguments[1];

                        _injectedList[injectedListIdx].InvokeEvent(() => {
                            var json = getDataFunction.Invoke(thisObj, new object[0]);
                            return json.ToString();
                        });
                    }
                }
            };
        }

        private Dictionary<string, List<WebViewJsInjection>> _injectionsPerUrlMatchPattern;
        private List<WebViewJsInjection> _injectedList;

        private const string extInvokeFunctionNamePrefix = "__js__interception_";

        private class InjectionResourceHandler : ResourceHandler
        {
            public InjectionResourceHandler(WebViewJsInterceptor interceptor)
            {
                _interceptor = interceptor;
            }

            public override bool Match(Request request)
            {
                return _interceptor._injectionsPerUrlMatchPattern.Keys.Any(x => Regex.IsMatch(request.Url, x));
            }

            public override void ProcessRequest(Request request, Response response)
            {
                var client = new HttpClient();
                var remoteFile = client.GetStringAsync(request.Url).Result;

                // Добавляем кастомные либы в начало файла
                remoteFile = Resources.WebViewJsInterceptorLibs + remoteFile;

                foreach (var injectionsForUrlMatchPattern in _interceptor._injectionsPerUrlMatchPattern)
                {
                    if (Regex.IsMatch(request.Url, injectionsForUrlMatchPattern.Key))
                    {
                        foreach (var injection in injectionsForUrlMatchPattern.Value)
                        {
                            var injectionStartIdxInRemoteFile = remoteFile.IndexOf(injection.SearchString) + injection.SearchString.Length;

                            if (injectionStartIdxInRemoteFile != -1)
                            {
                                var injectedListIdx = _interceptor._injectedList.Count;
                                _interceptor._injectedList.Add(injection);
                                remoteFile = remoteFile.Insert(injectionStartIdxInRemoteFile, GetCodeForInjection(injection, injectedListIdx));
                            }
                        }
                    }
                }

                response.Write(remoteFile);
            }

            private string GetCodeForInjection(WebViewJsInjection injection, int injectedListIdx)
            {
                return @"
;(function(){
    var __webviewinterceptor__getData = function () {
        var __webviewinterceptor__data = null;

        try {
            __webviewinterceptor__data = " + (injection.DataToIntercept ?? @"null") + @";
        }
        catch (error)
        {
            __webviewinterceptor__data = { name: error.name, message: error.message };
        }

        return __webviewinterceptorlibs__JSONstringify(__webviewinterceptor__data, " + (injection.IsCircularData ? "true" : "false") + @");
    };
    eoapi.extInvoke('" + extInvokeFunctionNamePrefix + injectedListIdx + @"', [ __webviewinterceptor__getData, this ]);
}).call(this);
                ";
            }
            private WebViewJsInterceptor _interceptor;
        }
    }
}
