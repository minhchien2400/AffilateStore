namespace AffiliateStoreBE.Common.Service
{
    public class BooleanHelper
    {
        public static List<string> BooleanFalse { get; set; } = new List<string>()
            {
                //"n",
                "no",
                //"0",
                //"false"
            };

        public static List<string> BooleanTrue = new List<string>()
            {
                //"y",
                "yes",
                //"1",
                //"true"
            };

        public static bool IsValidBooleanValue(string value)
        {
            return BooleanTrue.Contains(value.ToLower()) || BooleanFalse.Contains(value.ToLower());
        }

        public static string ConvertToBooleanString(object value)
        {
            if (value == null)
                return string.Empty;
            var str = value.ToString();
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            if (bool.TryParse(str, out var boolValue))
            {
                return boolValue ? "Yes" : "No";
            }
            return string.Empty;
        }

        public static bool? ConvertToBoolean(string input)
        {
            if (BooleanTrue.Contains(input.ToLower()))
                return true;
            if (BooleanFalse.Contains(input.ToLower()))
                return false;
            return null;
        }

        public static bool? ConvertToBooleanWithNA(string input)
        {
            if (BooleanTrue.Contains(input.ToLower()))
                return true;
            if (BooleanFalse.Contains(input.ToLower()))
                return false;
            if ("N/A".EqualsIgnoreCase(input))
                return false;
            return null;
        }

        public static bool IsTrue(string input)
        {
            var boolValue = ConvertToBoolean(input);
            return boolValue.HasValue && boolValue.Value;
        }

        public static bool IsFalse(string input)
        {
            var boolValue = ConvertToBoolean(input);
            return boolValue.HasValue && !boolValue.Value;
        }
    }
}
