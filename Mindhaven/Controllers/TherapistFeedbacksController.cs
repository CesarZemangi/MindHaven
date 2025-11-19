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
    public class TherapistFeedbacksController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: TherapistFeedbacks
        public async Task<ActionResult> Index()
        {
            

            var therapistFeedbacks = db.TherapistFeedbacks.Include(t => t.HomeworkSubmission).Include(t => t.User);
            return View(await therapistFeedbacks.ToListAsync());
        }

        // GET: TherapistFeedbacks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
           

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TherapistFeedback therapistFeedback = await db.TherapistFeedbacks.FindAsync(id);
            if (therapistFeedback == null)
            {
                return HttpNotFound();
            }
            return View(therapistFeedback);
        }

        // GET: TherapistFeedbacks/Create
        public ActionResult Create()
        {

            ViewBag.SubmissionId = new SelectList(db.HomeworkSubmissions, "SubmissionId", "TextResponse");
            ViewBag.TherapistId = new SelectList(db.Users, "UserID", "FullName");
            return View();
        }

        // POST: TherapistFeedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FeedbackId,SubmissionId,TherapistId,Notes,FeedbackDate")] TherapistFeedback therapistFeedback)
        {

            if (ModelState.IsValid)
            {
                db.TherapistFeedbacks.Add(therapistFeedback);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.SubmissionId = new SelectList(db.HomeworkSubmissions, "SubmissionId", "TextResponse", therapistFeedback.SubmissionId);
            ViewBag.TherapistId = new SelectList(db.Users, "UserID", "FullName", therapistFeedback.TherapistId);
            return View(therapistFeedback);
        }

        // GET: TherapistFeedbacks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TherapistFeedback therapistFeedback = await db.TherapistFeedbacks.FindAsync(id);
            if (therapistFeedback == null)
            {
                return HttpNotFound();
            }
            ViewBag.SubmissionId = new SelectList(db.HomeworkSubmissions, "SubmissionId", "TextResponse", therapistFeedback.SubmissionId);
            ViewBag.TherapistId = new SelectList(db.Users, "UserID", "FullName", therapistFeedback.TherapistId);
            return View(therapistFeedback);
        }

        // POST: TherapistFeedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "FeedbackId,SubmissionId,TherapistId,Notes,FeedbackDate")] TherapistFeedback therapistFeedback)
        {
            if (ModelState.IsValid)
            {
                db.Entry(therapistFeedback).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.SubmissionId = new SelectList(db.HomeworkSubmissions, "SubmissionId", "TextResponse", therapistFeedback.SubmissionId);
            ViewBag.TherapistId = new SelectList(db.Users, "UserID", "FullName", therapistFeedback.TherapistId);
            return View(therapistFeedback);
        }

        // GET: TherapistFeedbacks/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TherapistFeedback therapistFeedback = await db.TherapistFeedbacks.FindAsync(id);
            if (therapistFeedback == null)
            {
                return HttpNotFound();
            }
            return View(therapistFeedback);
        }

        // POST: TherapistFeedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TherapistFeedback therapistFeedback = await db.TherapistFeedbacks.FindAsync(id);
            db.TherapistFeedbacks.Remove(therapistFeedback);
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
