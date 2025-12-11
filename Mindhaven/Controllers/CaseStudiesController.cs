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
    public class CaseStudiesController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: CaseStudies
        public async Task<ActionResult> Index()
        {
            var caseStudies = db.CaseStudies.Include(c => c.User);
            return View(await caseStudies.ToListAsync());
        }

        // GET: CaseStudies/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CaseStudy caseStudy = await db.CaseStudies.FindAsync(id);
            if (caseStudy == null)
            {
                return HttpNotFound();
            }
            return View(caseStudy);
        }

        // GET: CaseStudies/Create
        public ActionResult Create()
        {
            ViewBag.AuthorId = new SelectList(db.Users, "UserID", "FullName");
            return View();
        }

        // POST: CaseStudies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CaseStudyId,Title,Summary,Content,ImageUrl,PublishedDate,AuthorId")] CaseStudy caseStudy)
        {
            if (ModelState.IsValid)
            {
                db.CaseStudies.Add(caseStudy);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AuthorId = new SelectList(db.Users, "UserID", "FullName", caseStudy.AuthorId);
            return View(caseStudy);
        }

        // GET: CaseStudies/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CaseStudy caseStudy = await db.CaseStudies.FindAsync(id);
            if (caseStudy == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "UserID", "FullName", caseStudy.AuthorId);
            return View(caseStudy);
        }

        // POST: CaseStudies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CaseStudyId,Title,Summary,Content,ImageUrl,PublishedDate,AuthorId")] CaseStudy caseStudy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(caseStudy).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorId = new SelectList(db.Users, "UserID", "FullName", caseStudy.AuthorId);
            return View(caseStudy);
        }

        // GET: CaseStudies/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CaseStudy caseStudy = await db.CaseStudies.FindAsync(id);
            if (caseStudy == null)
            {
                return HttpNotFound();
            }
            return View(caseStudy);
        }

        // POST: CaseStudies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CaseStudy caseStudy = await db.CaseStudies.FindAsync(id);
            db.CaseStudies.Remove(caseStudy);
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
