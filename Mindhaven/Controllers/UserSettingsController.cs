using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mindhaven.Helpers;
using Mindhaven.Models;

namespace Mindhaven.Controllers
{
    public class UserSettingsController : Controller
    {
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        // GET: UserSettings
        public ActionResult Index()
        {
            

            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");
            int userId = Convert.ToInt32(Session["UserId"]);

            var settings = db.UserSettings.FirstOrDefault(s => s.UserId == userId);
            return View(settings);
        }

        // POST: Update settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(UserSetting model)
        {

            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");
            int userId = Convert.ToInt32(Session["UserId"]);

            var settings = db.UserSettings.FirstOrDefault(s => s.UserId == userId);
            if (settings != null)
            {
                settings.ShowNotifications = model.ShowNotifications;
                settings.EnableChat = model.EnableChat;
                settings.ShowHistory = model.ShowHistory;
                settings.OfflineStatus = model.OfflineStatus;

                db.Entry(settings).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}