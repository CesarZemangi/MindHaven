using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mindhaven.Models;
using Newtonsoft.Json;

namespace Mindhaven.Controllers
{
    [Authorize]
    public class AssessmentResultsController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // List assessments available for the user
        public ActionResult Available()
        {
            var assessments = db.Assessments.ToList();
            return View(assessments);
        }

        // GET: TakeAssessment
        public ActionResult TakeAssessment(int assessmentId)
        {
            int userId = (int)Session["UserID"];
            var assessment = db.Assessments.Find(assessmentId);
            if (assessment == null) return HttpNotFound();

            var questions = db.AssessmentQuestions
                              .Where(q => q.AssessmentID == assessmentId)
                              .ToList();

            foreach (var q in questions)
            {
                if (!string.IsNullOrEmpty(q.Options))
                    q.ParsedOptions = JsonConvert.DeserializeObject<List<string>>(q.Options);
            }

            var model = new TakeAssessmentViewModel
            {
                UserID = userId,
                Assessment = assessment,
                Questions = questions
            };

            return View(model);
        }

        // POST: SubmitAssessment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitAssessment(int UserID, int AssessmentID, FormCollection form)
        {
            var answers = new Dictionary<int, object>();

            foreach (var key in form.AllKeys)
            {
                if (key.StartsWith("question_"))
                {
                    int questionId = int.Parse(key.Replace("question_", ""));
                    var values = form.GetValues(key);

                    if (values.Length > 1)
                        answers[questionId] = values.ToList();
                    else
                        answers[questionId] = values[0];
                }
            }

            var result = new AssessmentResult
            {
                UserID = UserID,
                AssessmentID = AssessmentID,
                Answers = JsonConvert.SerializeObject(answers),
                TakenAt = DateTime.Now
            };

            db.AssessmentResults.Add(result);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Display user results
        public async Task<ActionResult> Index()
        {
            int userId = (int)Session["UserID"];
            var results = db.AssessmentResults
                            .Include(a => a.Assessment)
                            .Where(r => r.UserID == userId)
                            .ToList();
            return await Task.FromResult(View(results));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
