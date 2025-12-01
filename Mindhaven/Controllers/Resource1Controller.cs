using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mindhaven.Models;
using Mindhaven.ViewModels;

namespace Mindhaven.Controllers
{
    public class Resource1Controller : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: Resource1
        public async Task<ActionResult> Index()
        {
            return View(await db.Resources1.ToListAsync());
        }

        // GET: Resource1/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Resource1 resource1 = await db.Resources1.FindAsync(id);
            if (resource1 == null)
            {
                return HttpNotFound();
            }
            return View(resource1);
        }

        // GET: Resource1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Resource1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ResourceID,Title,Content,MediaType,Link")] Resource1 resource1)
        {
            if (ModelState.IsValid)
            {
                db.Resources1.Add(resource1);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(resource1);
        }

        // GET: Resource1/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Resource1 resource1 = await db.Resources1.FindAsync(id);
            if (resource1 == null)
            {
                return HttpNotFound();
            }
            return View(resource1);
        }

        // POST: Resource1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ResourceID,Title,Content,MediaType,Link")] Resource1 resource1)
        {
            if (ModelState.IsValid)
            {
                db.Entry(resource1).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(resource1);
        }

        // GET: Resource1/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Resource1 resource1 = await db.Resources1.FindAsync(id);
            if (resource1 == null)
            {
                return HttpNotFound();
            }
            return View(resource1);
        }

        // POST: Resource1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Resource1 resource1 = await db.Resources1.FindAsync(id);
            db.Resources1.Remove(resource1);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public ActionResult Landing()
        {
            var viewModel = new LandingViewModel
            {
                Resources1 = db.Resources1.ToList(),
                LatestArticles = db.Articles.OrderByDescending(a => a.PublishedDate).Take(3).ToList(),
                Announcements = db.Announcements.OrderByDescending(a => a.Date).Take(5).ToList(),
                CaseStudies = db.CaseStudies.ToList(),
                MediaMentions = db.MediaMentions.ToList()
            };
            return View(viewModel);
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
