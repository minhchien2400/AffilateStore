namespace System
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string source, string target)
        {
            if (source == null && target == null)
            {
                return true;
            }
            if (source == null || target == null)
            {
                return false;
            }
            return source.Equals(target, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string source, string target)
        {
            if (source == null && target == null)
            {
                return true;
            }
            if (source == null || target == null)
            {
                return false;
            }
            if (source.Length < target.Length)
            {
                return false;
            }
            if (source.Length == target.Length)
            {
                return source.EqualsIgnoreCase(target);
            }
            return source.IndexOf(target, StringComparison.OrdinalIgnoreCase) != -1;
        }

        public static string ToUrl(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }
            return source.Replace(';', '&');
        }


        public static string DisplayHideMiddleFour(this string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length < 9)
            {
                return input;
            }
            return input.Substring(0, 1) + "****" + input.Substring(5, input.Length - 5);
        }
    }
}