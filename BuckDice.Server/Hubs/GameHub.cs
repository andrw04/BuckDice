using Microsoft.AspNetCore.SignalR;

namespace BuckDice.Server.Hubs
{
    public class GameHub : Hub
    {
        private GameManager _gameManager;

        public GameHub(GameManager manager)
        {
            _gameManager = manager;
        }

        public async Task Register(string groupname, string username)
        {
            if (!string.IsNullOrEmpty(groupname) && !string.IsNullOrEmpty(username))
            {
                try
                {
                    await _gameManager.Register(groupname, username);

                    await Groups.AddToGroupAsync(Context.ConnectionId, groupname);

                    await Clients.Group(groupname).SendAsync("Notify", $"{username} is connected.");

                    if (_gameManager.IsStartedGame(groupname))
                    {
                        await Clients.Group(groupname).SendAsync("Notify", $"Game {groupname} is started.");

                        await Clients.Group(groupname).SendAsync("UpdateGameState",
                            _gameManager.IsStartedGame(groupname), _gameManager.IsCompletedGame(groupname),
                            _gameManager.GetPlayers(groupname), _gameManager.GetPoint(groupname));
                    }
                }
                catch (Exception ex)
                {
                    await Clients.Caller.SendAsync("Notify", ex.Message);
                }
            }
            else
            {
                await Clients.Caller.SendAsync("Notify", "Groupname or username is empty.");
            }
        }

        public async Task RollTheDice(string groupname)
        {
            if (!string.IsNullOrEmpty(groupname))
            {
                try
                {
                    var diceRolls = _gameManager.RollTheDice(groupname);

                    await Clients.Group(groupname).SendAsync("UpdateDice", diceRolls);

                    await Clients.Group(groupname).SendAsync("UpdateGameState",
                        _gameManager.IsStartedGame(groupname), _gameManager.IsCompletedGame(groupname),
                        _gameManager.GetPlayers(groupname), _gameManager.GetPoint(groupname));
                }
                catch (Exception ex)
                {
                    await Clients.Group(groupname).SendAsync("Notify", ex.Message);
                }
            }
        }
    }
}
