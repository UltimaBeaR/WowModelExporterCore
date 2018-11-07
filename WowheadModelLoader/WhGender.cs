using System.Collections.Generic;

namespace WowheadModelLoader
{
    /// <summary>
    /// ZamModelViewer.Wow.Genders
    /// </summary>
    public enum WhGender
    {
        Undefined = -1,
        MALE = 0,
        FEMALE = 1,
        // Есть проверка на это, хз что это
        Undefined2 = 2,
        // Есть проверка на это, хз что это
        Undefined3 = 3
    }

    public static class WhGenderExtensions
    {
        /// <summary>
        /// Продолжение ZamModelViewer.Wow.Genders но для ключей = самим значениям энама
        /// </summary>
        public static string GetStringIdentifier(this WhGender gender)
        {
            return _stringIdentifiers[gender];
        }

        private static readonly Dictionary<WhGender, string> _stringIdentifiers = new Dictionary<WhGender, string>()
        {
            { WhGender.MALE, "male" },
            { WhGender.FEMALE, "female" }
        };
    }
}
