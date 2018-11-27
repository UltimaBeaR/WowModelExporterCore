using System;

namespace WebViewJsModifier
{
    public abstract class JsModifyAction
    {
        public JsModifyAction(string urlMatchPattern, string[] searchStrings)
        {
            UrlMatchPattern = urlMatchPattern;
            SearchStrings = searchStrings;
        }

        /// <summary>
        /// Паттерн совпадения файла по url (regex). Инжектор вызывается только для файлов, по url которых было совпадение с этим паттерном
        /// Содержимое Url по этому паттерну должно возвращать javascript код
        /// </summary>
        public string UrlMatchPattern { get; private set; }

        /// <summary>
        /// Строка, в исходном файле по которой идет поиск
        /// </summary>
        public string[] SearchStrings { get; private set; }
    }

    /// <summary>
    /// Позволяет заменять участок js кода на кастомный
    /// </summary>
    public class TextReplaceJsModifyAction : JsModifyAction
    {
        public TextReplaceJsModifyAction(string urlMatchPattern, string[] searchStrings, string searchStringToReplace, string replacementString)
            : base(urlMatchPattern, searchStrings)
        {
            SearchStringToReplace = searchStringToReplace;
            ReplacementString = replacementString;
        }

        public string SearchStringToReplace { get; private set; }
        public string ReplacementString { get; private set; }
    }

    /// <summary>
    /// Позволяет перехватывать данные в js коде. Перехваченные данные можно потом получить по событию Intercepted
    /// </summary>
    public class InterceptDataJsModifyAction : JsModifyAction
    {
        public InterceptDataJsModifyAction(string urlMatchPattern, string[] searchStrings, string dataToIntercept, bool isCircularData = false)
            : base(urlMatchPattern, searchStrings)
        {
            DataToIntercept = dataToIntercept;
            IsCircularData = isCircularData;
        }

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

        public delegate void InterceptedEventHandler(InterceptDataJsModifyAction sender, Func<string> getInterceptedDataJson);
    }
}
