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
    public class DeleteRequestsController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: DeleteRequests
        public async Task<ActionResult> Index()
        {
            // ✅ Manual Admin Access Check
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                TempData["Message"] = "Access Denied: Admins only.";
                return RedirectToAction("Login", "Login");
            }
            var deleteRequests = db.DeleteRequests.Include(d => d.User);
            return View(await deleteRequests.ToListAsync());
        }

        // GET: DeleteRequests/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeleteRequest deleteRequest = await db.DeleteRequests.FindAsync(id);
            if (deleteRequest == null)
            {
                return HttpNotFound();
            }
            return View(deleteRequest);
        }

        // GET: DeleteRequests/Create
        public ActionResult Create()
        {
            ViewBag.RequestedByUserID = new SelectList(db.Users, "UserID", "FullName");
            return View();
        }

        // POST: DeleteRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RequestID,ItemType,ItemID,RequestedByUserID,RequestDate,Status")] DeleteRequest deleteRequest)
        {
            if (ModelState.IsValid)
            {
                db.DeleteRequests.Add(deleteRequest);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.RequestedByUserID = new SelectList(db.Users, "UserID", "FullName", deleteRequest.RequestedByUserID);
            return View(deleteRequest);
        }

        // GET: DeleteRequests/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeleteRequest deleteRequest = await db.DeleteRequests.FindAsync(id);
            if (deleteRequest == null)
            {
                return HttpNotFound();
            }
            ViewBag.RequestedByUserID = new SelectList(db.Users, "UserID", "FullName", deleteRequest.RequestedByUserID);
            return View(deleteRequest);
        }

        // POST: DeleteRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RequestID,ItemType,ItemID,RequestedByUserID,RequestDate,Status")] DeleteRequest deleteRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deleteRequest).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.RequestedByUserID = new SelectList(db.Users, "UserID", "FullName", deleteRequest.RequestedByUserID);
            return View(deleteRequest);
        }

        // GET: DeleteRequests/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeleteRequest deleteRequest = await db.DeleteRequests.FindAsync(id);
            if (deleteRequest == null)
            {
                return HttpNotFound();
            }
            return View(deleteRequest);
        }

        // POST: DeleteRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DeleteRequest deleteRequest = await db.DeleteRequests.FindAsync(id);
            db.DeleteRequests.Remove(deleteRequest);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        //creating delete manually
        public ActionResult RequestDeletes(string itemType, int itemId)
        {
            if (Session["UserID"] == null)
            {
                TempData["Message"] = "Please log in to request deletion.";
                return RedirectToAction("Login", "Login");
            }

            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserID"]);

            // Check for existing pending request
            var existing = db.DeleteRequests
                             .FirstOrDefault(r => r.ItemType == itemType && r.ItemID == itemId && r.RequestedByUserID == userId && r.Status == "Pending");

            if (existing != null)
            {
                TempData["Message"] = "You have already requested deletion for this item.";
                return RedirectToAction("Index", itemType + "s"); // Redirect to original listing page
            }

            var request = new DeleteRequest
            {
                ItemType = itemType,
                ItemID = itemId,
                RequestedByUserID = userId,
                RequestDate = DateTime.Now,
                Status = "Pending"
            };

            db.DeleteRequests.Add(request);
            db.SaveChanges();

            TempData["Message"] = "Delete request submitted successfully.";
            return RedirectToAction("Index", itemType + "s"); // Assumes controller name matches plural
        }
        public ActionResult Approve(int id)
        {
            var request = db.DeleteRequests.Find(id);
            if (request == null)
                return HttpNotFound();

            request.Status = "Approved";
            db.Entry(request).State = EntityState.Modified;

            // Dynamically delete from the correct table
            switch (request.ItemType)
            {
                case "Meditation":
                    var meditation = db.Meditations.Find(request.ItemID);
                    if (meditation != null) db.Meditations.Remove(meditation);
                    break;

                case "MoodLog":
                    var mood = db.MoodLogs.Find(request.ItemID);
                    if (mood != null) db.MoodLogs.Remove(mood);
                    break;

                case "Assessment":
                    var assessment = db.Assessments.Find(request.ItemID);
                    if (assessment != null) db.Assessments.Remove(assessment);
                    break;

                case "AssessmentQuestion":
                    var question = db.AssessmentQuestions.Find(request.ItemID);
                    if (question != null) db.AssessmentQuestions.Remove(question);
                    break;

                case "Resource":
                    var resource = db.Resources.Find(request.ItemID);
                    if (resource != null) db.Resources.Remove(resource);
                    break;

                case "GroupPost":
                    var post = db.GroupPosts.Find(request.ItemID);
                    if (post != null) db.GroupPosts.Remove(post);
                    break;

                default:
                    TempData["Message"] = "Unknown item type.";
                    return RedirectToAction("Index");
            }

            db.SaveChanges();
            TempData["Message"] = "Request approved and item deleted.";
            return RedirectToAction("Index");
        }
        public ActionResult Reject(int id)
        {
            var request = db.DeleteRequests.Find(id);
            if (request == null)
                return HttpNotFound();

            request.Status = "Rejected";
            db.Entry(request).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Message"] = "Request rejected.";
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
