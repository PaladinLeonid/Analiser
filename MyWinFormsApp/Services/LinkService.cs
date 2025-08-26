using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using LinkManagerApp.Models;

namespace LinkManagerApp.Services
{
    public class LinkService
    {
        private readonly string _databasePath;

        public LinkService(string databasePath)
        {
            _databasePath = databasePath;
        }

        public List<UserLink> LoadUserLinks(string userName)
        {
            try
            {
                if (File.Exists(_databasePath))
                {
                    string json = File.ReadAllText(_databasePath);
                    var allLinks = JsonSerializer.Deserialize<List<UserLink>>(json) ?? new List<UserLink>();
                    return allLinks.Where(link => link.UserName == userName).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки базы данных: {ex.Message}");
            }

            return new List<UserLink>();
        }

        public void SaveUserLink(UserLink newLink)
        {
            try
            {
                var links = LoadAllLinks();
                links.Add(newLink);

                string json = JsonSerializer.Serialize(links, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_databasePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения ссылки: {ex.Message}");
            }
        }

        public void RemoveUserLink(UserLink linkToRemove)
        {
            try
            {
                var links = LoadAllLinks();
                links.RemoveAll(link => link.UserName == linkToRemove.UserName &&
                                      link.Url == linkToRemove.Url &&
                                      link.AddedDate == linkToRemove.AddedDate);

                string json = JsonSerializer.Serialize(links, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_databasePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка удаления ссылки: {ex.Message}");
            }
        }

        public List<UserLink> SearchLinks(string userName, string searchText)
        {
            var userLinks = LoadAllLinks()
                .Where(link => link.UserName == userName &&
                      (link.Url.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                       link.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                       link.Category.Contains(searchText, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return userLinks;
        }

        private List<UserLink> LoadAllLinks()
        {
            try
            {
                if (File.Exists(_databasePath))
                {
                    string json = File.ReadAllText(_databasePath);
                    return JsonSerializer.Deserialize<List<UserLink>>(json) ?? new List<UserLink>();
                }
            }
            catch
            {
                // Ignore errors and return empty list
            }

            return new List<UserLink>();
        }
    }
}