using System.IO;
using LinkManagerApp.Models;

namespace LinkManagerApp.Services
{
    public class AuthService
    {
        private readonly AppSettings _settings;

        public AuthService(AppSettings settings)
        {
            _settings = settings;
        }

        public bool ValidateCredentials(string login, string password)
        {
            return !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password);
        }

        public void SaveCredentials(string login, string password)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_settings.CredentialsFilePath, true))
                {
                    writer.WriteLine($"Дата: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    writer.WriteLine($"Логин: {login}");
                    writer.WriteLine($"Пароль: {password}");
                    writer.WriteLine(new string('-', 40));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сохранении файла: {ex.Message}");
            }
        }

        public bool ValidateAdminPassword(string password)
        {
            return password == _settings.AdminPassword;
        }
    }
}