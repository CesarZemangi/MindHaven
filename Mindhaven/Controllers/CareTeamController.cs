using System;
using System.Linq;
using System.Web.Mvc;
using Mindhaven.Models;

namespace Mindhaven.Controllers
{
    public class CareTeamController : Controller
    {
                private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        public ActionResult Dashboard()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserId"]);

            // Get users who shared entries with this care member
            var sharedJournals = db.JournalEntries
     .Where(j => j.IsShared== true && j.User.SharedAccesses
     .Any(a => a.CareTeamMemberId == userId))
     .ToList();

            var sharedMoods = db.MoodLogs
                .Where(m => m.IsShared== true && m.User.SharedAccesses
                .Any(a => a.CareTeamMemberId == userId))
                .ToList();


            ViewBag.SharedJournals = sharedJournals;
            ViewBag.SharedMoods = sharedMoods;
            return View();
        }
    }
}
