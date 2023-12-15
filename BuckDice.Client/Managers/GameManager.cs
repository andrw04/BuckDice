using BuckDice.Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BuckDice.Client.Managers
{
    public class GameManager : IAsyncDisposable
    {
        public event Action GameStateChanged;
        public HubConnection? hubConnection { get; set; }
        public string Username { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public bool GameStarted { get; set; } = false;
        public bool GameCompleted { get; set; } = false;
        public bool GameEnd { get; set; } = false;
        public List<string> Messages { get; set; } = new List<string>();
        public Queue<Player> Players { get; set; } = new Queue<Player>();
        public List<int> DiceRolls { get; set; } = new();
        public int Point { get; set; }


        public async Task StartAsync(NavigationManager navigationManager)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("https://localhost:7078/buckdice"))
                .Build();

            hubConnection.On<string>("Notify", (message) =>
            {
                Messages.Insert(0, message);
                GameStateChanged?.Invoke();
            });

            hubConnection.On<bool, bool, Queue<Player>, int>("UpdateGameState", (isStartedGame, isCompletedGame, players, point) =>
            {
                GameStarted = isStartedGame;
                GameCompleted = isCompletedGame;
                Players = players;
                Point = point;
                GameStateChanged?.Invoke();
            });

            hubConnection.On<List<int>>("UpdateDice", (diceRolls) =>
            {
                DiceRolls = diceRolls;
                GameStateChanged?.Invoke();
            });

            await hubConnection.StartAsync();
        }


        public async Task RollTheDice()
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync("RollTheDice", GroupName);
                GameStateChanged?.Invoke();
            }
        }


        public async Task ConnectGroup()
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync("Register", GroupName, Username);
                GameStateChanged?.Invoke();
            }
        }


        public bool IsConnected { get => hubConnection?.State == HubConnectionState.Connected; }


        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }
    }
}
