using EO.WebBrowser;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace WowModelExporterTester
{
    /// <summary>
    /// Перехватывает аргумент opts из конструктора ZamModelViewer при загрузке страниц, содержащих его. При получении аргумента генерирует событие
    /// </summary>
    public class WebViewOptsInterceptor
    {
        public WebViewOptsInterceptor(WebView webView)
        {
            webView.RegisterResourceHandler(new ContrustorInjectorResourceHandler());
            webView.JSExtInvoke += (obj, ev) =>
            {
                if (ev.FunctionName == jsExtFunctionName)
                    OptsIntercepted?.Invoke(ev.Arguments[0].ToString());
            };
        }

        public event Action<string> OptsIntercepted;

        private const string jsExtFunctionName = "ZamModelViewer_constructor_injection";
        private const string injectingCode = "eoapi.extInvoke('" + jsExtFunctionName + "', [JSON.stringify(opts)]);";
        private const string viewerContrustorBlockSearchString = "function ZamModelViewer(opts){";
        private static readonly Regex viewerJsFilePathRegex = new Regex("/modelviewer/viewer/viewer.min.js$", RegexOptions.Compiled);

        private class ContrustorInjectorResourceHandler : ResourceHandler
        {
            public override bool Match(Request request)
            {
                if (viewerJsFilePathRegex.IsMatch(request.Url))
                    return true;

                return false;
            }

            public override void ProcessRequest(Request request, Response response)
            {
                var client = new HttpClient();
                var viewerJsFileContents = client.GetStringAsync(request.Url).Result;

                var zamModelViewerConstructorBlockIndex = viewerJsFileContents.IndexOf(viewerContrustorBlockSearchString) + viewerContrustorBlockSearchString.Length;

                if (zamModelViewerConstructorBlockIndex > 0)
                {
                    viewerJsFileContents = viewerJsFileContents.Insert(zamModelViewerConstructorBlockIndex, ";(function(){" + injectingCode + "})();");
                }

                response.Write(viewerJsFileContents);
            }
        }
    }
}
