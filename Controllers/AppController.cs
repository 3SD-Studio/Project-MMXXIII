using Microsoft.AspNetCore.Mvc;
using Project_MMXXIII.Pages;

namespace Project_MMXXIII.Controllers {
    public class AppController : Controller {
        public IActionResult Index() {
            return View();
        }

        public IActionResult TicTacToe() {
            return View();
        }
    }
}
