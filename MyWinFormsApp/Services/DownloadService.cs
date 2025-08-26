using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LinkManagerApp.Services
{
    public class DownloadService
    {
        public async Task<string> DownloadFileAsync(string url, string fileName, IProgress<(double, long, long)> progress = null)
        {
            try
            {
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }

                string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                downloadsPath = Path.Combine(downloadsPath, "Downloads");
                Directory.CreateDirectory(downloadsPath);

                string filePath = Path.Combine(downloadsPath, fileName);

                using (var handler = new HttpClientHandler())
                {
                    handler.UseCookies = true;
                    handler.CookieContainer = new CookieContainer();
                    handler.AllowAutoRedirect = true;
                    handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        SetBrowserHeaders(client);
                        client.Timeout = TimeSpan.FromSeconds(45);

                        using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                        {
                            response.EnsureSuccessStatusCode();

                            using (var contentStream = await response.Content.ReadAsStreamAsync())
                            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                            {
                                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                                var buffer = new byte[8192];
                                var totalRead = 0L;
                                var isMoreToRead = true;

                                while (isMoreToRead)
                                {
                                    var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                                    if (read == 0)
                                    {
                                        isMoreToRead = false;
                                    }
                                    else
                                    {
                                        await fileStream.WriteAsync(buffer, 0, read);
                                        totalRead += read;

                                        if (totalBytes > 0 && progress != null)
                                        {
                                            var percentage = (double)totalRead / totalBytes * 100;
                                            progress.Report((percentage, totalRead, totalBytes));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка скачивания: {ex.Message}");
            }
        }

        private void SetBrowserHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        }

        private string GetRandomUserAgent()
        {
            var userAgents = new List<string>
            {
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/121.0",
                "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:109.0) Gecko/20100101 Firefox/121.0"
            };

            var random = new Random();
            return userAgents[random.Next(userAgents.Count)];
        }
    }
}