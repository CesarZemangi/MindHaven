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
using System.Data.Entity.Validation;

namespace Mindhaven.Controllers
{
    [Authorize]
    public class MeditationsController : Controller
    {
                private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: Meditations
        public async Task<ActionResult> Index()
        {
            ViewBag.IsAdmin = (Session["Role"] != null && Session["Role"].ToString() == "Admin");
            return View(await db.Meditations.ToListAsync());
        }
        public ActionResult RequestDelete(int id)
        {
            var meditation = db.Meditations.Find(id);
            if (meditation == null)
            {
                return HttpNotFound();
            }

            var userId = Session["UserID"]?.ToString();

            // Check if user is logged in
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Message"] = "You must be logged in to request deletion.";
                return RedirectToAction("Login", "Account");
            }

            // Check for duplicate requests
            bool alreadyRequested = db.MeditationDeleteRequests
                .Any(r => r.MeditationId == id && r.RequestedByUserId == userId && r.Status == "Pending");

            if (alreadyRequested)
            {
                TempData["Message"] = "You have already submitted a delete request for this meditation.";
                return RedirectToAction("Index");
            }

            // Create new request
            var request = new MeditationDeleteRequest
            {
                MeditationId = id,
                RequestedByUserId = userId,
                RequestDate = DateTime.Now,
                Status = "Pending"
            };

            try
            {
                db.MeditationDeleteRequests.Add(request);
                db.SaveChanges();

                TempData["Message"] = "Delete request submitted. Admin will review it.";
            }
            catch (DbEntityValidationException ex)
            {
                // Log detailed validation errors
                foreach (var failure in ex.EntityValidationErrors)
                {
                    foreach (var error in failure.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Property: {error.PropertyName} Error: {error.ErrorMessage}");
                    }
                }

                TempData["Message"] = "Something went wrong while submitting the delete request.";
            }

            return RedirectToAction("Index");
        }

        

        // GET: Meditations/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meditation meditation = await db.Meditations.FindAsync(id);
            if (meditation == null)
            {
                return HttpNotFound();
            }
            return View(meditation);
        }

        // GET: Meditations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Meditations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "MeditationID,Title,Description,MediaURL,Category")] Meditation meditation)
        {
            if (ModelState.IsValid)
            {
                db.Meditations.Add(meditation);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(meditation);
        }

        // GET: Meditations/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meditation meditation = await db.Meditations.FindAsync(id);
            if (meditation == null)
            {
                return HttpNotFound();
            }
            return View(meditation);
        }

        // POST: Meditations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "MeditationID,Title,Description,MediaURL,Category")] Meditation meditation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(meditation).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(meditation);
        }

        // GET: Meditations/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meditation meditation = await db.Meditations.FindAsync(id);
            if (meditation == null)
            {
                return HttpNotFound();
            }
            return View(meditation);
        }

        // POST: Meditations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Meditation meditation = await db.Meditations.FindAsync(id);
            db.Meditations.Remove(meditation);
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
