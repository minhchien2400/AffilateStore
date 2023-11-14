using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace AffiliateStoreBE.Common
{
    public class SearchStringFunction : ISearchStringFunction
    {
        public SearchStringFunction()
        { }
        public List<string> SearchString(string stringInput)
        {
            string result = string.Join(" ", stringInput.Split().Where(s => !string.IsNullOrWhiteSpace(s)));
            List<string> resultList = new List<string>();

            string[] words = result.Split(' ');

            for (int i = words.Length; i > 0; i--)
            {
                for (int j = 0; j <= words.Length - i; j++)
                {
                    string substring = string.Join("", words.Skip(j).Take(i)).ToLowerInvariant();
                    substring = RemoveAccents(substring);
                    resultList.Add(substring);
                }
            }
            return resultList;
        }

        // chuyen chu co dau ve khong dau cho 1 string
        static string RemoveAccents(string input)
        {
            string decomposed = input.Normalize(NormalizationForm.FormD);
            return Regex.Replace(decomposed, @"\p{Mn}", string.Empty);
        }

        // loai bo khoang trang va chuyen chu co dau ve khong dau cho 1 list string
        public string RemoveSpaceAndConvert(string listStringInput)
        {
            return RemoveWhitespaceAndDiacritics(listStringInput);

        }

        static string RemoveWhitespaceAndDiacritics(string input)
        {
            // Loại bỏ khoảng trắng
            string withoutWhitespace = new string(input.ToCharArray()
                                                 .Where(c => !Char.IsWhiteSpace(c))
                                                 .ToArray());

            // Chuyển chữ cái tiếng Việt có dấu về chữ không dấu
            string withoutDiacritics = RemoveDiacritics(withoutWhitespace);

            return withoutDiacritics;
        }

        static string RemoveDiacritics(string input)
        {
            string normalized = input.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
