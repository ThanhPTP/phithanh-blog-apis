using Diacritics.Extensions;
using Newtonsoft.Json;

namespace PhiThanh.Core.Extensions
{
    public static class StringExtensions
    {
        public static string CapitalizeFirstLetter(this string text)
        {
            return char.ToUpper(text[0]) + text[1..];
        }

        public static string Standardized(this string text)
        {
            var result = text.RemoveDiacritics().ToLower();
            return string.Join('/', result).Replace(' ', '-');
        }

        public static bool TryParseJson<T>(this string @this, out T? result)
        {
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(@this, settings);
            return success;
        }
    }
}
