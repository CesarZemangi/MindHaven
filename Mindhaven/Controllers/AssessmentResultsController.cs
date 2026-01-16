using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mindhaven.Models;
using Newtonsoft.Json;
using Mindhaven.ViewModels;


namespace Mindhaven.Controllers
{
    [Authorize]
    public class AssessmentResultsController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        public ActionResult Index()
        {
            var userObj = Session["UserID"];
            if (userObj == null)
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(userObj);

            var results = db.AssessmentResults
                .Where(r => r.UserID == userId)
                .Select(r => new AssessmentIndexViewModel
                {
                    AssessmentID = r.Assessment.AssessmentID,
                    Title = r.Assessment.Title,
                    Description = r.Assessment.Description,
                    Score = r.Score,
                    TakenAt = r.TakenAt
                })
                .ToList();

            ViewBag.IsAdmin = false;
            return View(results);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}