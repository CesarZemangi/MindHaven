using System;
using System.Linq;
using System.Web.Mvc;
using Mindhaven.Models;
using Mindhaven.ViewModel;
using Mindhaven.ViewModels;


namespace Mindhaven.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
                private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        public ActionResult Index()
        {
            var admin = db.Users.FirstOrDefault(x => x.Role == "admin");
            string adminName = admin != null ? admin.FullName : "Admin";


            // Fetch all meditations first
            var allMeditations = db.Meditations.ToList();

            // Filter recent ones in memory
            var recentMeditations = allMeditations
                .Where(m => m.DateCreated >= DateTime.Today.AddDays(-7))
                .ToList();

            var dashboardViewModel = new MindHavenDashboardViewModel(
                userName: adminName,
                moodLogs: db.MoodLogs.ToList(),
                assessmentResults: db.AssessmentResults.ToList(),
                appointments: db.Appointments.ToList(),
                recommendedMeditations: recentMeditations,
                meditationList: allMeditations,
                supportMessages: db.SupportGroups.ToList(),
                users: db.Users.ToList()

            );

            return View(dashboardViewModel);
        }
        public ActionResult Landing()
        {
            var model = new LandingViewModel
            {
                LatestArticles = db.Articles
                    .OrderByDescending(a => a.PublishedDate ?? DateTime.MinValue)
                    .Take(5)
                    .ToList(),

                Announcements = db.Announcements
                    .OrderByDescending(a => a.Date ?? DateTime.MinValue)
                    .Take(5)
                    .ToList(),

                CaseStudies = db.CaseStudies
                    .OrderByDescending(c => c.PublishedDate ?? DateTime.MinValue)
                    .Take(3)
                    .ToList(),

                MediaMentions = db.MediaMentions
                    .OrderByDescending(m => m.PublishedDate ?? DateTime.MinValue)
                    .Take(3)
                    .ToList(),

                Resources = db.Resources
                    .OrderByDescending(r => r.ResourceId) // Resources have no PublishedDate
                    .Take(5)
                    .ToList()
            };

            return View(model); // Returns Landing.cshtml
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Subscribe(string emailInput)
        {
            if (!string.IsNullOrEmpty(emailInput))
            {
                db.Subscribers.Add(new Subscriber
                {
                    Email = emailInput,
                    SubscribedDate = DateTime.Now
                });
                db.SaveChanges();
            }

            TempData["Message"] = "Thank you for subscribing!";
            return RedirectToAction("Landing");
        }
        public ActionResult HomeworkDashboard()
        {
            var userId = Convert.ToInt32(Session["UserID"]);
            var userRole = Session["Role"]?.ToString();

            using (var db = new mindhavenDBEntities1())
            {
                if (userRole == "Therapist")
                {
                    var therapistHomeworks = db.TherapeuticHomeworks
                        .Where(h => h.TherapistId == userId)
                        .OrderByDescending(h => h.AssignedDate)
                        .ToList();

                    var feedbacks = db.TherapistFeedbacks
                        .OrderByDescending(f => f.FeedbackDate)
                        .Take(5)
                        .ToList();

                    ViewBag.RecentFeedbacks = feedbacks;
                    return View("TherapistHomeworkDashboard", therapistHomeworks);
                }
                else
                {
                    var assignedHomeworks = db.TherapeuticHomeworks
                        .Where(h => h.UserId == userId)
                        .OrderByDescending(h => h.AssignedDate)
                        .ToList();

                    var mySubmissions = db.HomeworkSubmissions
                        .Where(s => s.UserId == userId)
                        .OrderByDescending(s => s.SubmittedDate)
                        .ToList();

                    ViewBag.MySubmissions = mySubmissions;
                    return View("UserHomeworkDashboard", assignedHomeworks);
                }
            }
        }

        public ActionResult UserHomeworkDashboard()
        {

            var userId = Convert.ToInt32(Session["UserID"]);

            var homeworks = db.TherapeuticHomeworks
                .Where(h => h.UserId == userId)
                .ToList();

            var feedbacks = db.Feedbacks
                .Where(f => f.UserId == userId)
                .ToList();

            var model = new HomeworkDashboardViewModel
            {
                Homeworks = homeworks,
                Feedbacks = feedbacks
            };

            return View("UserHomeworkDashboard", model);
        }


        public ActionResult TherapistHomeworkDashboard()
        {
            var therapistId = Convert.ToInt32(Session["UserID"]);

            var assignedHomeworks = db.TherapeuticHomeworks
                .Where(h => h.TherapistId == therapistId)
                .ToList();

            var feedbacks = db.Feedbacks
                .Where(f => assignedHomeworks.Select(hw => hw.HomeworkId).Contains(f.TherapeuticHomeworkId))
                .ToList();

            var model = new HomeworkDashboardViewModel
            {
                Homeworks = assignedHomeworks,
                Feedbacks = feedbacks
            };

            return View("TherapistHomeworkDashboard", model);
        }


        public ActionResult MyJournal()
        {
            var userId = Convert.ToInt32(Session["UserID"]);
            var entries = db.JournalEntries.Where(j => j.UserId == userId)
                                           .OrderByDescending(j => j.CreatedDate)
                                           .ToList();
            return View(entries);
        }
        public ActionResult HelpNow()
        {
            return View();
        }


    }
}
