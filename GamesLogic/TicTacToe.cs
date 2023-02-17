using Project_MMXXIII.Controllers;
using System.Collections;
using System.Data.SqlTypes;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace Project_MMXXIII.GamesLogic {
    public class TicTacToe {
        static char[,] table = new char[3, 3];
        static bool turn = true;
        static Thread? notifyThread;
        static bool finished = false;
        static char symbolWin = '\0';
        static List<int> notificationQueues = new List<int>();

        public static async Task Echo(WebSocket webSocket) {
            
            // Thread notifying players about changes in game status. 
            notifyThread = new Thread(async (arg) => {
                await Notify(webSocket);
                var count = (arg as List<int>)!.Count;
                ((List<int>)arg).Add(0);
                while (finished != true) {
                    //await Notify(webSocket);
                    //Thread.Sleep(100);
                    if (((List<int>)arg)[count]! > 0) {
                        await Notify(webSocket);
                        ((List<int>)arg)[count]!--;
                    }
                    Thread.Sleep(300);
                }

                await Notify(webSocket);
                await sendMessage($"{symbolWin} win.", webSocket);
                //var winningMessage = Encoding.Default.GetBytes($"{symbolWin} win.");
                //await webSocket.SendAsync(
                //    new ArraySegment<byte>(winningMessage, 0, winningMessage.Length),
                //    WebSocketMessageType.Text,
                //    WebSocketMessageFlags.EndOfMessage,
                //    CancellationToken.None
                //);
            });
            notifyThread.Start(notificationQueues);

            // Buffer needed for websocet.ReciveAsync method, used for storing data. 
            var buffer = new byte[1024 * 4];

            // Data fri
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None
            );


            var response = Encoding.Default.GetBytes("Something went wrong...");
            if (receiveResult.MessageType == WebSocketMessageType.Text) {
                var str = Encoding.Default.GetString(buffer, 0, receiveResult.Count);
                var temp = turn ? 'x' : 'o';
                turn = !turn;

                table[int.Parse(str.Substring(1, 1)) - 1, 
                    int.Parse(str.Substring(5, 1)) - 1] = temp;
                
                for (int i = 0; i < notificationQueues.Count; i++) {
                    Console.WriteLine(notificationQueues.Count);
                    notificationQueues[i]++;
                }
            }

            while (!receiveResult.CloseStatus.HasValue) {
                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None
                );

                response = Encoding.Default.GetBytes("Something went wrong...");
                if (receiveResult.MessageType == WebSocketMessageType.Text)
                {
                    var str = Encoding.Default.GetString(buffer, 0, receiveResult.Count);
                    var temp = turn ? 'x' : 'o';
                    turn = !turn;

                    var xIndex = int.Parse(str.Substring(1, 1)) - 1;
                    var yIndex = int.Parse(str.Substring(5, 1)) - 1;

                    if (table[xIndex, yIndex] == '\x00') {
                        table[xIndex, yIndex] = temp;

                        char symbol = ' ';
                        if (Check(ref symbol) && !finished) {
                            finished = true;
                            symbolWin = symbol;
                        }

                        for (int i = 0; i < notificationQueues.Count; i++) {
                            notificationQueues[i]++;
                        }
                    }
                }
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None
            );
        }


        public static async Task Notify(WebSocket webSocket) {
            var result = "";
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    result += $"x{i}, y{j} {table[i, j]} ;";
                }
            }

            //var response = Encoding.Default.GetBytes(result);

            //await webSocket.SendAsync(
            //    new ArraySegment<byte>(response, 0, response.Length),
            //    WebSocketMessageType.Text,
            //    WebSocketMessageFlags.EndOfMessage,
            //    CancellationToken.None
            //);

            await sendMessage(result, webSocket);
        }

        private static async Task sendMessage(string message, WebSocket webSocket) {
            var response = Encoding.Default.GetBytes(message);

            await webSocket.SendAsync(
                new ArraySegment<byte>(response, 0, response.Length),
                WebSocketMessageType.Text,
                WebSocketMessageFlags.EndOfMessage,
                CancellationToken.None
            );
        }

        private static bool Check(ref char symbol) {
            //Checking rows
            if ((table[0, 0] == table[1, 0]) && (table[1, 0] == table[2, 0]) && table[0, 0] != 0) {
                symbol = table[0, 0];
                return true;
            }
            else if((table[0, 1] == table[1, 1]) && (table[1, 1] == table[2, 1]) && table[0, 1] != 0)
            {
                symbol = table[0, 1];
                return true;
            }
            else if ((table[0, 2] == table[1, 2]) && (table[1, 2] == table[2, 2]) && table[0, 2] != 0)
            {
                symbol = table[0, 2];
                return true;
            }
            //Checking columns
            else if ((table[0, 0] == table[0, 1]) && (table[0, 1] == table[0, 2]) && table[0, 0] != 0)
            {
                symbol = table[0, 0];
                return true;
            }
            else if ((table[1, 0] == table[1, 1]) && (table[1, 1] == table[1, 2]) && table[1, 0] != 0)
            {
                symbol = table[1, 0];
                return true;
            }
            else if ((table[2, 0] == table[2, 1]) && (table[2, 1] == table[2, 2]) && table[2, 0] != 0)
            {
                symbol = table[2, 0];
                return true;
            }
            //Checking diagonals
            else if ((table[0, 0] == table[1, 1]) && (table[1, 1] == table[2, 2]) && table[0, 0] != 0)
            {
                symbol = table[0, 0];
                return true;
            }
            else if ((table[2, 0] == table[1, 1]) && (table[1, 1] == table[0, 2]) && table[2, 0] != 0)
            {
                symbol = table[2, 0];
                return true;
            }
            //Checking draws
            else if (table.Cast<char>().All(c => c != '\x00'))
            {
                symbol = 'd';
                return true;
            }
            //Checked ✔️
            else
                return false;
        }
    }        
}
