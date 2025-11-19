using System;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Mindhaven.Models;

public class MoodReminderJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        using (var db = new mindhavenDBEntities1())
        {
            var users = db.Users.ToList();
            foreach (var user in users)
            {
                bool hasPending = db.MoodReminders
                    .Any(r => r.UserID == user.UserID && !r.IsResponded);

                if (!hasPending)
                {
                    db.MoodReminders.Add(new MoodReminder
                    {
                        UserID = user.UserID,
                        SentAt = DateTime.Now,
                        IsResponded = false
                    });
                }
            }
            await db.SaveChangesAsync();
        }
    }
}
