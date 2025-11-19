using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mindhaven.Models;

namespace Mindhaven.Controllers
{
    [Authorize]
    public class SharedAccessesController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: SharedAccesses
        public async Task<ActionResult> Index()
        {
            var sharedAccesses = db.SharedAccesses
                .Include(s => s.User)
                .Include(s => s.CareTeamMember); // updated property
            return View(await sharedAccesses.ToListAsync());
        }

        // GET: SharedAccesses/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var sharedAccess = await db.SharedAccesses.FindAsync(id);
            if (sharedAccess == null)
                return HttpNotFound();

            return View(sharedAccess);
        }

        // GET: SharedAccesses/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName");
            ViewBag.CareTeamMemberId = new SelectList(db.Users, "UserID", "FullName");
            return View();
        }

        // POST: SharedAccesses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,UserID,CareTeamMemberId,GrantedAt")] SharedAccess sharedAccess)
        {
            if (ModelState.IsValid)
            {
                db.SharedAccesses.Add(sharedAccess);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", sharedAccess.UserId);
            ViewBag.CareTeamMemberId = new SelectList(db.Users, "UserID", "FullName", sharedAccess.CareTeamMemberId);
            return View(sharedAccess);
        }

        // GET: SharedAccesses/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var sharedAccess = await db.SharedAccesses.FindAsync(id);
            if (sharedAccess == null)
                return HttpNotFound();

            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", sharedAccess.UserId);
            ViewBag.CareTeamMemberId = new SelectList(db.Users, "UserID", "FullName", sharedAccess.CareTeamMemberId);
            return View(sharedAccess);
        }

        // POST: SharedAccesses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserID,CareTeamMemberId,GrantedAt")] SharedAccess sharedAccess)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sharedAccess).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", sharedAccess.UserId);
            ViewBag.CareTeamMemberId = new SelectList(db.Users, "UserID", "FullName", sharedAccess.CareTeamMemberId);
            return View(sharedAccess);
        }

        // GET: SharedAccesses/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var sharedAccess = await db.SharedAccesses.FindAsync(id);
            if (sharedAccess == null)
                return HttpNotFound();

            return View(sharedAccess);
        }

        // POST: SharedAccesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var sharedAccess = await db.SharedAccesses.FindAsync(id);
            db.SharedAccesses.Remove(sharedAccess);
            await db.SaveChangesAsync();
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
