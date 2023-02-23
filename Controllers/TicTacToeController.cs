using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Collections;

namespace Project_MMXXIII.Controllers {
    public class TicTacToeController : Controller {
        static Dictionary<string, GameInfo> games = new Dictionary<string, GameInfo>();
        static List<KeyValuePair<string, int>> lobbies = new List<KeyValuePair<string, int>>();

        public IActionResult Index() {
            ViewData["lobbies"] = lobbies;
            return View();
        }

        [HttpPost]
        public void CreateLobby(string id) {
            lobbies.Add(new KeyValuePair<string, int>(id, 0));
            Console.WriteLine("WTF");
            //return Redirect($"Game/{id}");
        }   

        public async Task<IActionResult> Game(string id) {
            if (lobbies.Where(e => e.Key == id).Count() >= 0) {
                Console.WriteLine("TUTAJ COŚ SIĘ DZIEJE I TO WAŻNE");
            }    


            ViewData["id"] = id;
            ViewData["symbol"] = 'o';
            GameInfo gameInfo = new GameInfo();

            if (!games.TryGetValue(id, out gameInfo)) {
                ViewData["symbol"] = 'x';
            }

            if (HttpContext.WebSockets.IsWebSocketRequest) {
                var symbol = 'o';
                if (!games.TryGetValue(id, out gameInfo)) {
                    gameInfo.Table = new char[3, 3];
                    gameInfo.Turn = new bool[1];
                    gameInfo.Turn[0] = true;
                    gameInfo.Finished = new bool[1];
                    gameInfo.Finished[0] = false;
                    gameInfo.SymbolWin = new char[1];
                    gameInfo.SymbolWin[0] = '\0';
                    gameInfo.Queues = new List<int>();
                    games.Add(id, gameInfo);
                    symbol = 'x';
                }
                else {
                    gameInfo = games[id];
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
    public struct GameInfo {
        public char[,] Table;
        public bool[] Turn;
        public bool[] Finished;
        public char[] SymbolWin;
        public List<int> Queues;
    }
}
