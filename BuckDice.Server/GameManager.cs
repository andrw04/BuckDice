using BuckDice.Domain.Models;

namespace BuckDice.Server
{
    public class GameManager
    {
        private Dictionary<string, Game> _games { get; set; } // ключ = имя группы

        public GameManager()
        {
            _games = new Dictionary<string, Game>();
        }

        /// <summary>
        /// Добавляет игрока в группу
        /// </summary>
        /// <param name="groupname">Имя группы</param>
        /// <param name="username">Имя пользователя</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task Register(string groupname, string username)
        {
            if (!_games.ContainsKey(groupname))
            {
                _games.Add(groupname, new Game());
            }

            var game = _games[groupname];

            if (!game.GameStarted)
            {
                if (!game.PlayerTurnOrder.Select(p => p.Username).Contains(username))
                {
                    game.PlayerTurnOrder.Enqueue(new Player() { Username = username, Points = 0 });

                    if (game.MaxPlayers == game.PlayerTurnOrder.Count)
                    {
                        StartGame(groupname);
                    }
                }
                else
                {
                    throw new Exception("This user already exists.");
                }
            }
            else
            {
                throw new Exception("Game already started.");
            }
        }

        /// <summary>
        /// Возвращает список игроков и их счет
        /// </summary>
        /// <param name="groupname"></param>
        /// <returns></returns>
        public Queue<Player> GetPlayers(string groupname)
        {
            var game = _games[groupname];

            return game.PlayerTurnOrder;
        }

        /// <summary>
        /// Подбрасывание костей
        /// </summary>
        /// <param name="groupname">Имя группы</param>
        /// <returns></returns>
        public List<int> RollTheDice(string groupname)
        {
            var game = _games[groupname];

            if (!game.GameCompleted)
            {
                Random random = new Random();

                List<int> diceRolls = new List<int>();

                for (int i = 0; i < 3; i++)
                {
                    diceRolls.Add(random.Next(1, 6));
                }

                var player = game.PlayerTurnOrder.Peek();

                if (diceRolls.All(r => r.Equals(game.Point))) // Большой Buck
                {
                    player.Points = game.Buck;

                    DequeueBuck(groupname);
                }
                else if (diceRolls.All(r => r.Equals(diceRolls.First()) && !r.Equals(game.Point))) // Малый Buck
                {
                    player.Points += game.SmallBuck;

                    DequeueBuck(groupname);
                }
                else
                {
                    player.Points += diceRolls.Where(r => r.Equals(game.Point)).Count();

                    if (!DequeueBuck(groupname))
                    {
                        game.PlayerTurnOrder.Dequeue();

                        game.PlayerTurnOrder.Enqueue(player);
                    }
                }

                CheckEndGame(groupname);

                return diceRolls;
            }
            else
            {
                throw new Exception("Game is completed.");
            }
        }


        /// <summary>
        /// Проверяет, что игра начата
        /// </summary>
        /// <param name="groupname"></param>
        /// <returns></returns>
        public bool IsStartedGame(string groupname)
        {
            var game = _games[groupname];

            return game.GameStarted;
        }

        /// <summary>
        /// Проверяет, что игра окончена
        /// </summary>
        /// <param name="groupname"></param>
        /// <returns></returns>
        public bool IsCompletedGame(string groupname)
        {
            var game = _games[groupname];

            return game.GameCompleted;
        }

        public int GetPoint(string groupname)
        {
            var game = _games[groupname];

            return game.Point;
        }

        /// <summary>
        /// Удаляет игрока, если у него Buck
        /// </summary>
        /// <param name="groupname">Имя группы</param>
        /// <returns></returns>
        private bool DequeueBuck(string groupname)
        {
            var game = _games[groupname];

            var player = game.PlayerTurnOrder.Peek();

            if (player.Points == game.Buck)
            {
                game.PlayerTurnOrder.Dequeue();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Начинает игру
        /// </summary>
        /// <param name="groupname">Имя группы</param>
        /// <returns></returns>
        /// <exception cref="Exception">Игра уже начата</exception>
        private void StartGame(string groupname)
        {
            var game = _games[groupname];

            if (!game.GameStarted)
            {
                Dictionary<string, int> rolledDice = new Dictionary<string, int>();

                Random random = new Random();

                foreach(var player in game.PlayerTurnOrder)
                {
                    int sum = 0;

                    for(int i = 0; i < 3; i++)
                    {
                        sum += random.Next(1, 6);
                    }

                    rolledDice.Add(player.Username, sum);
                }

                game.PlayerTurnOrder.OrderByDescending(p => rolledDice[p.Username]);

                game.Point = random.Next(1, 6);

                game.GameStarted = true;
            }
            else
            {
                throw new Exception("Game already started.");
            }
        }

        /// <summary>
        /// Проверяет 
        /// </summary>
        /// <param name="groupname">Имя группы</param>
        /// <returns></returns>
        private bool CheckEndGame(string groupname)
        {
            var game = _games[groupname];

            if (game.PlayerTurnOrder.Count == 1)
            {
                game.GameCompleted = true;
            }

            return game.GameCompleted;
        }
    }
}
