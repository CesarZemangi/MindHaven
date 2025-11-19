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
    public class BadgeConfigsController : Controller
    {
                private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: BadgeConfigs
        public async Task<ActionResult> Index()
        {
            return View(await db.BadgeConfigs.ToListAsync());
        }

        // GET: BadgeConfigs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BadgeConfig badgeConfig = await db.BadgeConfigs.FindAsync(id);
            if (badgeConfig == null)
            {
                return HttpNotFound();
            }
            return View(badgeConfig);
        }

        // GET: BadgeConfigs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BadgeConfigs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "BadgeId,BadgeType,Description,ConditionType,RequiredCount,IconPath")] BadgeConfig badgeConfig)
        {
            if (ModelState.IsValid)
            {
                db.BadgeConfigs.Add(badgeConfig);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(badgeConfig);
        }

        // GET: BadgeConfigs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BadgeConfig badgeConfig = await db.BadgeConfigs.FindAsync(id);
            if (badgeConfig == null)
            {
                return HttpNotFound();
            }
            return View(badgeConfig);
        }

        // POST: BadgeConfigs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "BadgeId,BadgeType,Description,ConditionType,RequiredCount,IconPath")] BadgeConfig badgeConfig)
        {
            if (ModelState.IsValid)
            {
                db.Entry(badgeConfig).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(badgeConfig);
        }

        // GET: BadgeConfigs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BadgeConfig badgeConfig = await db.BadgeConfigs.FindAsync(id);
            if (badgeConfig == null)
            {
                return HttpNotFound();
            }
            return View(badgeConfig);
        }

        // POST: BadgeConfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            BadgeConfig badgeConfig = await db.BadgeConfigs.FindAsync(id);
            db.BadgeConfigs.Remove(badgeConfig);
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
