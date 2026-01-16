using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mindhaven.Models;
using Mindhaven.ViewModels;

namespace Mindhaven.Controllers
{
    public class TakeAssessmentController : Controller
    {
        // GET: TakeAssessment
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        public ActionResult Start(int id)
        {
            var assessment = db.Assessments
                .Where(a => a.AssessmentID == id)
                .FirstOrDefault();

            if (assessment == null) return HttpNotFound();

            var questions = db.AssessmentQuestions
                .Where(q => q.AssessmentID == id)
                .OrderBy(q => q.QuestionType)
                .ToList();

            var model = new TakeAssessmentViewModel
            {
                AssessmentID = id,
                Title = assessment.Title,
                Questions = questions.Select(q => new QuestionViewModel
                {
                    QuestionID = q.QuestionID,
                    Text = q.QuestionText,
                    Type = q.QuestionType,
                    Options = q.Options
                .Split('|')
                .Select((opt, index) => new OptionViewModel
                {
                    OptionID = index + 1,
                    Text = opt
                })
                .ToList()
                }).ToList(),
                Answers = new List<AnswerViewModel>()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Submit(TakeAssessmentSubmitModel model)
        {
            var userId = Session["UserID"] as int?;
            if (userId == null) return RedirectToAction("Login", "Account");

            int totalScore = 0;

            foreach (var answer in model.Answers)
            {
                totalScore += answer.ScoreValue;
            }

            var result = new AssessmentResult
            {
                UserID = userId.Value,
                AssessmentID = model.AssessmentID,
                Score = totalScore,
                TakenAt = System.DateTime.Now
            };

            db.AssessmentResults.Add(result);
            db.SaveChanges();

            return RedirectToAction("Index", "AssessmentResults");
        }
    }
}