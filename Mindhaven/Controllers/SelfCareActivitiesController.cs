using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mindhaven.Helpers;
using Mindhaven.Models;

namespace Mindhaven.Controllers
{
    public class SelfCareActivitiesController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: SelfCareActivities
        public async Task<ActionResult> Index()
        {
            var authResult = RoleAuthorizationHelper.AuthorizeUserRole(this, "Therapist, Admin");
            if (authResult != null) return authResult;

            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");
            int userId = Convert.ToInt32(Session["UserId"]);

            // Fetch predefined activities
            var predefined = await db.SelfCareActivities
                .Where(a => a.IsActive)
                .Select(a => new
                {
                    Id = a.ActivityId,
                    a.Title,
                    a.Description,
                    a.ActivityType,
                    Completed = db.UserActivityLogs.Any(u => u.UserId == userId && u.ActivityId == a.ActivityId),
                    IsCustom = false
                }).ToListAsync();

            // Fetch user custom activities
            var custom = await db.CustomActivities
                .Where(c => c.UserId == userId)
                .Select(c => new
                {
                    Id = c.CustomId,
                    c.Title,
                    c.Description,
                    ActivityType = "Custom",
                    Completed = db.UserActivityLogs.Any(u => u.UserId == userId && u.ActivityId == c.CustomId),
                    IsCustom = true
                }).ToListAsync();

            // Combine both lists
            var activities = predefined.Concat(custom).ToList();

            // Progress calculation
            int total = activities.Count;
            int completedCount = activities.Count(a => a.Completed);
            ViewBag.Progress = total > 0 ? (completedCount * 100) / total : 0;

            return View(activities);
        }

        // POST: Mark activity as completed
        [HttpPost]
        public async Task<ActionResult> CompleteActivity(int id, bool isCustom, string notes = "")
        {
            if (Session["UserId"] == null) return Json(new { success = false, message = "Not logged in" });

            int userId = Convert.ToInt32(Session["UserId"]);

            bool alreadyCompleted = isCustom
                ? db.UserActivityLogs.Any(u => u.UserId == userId && u.ActivityId == id)
                : db.UserActivityLogs.Any(u => u.UserId == userId && u.ActivityId == id);

            if (!alreadyCompleted)
            {
                var log = new UserActivityLog
                {
                    UserId = userId,
                    CompletedAt = DateTime.Now,
                    Notes = notes
                };

                if (isCustom) log.ActivityId = id;
                else log.ActivityId = id;

                db.UserActivityLogs.Add(log);
                await db.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        // POST: Add custom activity
        [HttpPost]
        public async Task<ActionResult> AddCustomActivity(string title, string description)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserId"]);

            var custom = new CustomActivity
            {
                UserId = userId,
                Title = title,
                Description = description,
                CreatedAt = DateTime.Now
            };

            db.CustomActivities.Add(custom);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
