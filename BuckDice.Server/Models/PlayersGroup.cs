namespace BuckDice.Server.Models
{
    public class PlayersGroup
    {
        public string GroupName { get; set; }
        public List<string> Players { get; set; } = new();

        public PlayersGroup(string groupName)
        {
            GroupName = groupName;
        }
    }
}
