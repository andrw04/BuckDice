using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuckDice.Domain.Models
{
    public class Game
    {
        public Queue<Player> PlayerTurnOrder { get; set; } = new();
        public bool GameCompleted { get; set; } = false;
        public bool GameStarted { get; set; } = false;
        public Player CurrentTurn { get; set; } = null;
        public int Point { get; set; } = 0;
        public int MaxPlayers { get; } = 3;
        public int Buck { get; set; } = 15;
        public int SmallBuck { get; set; } = 5;
    }
}
