using System;

namespace LinkManagerApp.Models
{
    public class UserLink
    {
        public string UserName { get; set; } = "";
        public string Url { get; set; } = "";
        public string Title { get; set; } = "";
        public DateTime AddedDate { get; set; }
        public string Category { get; set; } = "Общее";
    }
}