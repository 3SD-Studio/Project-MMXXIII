using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var webSocketsOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(100)
};

app.UseWebSockets(webSocketsOptions);

app.Use(async (context, next) => {
    if (context.Request.Path == "/ws") {
        if (context.WebSockets.IsWebSocketRequest) {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await Echo(webSocket);
        } 
        else {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    } 
    else {
        await next(context);
    }

});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();


static async Task Echo(WebSocket webSocket) {
    var buffer = new byte[1024 * 4];
    var receiveResult = await webSocket.ReceiveAsync(
        new ArraySegment<byte>(buffer), CancellationToken.None);

    Console.WriteLine(receiveResult.ToString());

    while (!receiveResult.CloseStatus.HasValue)
    {
        Console.WriteLine(buffer);
        await webSocket.SendAsync(
            new ArraySegment<byte>(buffer, 0, receiveResult.Count),
            receiveResult.MessageType,
            receiveResult.EndOfMessage,
            CancellationToken.None);

        receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);
    }

    await webSocket.CloseAsync(
        receiveResult.CloseStatus.Value,
        receiveResult.CloseStatusDescription,
        CancellationToken.None);
}