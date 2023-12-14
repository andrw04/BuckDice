using BuckDice.Domain.Models;

namespace BuckDice.Server
{
    public class Game
    {
        public Queue<Player> PlayerTurnOrder { get; set; }
        public bool GameCompleted { get; private set; } = false;
        public bool GameStarted { get; private set; } = false;
        public Player CurrentTurn { get => PlayerTurnOrder.Peek(); }

        private int Buck = 15;
        private int SmallBuck = 5;

        // "Очко" количество очков, которое нужно набрать для победы
        public int Point { get; set; } = 0;

        public void AddPlayer(Player player)
        {
            if (GameStarted)
                throw new Exception("Game already started!");

            PlayerTurnOrder.Enqueue(player);
        }

        public List<int> TurnRollTheDice()
        {
            if (!GameCompleted)
            {
                Random rand = new Random();

                List<int> rolls = new();

                // Подбрасывание трех кубиков
                for (int i = 0; i < 3; i++)
                {
                    rolls.Add(rand.Next(1, 6));
                }

                var player = PlayerTurnOrder.Peek();

                if (rolls.All(r => r.Equals(Point))) // Большой бак
                {
                    player.Points = Buck;
                }
                else if (rolls.All(r => r.Equals(rolls[0]) && !r.Equals(Point))) // малый бак
                {
                    player.Points += SmallBuck;
                }
                else
                {
                    player.Points += rolls.Where(r => r.Equals(Point)).Count();
                }

                PlayerTurnOrder.Dequeue();

                if (!CheckBuck(player))
                {
                    PlayerTurnOrder.Enqueue(player);
                }

                CheckEndGame();

                return rolls;
            }
            else
            {
                throw new Exception("Game is already completed!");
            }

        }

        private bool CheckBuck(Player player) => player.Points >= Buck;

        private bool CheckEndGame()
        {
            if (PlayerTurnOrder.Count == 1)
            {
                GameCompleted = true;
            }

            return GameCompleted;
        }

        public void StartGame()
        {
            if (!GameStarted)
            {
                Dictionary<string, int> rolls = new Dictionary<string, int>();
                Random rand = new Random();

                foreach (var player in PlayerTurnOrder)
                {
                    int rollSum = rand.Next(1, 6) + rand.Next(1, 6) + rand.Next(1, 6);

                    rolls.Add(player.Username, rollSum);
                }

                // Формируем порядок ходов игроков
                PlayerTurnOrder.OrderByDescending(p => rolls[p.Username]);

                // Определяем "Очко"
                Point = rand.Next(1, 6);

                // Игра началась
                GameStarted = true;
            }
            else
            {
                throw new Exception("Game already started!");
            }
        }
    }
}
