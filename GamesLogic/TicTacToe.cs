using Project_MMXXIII.Controllers;
using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace Project_MMXXIII.GamesLogic {
    public class TicTacToe {
        static char[,] table = new char[3, 3];
        static bool turn = true;
        static bool finished = false;
        static char symbolWin = '\0';
        static List<int> notificationQueues = new List<int>();

        public static async Task Echo(WebSocket webSocket) {
            
            // Thread notifying players about changes in game status. 
            new Thread(async (arg) => {
                // Sends initialized board
                await Notify(webSocket);

                // Adds int item to list, that conatins number of not updated moves for each player. 
                var count = (arg as List<int>)!.Count;
                ((List<int>)arg).Add(0);

                while (finished != true) {
                    if (((List<int>)arg)[count]! > 0) {
                        await Notify(webSocket);
                        ((List<int>)arg)[count]!--;
                    }
                    Thread.Sleep(100);
                }

                // After finished game. 
                await Notify(webSocket);
                await SendMessage($"{symbolWin} win.", webSocket);

            }).Start(notificationQueues);


            // Buffer needed for websocet.ReciveAsync method, used for storing data. 
            var buffer = new byte[1024 * 4];

            // Data from user
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None
            );

            if (receiveResult.MessageType == WebSocketMessageType.Text) {
                ProcessRecievedMessage(buffer);
            }

            while (!receiveResult.CloseStatus.HasValue) {
                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None
                );

                if (receiveResult.MessageType == WebSocketMessageType.Text) {
                    ProcessRecievedMessage(buffer);
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

            await SendMessage(result, webSocket);
        }


        private static async Task SendMessage(string message, WebSocket webSocket) {
            var response = Encoding.Default.GetBytes(message);

            await webSocket.SendAsync(
                new ArraySegment<byte>(response, 0, response.Length),
                WebSocketMessageType.Text,
                WebSocketMessageFlags.EndOfMessage,
                CancellationToken.None
            );
        }


        private static void ProcessRecievedMessage(byte[] receivedMessage) {
            var str = Encoding.Default.GetString(receivedMessage, 0, receivedMessage.Length);
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
