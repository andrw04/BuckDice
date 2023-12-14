using BuckDice.Server.Models;
using Microsoft.AspNetCore.SignalR;

namespace BuckDice.Server.Hubs
{
    public class GameHub : Hub
    {
        private Dictionary<string, Game> _games { get; set; } // ключ = имя группы

        public async Task Enter(string username, string groupname)
        {
            if (!String.IsNullOrEmpty(username))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupname);

/*                if (!_games.ContainsKey(groupname))
                {
                    _games.Add(groupname, new Game());
                    _games[groupname].AddPlayer(username);
                }*/

                await Clients.Group(groupname).SendAsync("Notify", $"{username} connected");
            }
        }
    }


    /*public class GameHub : Hub
    {
        List<PlayersGroup> _groups { get; set; } = new();
        public GameHub()
        {
            _groups.Add(new PlayersGroup("Gr 1"));
        }

        public async Task SendGroupList()
        {
            await Clients.Caller.SendAsync("RecieveGroupList", _groups.Select(gr => gr.GroupName));
        }

        public async Task Enter(string groupName, string username)
        {
            var group = _groups.FirstOrDefault(x => x.GroupName == groupName);

            if (group != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                await Clients.Caller.SendAsync("Notify", $"Connected to group {groupName}");

                group.Players.Add(username);
            }
        }

        public async Task CreateGroup()
        {
            _groups.Add(new PlayersGroup($"Game {_groups.Count() + 1}"));
        }
    }*/
}
