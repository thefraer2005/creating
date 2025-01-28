using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net.Sockets;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography.Pkcs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
namespace Client1
{
    
    public partial class Form1 : Form
    {
        
        private GameClient gameClient;
        private int selectPLayers;
        private WinnerRound victoryForm;
        private EndGame endForm;
        private StartForm startForm;
        List<string> otherPlayers = new List<string>();
        private string nickName;
        public Form1(StartForm startForm, GameClient gameClient,int selectPlayers,string nickName)
        {
            this.nickName = nickName;

            InitializeComponent();
            this.Resize += Form1_Resize;
            AdjustLayout(); 
            this.startForm = startForm;
            this.gameClient = gameClient;
            this.selectPLayers = selectPlayers;

            gameClient.OnGameStarted += InitCards;
            gameClient.endRound += showRoundWinner;
            gameClient.UpdateGame += UpdateCardsState;
            gameClient.UnoAction += HideUnoButton;
            gameClient.onVictory += showVictory;

        }
        public void showRoundWinner(byte[] data)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => showRoundWinner(data)));
                return;
            }

            var winnerInfo = JsonConvert.DeserializeObject<WinnerInfo>(Encoding.UTF8.GetString(data));

            // Проверяем, существует ли уже форма и закрыта ли она
            if (victoryForm != null && !victoryForm.IsDisposed)
            {
                victoryForm.Close(); // Закрываем предыдущую форму
            }

            // Создаем новую форму для победителя
            victoryForm = new WinnerRound(winnerInfo.WinnerNickname, winnerInfo.TotalPoints, gameClient, this, selectPLayers);
            victoryForm.ShowDialog(this);
        }



        private void showVictory(byte[] data)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => showVictory(data)));
                return;
            }

            var winnerInfo = JsonConvert.DeserializeObject<WinnerInfo>(Encoding.UTF8.GetString(data));
            if (endForm == null || endForm.IsDisposed)
            {
                endForm = new EndGame(winnerInfo.WinnerNickname, winnerInfo.TotalPoints, gameClient, this);
                endForm.ShowDialog(this);
            } 
            else
            {
                endForm = new EndGame(winnerInfo.WinnerNickname, winnerInfo.TotalPoints, gameClient, this);
            }
        }
        private  Dictionary<string, List<Card>> playerHands = new Dictionary<string, List<Card>>();
        private int currentIndex;
        private int nextIndex;
        private int previousIndex;
        private int nextNextIndex;
        private async void InitCards(byte[] data)
        {
           

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => InitCards(data)));
                return;
            }

           

            this.pbLoading.Visible = false;
            this.lblWaiting.Visible = false; 
            this.currentPlayerIndicator.BackColor = Color.Black;

            this.currentPlayerIndicator.Visible = true;
            this.pnlCenterCard.Visible = true;
            this.pnlDeck.Visible = true;
            this.pnlColorIndicator.Visible = true;
           
           
            this.flpPlayerCards.Visible = true;
            this.flpOpponentCards.Visible = true;

            this.flpOpponentCards3.Visible = true;
            this.flpOpponentCards4.Visible = true;

               
            



            var gameStartInfo = JsonConvert.DeserializeObject<GameStartInfo>(Encoding.UTF8.GetString(data));
            currentPlayer = gameStartInfo.FirstPlayerNickname;
            pnlCenterCard.Controls.Clear();
            centralCard = gameStartInfo.FirstCard;
            UpdateColorIndicator(centralCard);
            DrawCard(centralCard, true, true, false,false); 
            DrawCard(centralCard, false, false, true,false);

            playerHands.Clear();
          

            flpPlayerCards.Controls.Clear();
            flpOpponentCards.Controls.Clear();
            flpOpponentCards3.Controls.Clear();
            flpOpponentCards4.Controls.Clear();

            playerCardPanels.Clear();
            opponentCardPanels.Clear();
            opponentCardPanels3.Clear();
            opponentCardPanels4.Clear();
            otherPlayers.Clear();

            foreach (var player in gameStartInfo.PlayerHands)
            {
                var playerName = player.Key;
                var cards = player.Value;
              
                playerHands[playerName] = cards;

                var playerLabel = new Label
                {
                    Text = $"{playerName}:",
                    AutoSize = true,
                    Font = new Font(FontFamily.GenericSansSerif, 4, FontStyle.Bold),
                    Margin = new Padding(1)
                };
                otherPlayers.Add(playerName);
                bool isPlayer = playerName.Equals(nickName);
            }

            currentIndex = otherPlayers.IndexOf(nickName);
            nextIndex = (currentIndex + 1) % otherPlayers.Count;
            previousIndex = (currentIndex - 1+ otherPlayers.Count) % otherPlayers.Count;
            nextNextIndex = (currentIndex + 2) % otherPlayers.Count;



            await UpdateCardList(playerCardPanels, playerHands[nickName], playerHands[nickName], flpPlayerCards, true, false,nickName);
            switch (otherPlayers.Count)
            {
                case 2:
                    // Обновление списка карт противника
                   await UpdateCardList(opponentCardPanels, playerHands[otherPlayers[nextIndex]], playerHands[otherPlayers[nextIndex]], flpOpponentCards, true, false, otherPlayers[nextIndex]);
                  // Обновление списка карт противника

                    break;
                case 3:
                    await UpdateCardList(opponentCardPanels3, playerHands[otherPlayers[nextIndex]], playerHands[otherPlayers[nextIndex]], flpOpponentCards3, true,true, otherPlayers[nextIndex]); // Обновление списка карт противника

                    await UpdateCardList(opponentCardPanels4, playerHands[otherPlayers[previousIndex]], playerHands[otherPlayers[previousIndex]], flpOpponentCards4, true,true, otherPlayers[previousIndex]); // Обновление списка карт противника

                    break;
                case 4:
                    await UpdateCardList(opponentCardPanels3, playerHands[otherPlayers[nextIndex]], playerHands[otherPlayers[nextIndex]], flpOpponentCards3, true,true, otherPlayers[nextIndex]); // Обновление списка карт противника
                    await UpdateCardList(opponentCardPanels, playerHands[otherPlayers[nextNextIndex]], playerHands[otherPlayers[nextNextIndex]], flpOpponentCards, true,false, otherPlayers[nextNextIndex]);
                    await UpdateCardList(opponentCardPanels4, playerHands[otherPlayers[previousIndex]], playerHands[otherPlayers[previousIndex]], flpOpponentCards4, true,true, otherPlayers[previousIndex]); // Обновление списка карт противника

                    break;
            }
            UpdateCurrentPlayerIndicator(currentPlayer);
            
        }


        void HideUnoButton(string command)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(HideUnoButton), command);
                return;
            }

            foreach (Control control in this.Controls)
            {
                if (control is Button && control.Text == "Uno")
                {
                    this.Controls.Remove(control); 
                    break;
                }
            }
        }

       
        private void UpdateColorIndicator(Card centralCard)
        {
            pnlColorIndicator.Visible = true;
            Color newColor;

            switch (centralCard.Color) 
            {
                case "Red":
                    newColor = Color.Red;
                    break;
                case "Blue":
                    newColor = Color.Blue;
                    break;
                case "Green":
                    newColor = Color.Green;
                    break;
                case "Yellow":
                    newColor = Color.Yellow;
                    break;
                default:
                    newColor = Color.Gray; 
                    break;
            }

            pnlColorIndicator.BackColor = newColor;
        }
       
        private string currentPlayer;
      

        private Card centralCard;
        
        

        private async void UpdateCardsState(byte[] data)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateCardsState(data)));
                return;
            }

            GameStartInfo updateInfo;

            string jsonData = Encoding.UTF8.GetString(data);
            try
            {
                updateInfo = JsonConvert.DeserializeObject<GameStartInfo>(jsonData);
            }
            catch (JsonReaderException ex)
            {
                MessageBox.Show($"Ошибка десериализации JSON: {ex.Message}");
                MessageBox.Show($"Некорректные данные: {jsonData}");
                throw;
            }
            //pnlCenterCard.Controls.Clear();

            currentPlayer = updateInfo.FirstPlayerNickname;
            centralCard = updateInfo.FirstCard;
          
            UpdateColorIndicator(centralCard);
            
           

          await UpdateCardList(playerCardPanels, updateInfo.PlayerHands[nickName], playerHands[nickName], flpPlayerCards, true,false,nickName);
            switch (otherPlayers.Count)
            {
                case 2:
                    await UpdateCardList(opponentCardPanels, updateInfo.PlayerHands[otherPlayers[nextIndex]], playerHands[otherPlayers[nextIndex]], flpOpponentCards, true, false, otherPlayers[nextIndex]); // Обновление списка карт противника
                   
                    break;
                case 3:
                   await UpdateCardList(opponentCardPanels3, updateInfo.PlayerHands[otherPlayers[nextIndex]], playerHands[otherPlayers[nextIndex]], flpOpponentCards3, true,true, otherPlayers[nextIndex]); // Обновление списка карт противника

                   await UpdateCardList(opponentCardPanels4, updateInfo.PlayerHands[otherPlayers[previousIndex]], playerHands[otherPlayers[previousIndex]], flpOpponentCards4, true,true, otherPlayers[previousIndex]); // Обновление списка карт противника

                    break;
                case 4:
                   await UpdateCardList(opponentCardPanels3, updateInfo.PlayerHands[otherPlayers[nextIndex]],    playerHands[otherPlayers[nextIndex]], flpOpponentCards3, true,true, otherPlayers[nextIndex]); // Обновление списка карт противника
                   await UpdateCardList(opponentCardPanels, updateInfo.PlayerHands[otherPlayers[nextNextIndex]], playerHands[otherPlayers[nextNextIndex]], flpOpponentCards, true,false, otherPlayers[nextNextIndex]);
                   await UpdateCardList(opponentCardPanels4, updateInfo.PlayerHands[otherPlayers[previousIndex]], playerHands[otherPlayers[previousIndex]], flpOpponentCards4, true,true, otherPlayers[previousIndex]); // Обновление списка карт противника

                    break;
            }


            playerHands = updateInfo.PlayerHands;
            UpdateCurrentPlayerIndicator(currentPlayer);

            if (playerHands[nickName].Count == 1)
            {
                ShowUnoButton();
            }
            else
            {
                HideUnoButton(currentPlayer);
            }
        }
       
        private async Task UpdateCardList(Dictionary<int, Panel> cardPanels,  List<Card> newCards, List<Card> currentCards, Panel targetPanel, bool isPlayer,bool horizontal,string Nickname=null)
        {

            foreach (var card in currentCards.ToList())
            {
                if (!newCards.Contains(card))
                {
                    if (cardPanels.TryGetValue(card.Id, out var cardPanel))
                    {
                        var cardPictureBox = cardPanel.Controls.OfType<PictureBox>().FirstOrDefault();
                        if (horizontal)
                        {
                            SetCardSize(cardPictureBox.Height, cardPictureBox.Width);

                        }
                        else
                        {
                            SetCardSize(cardPictureBox.Width, cardPictureBox.Height);

                        }
                        //await Task.Delay(100);

                        await MoveCardToCenter(cardPictureBox, cardPanel, targetPanel, cardPanels, card,Nickname, isPlayer, horizontal);
                     
                       await Task.Delay(1000);


                    }
                }
            }

            int currentXPosition = 0;
            int upgrade = 0;
            foreach (var card in currentCards)
            {
                if (cardPanels.TryGetValue(card.Id, out var cardPanel))
                {
                    cardPanel.Location = horizontal ? new Point(cardPanel.Location.X, currentXPosition) : new Point(currentXPosition, cardPanel.Location.Y);
                    var cardPictureBox = cardPanel.Controls.OfType<PictureBox>().FirstOrDefault();
                    var currentCard = card;
                    /* cardPictureBox.Click += async (sender, e) =>
                    {
                        var currentCard = card;
                        if (currentPlayer.Equals(nickName))
                        {
                            await Task.Run(() => OnCardClick(currentCard, targetPanel, Nickname));
                        }
                        else
                        {
                            MessageBox.Show("Сейчас не ваш ход!");
                        }
                    };*/
                    // UpdatePictureBoxPosition(cardPanel,true, card);
                    currentXPosition += horizontal ? cardPanel.Height : cardPanel.Width;
                    upgrade = horizontal ? cardPanel.Height : cardPanel.Width;
                }
            }
            try
            {
                List<Task> moveTasks = new List<Task>();
                foreach (var card in newCards)
                {
                    if (!cardPanels.ContainsKey(card.Id))
                    {
                        var newCardPanel = DrawCard(card, isPlayer, false, false, horizontal);
                        //newCardPanel.Location = horizontal ? new Point(newCardPanel.Location.X, currentXPosition) : new Point(currentXPosition, newCardPanel.Location.Y);
                        var currentCard = card;
                        var cardPictureBox = newCardPanel.Controls.OfType<PictureBox>().FirstOrDefault();
                        cardPictureBox.Click += async  (sender, e) =>
                        {
                            if (currentPlayer.Equals(nickName))
                            {
                                await Task.Run(() => OnCardClick(currentCard, targetPanel, Nickname));
                            }
                            else
                            {
                                MessageBox.Show("Сейчас не ваш ход!");
                            }
                        };
                        if (horizontal)
                        {
                            SetCardSize(cardPictureBox.Height, cardPictureBox.Width);

                        }
                        else
                        {
                            SetCardSize(cardPictureBox.Width, cardPictureBox.Height);

                        }
                        await Task.Delay(200);

                        await MoveCardToPanel(cardPictureBox, newCardPanel, targetPanel, cardPanels, card, Nickname, isPlayer, horizontal);

                        newCardPanel.Location = new Point(0, 0);
                        // Установите нужные координаты
                        targetPanel.Controls.Add(newCardPanel);
                       
                        cardPanels[card.Id] = newCardPanel;
                        int panealCount = targetPanel.Controls.OfType<Panel>().Count();
                        targetPanel.Invalidate();


                       
                        currentXPosition += horizontal ? newCardPanel.Height : newCardPanel.Width;
                    }
                }
                if (currentCards.Count() > newCards.Count()) { currentXPosition = newCards.Count() * upgrade; }


                if (currentXPosition > (targetPanel.Height * 0.7) && horizontal || currentXPosition > (targetPanel.Width * 0.7) && !horizontal)

                {
                    int overlapOffset = (int)((currentXPosition - (horizontal ? (targetPanel.Height * 0.7) : targetPanel.Width * 0.7)) / newCards.Count);

                    currentXPosition = 0;

                    foreach (var card in newCards)
                    {
                        if (cardPanels.TryGetValue(card.Id, out var cardPanel))
                        {
                            cardPanel.SendToBack();

                            cardPanel.Location = horizontal ? new Point(cardPanel.Location.X, currentXPosition) : new Point(currentXPosition, cardPanel.Location.Y);

                            currentXPosition += horizontal ? cardPanel.Height - overlapOffset : (cardPanel.Width) - overlapOffset;
                         

                        }
                    }
                }
                

                targetPanel.Invalidate();
              
            }


            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            

        }

        private Size cardSize;
        private Point cardPosition; // Текущая позиция панели
        private bool isMoving = false; // Флаг, указывающий, движется ли панель
        private Image cardImage; // Изображение карты

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Рисуем карту в текущей позиции, если она движется
            if (isMoving && cardImage != null)
            {
                // Создаем прямоугольник назначения с текущей позицией и заданными размерами карты
                Rectangle destinationRect = new Rectangle(cardPosition.X, cardPosition.Y, cardSize.Width, cardSize.Height);

                e.Graphics.DrawImage(cardImage, destinationRect);
            }
        }
       
        private async Task MoveCardToPanel(PictureBox cardPictureBox, Panel cardPanel, Panel parent, Dictionary<int, Panel> cardPanels, Card card, string Nickname, bool isPlayer, bool horizontal)
        {





            cardImage = cardPictureBox.Image;
            cardSize = cardPictureBox.Size; // Убедитесь, что вы задаете размер карты

            // Получаем целевую позицию для перемещения
            Point targetPosition = new Point(parent.Location.X, parent.Location.Y);

            // Начальная позиция карты
            cardPosition = pnlDeck.Location; // Используем поле класса

            var tcs = new TaskCompletionSource<bool>();
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 30; // Интервал обновления (примерно 20 FPS)

            isMoving = true; // Устанавливаем флаг движения

            timer.Tick += async (sender, e) =>
            {
                // Проверяем, достигли ли мы целевой позиции
                if (Math.Abs(cardPosition.X - targetPosition.X) > 2 || Math.Abs(cardPosition.Y - targetPosition.Y) > 2)
                {
                    // Вычисляем шаги
                    int stepX = (targetPosition.X - cardPosition.X) / 2; // Делим на 10 для более плавного движения
                    int stepY = (targetPosition.Y - cardPosition.Y) / 2;

                    // Обновляем позицию карты
                    cardPosition = new Point(cardPosition.X + stepX, cardPosition.Y + stepY);
                    cardPanel.Location = cardPosition;

                    this.Invalidate(); // Перерисовываем только панель карты
                                       // Перерисовываем всю форму
                }
                else
                {
                    // Устанавливаем окончательное положение для cardPanel
                    cardPanel.Location = targetPosition; // Устанавливаем в целевую позицию
                    timer.Stop();
                    timer.Dispose();
                   

                    tcs.SetResult(true);
                    isMoving = false;



                    // Сбрасываем флаг движения
                   

                    // Добавляем карту в родительский элемент

                    // Обновляем словарь панелей
                    // Перерисовываем родительский элемент
                }
            };

            timer.Start();
            await tcs.Task;
        }



        // Отдельный метод для обработки клика по картам
        private void PictureBox_Click(object sender, EventArgs e)
        {
            // Здесь можно добавить дополнительную логику, если потребуется
        }

        private bool isCardSwapped = false; // Флаг для отслеживания, был ли уже выполнен обмен

        public async Task OnCardClick(Card card,Panel parent,string Nickname)
        {

            var cardPanel = parent.Controls.OfType<Panel>()
.FirstOrDefault(panel =>
{
 var pictureBox = panel.Controls.OfType<CardPictureBox>().FirstOrDefault();
 return pictureBox != null && pictureBox.CardId == card.Id;
});

            var firstCard = playerHands[Nickname][0];
            // Первая карта в словаре

            var clickedCard = card; // Нажатая карта

            var firstCardPanel = parent.Controls.OfType<Panel>()
.FirstOrDefault(panel =>
{
var pictureBox = panel.Controls.OfType<CardPictureBox>().FirstOrDefault();
return pictureBox != null && pictureBox.CardId == firstCard.Id;


});




            if (clickedCard.Id != firstCard.Id)
            {
                await SwapCardPositions(cardPanel, firstCardPanel, clickedCard, firstCard);
                // Если нажатая карта первая, вызываем функцию перемещения
               
            }
            else
            {
                if (IsValidMove(card, centralCard))
                {
                    if (card.Type == CardType.Wild)
                    {
                        using (var colorForm = new ColorSelectionForm())
                        {
                            if (colorForm.ShowDialog() == DialogResult.OK)
                            {
                                CardColor selectedColor = colorForm.SelectedColor;

                                SendPlayCardPacket(card, selectedColor);
                            }
                        }
                    }
                    else if (card.Type == CardType.WildDrawFour)
                    {
                        if (HasNoMatchingCard(playerHands[nickName], centralCard))
                        {
                            using (var colorForm = new ColorSelectionForm())
                            {
                                if (colorForm.ShowDialog() == DialogResult.OK)
                                {
                                    CardColor selectedColor = colorForm.SelectedColor;
                                    // Получаем панель нажатой карты

                                    SendPlayCardPacket(card, selectedColor);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("У вас есть  карта, соответствующая центральной. Вы не можете сыграть эту карту.");
                        }
                    }
                    else
                    {
                        var cardPictureBox = cardPanel.Controls.OfType<CardPictureBox>().FirstOrDefault();
                        if (cardPictureBox != null)
                        {

                            SendPlayCardPacket(card);
                            // MoveCardToCenter(cardPictureBox, cardPanel);
                        }



                    }
                }

            }

            
            
        }
       
        public void SetCardSize(int width, int height)
        {
            cardSize = new Size(width, height);


        }
        private float rotationAngle = 0;
        private async Task  MoveCardToCenter(PictureBox cardPictureBox, Panel cardPanel, Panel parent, Dictionary<int, Panel> cardPanels, Card card, string Nickname, bool isPlayer, bool horizontal)
        {
           

            // Получаем центр контейнера
            int centerX = pnlCenterCard.Location.X ; // Центр по X
            int centerY = pnlCenterCard.Location.Y; // Центр по Y

            // Сохраняем текущее изображение карты
            cardImage = cardPictureBox.Image;

            // Получаем начальную позицию карты относительно родительского контейнера (cardPanel)
            Point cardRelativePosition = parent.Location;

            // Начальная позиция карты относительно flpPlayerCards
            cardPosition = new Point(cardRelativePosition.X, cardRelativePosition.Y);

            // Создаем таймер для анимации
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 30; // Интервал обновления в миллисекундах

            isMoving = true; // Устанавливаем флаг движения

            // Переменная для отслеживания угла поворота
             rotationAngle = 0; // Начальный угол поворота

            timer.Tick +=async (sender, e) =>
            {
                // Проверяем, достигли ли мы центра
                if (Math.Abs(cardPosition.X - centerX) > 5 || Math.Abs(cardPosition.Y - centerY) > 5)
                {
                    // Вычисляем шаги
                    int stepX = (centerX - cardPosition.X) / 5; // Делим на 5 для более плавного движения
                    int stepY = (centerY - cardPosition.Y) / 5;

                    // Обновляем позицию карты
                    cardPosition = new Point(cardPosition.X + stepX, cardPosition.Y + stepY);
                    cardPanel.Location = cardPosition;

                    // Увеличиваем угол поворота, если horizontal == true
                    /*if (horizontal)
                    {
                        rotationAngle += 90f / (500f / timer.Interval); // Поворачиваем на 90 градусов за 1 секунду
                        if (rotationAngle >= 90) rotationAngle = 90; // Ограничиваем угол до 90 градусов
                    }*/

                    this.Invalidate(); // Перерисовываем форму
                }
                else
                {
                    pnlCenterCard.Controls.Clear();

                    // Устанавливаем окончательное положение для cardPanel
                    cardPanel.Location = new Point(0, 0); // Устанавливаем положение в верхний левый угол pnlCenterCard
                   
                    var pp = DrawCard(card, true, false, false,false);
                    pnlCenterCard.Controls.Add(pp);

                    // Останавливаем таймер и сбрасываем флаг движения
                    timer.Stop();
                    timer.Dispose();
                    isMoving = false; // Сбрасываем флаг движения

                    parent.Controls.Remove(cardPanel);
                   // cardPanels.Remove(card.Id);
                    var cardToRemove = playerHands[Nickname].FirstOrDefault(c => c.Id == card.Id);
                    if (cardToRemove != null)
                    {
                       playerHands[nickName].Remove(cardToRemove);
                    }

                    this.Invalidate();
                 // await UpdateCardList(cardPanels, playerHands[Nickname], playerHands[Nickname], parent, isPlayer, horizontal, Nickname);


                }
            };

            timer.Start();
        }



        private  async Task SwapCardPositions(Panel clickedCardPanel, Panel firstCardPanel, Card clickedCard, Card firstCard)
         {
            if (flpPlayerCards.InvokeRequired)
            {
                // Если да, то вызываем этот же метод в UI-потоке без await
                flpPlayerCards.Invoke(new Action(() => SwapCardPositions(clickedCardPanel, firstCardPanel, clickedCard, firstCard)));
                return; // Завершаем выполнение текущего метода
            }
          
            int indexClicked = flpPlayerCards.Controls.IndexOf(clickedCardPanel);
            int indexFirst = 0;

            playerHands[nickName][indexClicked] = firstCard; // Обновляем карту на выбранную
            playerHands[nickName][indexFirst] = clickedCard;
          


            string jsonData = JsonConvert.SerializeObject(clickedCard.Id);
            byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);



            gameClient.SendMessage(UnoCommand.SWAP, dataBytes);
            Point tempLocation = clickedCardPanel.Location;
             clickedCardPanel.Location = firstCardPanel.Location;
             firstCardPanel.Location = tempLocation;
            flpPlayerCards.Controls.SetChildIndex(clickedCardPanel, 0);
            flpPlayerCards.Controls.SetChildIndex(firstCardPanel, indexClicked);

            for (int i = 0; i < flpPlayerCards.Controls.Count; i++)
            {
                Panel cardPanel = flpPlayerCards.Controls[i] as Panel;
               // Console.WriteLine($"Индекс: {i}, Панель: {cardPanel.Name}"); // Вывод имени панели или другого идентификатора
            }

           await  BringPanelsToFront( flpPlayerCards,  clickedCardPanel);

           //await UpdateCardList(cardPanels, playerHands[nickName], playerHands[nickName], parent, isPlayer, horizontal, Nickname);
            
        }
       
        private async Task BringPanelsToFront(Panel flpPlayerCards, Panel clickedCardPanel)
        {
            for (int i = 0; i < flpPlayerCards.Controls.Count; i++)
            {
                if (flpPlayerCards.Controls[i] is Panel cardPanelw)
                {
                    // Проверяем, находится ли панель справа от clickedCardPanel


                    cardPanelw.SendToBack(); // Перемещаем панель на передний план
                    flpPlayerCards.Controls.SetChildIndex(cardPanelw, i); // Устанавливаем индекс

                }
            }
        }




        private bool isMouseOverPictureBox = false;

        private void IncreasePictureBoxSize(PictureBox pictureBox, Panel cardPanel)
        {
            pictureBox.Size = new Size(cardPanel.Width + 20, cardPanel.Height + 20); // Увеличиваем на 20 пикселей по ширине и высоте
            pictureBox.Location = new Point(pictureBox.Location.X - 10, pictureBox.Location.Y - 10); // Сдвигаем на 10 пикселей влево и вверх
        }

        private void ResetPictureBoxSize(PictureBox pictureBox, Panel cardPanel)
        {
            pictureBox.Size = cardPanel.Size; // Возвращаем размер изображения к исходному
            pictureBox.Location = new Point(0, 0); // Возвращаем на исходную позицию
        }



        void ShowUnoButton()
        {
            Button btnUno = new Button();
            btnUno.Text = "Uno";
            btnUno.Location = new System.Drawing.Point(750, 350);
            btnUno.Size = new System.Drawing.Size(100, 80);

            btnUno.Click += (sender, e) =>
            {
               
                gameClient.SendMessage(UnoCommand.UNO);
                MessageBox.Show("1");

                HideUnoButton(currentPlayer);
            };

            this.Controls.Add(btnUno);
        }

        





        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            gameClient.Disconnect(); 
            base.OnFormClosing(e);
        }

       
    }
}
