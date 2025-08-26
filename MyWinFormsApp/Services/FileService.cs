using System.IO;
using System.Linq;

namespace LinkManagerApp.Services
{
    public class FileService
    {
        public void CreateTextFile(string fileName, string content)
        {
            try
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string filePath = Path.Combine(documentsPath, fileName);

                File.WriteAllText(filePath, content);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при создании файла: {ex.Message}");
            }
        }

        public (int, int) SearchFiles(string extension)
        {
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string[] desktopFiles = Directory.GetFiles(desktopPath, $"*{extension}", SearchOption.TopDirectoryOnly);
            string[] documentsFiles = Directory.GetFiles(documentsPath, $"*{extension}", SearchOption.TopDirectoryOnly);

            return (desktopFiles.Length, documentsFiles.Length);
        }

        public string[] GetDesktopFiles(string extension)
        {
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            return Directory.GetFiles(desktopPath, $"*{extension}", SearchOption.TopDirectoryOnly);
        }

        public string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double len = bytes;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
    }
}