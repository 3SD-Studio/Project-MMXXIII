using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Collections;
using System.Net;

namespace Project_MMXXIII.Controllers {
    public class TicTacToeController : Controller {
        static Dictionary<string, GameInfo> games = new Dictionary<string, GameInfo>();
        //static Dictionary<string, int> lobbies = new Dictionary<string, int>();

        public IActionResult Index() {
            var lobbiesToRemove = games.Where(e => e.Value.Counter[0] == -400)
                                       .Select(e => e.Key)
                                       .ToList();
            
            foreach (var id in lobbiesToRemove) {
                games.Remove(id);
            }


            ViewData["lobbies"] = games.Select(e =>
                new KeyValuePair<string, int>(e.Key, e.Value.Counter[0])
            );

            
            return View();
        }

        public async Task<IActionResult> Game(string id) {
            ViewData["id"] = id;
            ViewData["symbol"] = 'o';
            GameInfo gameInfo = new GameInfo();

            if (!games.TryGetValue(id, out gameInfo)) {
                ViewData["symbol"] = 'x';
            }
            else if (games[id].Counter[0] >= 2) {
                return Redirect("/tictactoe/");
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
                    gameInfo.Counter = new int[1];
                    gameInfo.Counter[0] = 1;
                    games.Add(id, gameInfo);
                    symbol = 'x';
                }
                else if (games[id].Counter[0] < 2){
                    games[id].Counter[0]++;
                }
                else {
                    return Redirect(nameof(Index));
                }

                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await new GamesLogic.TicTacToe(games[id], symbol).Echo(webSocket);
            } 
            else {
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
        public int[] Counter; 
    }
}
