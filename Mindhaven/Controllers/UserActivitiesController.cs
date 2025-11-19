using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mindhaven.Models;

namespace Mindhaven.Controllers
{
    public class UserActivitiesController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: UserActivities
        public async Task<ActionResult> Index()
        {
            ViewBag.IsAdmin = (Session["Role"] != null && Session["Role"].ToString() == "Admin");

            // Make this query truly async
            var activities = await db.UserActivities
                                     .Include(a => a.User)
                                     .OrderByDescending(a => a.ActivityDate)
                                     .ToListAsync();

            return View(activities);
        }

        // GET: UserActivities/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userActivity = await db.UserActivities.FindAsync(id);
            if (userActivity == null)
                return HttpNotFound();

            return View(userActivity);
        }

        // GET: UserActivities/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName");
            return View();
        }

        // POST: UserActivities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ActivityID,UserID,ActivityDescription,ActivityDate")] UserActivity userActivity)
        {
            if (ModelState.IsValid)
            {
                db.UserActivities.Add(userActivity);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", userActivity.UserID);
            return View(userActivity);
        }

        // GET: UserActivities/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userActivity = await db.UserActivities.FindAsync(id);
            if (userActivity == null)
                return HttpNotFound();

            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", userActivity.UserID);
            return View(userActivity);
        }

        // POST: UserActivities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ActivityID,UserID,ActivityDescription,ActivityDate")] UserActivity userActivity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userActivity).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", userActivity.UserID);
            return View(userActivity);
        }

        // GET: UserActivities/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userActivity = await db.UserActivities.FindAsync(id);
            if (userActivity == null)
                return HttpNotFound();

            return View(userActivity);
        }

        // POST: UserActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var userActivity = await db.UserActivities.FindAsync(id);
            db.UserActivities.Remove(userActivity);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
