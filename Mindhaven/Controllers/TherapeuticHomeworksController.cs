using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mindhaven.Helpers;
using Mindhaven.Models;

namespace Mindhaven.Controllers
{
    public class TherapeuticHomeworksController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: TherapeuticHomeworks
        public async Task<ActionResult> Index()
        {
            // Role restriction: only Admin or Therapist can view all homeworks
            

            var therapeuticHomeworks = db.TherapeuticHomeworks.Include(t => t.User).Include(t => t.User1);
            return View(await therapeuticHomeworks.ToListAsync());
        }

        // GET: TherapeuticHomeworks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var therapeuticHomework = await db.TherapeuticHomeworks.FindAsync(id);
            if (therapeuticHomework == null)
                return HttpNotFound();

            return View(therapeuticHomework);
        }

        // GET: TherapeuticHomeworks/Create
        public ActionResult Create()
        {
            

            ViewBag.TherapistId = new SelectList(db.Users, "UserID", "FullName");
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FullName");
            return View();
        }

        // POST: TherapeuticHomeworks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "HomeworkId,Title,Description,AssignedDate,DueDate,TherapistId,UserId,Status")] TherapeuticHomework therapeuticHomework)
        {
            

            if (ModelState.IsValid)
            {
                db.TherapeuticHomeworks.Add(therapeuticHomework);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.TherapistId = new SelectList(db.Users, "UserID", "FullName", therapeuticHomework.TherapistId);
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FullName", therapeuticHomework.UserId);
            return View(therapeuticHomework);
        }

        // GET: TherapeuticHomeworks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var therapeuticHomework = await db.TherapeuticHomeworks.FindAsync(id);
            if (therapeuticHomework == null)
                return HttpNotFound();

            ViewBag.TherapistId = new SelectList(db.Users, "UserID", "FullName", therapeuticHomework.TherapistId);
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FullName", therapeuticHomework.UserId);
            return View(therapeuticHomework);
        }

        // POST: TherapeuticHomeworks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "HomeworkId,Title,Description,AssignedDate,DueDate,TherapistId,UserId,Status")] TherapeuticHomework therapeuticHomework)
        {
            

            if (ModelState.IsValid)
            {
                db.Entry(therapeuticHomework).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.TherapistId = new SelectList(db.Users, "UserID", "FullName", therapeuticHomework.TherapistId);
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FullName", therapeuticHomework.UserId);
            return View(therapeuticHomework);
        }

        // GET: TherapeuticHomeworks/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
           

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var therapeuticHomework = await db.TherapeuticHomeworks.FindAsync(id);
            if (therapeuticHomework == null)
                return HttpNotFound();

            return View(therapeuticHomework);
        }

        // POST: TherapeuticHomeworks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
           

            var therapeuticHomework = await db.TherapeuticHomeworks.FindAsync(id);
            db.TherapeuticHomeworks.Remove(therapeuticHomework);
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
