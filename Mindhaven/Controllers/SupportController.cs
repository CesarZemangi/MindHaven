using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mindhaven.Models;

namespace Mindhaven.Controllers
{
    public class SupportController : Controller
    {
        // GET: Support/HelpNow
        private readonly mindhavenDBEntities1 db = new mindhavenDBEntities1();

        public ActionResult HelpNow()
        {
            var emergencyContacts = db.EmergencyContacts.ToList();  new List<EmergencyContact>();
            return View(emergencyContacts);
        }
    }
}