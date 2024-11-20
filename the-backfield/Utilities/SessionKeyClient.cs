namespace TheBackfield.Utilities
{
    public class SessionKeyClient
    {
        public static string Generate(int userId)
        {
            string sessionKey = "0";
            string characters = "zAyBxCwDvEuFtGsHrIqJpKoLnMmNlOkPjQiRhSgTfUeVdWcXbYaZ0192837465";

            Random selector = new();
            int keyLength = selector.Next(5, 13);

            for (int i = 1; i <= keyLength; i++)
            {
                sessionKey += characters[selector.Next(characters.Length)];
            }

            return $"{sessionKey}_{userId}";
        }
    }
}
