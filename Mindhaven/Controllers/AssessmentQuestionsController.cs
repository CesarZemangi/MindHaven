using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mindhaven.Models;
using Newtonsoft.Json;
using Mindhaven.ViewModels;
namespace Mindhaven.Controllers
{
    [Authorize]
    public class AssessmentQuestionsController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: AssessmentQuestions
        public async Task<ActionResult> Index()
        {
            ViewBag.IsAdmin = (Session["Role"] != null && Session["Role"].ToString() == "Admin");
            var assessmentQuestions = db.AssessmentQuestions.Include(a => a.Assessment);
            return View(await assessmentQuestions.ToListAsync());
        }

        // GET: AssessmentQuestions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var assessmentQuestion = await db.AssessmentQuestions.FindAsync(id);
            if (assessmentQuestion == null)
                return HttpNotFound();

            if (!string.IsNullOrEmpty(assessmentQuestion.Options))
                assessmentQuestion.ParsedOptions = JsonConvert.DeserializeObject<List<string>>(assessmentQuestion.Options);

            return View(assessmentQuestion);
        }

        // GET: AssessmentQuestions/Create
        public ActionResult Create()
        {
            ViewBag.AssessmentID = new SelectList(db.Assessments, "AssessmentID", "Title");
            return View();
        }

        // POST: AssessmentQuestions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "QuestionID,AssessmentID,QuestionText,QuestionType,Options")] AssessmentQuestion assessmentQuestion)
        {
            if (ModelState.IsValid)
            {
                db.AssessmentQuestions.Add(assessmentQuestion);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AssessmentID = new SelectList(db.Assessments, "AssessmentID", "Title", assessmentQuestion.AssessmentID);
            return View(assessmentQuestion);
        }

        // GET: AssessmentQuestions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var assessmentQuestion = await db.AssessmentQuestions.FindAsync(id);
            if (assessmentQuestion == null)
                return HttpNotFound();

            ViewBag.AssessmentID = new SelectList(db.Assessments, "AssessmentID", "Title", assessmentQuestion.AssessmentID);
            return View(assessmentQuestion);
        }

        // POST: AssessmentQuestions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "QuestionID,AssessmentID,QuestionText,QuestionType,Options")] AssessmentQuestion assessmentQuestion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assessmentQuestion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AssessmentID = new SelectList(db.Assessments, "AssessmentID", "Title", assessmentQuestion.AssessmentID);
            return View(assessmentQuestion);
        }

        // GET: AssessmentQuestions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var assessmentQuestion = await db.AssessmentQuestions.FindAsync(id);
            if (assessmentQuestion == null)
                return HttpNotFound();

            return View(assessmentQuestion);
        }

        // POST: AssessmentQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var assessmentQuestion = await db.AssessmentQuestions.FindAsync(id);
            db.AssessmentQuestions.Remove(assessmentQuestion);
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
