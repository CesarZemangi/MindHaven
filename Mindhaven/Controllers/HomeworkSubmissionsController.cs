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
    public class HomeworkSubmissionsController : Controller
    {
                private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: HomeworkSubmissions
        public async Task<ActionResult> Index()
        {
            

            var homeworkSubmissions = db.HomeworkSubmissions.Include(h => h.TherapeuticHomework).Include(h => h.User);
            return View(await homeworkSubmissions.ToListAsync());
        }

        // GET: HomeworkSubmissions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HomeworkSubmission homeworkSubmission = await db.HomeworkSubmissions.FindAsync(id);
            if (homeworkSubmission == null)
            {
                return HttpNotFound();
            }
            return View(homeworkSubmission);
        }

        // GET: HomeworkSubmissions/Create
        public ActionResult Create()
        {
            ViewBag.HomeworkId = new SelectList(db.TherapeuticHomeworks, "HomeworkId", "Title");
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FullName");
            return View();
        }

        // POST: HomeworkSubmissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SubmissionId,HomeworkId,UserId,TextResponse,AudioPath,ImagePath,SubmittedDate")] HomeworkSubmission homeworkSubmission)
        {
            if (ModelState.IsValid)
            {
                db.HomeworkSubmissions.Add(homeworkSubmission);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.HomeworkId = new SelectList(db.TherapeuticHomeworks, "HomeworkId", "Title", homeworkSubmission.HomeworkId);
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FullName", homeworkSubmission.UserId);
            return View(homeworkSubmission);
        }

        // GET: HomeworkSubmissions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HomeworkSubmission homeworkSubmission = await db.HomeworkSubmissions.FindAsync(id);
            if (homeworkSubmission == null)
            {
                return HttpNotFound();
            }
            ViewBag.HomeworkId = new SelectList(db.TherapeuticHomeworks, "HomeworkId", "Title", homeworkSubmission.HomeworkId);
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FullName", homeworkSubmission.UserId);
            return View(homeworkSubmission);
        }

        // POST: HomeworkSubmissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SubmissionId,HomeworkId,UserId,TextResponse,AudioPath,ImagePath,SubmittedDate")] HomeworkSubmission homeworkSubmission)
        {
            if (ModelState.IsValid)
            {
                db.Entry(homeworkSubmission).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.HomeworkId = new SelectList(db.TherapeuticHomeworks, "HomeworkId", "Title", homeworkSubmission.HomeworkId);
            ViewBag.UserId = new SelectList(db.Users, "UserID", "FullName", homeworkSubmission.UserId);
            return View(homeworkSubmission);
        }

        // GET: HomeworkSubmissions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HomeworkSubmission homeworkSubmission = await db.HomeworkSubmissions.FindAsync(id);
            if (homeworkSubmission == null)
            {
                return HttpNotFound();
            }
            return View(homeworkSubmission);
        }

        // POST: HomeworkSubmissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            HomeworkSubmission homeworkSubmission = await db.HomeworkSubmissions.FindAsync(id);
            db.HomeworkSubmissions.Remove(homeworkSubmission);
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
