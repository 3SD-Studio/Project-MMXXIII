using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Collections;

namespace Project_MMXXIII.Controllers {
    public class TicTacToeController : Controller {
        static Dictionary<int, char[,]> games = new Dictionary<int, char[,]>();

        public IActionResult Index() {
            return View();
        }

        public async Task<IActionResult> Game(int id) {
            ViewData["id"] = id;
            ViewData["symbol"] = 'o';
            char[,] new_table;

            if (!games.TryGetValue(id, out new_table)) {
                ViewData["symbol"] = 'x';
            }

            if (HttpContext.WebSockets.IsWebSocketRequest) {
                char symbol = 'o';
                if (!games.TryGetValue(id, out new_table)) {
                    games.Add(id, new char[3, 3]);
                    symbol = 'x';
                    
                }

                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                //await Project_MMXXIII.GamesLogic.TicTacToe.Echo(webSocket);
                await new GamesLogic.TicTacToe(games[id], symbol).Echo(webSocket);
            } else {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            return View();
        }
    }
}
