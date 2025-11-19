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
    [Authorize]
    public class SupportGroupsController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: SupportGroups
        public async Task<ActionResult> Index()
        {
            ViewBag.IsAdmin = (Session["Role"] != null && Session["Role"].ToString() == "Admin");
            return View(await db.SupportGroups.ToListAsync());
        }

        // GET: SupportGroups/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupportGroup supportGroup = await db.SupportGroups.FindAsync(id);
            if (supportGroup == null)
            {
                return HttpNotFound();
            }
            return View(supportGroup);
        }

        // GET: SupportGroups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SupportGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "GroupID,Name,Description,CreatedAt")] SupportGroup supportGroup)
        {
            if (ModelState.IsValid)
            {
                db.SupportGroups.Add(supportGroup);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(supportGroup);
        }

        // GET: SupportGroups/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupportGroup supportGroup = await db.SupportGroups.FindAsync(id);
            if (supportGroup == null)
            {
                return HttpNotFound();
            }
            return View(supportGroup);
        }

        // POST: SupportGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "GroupID,Name,Description,CreatedAt")] SupportGroup supportGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supportGroup).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(supportGroup);
        }

        // GET: SupportGroups/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SupportGroup supportGroup = await db.SupportGroups.FindAsync(id);
            if (supportGroup == null)
            {
                return HttpNotFound();
            }
            return View(supportGroup);
        }

        // POST: SupportGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SupportGroup supportGroup = await db.SupportGroups.FindAsync(id);
            db.SupportGroups.Remove(supportGroup);
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
