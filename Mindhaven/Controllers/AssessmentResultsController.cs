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

namespace Mindhaven.Controllers
{
    [Authorize]
    public class AssessmentResultsController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: AssessmentResults
        public async Task<ActionResult> Index()
        {
            ViewBag.IsAdmin = (Session["Role"] != null && Session["Role"].ToString() == "Admin");
            var assessmentResults = db.AssessmentResults.Include(a => a.Assessment).Include(a => a.User);
            return View(await assessmentResults.ToListAsync());
        }

        // GET: AssessmentResults/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssessmentResult assessmentResult = await db.AssessmentResults.FindAsync(id);
            if (assessmentResult == null)
            {
                return HttpNotFound();
            }
            return View(assessmentResult);
        }

        // GET: AssessmentResults/Create
        public ActionResult Create()
        {
            ViewBag.AssessmentID = new SelectList(db.Assessments, "AssessmentID", "Title");
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName");
            return View();
        }

        // POST: AssessmentResults/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ResultID,UserID,AssessmentID,Score,TakenAt")] AssessmentResult assessmentResult)
        {
            if (ModelState.IsValid)
            {
                db.AssessmentResults.Add(assessmentResult);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AssessmentID = new SelectList(db.Assessments, "AssessmentID", "Title", assessmentResult.AssessmentID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", assessmentResult.UserID);
            return View(assessmentResult);
        }

        // GET: AssessmentResults/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssessmentResult assessmentResult = await db.AssessmentResults.FindAsync(id);
            if (assessmentResult == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssessmentID = new SelectList(db.Assessments, "AssessmentID", "Title", assessmentResult.AssessmentID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", assessmentResult.UserID);
            return View(assessmentResult);
        }

        // POST: AssessmentResults/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ResultID,UserID,AssessmentID,Score,TakenAt")] AssessmentResult assessmentResult)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assessmentResult).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AssessmentID = new SelectList(db.Assessments, "AssessmentID", "Title", assessmentResult.AssessmentID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", assessmentResult.UserID);
            return View(assessmentResult);
        }

        // GET: AssessmentResults/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssessmentResult assessmentResult = await db.AssessmentResults.FindAsync(id);
            if (assessmentResult == null)
            {
                return HttpNotFound();
            }
            return View(assessmentResult);
        }

        // POST: AssessmentResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AssessmentResult assessmentResult = await db.AssessmentResults.FindAsync(id);
            db.AssessmentResults.Remove(assessmentResult);
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
