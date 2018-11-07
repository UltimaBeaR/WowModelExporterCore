using System;

namespace WowheadModelLoader
{
    public static class JavaScriptDigger
    {
        public static string GetItemsFromGathererResult(string gathererResultJavascriptCode)
        {
            var defineItems = "window.g_items = {};";

            var defineSpells = "window.g_spells = {};";

            var defineJqueryExtend = @"
                window.$ = {
                    extend: function (deep, target, object1) {
                        for (prop in object1) {
                            target[prop] = object1[prop];
                        }
                    }
                };
                ";

            var code = defineItems + defineSpells + defineJqueryExtend + gathererResultJavascriptCode;

            return GetJsonFromJavascriptCode(code, "window.g_items");
        }

        /// <summary>
        /// Выполняет заданный кусок javascript кода и возвращает в виде JSON заданный объект (задается в виде выражения, возвращающего объект, JSON от которого нужно получить)
        /// </summary>
        private static string GetJsonFromJavascriptCode(string javascriptCode, string expressionReturningObject)
        {
            string result = null;

            var engine = new Jint.Engine();

            engine.SetValue("window", engine.Global);
            engine.SetValue("_____outputResult", new Action<string>(x => result = x));

            var additionScript = $";_____outputResult(JSON.stringify({expressionReturningObject}));";

            engine.Execute(javascriptCode + additionScript);

            return result;
        }
    }
}
