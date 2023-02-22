using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Collections;

namespace Project_MMXXIII.Controllers {
    public class TicTacToeController : Controller {
        static Dictionary<string, char[,]> games = new Dictionary<string, char[,]>();
        static List<KeyValuePair<string, int>> lobbies = new List<KeyValuePair<string, int>>();

        public IActionResult Index() {
            ViewData["lobbies"] = lobbies;
            return View();
        }

        [HttpPost]
        public IActionResult CreateLobby(string id) {
            lobbies.Add(new KeyValuePair<string, int>(id, 0));
            Console.WriteLine("WTF");
            return Redirect($"Game/{id}");
        }   

        public async Task<IActionResult> Game(string id) {
            if (lobbies.Where(e => e.Key == id).Count() > 0) {
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
            }
            
            return View();
        }
    }
}
