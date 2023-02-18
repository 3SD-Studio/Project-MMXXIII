using Microsoft.AspNetCore.Mvc;
using Project_MMXXIII.Pages;

namespace Project_MMXXIII.Controllers {
    public class LobbyController : Controller {
        public IActionResult Index() {
            return View();
        }

        public IActionResult LobbyCenter() {
            ViewData["test"] = "hello";
            return View();
        }
    }
}
