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
    

    public class MeditationDeleteRequestsController : Controller
    {
                private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();


        // GET: MeditationDeleteRequests
        public async Task<ActionResult> Index()
        {
            // Manual session-based admin check
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")

            {
                TempData["Message"] = "Access Denied: Admins only.";
                return RedirectToAction("Login", "Login");
            }

            var meditationDeleteRequests = db.MeditationDeleteRequests.Include(m => m.Meditation);
            return View(await meditationDeleteRequests.ToListAsync());
        }

        // GET: MeditationDeleteRequests/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MeditationDeleteRequest meditationDeleteRequest = await db.MeditationDeleteRequests.FindAsync(id);
            if (meditationDeleteRequest == null)
            {
                return HttpNotFound();
            }
            return View(meditationDeleteRequest);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Approve(int id)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                TempData["Message"] = "Access denied.";
                return RedirectToAction("Login", "Login");
            }

            var request = db.MeditationDeleteRequests.Find(id);
            if (request == null) return HttpNotFound();

            request.Status = "Approved";

            // Optionally delete the meditation
            var meditation = db.Meditations.Find(request.MeditationId);
            if (meditation != null)
                db.Meditations.Remove(meditation);

            db.SaveChanges();

            TempData["Message"] = "Meditation approved and deleted.";
            return RedirectToAction("Index");
        }

        public ActionResult Reject(int id)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                TempData["Message"] = "Access denied.";
                return RedirectToAction("Login", "Login");
            }

            var request = db.MeditationDeleteRequests.Find(id);
            if (request == null) return HttpNotFound();

            request.Status = "Rejected";
            db.SaveChanges();

            TempData["Message"] = "Request rejected.";
            return RedirectToAction("Index");
        }

        // GET: MeditationDeleteRequests/Create
        public ActionResult Create()
        {
            ViewBag.MeditationId = new SelectList(db.Meditations, "MeditationID", "Title");
            return View();
        }

        // POST: MeditationDeleteRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,MeditationId,RequestedByUserId,Status,RequestDate")] MeditationDeleteRequest meditationDeleteRequest)
        {
            if (ModelState.IsValid)
            {
                db.MeditationDeleteRequests.Add(meditationDeleteRequest);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MeditationId = new SelectList(db.Meditations, "MeditationID", "Title", meditationDeleteRequest.MeditationId);
            return View(meditationDeleteRequest);
        }

        // GET: MeditationDeleteRequests/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MeditationDeleteRequest meditationDeleteRequest = await db.MeditationDeleteRequests.FindAsync(id);
            if (meditationDeleteRequest == null)
            {
                return HttpNotFound();
            }
            ViewBag.MeditationId = new SelectList(db.Meditations, "MeditationID", "Title", meditationDeleteRequest.MeditationId);
            return View(meditationDeleteRequest);
        }

        // POST: MeditationDeleteRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,MeditationId,RequestedByUserId,Status,RequestDate")] MeditationDeleteRequest meditationDeleteRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(meditationDeleteRequest).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MeditationId = new SelectList(db.Meditations, "MeditationID", "Title", meditationDeleteRequest.MeditationId);
            return View(meditationDeleteRequest);
        }

        // GET: MeditationDeleteRequests/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MeditationDeleteRequest meditationDeleteRequest = await db.MeditationDeleteRequests.FindAsync(id);
            if (meditationDeleteRequest == null)
            {
                return HttpNotFound();
            }
            return View(meditationDeleteRequest);
        }

        // POST: MeditationDeleteRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MeditationDeleteRequest meditationDeleteRequest = await db.MeditationDeleteRequests.FindAsync(id);
            db.MeditationDeleteRequests.Remove(meditationDeleteRequest);
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
