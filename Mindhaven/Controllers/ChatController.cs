using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mindhaven.Service;

namespace Mindhaven.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        private readonly OpenAiChatService _chatService;

        public ChatController()
        {
            _chatService = new OpenAiChatService();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Default()
        {
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<JsonResult> Send(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return Json(new { success = false, reply = "Please enter a message." });

            string reply = await _chatService.GetResponseAsync(message);
            return Json(new { success = true, reply });
        }
    }
}
    