using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mindhaven.Models;
using System.Security.Cryptography;

namespace Mindhaven.Controllers
{
    public class UsersController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: Users
        public async Task<ActionResult> Index()
        {

            ViewBag.IsAdmin = (Session["Role"] != null && Session["Role"].ToString() == "Admin");
            return View(await db.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User user = await db.Users.FindAsync(id);
            if (user == null)
                return HttpNotFound();

            var logger = new ActivityLogger();
            logger.LogActivity(Convert.ToInt32(Session["UserID"]), "Viewed user details: " + user.FullName);

            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "UserID,FullName,Email,PasswordHash,Role,BirthDate,CreatedAt")] User user)
        {
            if (ModelState.IsValid)
            {
                user.PasswordHash = Encrypt(user.PasswordHash);
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User user = await db.Users.FindAsync(id);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "UserID,FullName,Email,PasswordHash,Role,BirthDate,CreatedAt")] User user)
        {
            if (ModelState.IsValid)
            {
                user.PasswordHash = Encrypt(user.PasswordHash);
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/ChangePassword/5
        public async Task<ActionResult> ChangePassword(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User user = await db.Users.FindAsync(id);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // POST: Users/ChangePassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword([Bind(Include = "UserID,PasswordHash")] User user)
        {
            if (ModelState.IsValid)
            {
                user.PasswordHash = Encrypt(user.PasswordHash);
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User user = await db.Users.FindAsync(id);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            User user = await db.Users.FindAsync(id);
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Users/Register
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string FullName, string Email, string Password)
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                TempData["Message"] = "All fields are required.";
                return View();
            }

            var existing = db.Users.FirstOrDefault(u => u.Email == Email);
            if (existing != null)
            {
                TempData["Message"] = "User already exists.";
                return View();
            }

            var user = new User
            {
                FullName = FullName,
                Email = Email,
                PasswordHash = Encrypt(Password),
                Role = "User",
                CreatedAt = DateTime.Now
            };

            db.Users.Add(user);
            db.SaveChanges();

            TempData["Message"] = "Registration successful. You can now log in.";
            return RedirectToAction("Login", "Login");
        }

        private string Encrypt(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
