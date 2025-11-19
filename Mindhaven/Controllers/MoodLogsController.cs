using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mindhaven.Models;

namespace Mindhaven.Controllers
{
    [Authorize]
    public class MoodLogsController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: MoodLogs
        public async Task<ActionResult> Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserId"]);
            string role = Session["Role"]?.ToString();

            IQueryable<MoodLog> moodLogsQuery = db.MoodLogs.Include(m => m.User);

            if (role == "Admin" || role == "Therapist")
            {
                ViewBag.ViewTitle = "All Mood Logs";
            }
            else
            {
                moodLogsQuery = moodLogsQuery.Where(m => m.UserID == userId);
                ViewBag.ViewTitle = "Your Mood Logs";
            }

            var moodLogs = await moodLogsQuery.OrderByDescending(m => m.LoggedAt).ToListAsync();

            var userMoods = moodLogs.Where(m => m.UserID == userId).ToList();
            ViewBag.AvgMood = userMoods.Any() ? userMoods.Average(m => m.MoodLevel) : 0;
            ViewBag.TotalLogs = userMoods.Count;
            ViewBag.LastMood = userMoods.LastOrDefault()?.MoodLevel ?? 0;

            ViewBag.IsAdmin = (Session["Role"] != null && Session["Role"].ToString() == "Admin");
            ViewBag.HasPendingReminder = db.MoodReminders
                .Any(r => r.UserID == userId && r.IsResponded == false);

            return View(moodLogs);
        }

        // GET: MoodLogs/Create
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: MoodLogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MoodLevel,MoodNote,MoodTag")] MoodLog moodLog)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                moodLog.UserID = Convert.ToInt32(Session["UserId"]);
                moodLog.LoggedAt = DateTime.Now;
                moodLog.ShareWithCareTeam = Request["ShareWithCareTeam"] == "true";

                db.MoodLogs.Add(moodLog);
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Mood log recorded successfully.";
                return RedirectToAction("Index");
            }

            return View(moodLog);
        }
        // test build

        // GET: MoodLogs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            MoodLog moodLog = await db.MoodLogs.FindAsync(id);
            if (moodLog == null)
                return HttpNotFound();

            int userId = Convert.ToInt32(Session["UserId"]);
            string role = Session["Role"]?.ToString();

            if (moodLog.UserID != userId && role != "Admin" && role != "Therapist")
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View(moodLog);
        }

        // GET: MoodLogs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            MoodLog moodLog = await db.MoodLogs.FindAsync(id);
            if (moodLog == null)
                return HttpNotFound();

            int userId = Convert.ToInt32(Session["UserId"]);
            string role = Session["Role"]?.ToString();

            if (moodLog.UserID != userId && role != "Admin" && role != "Therapist")
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View(moodLog);
        }

        // POST: MoodLogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MoodID,MoodLevel,MoodNote,MoodTag")] MoodLog moodLog)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                moodLog.UserID = Convert.ToInt32(Session["UserId"]);
                moodLog.LoggedAt = DateTime.Now;
                moodLog.ShareWithCareTeam = Request["ShareWithCareTeam"] == "true";

                db.Entry(moodLog).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(moodLog);
        }

        // GET: MoodLogs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            MoodLog moodLog = await db.MoodLogs.FindAsync(id);
            if (moodLog == null)
                return HttpNotFound();

            int userId = Convert.ToInt32(Session["UserId"]);
            string role = Session["Role"]?.ToString();

            if (moodLog.UserID != userId && role != "Admin" && role != "Therapist")
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View(moodLog);
        }

        // POST: MoodLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MoodLog moodLog = await db.MoodLogs.FindAsync(id);
            db.MoodLogs.Remove(moodLog);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // DASHBOARD
        public async Task<ActionResult> Dashboard()
        {
            ViewBag.IsAdmin = (Session["Role"] != null && Session["Role"].ToString() == "Admin");

            int userId = Convert.ToInt32(Session["UserId"]);
            var userMoods = await db.MoodLogs
                .Where(m => m.UserID == userId)
                .OrderBy(m => m.LoggedAt)
                .ToListAsync();

            if (!userMoods.Any())
            {
                ViewBag.Message = "No mood logs available. Start logging to see your progress.";
                return View(new List<MoodLog>());
            }

            ViewBag.MoodLevels = string.Join(",", userMoods.Select(m => m.MoodLevel));
            ViewBag.Dates = string.Join(",", userMoods
                .Where(m => m.LoggedAt.HasValue)
.Select(m => $"'{m.LoggedAt:MMM dd}'"));

            ViewBag.AvgMood = userMoods.Any() ? userMoods.Average(m => m.MoodLevel) : 0;
            ViewBag.TotalLogs = userMoods.Count;
            ViewBag.LastMood = userMoods.LastOrDefault()?.MoodLevel ?? 0;

            return View(userMoods);
        }

        // CARE TEAM DASHBOARD
        public async Task<ActionResult> CareTeamDashboard()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserId"]);

            var sharedUserIds = db.SharedAccesses
    .Where(s => s.CareTeamMemberId == userId)
    .Select(s => s.UserId)
    .ToList();


            var sharedMoods = await db.MoodLogs
                .Where(m => m.UserID.HasValue && sharedUserIds.Contains(m.UserID.Value) && m.ShareWithCareTeam == true)
                .Include(m => m.User)
                .OrderByDescending(m => m.LoggedAt)
                .ToListAsync();

            return View(sharedMoods);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
