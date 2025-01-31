
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
using static Common.Package11207Helper;
using System.Reflection.Metadata;
using System.IO.Compression;

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
        private CancellationTokenSource ctxSource = new CancellationTokenSource();
        private const int MaxTimeout = 5 * 60 * 1000;
        private List<Player> players = new List<Player>();
        List<string> actions = new List<string>();
        private Socket serverSocket;
        private Socket clientSocket;

        private readonly List<Socket> _clients = new();

        private Deck deck; 
        private bool gameStarted = false;
  
        private string turnPlayer;

        private Card centralCard;
        private bool UnoFlag = false;
        
        private int playerInit=0;
        private int playerInits=0;
        public async Task Start()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5000));
           

           

            try
            {
                serverSocket.Listen();
                Console.WriteLine("Сервер запущен...");
                do
                {
                   clientSocket = await serverSocket.AcceptAsync();
                    _clients.Add(clientSocket);
                
                    Player player = new Player(clientSocket);
                    players.Add(player);

                    Console.WriteLine($"Игрн: {players.Count}");

                    _ = Task.Run(async () => await HandleClient(player));

                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"meesaggee   {ex}");
                StopAsync();
            }



        }

        private void StopAsync()
        {
            serverSocket.Close();
        }


        private async Task HandleClient(Player player)
        {
            Console.WriteLine($"Игрок: {player.Hand.Count}");

            try
            {
               
                while (player.Socket.Connected)
                {
                    byte[] buffer = new byte[MaxPacketSize];
                    var contentLength = await player.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

                 

                    var content = new List<byte>();
                    content.AddRange(GetContent(buffer, contentLength));

                    if (!IsQueryValid(buffer, contentLength) || IsPartial(buffer[Fullness]))
                    {
                        Console.WriteLine($"Неверный запрос: {contentLength}");
                        continue;
                    }

                    var command = GetCommand(buffer[Command]);
                 

                    await ProcessSocketConnect(player, content, command, player.Socket);
                }
            }
            catch (OutOfMemoryException ex)
            {
                Console.WriteLine("Недостаточно памяти при обработке данных: " + ex.Message);
                // Логика для обработки ситуации
                 // Прерываем цикл при ошибке
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Ошибка сокета: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}\nСтек вызовов: {ex.StackTrace}");
            }
            finally
            {
                Console.WriteLine($"Отключение игрока");
                player.Socket.Close();
                _clients.Remove(player.Socket);
                players.Remove(player);
                
              
            }
        }
        private async Task BroadcastMessageAsync(Socket socket, byte[] message, CancellationToken ctx)
        {
            var semaphore = new SemaphoreSlim(1);
            var timeout = TimeSpan.FromSeconds(10000); // Установите желаемый тайм-аут

            foreach (var player in players)
            {
                Console.WriteLine("1");
                await semaphore.WaitAsync(ctx);
                try
                {
                    using (var cts = CancellationTokenSource.CreateLinkedTokenSource(ctx))
                    {
                        cts.CancelAfter(timeout); // Установите тайм-аут

                        try
                        {
                            await player.Socket.SendAsync(message, SocketFlags.None, cts.Token);
                        }
                        catch (TaskCanceledException)
                        {
                            Console.WriteLine($"Тайм-аут при отправке сообщения игроку {player.Nickname}.");
                            // Обработка тайм-аута, если необходимо
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при отправке сообщения игроку {player.Nickname}: {ex.Message}");
                            // Удалите игрока из списка или выполните другие действия
                        }
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }

            semaphore.Dispose();
        }

        private Dictionary<Socket, int> selectCountPlayers = new Dictionary<Socket, int>();
        private int initialPlayerCount = -1;
        private int playerInitss = 0;
        private async Task ProcessSocketConnect( Player player,List<byte> content,UnoCommand comm, Socket socket)
        {
            Console.WriteLine($"command --- {comm}");


            try
            {
                switch (comm)
                {
                    case UnoCommand.CONNECT:
                        // Обработка команды подключения
                        player.Nickname = System.Text.Encoding.UTF8.GetString(content.ToArray());
                        Console.WriteLine($"игрок добавлавыен --- {player.Nickname}");
                        break;

                    case UnoCommand.START:
                        int selectedPlayersCount = BitConverter.ToInt32(content.ToArray(), 0);

                        // Проверяем, является ли текущий игрок первым, выбравшим количество игроков
                        if (initialPlayerCount == -1)
                        {
                            // Если это первый игрок, устанавливаем его выбор
                            initialPlayerCount = selectedPlayersCount;
                        }

                        // Добавляем или обновляем количество выбранных игроков для данного сокета
                        selectCountPlayers[socket] = selectedPlayersCount;

                        bool allInitialPlayersMatch = selectCountPlayers.Values.Take(initialPlayerCount).All(count => count == initialPlayerCount);
                        await SendMessageToSocket(socket, initialPlayerCount);

                        if (selectCountPlayers.Count() == initialPlayerCount && allInitialPlayersMatch)
                        {
                        Console.WriteLine($" {initialPlayerCount} --- {selectCountPlayers.Count()}");

                            await StartGame(socket);
                        }
                        


                        break;
                       

                    case UnoCommand.PLAY_CARD:
                        // Обработка команды сыграть карту
                        await HandlePlayCard(player, socket, content.ToArray());
                        break;
                    case UnoCommand.SWAP:
                        int cardIdToSwap = JsonConvert.DeserializeObject<int>(Encoding.UTF8.GetString(content.ToArray()));

                        // Получаем руку игрока
                       

                        // Проверяем, есть ли у игрока хотя бы одна карта
                        if (player.Hand.Count > 0)
                        {
                            // Находим индекс карты, которую нужно поменять
                            int indexToSwap = player.Hand.FindIndex(c => c.Id == cardIdToSwap);

                            // Проверяем, что карта найдена
                            if (indexToSwap >= 0)
                            {
                                // Меняем первую карту с картой, идентификатор которой был получен
                                var tempCard = player.Hand[0];
                                player.Hand[0] = player.Hand[indexToSwap];
                                player.Hand[indexToSwap] = tempCard;

                                Console.WriteLine("поменялись местами");

                            }
                            else
                            {
                                Console.WriteLine("Карта с указанным ID не найдена в руке игрока.");
                            }
                        }

                        break;

                    case UnoCommand.UNO:
                        // Обработка команды UNO
                        actions.Add("uno");

                        UnoFlag = true;
                        Console.WriteLine($"Now uno flag truuuueee {player.Nickname}");

                        break;

                    case UnoCommand.GIVECARD:
                        // Обработка команды раздать карты
                        await GiveCard(player, socket);
                        break;

                    case UnoCommand.NEW_ROUND:
                        playerInits += 1;
                        if (playerInits == players.Count)
                        {
                           await  StartGame(socket);

                        }
                        break;
                    
                }
                content.Clear();
                
            }
            catch
            {
                Console.WriteLine($"Now u {player.Nickname}");

                socket.Disconnect(false);
            }
        }
        private async Task SendMessageToSocket(Socket socket, int number)
        {
            byte[] messageBytes = BitConverter.GetBytes(number);

            // Получаем пакеты на основе байтового массива
            var packages = GetPackagesByMessage(messageBytes, UnoCommand.ERROR_START, QueryType.Response);

            // Отправляем каждый пакет
            foreach (var package in packages)
            {
                await socket.SendAsync(new ArraySegment<byte>(package), SocketFlags.None);
            }
        }


        private void CheckStartGame(byte[] data, Socket socket)
        {
            int playerCount = BitConverter.ToInt32(data, 0);
            Console.WriteLine($"ayoue countood {playerCount}");
            Console.WriteLine($"acount {players.Count}");



            if (players.Count == playerCount)
            {
                Console.WriteLine("alles good");


                StartGame(socket);
            }
        }
        private async Task HandlePlayCard(Player player, Socket socket, byte[] packet)
        {
            var cardUpdater = new CardUpdater();
            int currentPlayerIndex = players.FindIndex(player => player.Nickname == turnPlayer);
            int previousPlayerIndex = (currentPlayerIndex - 1 + players.Count) % players.Count; // Определяем предыдущего игрока
            Player previousPlayer = players[previousPlayerIndex];
            Console.WriteLine($"не count cars {previousPlayer.Hand.Count}.");
            Console.WriteLine($"index {previousPlayerIndex}----{currentPlayerIndex}.");




            string jsonData = Encoding.UTF8.GetString(packet);
            var cardData = JsonConvert.DeserializeObject<Card>(jsonData);
            Console.WriteLine($"Card received - Id: {cardData.Id}, Value: {cardData.Value}, Color: {cardData.Color}");
            if (!UnoFlag && players[previousPlayerIndex].Hand.Count == 1)
            {
                var (updatedHandUno, previosIndexs) = cardUpdater.NotUno(turnPlayer, players, 2, deck);
                Console.WriteLine($"не sdfghddddfуспел.");

                players[previosIndexs].Hand = updatedHandUno;
                var packages = GetPackagesByMessage(null, UnoCommand.UNO, QueryType.Response);

                packages.ForEach(
                    async p => await BroadcastMessageAsync(socket, p, ctxSource.Token));


            }
            UnoFlag = false;
            Console.WriteLine($"не count s {previousPlayer.Hand.Count}.");

            if (IsValidMove(cardData, centralCard))
            {
                Card playerCard = player.Hand.FirstOrDefault(c => c.Id == cardData.Id);
                if (playerCard != null)
                {
                    playerCard.Color = cardData.Color;
                }
                // Удаляем карту из руки игрока
                await UpdatePlayerCards(player, cardData.Id, false, socket);

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
        private async Task StartGame(Socket socket)
        {
            selectCountPlayers.Clear();
            initialPlayerCount = -1;

            Console.WriteLine($"Central card :{centralCard}");

            playerInit = 0;
            playerInits = 0;
            gameStarted = true;
            deck = new Deck();
            deck.Shuffle();
            centralCard = deck.Draw();

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
           
            var packages = GetPackagesByMessage(dataBytes, UnoCommand.START, QueryType.Response);
            packages.ForEach(async p => await BroadcastMessageAsync(socket, p, ctxSource.Token));


        }
       

       




        private bool IsValidMove(Card card, Card centralCard)
        {
            if (card.Type == CardType.Wild || card.Type == CardType.WildDrawFour)
            {
                return true;
            }

            return card.Color == centralCard.Color || card.Value == centralCard.Value;
        }



        private async Task GiveCard(Player player, Socket socket)
        {
            if (deck.Cards.Count > 0)
            {
                try
                {
                    Card drawnCard = deck.Draw();

                    player.Hand.Add(drawnCard);
                    Console.WriteLine($"{player.Nickname} получил кnnарту {drawnCard}.");

                    await UpdatePlayerCards(player, drawnCard.Id, true, socket);
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
                        totalPoints += (int)(byte)card.Value;
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
       

        private async Task CalculateTotalPointsWinner(Socket socket)
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
                    if (player.Score >= 30)
                    {
                        var winnerInfo = new WinnerInfo
                        {
                            WinnerNickname = winnerNickname,
                            TotalPoints = player.Score
                        };
                        Console.WriteLine($"Victory achieved by {winnerInfo.WinnerNickname} with score {winnerInfo.TotalPoints}");

                        string jsonData = JsonConvert.SerializeObject(winnerInfo);
                        byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);

                        var packages = GetPackagesByMessage(dataBytes, UnoCommand.VICTORY, QueryType.Response);

                        packages.ForEach(
                            async p => await BroadcastMessageAsync(socket, p, ctxSource.Token));
                        victoryAchieved = true;
                        foreach (var playe in players)
                        {
                            try
                            {
                                if (playe.Socket.Connected)
                                {
                                    playe.Socket.Shutdown(SocketShutdown.Both); // Остановка сокета
                                }
                            }
                            catch (SocketException ex)
                            {
                                Console.WriteLine($"Ошибка при закрытии сокета игрока {player.Nickname}: {ex.Message}");
                            }
                            finally
                            {
                                playe.Socket.Close(); // Закрытие сокета
                            }
                        }


                        Console.WriteLine($"All players have been removed from the game--{players.Count()}.");
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

                    var packages = GetPackagesByMessage(dataBytes, UnoCommand.NEW_ROUND, QueryType.Response);

                    packages.ForEach(
                        async p => await BroadcastMessageAsync(socket, p, ctxSource.Token));
                  


                }
            }
            else
            {
                Console.WriteLine("No winner found or total points is zero.");
            }
        }



        private async Task UpdatePlayerCards(Player player, int cardId, bool isAdding, Socket socket)
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
                Console.WriteLine($"{p.Nickname}'s hand:");
                foreach (var card in p.Hand)
                {
                    Console.WriteLine($" - {card}");
                }
            }
            string jsonData = JsonConvert.SerializeObject(updateInfo);
            byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);



            Console.WriteLine($"now central card id {centralCard.Id},{centralCard.Value}.-----{centralCard.Color}");
            var packages = GetPackagesByMessage(dataBytes, UnoCommand.UPDATE_FIELD, QueryType.Response);

            packages.ForEach(
                async p => await BroadcastMessageAsync(socket, p, ctxSource.Token));

           await CalculateTotalPointsWinner(socket);



        }
       












    }
}
