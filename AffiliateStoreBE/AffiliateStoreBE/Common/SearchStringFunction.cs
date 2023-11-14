using System.Text.RegularExpressions;
using System.Text;

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
        static string RemoveAccents(string input)
        {
            string decomposed = input.Normalize(NormalizationForm.FormD);
            return Regex.Replace(decomposed, @"\p{Mn}", string.Empty);
        }
    }
}
