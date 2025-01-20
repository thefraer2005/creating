using Common;
using System.Net.Sockets;
using System.Text;

namespace UnoServer
{
    public class Player
    {
        public Socket Socket { get; }
        public string Nickname { get; set; }
        public List<Card> Hand { get; set; }
        public int Score { get; set; } 

        public Player(Socket socket)
        {
            Socket = socket;
            Nickname = "Игрок";
            Hand = new List<Card>();
            Score = 0; 
        }

        public void SendMessage(byte[] data)
        {
            try
            {
                Socket.Send(data);
                Console.WriteLine($"Сообщение отправлено на сокет для игрока {Nickname}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке данных игроку {Nickname}: {ex.Message}");
            }
        }

        public void AddScore(int points)
        {
            Score += points;
            Console.WriteLine($"Игрок {Nickname} получил {points} очков. Текущий счет: {Score}.");
        }

        public void ResetScore()
        {
            Score = 0;
            Console.WriteLine($"Счет игрока {Nickname} сброшен.");
        }
    }

}
