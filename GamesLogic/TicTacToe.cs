using Project_MMXXIII.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.Metrics;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace Project_MMXXIII.GamesLogic {
    public class TicTacToe {
        char symbol;
        GameInfo gameInfo;

        public TicTacToe(GameInfo gameInfo, char symbol) {
            this.symbol = symbol;
            this.gameInfo = gameInfo;
            Console.WriteLine(gameInfo.Turn);
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
            gameInfo.Counter[0]--;
            if (gameInfo.Counter[0] == 0) {
                //code to remove from lobbies center
                gameInfo.Counter[0] = LobbyControl.ToRemove;
            }
            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None
            ); ; ;
        }


        public async Task Notify(WebSocket webSocket) {
            var result = "";
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    result += $"x{i}, y{j} {gameInfo.Table[i, j]} ;";
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

                while (gameInfo.Finished[0] != true) {
                    if (((List<int>)arg)[count]! > 0) {
                        await Notify(webSocket);
                        ((List<int>)arg)[count]!--;
                    }
                    Thread.Sleep(100);
                }

                // After finished game.
                await Notify(webSocket);
                await SendMessage($"{gameInfo.SymbolWin[0]} win.", webSocket);

            }).Start(gameInfo.Queues);
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


            if ((gameInfo.Turn[0] && symbol == 'x') || (!gameInfo.Turn[0] && symbol == 'o')) {
                var xIndex = int.Parse(message.Substring(1, 1)) - 1;
                var yIndex = int.Parse(message.Substring(5, 1)) - 1;

                if (gameInfo.Table[xIndex, yIndex] == '\x00') {
                    gameInfo.Table[xIndex, yIndex] = symbol;

                    char symbolTemp = ' ';
                    if (Check(ref symbolTemp) && !gameInfo.Finished[0]) {
                        gameInfo.Finished[0] = true;
                        gameInfo.SymbolWin[0] = symbolTemp;
                    }

                    for (int i = 0; i < gameInfo.Queues.Count; i++) {
                        gameInfo.Queues[i]++;
                    }
                    gameInfo.Turn[0] = !gameInfo.Turn[0];
                }
                
            }
        }

        private void RestartGame(WebSocket webSocket) {
            // Clearing table, only one time, prevents restart when moves were already made by other player. 
            if (gameInfo.Finished[0]) {
                for (int i = 0; i < 3; i++) {
                    for (int j = 0; j < 3; j++) {
                        gameInfo.Table[i, j] = '\x00';
                    }
                }

                // Restart state
                gameInfo.Finished[0] = false;
                gameInfo.Turn[0] = true;    
            }
            
            SetUpNotifyingThread(webSocket);
        }

        private bool Check(ref char symbol) {
            //Checking rows
            for (int row = 0; row < 3; row++) {
                if ((gameInfo.Table[row, 0] == gameInfo.Table[row, 1]) && (gameInfo.Table[row, 1] == gameInfo.Table[row, 2]) && gameInfo.Table[row, 0] != 0) {
                    symbol = gameInfo.Table[row, 0];
                    return true;
                }
            }
            //Checking columns
            for (int column = 0; column < 3; column++)
            {
                if ((gameInfo.Table[0, column] == gameInfo.Table[1, column]) && (gameInfo.Table[1, column] == gameInfo.Table[2, column]) && gameInfo.Table[0, column] != 0) {
                    symbol = gameInfo.Table[0, column];
                    return true;
                }
            }
            //Checking diagonals
            if ((gameInfo.Table[0, 0] == gameInfo.Table[1, 1]) && (gameInfo.Table[1, 1] == gameInfo.Table[2, 2]) && gameInfo.Table[0, 0] != 0) {
                symbol = gameInfo.Table[0, 0];
                return true;
            }
            else if ((gameInfo.Table[2, 0] == gameInfo.Table[1, 1]) && (gameInfo.Table[1, 1] == gameInfo.Table[0, 2]) && gameInfo.Table[2, 0] != 0) {
                symbol = gameInfo.Table[2, 0];
                return true;
            }
            //Checking draws
            else if (gameInfo.Table.Cast<char>().All(c => c != '\x00')) {
                symbol = 'd';
                return true;
            }
            //Checked ✔️
            else {
                return false;
            }
                
        }
    }        
}
