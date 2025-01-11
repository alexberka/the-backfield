using Microsoft.AspNetCore.SignalR;
using TheBackfield.DTOs.GameStream;
using TheBackfield.Interfaces;

namespace TheBackfield.Data
{
    public class WatchGame : Hub<IWatchClient>
    {
        private readonly IGameStreamService _gameStreamService;
        public WatchGame(IGameStreamService gameStreamService)
        {
            _gameStreamService = gameStreamService;
        }
        public override async Task OnConnectedAsync()
        {
            int.TryParse(Context.GetHttpContext()?.Request.Query["gameId"], out int gameId);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"watch-{gameId}");

            await Clients.Group($"watch-{gameId}").SayHello($"Greetings {Context.ConnectionId}, you're watching {gameId}!");
        }
        public async Task Report(int gameId)
        {
            GameStreamDTO? gameStream = await _gameStreamService.GetGameStreamAsync(gameId);
            if (gameStream != null)
            {
                await Clients.Group($"watch-{gameId}").UpdateGameStream(gameStream);
            }
        }
    }
}
