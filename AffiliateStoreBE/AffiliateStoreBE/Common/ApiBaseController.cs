using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using AffiliateStoreBE.Common.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace AffiliateStoreBE.Common
{
    public class ApiBaseController : Controller
    {
        public readonly IHttpContextAccessor _httpContextAccessor = null;

        public ApiBaseController()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }


        protected IQueryable<T> DoTake<T>(IQueryable<T> query, FilterModel filter)
        {
            var offsetFinal = filter.Offset <= 0 ? 1 : filter.Offset;
            var limitFinal = filter.Limit <= 0 ? 10 : filter.Limit;
            return query.Skip((offsetFinal - 1) * limitFinal)
                        .Take(limitFinal);
        }

        protected IEnumerable<T> DoTake<T>(IEnumerable<T> query, FilterModel filter)
        {
            var offsetFinal = filter.Offset <= 0 ? 1 : filter.Offset;
            var limitFinal = filter.Limit <= 0 ? 10 : filter.Limit;
            return query.Skip((offsetFinal - 1) * limitFinal)
                        .Take(limitFinal);
        }

        public List<string> SearchString(string stringInput, List<string> listNamesConvert)
        {
            string result = string.Join(" ", stringInput.Split().Where(s => !string.IsNullOrWhiteSpace(s)));
            List<string> resultList = new List<string>();
            List<string> listNamesReturn = new List<string>();

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

            foreach(var str in resultList)
            {
                foreach(var name in listNamesConvert)
                {
                    if(RemoveSpaceAndConvert(name).Contains(str.ToLower()))
                    {
                        listNamesReturn.Add(name);
                    }
                }
            }
            return listNamesReturn;
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
            return RemoveWhitespaceAndDiacritics(listStringInput).ToLower();

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
