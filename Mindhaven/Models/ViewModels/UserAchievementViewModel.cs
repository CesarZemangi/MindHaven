using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mindhaven.ViewModels
{
    public class UserAchievementViewModel
    {
        public string AchievementName { get; set; }
        public string Description { get; set; }
        public int? Points { get; set; }
        public DateTime? DateEarned { get; set; }
        public string BadgeType { get; set; }
        public int? Progress { get; set; }
    }

    public class LeaderboardViewModel
    {
        public string UserFullName { get; set; }
        public int TotalPoints { get; set; }
    }
}
