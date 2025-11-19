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
    public class GroupPostsController : Controller
    {
                private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: GroupPosts
        public async Task<ActionResult> Index()
        {
            ViewBag.IsAdmin = (Session["Role"] != null && Session["Role"].ToString() == "Admin");
            var groupPosts = db.GroupPosts.Include(g => g.SupportGroup).Include(g => g.User);
            return View(await groupPosts.ToListAsync());
        }

        // GET: GroupPosts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupPost groupPost = await db.GroupPosts.FindAsync(id);
            if (groupPost == null)
            {
                return HttpNotFound();
            }
            return View(groupPost);
        }

        // GET: GroupPosts/Create
        public ActionResult Create()
        {
            ViewBag.GroupID = new SelectList(db.SupportGroups, "GroupID", "Name");
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName");
            return View();
        }

        // POST: GroupPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PostID,GroupID,UserID,PostContent,PostedAt")] GroupPost groupPost)
        {
            if (ModelState.IsValid)
            {
                db.GroupPosts.Add(groupPost);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.GroupID = new SelectList(db.SupportGroups, "GroupID", "Name", groupPost.GroupID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", groupPost.UserID);
            return View(groupPost);
        }

        // GET: GroupPosts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupPost groupPost = await db.GroupPosts.FindAsync(id);
            if (groupPost == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupID = new SelectList(db.SupportGroups, "GroupID", "Name", groupPost.GroupID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", groupPost.UserID);
            return View(groupPost);
        }

        // POST: GroupPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PostID,GroupID,UserID,PostContent,PostedAt")] GroupPost groupPost)
        {
            if (ModelState.IsValid)
            {
                db.Entry(groupPost).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.GroupID = new SelectList(db.SupportGroups, "GroupID", "Name", groupPost.GroupID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", groupPost.UserID);
            return View(groupPost);
        }

        // GET: GroupPosts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupPost groupPost = await db.GroupPosts.FindAsync(id);
            if (groupPost == null)
            {
                return HttpNotFound();
            }
            return View(groupPost);
        }

        // POST: GroupPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            GroupPost groupPost = await db.GroupPosts.FindAsync(id);
            db.GroupPosts.Remove(groupPost);
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
