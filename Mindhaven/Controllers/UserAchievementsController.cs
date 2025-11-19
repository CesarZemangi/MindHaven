using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mindhaven.Models;
using Mindhaven.ViewModels;

namespace Mindhaven.Controllers
{
    public class UserAchievementsController : Controller
    {
                private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: UserAchievements
        public async Task<ActionResult> Index()
        {
            var userAchievements = db.UserAchievements.Include(u => u.User);
            return View(await userAchievements.ToListAsync());
        }

        // GET: UserAchievements/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAchievement userAchievement = await db.UserAchievements.FindAsync(id);
            if (userAchievement == null)
            {
                return HttpNotFound();
            }
            return View(userAchievement);
        }

        // GET: UserAchievements/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName");
            return View();
        }

        // POST: UserAchievements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AchievementId,UserID,BadgeType,DateEarned,Progress")] UserAchievement userAchievement)
        {
            if (ModelState.IsValid)
            {
                db.UserAchievements.Add(userAchievement);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", userAchievement.UserID);
            return View(userAchievement);
        }

        // GET: UserAchievements/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAchievement userAchievement = await db.UserAchievements.FindAsync(id);
            if (userAchievement == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", userAchievement.UserID);
            return View(userAchievement);
        }

        // POST: UserAchievements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AchievementId,UserID,BadgeType,DateEarned,Progress")] UserAchievement userAchievement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userAchievement).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", userAchievement.UserID);
            return View(userAchievement);
        }

        // GET: UserAchievements/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAchievement userAchievement = await db.UserAchievements.FindAsync(id);
            if (userAchievement == null)
            {
                return HttpNotFound();
            }
            return View(userAchievement);
        }

        // POST: UserAchievements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UserAchievement userAchievement = await db.UserAchievements.FindAsync(id);
            db.UserAchievements.Remove(userAchievement);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public ActionResult Dashboard()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Login");

            int userId = (int)Session["UserID"];
            var user = db.Users.FirstOrDefault(u => u.UserID == userId);

            // Fetch user achievements with Achievement details
            var userAchievements = db.UserAchievements
    .Where(u => u.UserID == userId)
    .Select(u => new UserAchievementViewModel
    {
        AchievementName = u.Achievement.AchievementName,
        Description = u.Achievement.Description,
        Points = u.Achievement.Points,
        DateEarned = u.DateEarned,
        BadgeType = u.BadgeType,
        Progress = u.Progress
    })
    .ToList();

            ViewBag.Leaderboard = db.UserAchievements
                .GroupBy(a => a.User.FullName)
                .Select(g => new LeaderboardViewModel
                {
                    UserFullName = g.Key,
                    TotalPoints = g.Sum(x => x.Progress ?? 0)
                })
                .OrderByDescending(g => g.TotalPoints)
                .Take(10)
                .ToList();

            return View(userAchievements);
        }

        private string GetUserLevel(int progress)
        {
            if (progress < 25) return "Beginner";
            if (progress < 50) return "Intermediate";
            if (progress < 75) return "Advanced";
            return "Expert";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
