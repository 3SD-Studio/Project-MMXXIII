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


// Handles websocket requests.
app.Use(async (context, next) => {
    if (context.Request.Path == "/ws") {
        if (context.WebSockets.IsWebSocketRequest) {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            // Function that handle network ang game logic for tic tac toe, 
            // Class located in GamesLogic folder
            await Project_MMXXIII.GamesLogic.TicTacToe.Echo(webSocket);
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
