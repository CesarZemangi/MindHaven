using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Mindhaven.Models;

namespace Mindhaven.Views
{
    public class UserActivityLogsController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: UserActivityLogs
        public async Task<ActionResult> Index()
        {
            var userActivityLogs = db.UserActivityLogs.Include(u => u.SelfCareActivity).Include(u => u.User);
            return View(await userActivityLogs.ToListAsync());
        }

        // GET: UserActivityLogs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserActivityLog userActivityLog = await db.UserActivityLogs.FindAsync(id);
            if (userActivityLog == null)
            {
                return HttpNotFound();
            }
            return View(userActivityLog);
        }

        // GET: UserActivityLogs/Create
        public ActionResult Create()
        {
            ViewBag.ActivityId = new SelectList(db.SelfCareActivities, "ActivityId", "Title");
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FullName");
            return View();
        }

        // POST: UserActivityLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LogId,UserId,ActivityId,CompletedAt,Notes")] UserActivityLog userActivityLog)
        {
            if (ModelState.IsValid)
            {
                db.UserActivityLogs.Add(userActivityLog);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ActivityId = new SelectList(db.SelfCareActivities, "ActivityId", "Title", userActivityLog.ActivityId);
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FullName", userActivityLog.UserId);
            return View(userActivityLog);
        }

        // GET: UserActivityLogs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserActivityLog userActivityLog = await db.UserActivityLogs.FindAsync(id);
            if (userActivityLog == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivityId = new SelectList(db.SelfCareActivities, "ActivityId", "Title", userActivityLog.ActivityId);
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FullName", userActivityLog.UserId);
            return View(userActivityLog);
        }

        // POST: UserActivityLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "LogId,UserId,ActivityId,CompletedAt,Notes")] UserActivityLog userActivityLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userActivityLog).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ActivityId = new SelectList(db.SelfCareActivities, "ActivityId", "Title", userActivityLog.ActivityId);
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FullName", userActivityLog.UserId);
            return View(userActivityLog);
        }

        // GET: UserActivityLogs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserActivityLog userActivityLog = await db.UserActivityLogs.FindAsync(id);
            if (userActivityLog == null)
            {
                return HttpNotFound();
            }
            return View(userActivityLog);
        }

        // POST: UserActivityLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UserActivityLog userActivityLog = await db.UserActivityLogs.FindAsync(id);
            db.UserActivityLogs.Remove(userActivityLog);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
