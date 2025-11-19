using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mindhaven.Models
{
    public class ActivityLogger
    {
        private readonly mindhavenDBEntities1 db ;

        public ActivityLogger()
        {
            db = new  mindhavenDBEntities1();
        }

        public void LogActivity(int userId, string activityDescription)
        {
            var activity = new UserActivity
            {
                UserID = userId,
                ActivityDescription = activityDescription,
                ActivityDate = DateTime.Now
            };

            db.UserActivities.Add(activity);
            db.SaveChanges();
        }
    }
}