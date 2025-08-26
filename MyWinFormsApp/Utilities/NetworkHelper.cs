using System.Net.Http;

namespace LinkManagerApp.Utilities
{
    public static class NetworkHelper
    {
        public static string GetRandomUserAgent()
        {
            var userAgents = new List<string>
            {
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/121.0"
            };

            var random = new Random();
            return userAgents[random.Next(userAgents.Count)];
        }
    }
}