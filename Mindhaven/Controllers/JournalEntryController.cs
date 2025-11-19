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
    public class JournalEntryController : Controller
    {
                private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: JournalEntries/MyJournal
        public ActionResult MyJournal(string search, DateTime? date)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserId"]);
            var entries = db.JournalEntries.Where(j => j.UserId == userId);

            if (!string.IsNullOrEmpty(search))
                entries = entries.Where(j => j.Title.Contains(search));

            if (date.HasValue)
                entries = entries.Where(j => DbFunctions.TruncateTime(j.CreatedDate) == date.Value.Date);

            return View(entries.OrderByDescending(j => j.CreatedDate).ToList());
        }

        // GET: JournalEntries/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            JournalEntry journalEntry = await db.JournalEntries.FindAsync(id);
            if (journalEntry == null)
                return HttpNotFound();

            int userId = Convert.ToInt32(Session["UserId"]);

            // Allow the owner or care team members with shared access
            bool canView = journalEntry.UserId == userId ||
                db.SharedAccesses.Any(a =>
                    a.CareTeamMemberId == userId &&
                    a.UserId == journalEntry.UserId &&
                    journalEntry.IsShared== true);


            if (!canView)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View(journalEntry);
        }

        // GET: JournalEntries/Create
        public ActionResult Create()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: JournalEntries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JournalEntry journalEntry)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                journalEntry.CreatedDate = DateTime.Now;
                journalEntry.UserId = Convert.ToInt32(Session["UserId"]);

                // Save IsShared toggle from the UI
                db.JournalEntries.Add(journalEntry);
                db.SaveChanges();

                return RedirectToAction("MyJournal");
            }

            return View(journalEntry);
        }

        // GET: JournalEntries/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            JournalEntry journalEntry = await db.JournalEntries.FindAsync(id);
            if (journalEntry == null)
                return HttpNotFound();

            int userId = Convert.ToInt32(Session["UserId"]);
            if (journalEntry.UserId != userId)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View(journalEntry);
        }

        // POST: JournalEntries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Content,CreatedDate,UserId,IsShared")] JournalEntry journalEntry)
        {
            if (Session["UserId"] == null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            int userId = Convert.ToInt32(Session["UserId"]);
            if (journalEntry.UserId != userId)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            if (ModelState.IsValid)
            {
                db.Entry(journalEntry).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("MyJournal");
            }

            return View(journalEntry);
        }

        // GET: JournalEntries/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            JournalEntry journalEntry = await db.JournalEntries.FindAsync(id);
            if (journalEntry == null)
                return HttpNotFound();

            int userId = Convert.ToInt32(Session["UserId"]);
            if (journalEntry.UserId != userId)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View(journalEntry);
        }

        // POST: JournalEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (Session["UserId"] == null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            JournalEntry journalEntry = await db.JournalEntries.FindAsync(id);
            if (journalEntry == null)
                return HttpNotFound();

            int userId = Convert.ToInt32(Session["UserId"]);
            if (journalEntry.UserId != userId)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            db.JournalEntries.Remove(journalEntry);
            await db.SaveChangesAsync();

            return RedirectToAction("MyJournal");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
