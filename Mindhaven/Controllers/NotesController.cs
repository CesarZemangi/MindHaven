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
    public class NotesController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: Notes
        public async Task<ActionResult> Index()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");
            int userId = Convert.ToInt32(Session["UserId"]);

            var notes = await db.Notes
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notes);
        }

        // POST: Add Note
        [HttpPost]
        public async Task<ActionResult> Create(string content)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");
            int userId = Convert.ToInt32(Session["UserId"]);

            var note = new Note
            {
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.Now
            };

            db.Notes.Add(note);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Delete Note
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var note = await db.Notes.FindAsync(id);
            if (note != null) db.Notes.Remove(note);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}