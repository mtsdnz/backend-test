using System.Globalization;
using System.Text;

namespace Core.Extensions
{
    public static class StringExtensions
    {
        public static string ClearSpaces(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? "" : str.Replace(" ", string.Empty);
        }

        public static string RemoveDiacritics(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "";

            var normalizedString = str.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}