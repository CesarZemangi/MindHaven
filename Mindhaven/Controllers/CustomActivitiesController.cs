using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Mindhaven.Models;

namespace Mindhaven.Controllers
{
    public class CustomActivitiesController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: CustomActivities
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var userRole = Session["Role"]?.ToString();
            int userId = Convert.ToInt32(Session["UserId"]);

            if (userRole == "Admin" || userRole == "Therapist")
            {
                var allActivities = db.CustomActivities.ToList();
                return View(allActivities);
            }

            var userActivities = db.CustomActivities
                .Where(a => a.UserId == userId && !a.DeleteRequested)
                .ToList();

            return View(userActivities);
        }

        // GET: CustomActivities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var activity = db.CustomActivities.Find(id);
            if (activity == null)
                return HttpNotFound();

            return View(activity);
        }

        // GET: CustomActivities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomActivities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomActivity customActivity)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserId"]);
            customActivity.UserId = userId;
            customActivity.DeleteRequested = false;

            if (ModelState.IsValid)
            {
                db.CustomActivities.Add(customActivity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customActivity);
        }

        // GET: CustomActivities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var activity = db.CustomActivities.Find(id);
            if (activity == null)
                return HttpNotFound();

            int userId = Convert.ToInt32(Session["UserId"]);
            string role = Session["Role"]?.ToString();

            if (role == "User" && activity.UserId != userId)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View(activity);
        }

        // POST: CustomActivities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CustomActivity customActivity)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserId"]);
            var existing = db.CustomActivities.Find(customActivity.CustomId);

            if (existing == null)
                return HttpNotFound();

            string role = Session["Role"]?.ToString();
            if (role == "User" && existing.UserId != userId)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            existing.Title = customActivity.Title;
            existing.Description = customActivity.Description;

            db.Entry(existing).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: CustomActivities/RequestDelete (User soft delete)
        [HttpPost]
        public ActionResult RequestDelete(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserId"]);
            var activity = db.CustomActivities.Find(id);

            if (activity == null)
                return HttpNotFound();

            if (activity.UserId != userId)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            activity.DeleteRequested = true;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: CustomActivities/HardDelete (Admin hard delete)
        [HttpPost]
        public ActionResult HardDelete(int id)
        {
            string role = Session["Role"]?.ToString();
            if (role != "Admin")
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            var activity = db.CustomActivities.Find(id);
            if (activity == null)
                return HttpNotFound();

            db.CustomActivities.Remove(activity);
            db.SaveChanges();

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
