namespace LinkManagerApp.Models
{
    public class AppSettings
    {
        public string AdminPassword { get; set; } = "admin123";
        public string CredentialsFilePath { get; set; } = "";
        public string LinksDatabasePath { get; set; } = "";
        public string CurrentUser { get; set; } = "";
    }
}