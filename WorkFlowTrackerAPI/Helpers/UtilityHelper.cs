namespace DotnetAPI.Helpers
{
    public class UtilityHelper
    {
        public static string EscapeSingleQuotes(string input)
        {
            if (input == null) return string.Empty;
            return input.Replace("'", "''");
        }
    }
}
