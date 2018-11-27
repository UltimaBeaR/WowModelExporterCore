using EO.WebBrowser;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Linq;
using WowModelExporterTester.Properties;

namespace WebViewJsModifier
{
    public class JsModifier
    {
        public JsModifier(WebView webView, IEnumerable<JsModifyAction> jsModifyActions)
        {
            _jsModifyActionsPerUrlMatchPattern = new Dictionary<string, List<JsModifyAction>>();
            foreach (var jsModifyAction in jsModifyActions)
            {
                if (jsModifyAction == null || jsModifyAction.UrlMatchPattern == null)
                    continue;

                List<JsModifyAction> jsModifyActionsForCurrentUrlMatchPattern;
                if (!_jsModifyActionsPerUrlMatchPattern.TryGetValue(jsModifyAction.UrlMatchPattern, out jsModifyActionsForCurrentUrlMatchPattern))
                {
                    jsModifyActionsForCurrentUrlMatchPattern = new List<JsModifyAction>();
                    _jsModifyActionsPerUrlMatchPattern.Add(jsModifyAction.UrlMatchPattern, jsModifyActionsForCurrentUrlMatchPattern);
                }

                jsModifyActionsForCurrentUrlMatchPattern.Add(jsModifyAction);
            }

            _interceptDataList = new List<InterceptDataJsModifyAction>();

            webView.BeforeNavigate += (o, e) =>
            {
                if (e.IsMainFrame)
                    _interceptDataList.Clear();
            };

            webView.RegisterResourceHandler(new JsModifierResourceHandler(this));

            webView.JSExtInvoke += (o, e) =>
            {
                if (e.FunctionName != null && e.FunctionName.StartsWith(extInvokeFunctionNamePrefix))
                {
                    var interceptDataListIdx = Convert.ToInt32(e.FunctionName.Remove(0, extInvokeFunctionNamePrefix.Length));

                    if (interceptDataListIdx < _interceptDataList.Count)
                    {
                        var getDataFunction = (JSFunction)e.Arguments[0];
                        var thisObj = (JSObject)e.Arguments[1];

                        _interceptDataList[interceptDataListIdx].InvokeEvent(() => {
                            var json = getDataFunction.Invoke(thisObj, new object[0]);
                            return json.ToString();
                        });
                    }
                }
            };
        }

        private Dictionary<string, List<JsModifyAction>> _jsModifyActionsPerUrlMatchPattern;
        private List<InterceptDataJsModifyAction> _interceptDataList;

        private const string extInvokeFunctionNamePrefix = "__js__interception_";

        private class JsModifierResourceHandler : ResourceHandler
        {
            public JsModifierResourceHandler(JsModifier modifier)
            {
                _modifier = modifier;
            }

            public override bool Match(Request request)
            {
                return _modifier._jsModifyActionsPerUrlMatchPattern.Keys.Any(x => Regex.IsMatch(request.Url, x));
            }

            public override void ProcessRequest(Request request, Response response)
            {
                var client = new HttpClient();
                var remoteFile = client.GetStringAsync(request.Url).Result;

                // Добавляем кастомные либы в начало файла
                remoteFile = Resources.WebViewJsModifierLibs + remoteFile;

                foreach (var jsModifyActionsForUrlMatchPattern in _modifier._jsModifyActionsPerUrlMatchPattern)
                {
                    if (Regex.IsMatch(request.Url, jsModifyActionsForUrlMatchPattern.Key))
                    {
                        foreach (var jsModifyAction in jsModifyActionsForUrlMatchPattern.Value)
                        {
                            int foundIdx = 0;
                            if (jsModifyAction.SearchStrings != null && jsModifyAction.SearchStrings.Length > 0)
                            {
                                foreach (var searchString in jsModifyAction.SearchStrings)
                                {
                                    if (searchString == null)
                                        continue;

                                    foundIdx = remoteFile.IndexOf(searchString, foundIdx);
                                    if (foundIdx < 0)
                                        break;

                                    foundIdx += searchString.Length;
                                }
                            }

                            if (foundIdx < 0)
                                break;

                            if (jsModifyAction is InterceptDataJsModifyAction)
                            {
                                var interceptDataAction = jsModifyAction as InterceptDataJsModifyAction;

                                var interceptDataListIdx = _modifier._interceptDataList.Count;
                                _modifier._interceptDataList.Add(interceptDataAction);
                                remoteFile = remoteFile.Insert(foundIdx, GetCodeForInterceptDataAction(interceptDataAction, interceptDataListIdx));
                            }
                            else if (jsModifyAction is TextReplaceJsModifyAction)
                            {
                                var textReplaceAction = jsModifyAction as TextReplaceJsModifyAction;

                                var before = remoteFile.Substring(0, foundIdx);
                                var after = remoteFile.Substring(foundIdx);

                                remoteFile = before + ReplaceFirst(after, textReplaceAction.SearchStringToReplace, textReplaceAction.ReplacementString);
                            }
                        }
                    }
                }

                response.Write(remoteFile);
            }

            private static string ReplaceFirst(string text, string search, string replace)
            {
                int pos = text.IndexOf(search);
                if (pos < 0)
                {
                    return text;
                }
                return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
            }

            private string GetCodeForInterceptDataAction(InterceptDataJsModifyAction interceptDataAction, int interceptDataListIdx)
            {
                return @"
;(function(){
    var __jsModifier__getData = function () {
        var __jsModifier__data = null;

        try {
            __jsModifier__data = " + (interceptDataAction.DataToIntercept ?? @"null") + @";
        }
        catch (error)
        {
            __jsModifier__data = { name: error.name, message: error.message };
        }

        return __jsModifierLibs__JSONstringify(__jsModifier__data, " + (interceptDataAction.IsCircularData ? "true" : "false") + @");
    };
    eoapi.extInvoke('" + extInvokeFunctionNamePrefix + interceptDataListIdx + @"', [ __jsModifier__getData, this ]);
}).call(this);
                ";
            }
            private JsModifier _modifier;
        }
    }
}
