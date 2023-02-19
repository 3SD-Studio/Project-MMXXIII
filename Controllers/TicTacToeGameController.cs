using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace Project_MMXXIII.Controllers {
    public class TicTacToeGameController : Controller {
        static Dictionary<int, char[,]> games = new Dictionary<int, char[,]>();

        public IActionResult Index() {
            return View();
        }


        public async Task<IActionResult> Game(int id) {
            ViewData["id"] = id;

           
            if (HttpContext.WebSockets.IsWebSocketRequest) {
                char[,] new_table;
                char symbol = 'o';
                if (!games.TryGetValue(id, out new_table)) {
                    games.Add(id, new char[3, 3]);
                    symbol = 'x';
                }

                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                //await Project_MMXXIII.GamesLogic.TicTacToe.Echo(webSocket);
                await new GamesLogic.TTT(games[id], symbol).Echo(webSocket);
            } else {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            return View();
        }


        
    }
}
