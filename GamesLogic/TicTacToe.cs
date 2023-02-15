using System.Data.SqlTypes;
using System.Net.WebSockets;

namespace Project_MMXXIII.GamesLogic {
    public class TicTacToe {
        static char[,] table = new char[3, 3];
        static bool turn = true;

        public static async Task Update(WebSocket webSocket) {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);


            var response = System.Text.Encoding.Default.GetBytes("Something went wrong...");
            if (receiveResult.MessageType == WebSocketMessageType.Text)
            {
                var str = System.Text.Encoding.Default.GetString(buffer, 0, receiveResult.Count);
                var temp = turn ? 'x' : 'o';
                turn = !turn;

                table[int.Parse(str.Substring(1, 1)), int.Parse(str.Substring(5, 1))] = temp;

                
                
                str += $" {temp}";

                response = System.Text.Encoding.Default.GetBytes(str);
            }


            while (!receiveResult.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(
                    new ArraySegment<byte>(response, 0, response.Length),
                    receiveResult.MessageType,
                    receiveResult.EndOfMessage,
                    CancellationToken.None
                );

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None
                );

                response = System.Text.Encoding.Default.GetBytes("Something went wrong...");
                if (receiveResult.MessageType == WebSocketMessageType.Text)
                {
                    var str = System.Text.Encoding.Default.GetString(buffer, 0, receiveResult.Count);
                    var temp = turn ? 'x' : 'o';
                    turn = !turn;

                    table[int.Parse(str.Substring(1, 1)) - 1, int.Parse(str.Substring(5, 1)) - 1] = temp;

                    var result = "";
                    for (int i = 0; i < 3; i++) { 
                        for (int j = 0; j < 3; j++) {
                            result += $"x{i}, y{j} {table[i, j]} ;";
                        }
                    }


                    str += $" {temp}";
                    Console.WriteLine(result);

                    response = System.Text.Encoding.Default.GetBytes(result);
                }
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None
            );
        }
    }
}
