using BuckDice.Domain.Models;
using System.Drawing;

namespace BuckDice.Server
{
    public class Game
    {
        public Queue<Player> PlayerTurnOrder { get; set; }
        public bool GameCompleted { get; } = false;
        public bool GameStarted { get; } = false;
        public Player CurrentTurn { get => PlayerTurnOrder.Peek(); }

        // "Очко" количество очков, которое нужно набрать
        public int Point { get; set; } = 0;

        public void StartGame()
        {
            if (GameStarted)
                throw new Exception("Game already started!");

            if (GameCompleted)
                throw new Exception("Game already completed!");

            if (Point == 0)
                throw new Exception("Point is undefined!");


        }

        public void SetTurnOrder()
        {

        }

        // Последний игрок бросает одну кость и это и есть "Очко"
        public void GetPoint(Player player, RollOneDice roll)
        {
            var lastPlayerUsername = PlayerTurnOrder.Last().Username;

            if (lastPlayerUsername == player.Username)
            {
                Point = roll.Dice;
            }
        }




/*        public void AddPlayer(string username)
        {
            if (!GameStarted)
            {
                PlayerTurnOrder.Enqueue(new Player() { Username = username, Points = 0 });
            }
            else
            {
                throw new Exception("Game already started!");
            }
        }

        public void StartGame()
        {
            if (GameStarted)
            {
                throw new Exception("Game already started!");
            }

            if (PlayerTurnOrder.All(p => p.Points > 0))
            {
                GameStarted = true;

                var players = PlayerTurnOrder.ToArray();

                PlayerTurnOrder = new Queue<Player>(players
                    .OrderByDescending(player => player.Points));

                foreach (var player in players)
                {
                    player.Points = 0;
                }
            }
            else
            {
                throw new Exception("Not all players rolled the dice!");
            }
        }

        public void RollTheDice(int points)
        {
            if (!GameCompleted)
            {
                // Получаем текущего игрока
                var currentPlayer = PlayerTurnOrder.Dequeue();

                // Прибавляем его результат
                currentPlayer.Points += points;

                // Возвращаем в конец очереди
                PlayerTurnOrder.Enqueue(currentPlayer);
            }
        }*/

        // получить список игроков, отсортированный по имени
        public List<Player> GetPlayerScores()
        {
            return PlayerTurnOrder.OrderBy(player => player.Username).ToList();
        }
    }
}
