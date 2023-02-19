using Microsoft.AspNetCore.Mvc;

namespace Project_MMXXIII.Controllers {
    public class RockPaperScissorsController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
