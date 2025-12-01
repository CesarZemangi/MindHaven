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
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace Mindhaven.Controllers
{
    public class LoginController : Controller
    {
                private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: Login
        // GET: Login/Login
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Login2(string username, string password)
        {
            if (username == null && password == null)
            {
                return View("Login");
            }

            password = Encrypt(password);
            foreach (User user in db.Users)
            {
                if (username == user.Email && password == user.PasswordHash)
                {
                    FormsAuthentication.SetAuthCookie(user.Email, false);

                    User u = db.Users.Find(user.UserID);
                    Session["Email"] = user.Email;
                    Session["Role"] = user.Role;
                    Session["Name"] = user.FullName;
                    Session["UserID"] = user.UserID;
                    // ✅ Log Activity
                    var logger = new ActivityLogger();
                    logger.LogActivity(user.UserID, "User logged in.");


                    return RedirectToAction("Index", "Home");

                }
            }

            // If login fails
            ViewBag.Error = "Invalid username or password";
            return View("Login");
        }

        private void LogActivity(int userID, string v)
        {
            throw new NotImplementedException();
        }

        string Encrypt(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // Convert byte to hex format
                }

                return sb.ToString();
            }
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Content("Please provide an email address.");
            }

            // Check if a user exists with the provided email
            var user = await db.Users.FirstOrDefaultAsync(users => users.Email == email);
            if (user == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Content("No user found with that email address.");
            }

            // Generate a random new password
            string newPassword = GenerateRandomPassword(8); // 8 characters
                                                            // Encrypt the new password using MD5
            user.PasswordHash = Encrypt(newPassword);
            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            await db.SaveChangesAsync();

            // Send the email using Gmail's SMTP
            try
            {
                var fromAddress = new MailAddress("yourEmail@gmail.com", "Stock Manag App");
                var toAddress = new MailAddress(user.Email);
                const string fromPassword = "yourGmailPassword"; // Replace with your Gmail password or app-specific password
                const string subject = "Password Reset";
                string body = $"Your new password is: {newPassword}\nPlease change it after logging in.";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content("An error occurred while sending the email. " + ex.Message);
            }

            return Json(new { message = "A new password has been sent to your email address. Please change it after logging in." });
        }

        // Method to generate a random password
        private string GenerateRandomPassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut(); // Déconnexion de l'utilisateur
            Session.Clear(); // Supprime toutes les sessions
            Session.Abandon(); // Abandonne la session actuelle

            return RedirectToAction("Login", "Login"); // Redirection to the connexion page
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
