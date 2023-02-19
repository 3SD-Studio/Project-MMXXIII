using Microsoft.AspNetCore.Mvc;

namespace Project_MMXXIII.Controllers {
    public class TicTacToeController : Controller {
        public IActionResult Index() {
            return View();
        }

        [Route("/Game/{id?}")]
        public IActionResult Game(int id) {
            ViewData["id"] = id;
            return View();
        } 


    }
}
