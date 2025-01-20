
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

using Common;
using System.Threading;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;

namespace UnoServer
{
    public enum CardColor
    {
        Red,
        Green,
        Blue,
        Yellow
    }
    public class GameServer
    {
        private List<Player> players = new List<Player>();
        List<string> actions = new List<string>();
        private Socket serverSocket;
        private Deck deck; 
        private bool gameStarted = false;
  
        private string turnPlayer;

        private Card centralCard;
        private bool UnoFlag = false;
        
        private int playerInit=0;
        private int playerInits=0;
        public void Start()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 12345));
            serverSocket.Listen(10);

            Console.WriteLine("Сервер запущен...");

            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                Player player = new Player(clientSocket);
                 players.Add(player);

                Console.WriteLine($"Игрн: {player.Nickname}");

                Thread clientThread = new Thread(() => HandleClient(player));
                clientThread.Start();
                

            }
        }
        private void HandleClient(Player player)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = player.Socket.Receive(buffer);

                    var packet = Packet.FromBytes(buffer.Take(bytesRead).ToArray());

                    Console.WriteLine($"Игрок добавлен: {player.Nickname}");
                    Console.WriteLine($"Получено сообщение от игрока {player.Nickname}: Команда - {packet.Command}");

                    ProcessCommand(player, packet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                player.Socket.Close();
                players.Remove(player);
                if (gameStarted)
                {
                    gameStarted = false;

                }
            }
        }

        private void ProcessCommand(Player player, Packet packet)
        {
            switch (packet.Command)
            {
                case Protocol.Connect:
                   
                    player.Nickname = System.Text.Encoding.UTF8.GetString(packet.Data);
                    Console.WriteLine($"игрок добавлавыен {player.Nickname}");
                   
                    break;
                case Protocol.StartGame:
                    playerInit += 1;
                    if (playerInit == 2)
                    {
                        StartGame();
                       
                    }
                    break;
                case Protocol.PlayCard:
                    HandlePlayCard(player, packet);
                    break;
                case Protocol.UNO:
                    actions.Add("uno");
                    UnoFlag = true;
                    break;
                case Protocol.GiveCard:
                    GiveCard(player);
                    break;
                case Protocol.NewRound:
                    playerInits += 1;
                    if(playerInits == 2)
                    {
                        StartGame();
                       
                    }
                     
                    
                   
                    break;





                default:
                    
                    break;
            }
        }
        private void CheckStartGame(byte[] data)
        {
            int playerCount = BitConverter.ToInt32(data, 0);
            Console.WriteLine($"ayoue countood {playerCount}");
            Console.WriteLine($"acount {players.Count}");

        

            if ( players.Count == playerCount)
            {
                Console.WriteLine("alles good");

               
                StartGame();
            }
        }
        private void HandlePlayCard(Player player, Packet packet)
        {
            var cardUpdater = new CardUpdater();
            int currentPlayerIndex = players.FindIndex(player => player.Nickname == turnPlayer);
            int previousPlayerIndex = (currentPlayerIndex - 1 + players.Count) % players.Count; // Определяем предыдущего игрока
            Player previousPlayer = players[previousPlayerIndex];
            Console.WriteLine($"не count cars {previousPlayer.Hand.Count}.");
            Console.WriteLine($"index {previousPlayerIndex}----{currentPlayerIndex}.");




            string jsonData = Encoding.UTF8.GetString(packet.Data);
            var cardData = JsonConvert.DeserializeObject<Card>(jsonData);
            Console.WriteLine($"Card received - Id: {cardData.Id}, Value: {cardData.Value}, Color: {cardData.Color}");
            if (!UnoFlag&& players[previousPlayerIndex].Hand.Count == 1)
            {
                var (updatedHandUno, previosIndexs) = cardUpdater.NotUno(turnPlayer, players, 2, deck);
                Console.WriteLine($"не sdfghddddfуспел.");

                players[previosIndexs].Hand = updatedHandUno;
                var updatePacket = new Packet
                {
                    Command = Protocol.UNO,

                };

                byte[] packetData = updatePacket.ToBytes();


                foreach (var p in players)
                {
                    try
                    {
                        p.SendMessage(packetData);
                        Console.WriteLine($"не успел.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при отправкеоку {p.Nickname}: {ex.Message}");
                    }
                }


            }
            UnoFlag = false;
            Console.WriteLine($"не count cars {previousPlayer.Hand.Count}.");

            if (IsValidMove(cardData, centralCard))
            {
                Card playerCard = player.Hand.FirstOrDefault(c => c.Id == cardData.Id);
                if (playerCard != null)
                {
                    playerCard.Color = cardData.Color;
                }
                // Удаляем карту из руки игрока
                UpdatePlayerCards(player, cardData.Id, false);

                // Обновляем центральную карту


                // Уведомляем других игроков о ходе
                Console.WriteLine($"{player.Nickname} сыграл карту: {cardData}");
            }
            else
            {
                // Если карта не подходит, уведомляем игрока
                Console.WriteLine("Неверный ход! Карта не соответствует центральной.");
            }
        }


        private void DisplayPlayerHand(Player player)
        {
            Console.WriteLine($"Игрок {player.Nickname} имеет следующие карты:");
            foreach (var card in player.Hand)
            {
                Console.WriteLine(card.ToString());
            }
            Console.WriteLine();
        }
        private void StartGame()
        {
            playerInit = 0;
            playerInits = 0;
            gameStarted = true;
            deck = new Deck(); 
            deck.Shuffle();
            centralCard = deck.Draw();
            Console.WriteLine($"Central card :{centralCard}");

            var gameStartInfo = new GameStartInfo();

            foreach (var player in players)
            {
                List<Card> hand = new List<Card>();
                for (int i = 0; i < 7; i++)
                {
                    Card drawnCard = deck.Draw(); 
                    hand.Add(drawnCard);
                }
                player.Hand.Clear();
                player.Hand = hand;
               
                gameStartInfo.PlayerHands[player.Nickname] = player.Hand; 
                DisplayPlayerHand(player);
            }
            
            turnPlayer = players[0].Nickname;
           
            Console.WriteLine(turnPlayer);
            gameStartInfo.FirstPlayerNickname = turnPlayer;
            gameStartInfo.FirstCard = centralCard;
            Console.WriteLine(centralCard.Color);
            string jsonData = JsonConvert.SerializeObject(gameStartInfo);
            byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);

            var startPacket = new Packet
            {
                Command = Protocol.StartGame,
                Data = dataBytes
            };

            byte[] packetData = startPacket.ToBytes();

            foreach (var player in players)
            {
                try
                {
                    player.SendMessage(packetData);
                    Console.WriteLine($"Сообщение отправлено игроку {player.Nickname}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при отправке сообщения игроку {player.Nickname}: {ex.Message}");
                }
            }
        }

        

        private bool IsValidMove(Card card, Card centralCard)
        {
            if (card.Type == CardType.Wild || card.Type == CardType.WildDrawFour)
            {
                return true;
            }

            return card.Color == centralCard.Color || card.Value == centralCard.Value;
        }
       


        private void GiveCard(Player player)
        {
            if (deck.Cards.Count > 0)
            {
                try
                {
                    Card drawnCard = deck.Draw();

                    player.Hand.Add(drawnCard);
                    Console.WriteLine($"{player.Nickname} получил кnnарту {drawnCard}.");

                    UpdatePlayerCards(player, drawnCard.Id, true);
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Ошибка при выдаче карты игроку {player.Nickname}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Колода пуста! Невозможно выдать карту игроку {player.Nickname}.");
            }
        }
        public static int CalculateTotalPoints(List<Card> cards)
        {
            int totalPoints = 0;

            foreach (var card in cards)
            {
                switch (card.Type)
                {
                    case CardType.Number:
                        totalPoints += int.Parse(card.Value); 
                        break;
                    case CardType.Skip:
                    case CardType.Reverse:
                    case CardType.DrawTwo:
                        totalPoints += 20;
                        break;
                    case CardType.Wild:
                    case CardType.WildDrawFour:
                        totalPoints += 50;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Неизвестный тип карты: {card.Type}");
                }
            }

            return totalPoints;
        }
        private void CalculateTotalPointsWinner()
        {
            Console.WriteLine($"Starting CalculateTotalPointsWinner");

            int totalPoints = 0;
            string winnerNickname = null;

            foreach (var player in players)
            {
                if (player.Hand.Count == 0)
                {
                    winnerNickname = player.Nickname;
                    
                }
                totalPoints += CalculateTotalPoints(player.Hand);
            }

            if (winnerNickname != null && totalPoints > 0)
            {
                Console.WriteLine($"Winner found: {winnerNickname} with total points: {totalPoints}");

                foreach (var player in players)
                {
                    if (player.Nickname == winnerNickname)
                    {
                        player.Score += totalPoints;
                        Console.WriteLine($"Player {player.Nickname} score updated to {player.Score}");
                    }
                   
                }

                bool victoryAchieved = false;
                foreach (var player in players)
                {
                    if (player.Score >= 500)
                    {
                        var winnerInfo = new WinnerInfo
                        {
                            WinnerNickname = winnerNickname,
                            TotalPoints = player.Score
                        };
                        Console.WriteLine($"Victory achieved by {winnerInfo.WinnerNickname} with score {winnerInfo.TotalPoints}");

                        string jsonData = JsonConvert.SerializeObject(winnerInfo);
                        byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);

                        var updatePacket = new Packet
                        {
                            Command = Protocol.Victory,
                            Data = dataBytes
                        };

                        byte[] packetData = updatePacket.ToBytes();
                        Broadcast(packetData);
                        victoryAchieved = true;
                        break;
                    }
                }

                if (!victoryAchieved)
                {
                    var winnerInfo = new WinnerInfo
                    {
                        WinnerNickname = winnerNickname,
                        TotalPoints = totalPoints
                    };
                    Console.WriteLine($"No victory. Sending round info for {winnerInfo.WinnerNickname} with total points {totalPoints}");
                    string jsonData = JsonConvert.SerializeObject(winnerInfo);
                    byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);

                    var updatePacket = new Packet
                    {
                        Command = Protocol.Round,
                        Data = dataBytes
                    };

                    byte[] packetData = updatePacket.ToBytes();
                    Broadcast(packetData);
                }
            }
            else
            {
                Console.WriteLine("No winner found or total points is zero.");
            }
        }



        private void UpdatePlayerCards(Player player, int cardId, bool isAdding)
        {
            var cardUpdater = new CardUpdater();
            if (isAdding)
            {
               
                Card cardToUpdate = player.Hand.FirstOrDefault(card => card.Id == cardId);

                if (IsValidMove(cardToUpdate, centralCard))
                {
                    Console.WriteLine($"новая карта подходит");
                   
                }
                else
                {
                    turnPlayer = cardUpdater.HandleNumber(turnPlayer, players);
                    Console.WriteLine($"Паассс");

                }

            }
            else
            {
               
                Card cardToRemove = player.Hand.FirstOrDefault(card => card.Id == cardId);
                switch (cardToRemove.Type)
                {
                    case CardType.Number:
                        Console.WriteLine($"turn before {turnPlayer}");
                        turnPlayer = cardUpdater.HandleNumber(turnPlayer, players);
                        Console.WriteLine($"turn after {turnPlayer}");


                        break;
                    case CardType.Skip:
                        Console.WriteLine($"turn before {turnPlayer}");
                        turnPlayer = cardUpdater.HandleSkipCard(turnPlayer, players);
                        Console.WriteLine($"turn after {turnPlayer}");
                        break;
                    case CardType.DrawTwo:
                        Console.WriteLine($"turn befodfgre {turnPlayer}");

                        var (newTurnPlayers, updatedHands, newIndexs) = cardUpdater.HandleDraw(turnPlayer, players, 2, deck);
                        players[newIndexs].Hand = updatedHands;
                        turnPlayer = newTurnPlayers;
                        Console.WriteLine($"turn after {turnPlayer}");
                        break;



                    case CardType.Reverse:
                        Console.WriteLine($"turn before {turnPlayer}");
                        var (reverseList, turnsPlayer) = cardUpdater.HandleReverse(turnPlayer, players);
                        players = reverseList;
                        turnPlayer = turnsPlayer;
                        Console.WriteLine($"turn after {turnPlayer}");

                        break;
                    case CardType.Wild:

                        Console.WriteLine($"turn befodfgre {turnPlayer}");
                        turnPlayer = cardUpdater.HandleNumber(turnPlayer, players);
                        Console.WriteLine($"turn aftgfdser {turnPlayer}"); 
                        break;

                    case CardType.WildDrawFour:
                        Console.WriteLine($"turn befodfgre {turnPlayer}");

                        var (newTurnPlayer, updatedHand, newIndex) = cardUpdater.HandleDraw(turnPlayer, players, 4, deck);
                        players[newIndex].Hand = updatedHand;
                        turnPlayer = newTurnPlayer;
                        Console.WriteLine($"turn after {turnPlayer}");
                        break;


                    default:
                        throw new ArgumentOutOfRangeException($"Неизвестныdfghй тип карты: {centralCard.Type}");
                }

                centralCard = cardToRemove;
                if (cardToRemove != null)
                {
                  
                    player.Hand.Remove(cardToRemove);
                    Console.WriteLine($"Карта с ID {cardId} убрана из руки и добавлена на стол {player.Nickname}.");
                }
                else
                {
                    Console.WriteLine($"Карта с ID {cardId} не найдена в колоде.");
                    return; 
                }
            }
             var updateInfo = new GameStartInfo
            {
                FirstPlayerNickname = turnPlayer, 
                FirstCard = centralCard,
                PlayerHands = new Dictionary<string, List<Card>>() 
            };

            foreach (var p in players)
            {
                updateInfo.PlayerHands[p.Nickname] = p.Hand; 
            }
            string jsonData = JsonConvert.SerializeObject(updateInfo);
            byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);

            var updatePacket = new Packet
            {
                Command = Protocol.UpdateGameState, 
                Data = dataBytes 
            };

            byte[] packetData = updatePacket.ToBytes();

            Console.WriteLine($"now central card id {centralCard.Id},{centralCard.Value}.-----{centralCard.Color}");
            Broadcast(packetData);
            CalculateTotalPointsWinner();



        }
        private void Broadcast(byte[] data)
        {
            foreach (var player in players)
            {
                try
                {
                    player.SendMessage(data); 
                    Console.WriteLine($"Обновленная информация отправлена игроку {player.Nickname}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при отправке обновленной информации игроку {player.Nickname}: {ex.Message}");
                }
            }
        }









       


        
    }
}
