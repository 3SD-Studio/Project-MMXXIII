using Project_MMXXIII.Controllers;
using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace Project_MMXXIII.GamesLogic {
    public class TicTacToe {
        char[,] table;
        static bool turn = true;
        char symbol;
        static bool finished = false;
        static char symbolWin = '\0';
        static List<int> notificationQueues = new List<int>();

        public TicTacToe(char[,] table, char symbol) {
            this.table = table;
            this.symbol = symbol;
        }

        public async Task Echo(WebSocket webSocket) {

            SetUpNotifyingThread(webSocket);

            // Buffer needed for websocet.ReciveAsync method, used for storing data. 
            var buffer = new byte[1024 * 4];

            // Data from user
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None
            );

            if (receiveResult.MessageType == WebSocketMessageType.Text) {
                ProcessRecievedMessage(buffer, receiveResult.Count, webSocket);
            }

            while (!receiveResult.CloseStatus.HasValue) {
                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None
                );

                if (receiveResult.MessageType == WebSocketMessageType.Text) {
                    ProcessRecievedMessage(buffer, receiveResult.Count, webSocket);
                }
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None
            );
        }


        public async Task Notify(WebSocket webSocket) {
            var result = "";
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    result += $"x{i}, y{j} {table[i, j]} ;";
                }
            }

            await SendMessage(result, webSocket);
        }


        private void SetUpNotifyingThread(WebSocket webSocket) {
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
        }

        private async Task SendMessage(string message, WebSocket webSocket) {
            var response = Encoding.Default.GetBytes(message);

            if (webSocket.State != WebSocketState.Open) { return; }
            await webSocket.SendAsync(
                new ArraySegment<byte>(response, 0, response.Length),
                WebSocketMessageType.Text,
                WebSocketMessageFlags.EndOfMessage,
                CancellationToken.None
            );
        }


        private void ProcessRecievedMessage(byte[] receivedMessage, int receivedMessageLength, WebSocket webSocket) {
            var message = Encoding.Default.GetString(receivedMessage, 0, receivedMessageLength);
            Console.WriteLine(message);
            
            // Restart logic
            if (message == "restart") {
                RestartGame(webSocket);
                return;
            }


            if ((turn && symbol == 'x') || (!turn && symbol == 'o')) {
                var xIndex = int.Parse(message.Substring(1, 1)) - 1;
                var yIndex = int.Parse(message.Substring(5, 1)) - 1;

                if (table[xIndex, yIndex] == '\x00') {
                    table[xIndex, yIndex] = symbol;

                    char symbolTemp = ' ';
                    if (Check(ref symbolTemp) && !finished) {
                        finished = true;
                        symbolWin = symbolTemp;
                    }

                    for (int i = 0; i < notificationQueues.Count; i++) {
                        notificationQueues[i]++;
                    }
                }
                turn = !turn;
            }
        }

        private void RestartGame(WebSocket webSocket) {
            // Clearing table, only one time, prevents restart when moves were already made by other player. 
            if (finished) {
                for (int i = 0; i < 3; i++) {
                    for (int j = 0; j < 3; j++) {
                        table[i, j] = '\x00';
                    }
                }

                // Restart state
                finished = false;
                turn = true;    
            }
            
            SetUpNotifyingThread(webSocket);
        }

        private bool Check(ref char symbol) {
            //Checking rows
            for (int row = 0; row < 3; row++)
            {
                if ((table[row, 0] == table[row, 1]) && (table[row, 1] == table[row, 2]) && table[row, 0] != 0)
                {
                    symbol = table[row, 0];
                    return true;
                }
            }
            //Checking columns
            for (int column = 0; column < 3; column++)
            {
                if ((table[0, column] == table[1, column]) && (table[1, column] == table[2, column]) && table[0, column] != 0)
                {
                    symbol = table[0, column];
                    return true;
                }
            }
            //Checking diagonals
            if ((table[0, 0] == table[1, 1]) && (table[1, 1] == table[2, 2]) && table[0, 0] != 0)
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
