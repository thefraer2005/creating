using Newtonsoft.Json;


using System.Text;

using Common;

namespace Client3
{
    
    public partial class Form1 : Form
    {
        private Dictionary<string, List<Card>> playerHands = new Dictionary<string, List<Card>>();
        private int currentIndex;
        private int nextIndex;
        private int previousIndex;
        private int nextNextIndex;
        private Dictionary<string, Panel> playerPanels = new Dictionary<string, Panel>();
        private GameClient gameClient;
        private int selectPLayers;
        private WinnerRound victoryForm;
        private EndGame endForm;
        private StartForm startForm;
        List<string> otherPlayers = new List<string>();
        private string nickName;
        private Size cardSize;
        private Point cardPosition;
        private bool isMoving = false; 
        private Image cardImage; 



        public Form1(StartForm startForm, GameClient gameClient, int selectPlayers, string nickName)
        {
            this.nickName = nickName;
            this.DoubleBuffered = true;
            InitializeComponent();
            this.Resize += Form1_Resize;
            AdjustLayout();
            this.startForm = startForm;
            this.Size = startForm.Size;
            this.Location = startForm.Location;
            this.gameClient = gameClient;
            this.selectPLayers = selectPlayers;

            gameClient.OnGameStarted += InitCards;
            gameClient.endRound += showRoundWinner;
            gameClient.UpdateGame += UpdateCardsState;
            gameClient.UnoAction += HideUnoButton;
            gameClient.onVictory += showVictory;

        }
        private Dictionary<string, int> winnersScores = new Dictionary<string, int>();

        public void showRoundWinner(byte[] data)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => showRoundWinner(data)));
                return;
            }

            var winnerInfo = JsonConvert.DeserializeObject<WinnerInfo>(Encoding.UTF8.GetString(data));
            if (winnersScores.ContainsKey(winnerInfo.WinnerNickname))
            {
                winnersScores[winnerInfo.WinnerNickname] += winnerInfo.TotalPoints; 
            }
            else
            {
                winnersScores[winnerInfo.WinnerNickname] = winnerInfo.TotalPoints;
            }
           
            if (victoryForm != null && !victoryForm.IsDisposed)
            {
                victoryForm.Close(); 
            }

            
            victoryForm = new WinnerRound(winnerInfo.WinnerNickname, winnerInfo.TotalPoints, gameClient, this, selectPLayers, startForm, nickName);
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
            if (endForm != null && !endForm.IsDisposed)
            {
                victoryForm.Close();

            }
            endForm = new EndGame(winnerInfo.WinnerNickname, winnerInfo.TotalPoints, gameClient, this);
            endForm.ShowDialog(this);
        }
     
      
        private async void InitCards(byte[] data)
        {


            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => InitCards(data)));
                return;
            }
            this.pbLoading.Visible = false;
            this.lblWaiting.Visible = false;
            this.pnlCenterCard.Visible = true;
            this.pnlDeck.Visible = true;
            this.pnlColorIndicator.Visible = true;
            this.flpPlayerCards.Visible = true;
            this.flpOpponentCards.Visible = true;
            this.flpOpponentCards3.Visible = true;
            this.flpOpponentCards4.Visible = true;
            player1.Visible = true;
            player2.Visible = true;
            player3.Visible = true;
            player4.Visible = true;




            var gameStartInfo = JsonConvert.DeserializeObject<GameStartInfo>(Encoding.UTF8.GetString(data));
            currentPlayer = gameStartInfo.FirstPlayerNickname;
            pnlCenterCard.Controls.Clear();
            centralCard = gameStartInfo.FirstCard;

            UpdateColorIndicator(gameStartInfo.FirstCard);
            await DrawCard(centralCard, true, true, false, false);
            await DrawCard(centralCard, false, false, true, false);
            

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

                otherPlayers.Add(playerName);
                bool isPlayer = playerName.Equals(nickName);
               
            }
          


            currentIndex = otherPlayers.IndexOf(nickName);
            nextIndex = (currentIndex + 1) % otherPlayers.Count;
            previousIndex = (currentIndex - 1+ otherPlayers.Count) % otherPlayers.Count;
            nextNextIndex = (currentIndex + 2) % otherPlayers.Count;

            playerPanels[nickName] = flpPlayerCards;
            foreach(string name in otherPlayers)
            {
                if (!winnersScores.ContainsKey(name))
                {
                    winnersScores[name] = 0; 
                }
            }

            player1.Text = $"{nickName}:{winnersScores[nickName]} очков";
            await UpdateCardList(playerCardPanels, playerHands[nickName], playerHands[nickName], flpPlayerCards, true, false, nickName);
            switch (otherPlayers.Count)
            {
                case 2:

                    playerPanels[otherPlayers[nextIndex]] = flpOpponentCards;
                    player2.Text = $"{otherPlayers[nextIndex]}:{winnersScores[otherPlayers[nextIndex]]} очков";

                    
                    await UpdateCardList(opponentCardPanels, playerHands[otherPlayers[nextIndex]], playerHands[otherPlayers[nextIndex]], flpOpponentCards, false, false, otherPlayers[nextIndex]);
                  

                    break;
                case 3:

                    playerPanels[otherPlayers[nextIndex]] = flpOpponentCards3;
                    player3.Text = $"{otherPlayers[nextIndex]}:{winnersScores[otherPlayers[nextIndex]]} очков";


                    await UpdateCardList(opponentCardPanels3, playerHands[otherPlayers[nextIndex]], playerHands[otherPlayers[nextIndex]], flpOpponentCards3, false, true, otherPlayers[nextIndex]); // Обновление списка карт противника
                    
                    playerPanels[otherPlayers[previousIndex]] = flpOpponentCards4;
                    player4.Text = $"{otherPlayers[previousIndex]}:{winnersScores[otherPlayers[previousIndex]]} очков";
                    await UpdateCardList(opponentCardPanels4, playerHands[otherPlayers[previousIndex]], playerHands[otherPlayers[previousIndex]], flpOpponentCards4, false, true, otherPlayers[previousIndex]); // Обновление списка карт противника

                    break;
                case 4:
                    playerPanels[otherPlayers[nextIndex]] = flpOpponentCards3;
                    playerPanels[otherPlayers[nextNextIndex]] = flpOpponentCards;
                    playerPanels[otherPlayers[previousIndex]] = flpOpponentCards4;

                    player3.Text = $"{otherPlayers[nextIndex]}:{winnersScores[otherPlayers[nextIndex]]} очков";
                    player2.Text = $"{otherPlayers[nextNextIndex]}:{winnersScores[otherPlayers[nextNextIndex]]} очков";
                    player4.Text = $"{otherPlayers[previousIndex]}:{winnersScores[otherPlayers[previousIndex]]} очков";

                    await UpdateCardList(opponentCardPanels3, playerHands[otherPlayers[nextIndex]], playerHands[otherPlayers[nextIndex]], flpOpponentCards3, false, true, otherPlayers[nextIndex]); // Обновление списка карт противника
                    await UpdateCardList(opponentCardPanels, playerHands[otherPlayers[nextNextIndex]], playerHands[otherPlayers[nextNextIndex]], flpOpponentCards, false, false, otherPlayers[nextNextIndex]);
                    await UpdateCardList(opponentCardPanels4, playerHands[otherPlayers[previousIndex]], playerHands[otherPlayers[previousIndex]], flpOpponentCards4, false, true, otherPlayers[previousIndex]); // Обновление списка карт противника

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
                if (control is Button && control.Text == "UNO")
                {
                    this.Controls.Remove(control); 
                    break;
                }
            }
        }


        private void UpdateColorIndicator(Card centralCard)
        {


            Color newColor;

            switch (centralCard.Color)
            {
                case CardColor.Red:
                    newColor = Color.Red;
                    break;
                case CardColor.Blue:
                    newColor = Color.Blue;
                    break;
                case CardColor.Green:
                    newColor = Color.Green;
                    break;
                case CardColor.Yellow:
                    newColor = Color.Yellow;
                    break;
                default:
                    newColor = Color.Gray;
                    break;
            }

            pnlColorIndicator.CircleColor = newColor;
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
          
            currentPlayer = updateInfo.FirstPlayerNickname;
            centralCard = updateInfo.FirstCard;
            UpdateCurrentPlayerIndicator(currentPlayer);

            UpdateColorIndicator(updateInfo.FirstCard);



            await UpdateCardList(playerCardPanels, updateInfo.PlayerHands[nickName], playerHands[nickName], flpPlayerCards, true, false, nickName);
            switch (otherPlayers.Count)
            {
                case 2:
                    await UpdateCardList(opponentCardPanels, updateInfo.PlayerHands[otherPlayers[nextIndex]], playerHands[otherPlayers[nextIndex]], flpOpponentCards, false, false, otherPlayers[nextIndex]); // Обновление списка карт противника

                    break;
                case 3:
                    await UpdateCardList(opponentCardPanels3, updateInfo.PlayerHands[otherPlayers[nextIndex]], playerHands[otherPlayers[nextIndex]], flpOpponentCards3, false, true, otherPlayers[nextIndex]); // Обновление списка карт противника

                    await UpdateCardList(opponentCardPanels4, updateInfo.PlayerHands[otherPlayers[previousIndex]], playerHands[otherPlayers[previousIndex]], flpOpponentCards4, false, true, otherPlayers[previousIndex]); // Обновление списка карт противника

                    break;
                case 4:
                    await UpdateCardList(opponentCardPanels3, updateInfo.PlayerHands[otherPlayers[nextIndex]], playerHands[otherPlayers[nextIndex]], flpOpponentCards3, false, true, otherPlayers[nextIndex]); // Обновление списка карт противника
                    await UpdateCardList(opponentCardPanels, updateInfo.PlayerHands[otherPlayers[nextNextIndex]], playerHands[otherPlayers[nextNextIndex]], flpOpponentCards, false, false, otherPlayers[nextNextIndex]);
                    await UpdateCardList(opponentCardPanels4, updateInfo.PlayerHands[otherPlayers[previousIndex]], playerHands[otherPlayers[previousIndex]], flpOpponentCards4, false, true, otherPlayers[previousIndex]); // Обновление списка карт противника

                    break;
            }

            playerHands = updateInfo.PlayerHands;

            if (playerHands[nickName].Count == 1&&currentPlayer == otherPlayers[nextIndex])
            {
                ShowUnoButton();
            }
            else
            {
                HideUnoButton(currentPlayer);
            }
        }
        void ShowUnoButton()
        {
            Button btnUno = new Button();
            btnUno.Text = "UNO";
            btnUno.BackColor = Color.Red;
            btnUno.ForeColor = Color.Yellow;
            btnUno.Font = new Font("Arial", (float)(this.Height * 0.04), FontStyle.Bold);
            
            //btnUno.ApplyRoundedCorners(30);
            btnUno.Size = new Size((int)(this.Width * 0.2), (int)(this.Height * 0.1));
            btnUno.Location = new Point((int)(this.Width * 0.51), (int)(this.Height * 0.3));
            btnUno.Click += (sender, e) =>
            {

                gameClient.SendMessage(UnoCommand.UNO);
               

                HideUnoButton(currentPlayer);
            };

            this.Controls.Add(btnUno);

        }
        private async Task UpdateCardList(Dictionary<int, Panel> cardPanels, List<Card> newCards, List<Card> currentCards, Panel targetPanel, bool isPlayer, bool horizontal, string Nickname = null)
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


                        await MoveCardToCenter(cardPictureBox, cardPanel, targetPanel, cardPanels, card, Nickname, isPlayer, horizontal);
                        await Task.Delay(1000);


                    }
                }
            }

            int currentXPosition = 20;
            int upgrade = 0;
            foreach (var card in currentCards)
            {
                if (cardPanels.TryGetValue(card.Id, out var cardPanel))
                {
                    cardPanel.Location = horizontal ? new Point((targetPanel.Width - cardPanel.Width) / 2, currentXPosition) : new Point(currentXPosition, (targetPanel.Height - cardPanel.Height) / 2);
                    var cardPictureBox = cardPanel.Controls.OfType<PictureBox>().FirstOrDefault();
                    var currentCard = card;

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
                        var newCardPanel = await DrawCard(card, isPlayer, false, false, horizontal);
                        var currentCard = card;

                       
                        var cardPictureBox = newCardPanel.Controls.OfType<PictureBox>().FirstOrDefault();
                        cardPictureBox.Click += async (sender, e) =>
                        {
                            if (currentPlayer.Equals(nickName))
                            {
                                await Task.Run(() => OnCardClick(currentCard, targetPanel, Nickname));
                            }
                            else
                            {
                                ShowErrorDialog("Сейчас не ваш ход.");

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

                        //

                        targetPanel.Controls.Add(newCardPanel);
                        newCardPanel.Location = horizontal ? new Point((targetPanel.Width - newCardPanel.Width) / 2, currentXPosition) : new Point(currentXPosition, (targetPanel.Height - newCardPanel.Height) / 2);
                        newCardPanel.BringToFront();
                        cardPanels[card.Id] = newCardPanel;
                        int panealCount = targetPanel.Controls.OfType<Panel>().Count();
                        targetPanel.Invalidate();
                        await UpdateCardList(cardPanels, playerHands[Nickname], playerHands[Nickname], targetPanel, isPlayer, horizontal, Nickname);



                        currentXPosition += horizontal ? newCardPanel.Height : newCardPanel.Width;
                    }
                }
                if (currentCards.Count() > newCards.Count()) { currentXPosition = newCards.Count() * upgrade; }


                if (currentXPosition > (targetPanel.Height * 0.7) && horizontal || currentXPosition > (targetPanel.Width * 0.7) && !horizontal)

                {
                    int overlapOffset = (int)((currentXPosition - (horizontal ? (targetPanel.Height * 0.7) : targetPanel.Width * 0.7)) / newCards.Count);

                    currentXPosition = 20;

                    foreach (var card in newCards)
                    {
                        if (cardPanels.TryGetValue(card.Id, out var cardPanel))
                        {
                            cardPanel.SendToBack();

                            cardPanel.Location = horizontal ? new Point((targetPanel.Width - cardPanel.Width) / 2, currentXPosition) : new Point(currentXPosition, (targetPanel.Height - cardPanel.Height) / 2);

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


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

         
            if (isMoving && cardImage != null)
            {
               
                Rectangle destinationRect = new Rectangle(cardPosition.X, cardPosition.Y, cardSize.Width, cardSize.Height);

                e.Graphics.DrawImage(cardImage, destinationRect);
            }
        }

        private async Task MoveCardToPanel(PictureBox cardPictureBox, Panel cardPanel, Panel parent, Dictionary<int, Panel> cardPanels, Card card, string Nickname, bool isPlayer, bool horizontal)
        {
            cardImage = cardPictureBox.Image;
            cardSize = cardPictureBox.Size;


            Point targetPosition = new Point(parent.Location.X, parent.Location.Y);


            cardPosition = pnlDeck.Location;

            var tcs = new TaskCompletionSource<bool>();
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 10;

            isMoving = true;

            timer.Tick += async (sender, e) =>
            {

                if (Math.Abs(cardPosition.X - targetPosition.X) > 2 || Math.Abs(cardPosition.Y - targetPosition.Y) > 2)
                {

                    int stepX = (targetPosition.X - cardPosition.X) / 2;
                    int stepY = (targetPosition.Y - cardPosition.Y) / 2;


                    cardPosition = new Point(cardPosition.X + stepX, cardPosition.Y + stepY);
                    cardPanel.Location = cardPosition;

                    this.Invalidate();

                }
                else
                {

                    cardPanel.Location = targetPosition;
                    timer.Stop();
                    timer.Dispose();
                    tcs.SetResult(true);
                    isMoving = false;
                 

                }
            };

            timer.Start();
            await tcs.Task;
        }

        private void ShowColorSelectionForm(Card card)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowColorSelectionForm(card)));
                return;
            }

            using (var colorForm = new ColorSelectionForm(this))
            {
                colorForm.Owner = this;
                if (colorForm.ShowDialog(this) == DialogResult.OK)
                {
                    CardColor? selectedColor = colorForm.SelectedColor;
                    SendPlayCardPacket(card, selectedColor);
                }
            }
        }

        private bool isCardSwapped = false;

        public async Task OnCardClick(Card card,Panel parent,string Nickname)
        {

            var cardPanel = parent.Controls.OfType<Panel>()
.FirstOrDefault(panel =>
{
 var pictureBox = panel.Controls.OfType<CardPictureBox>().FirstOrDefault();
 return pictureBox != null && pictureBox.CardId == card.Id;
});

            var firstCard = playerHands[Nickname][0];
         

            var clickedCard = card; 

            var firstCardPanel = parent.Controls.OfType<Panel>()
.FirstOrDefault(panel =>
{
var pictureBox = panel.Controls.OfType<CardPictureBox>().FirstOrDefault();
return pictureBox != null && pictureBox.CardId == firstCard.Id;


});




            if (clickedCard.Id != firstCard.Id)
            {
                await SwapCardPositions(cardPanel, firstCardPanel, clickedCard, firstCard);
               
               
            }
            else
            {
                if (IsValidMove(card, centralCard))
                {
                    if (card.Type == CardType.Wild)
                    {
                        ShowColorSelectionForm(card);
                    }
                    else if (card.Type == CardType.WildDrawFour)
                    {
                        if (HasNoMatchingCard(playerHands[nickName], centralCard))
                        {
                            ShowColorSelectionForm(card);
                        }
                        else
                        {

                            ShowErrorDialog("У вас есть карта, соответствующая центральной.Вы не можете сыграть выбранную карту");
                        }
                    }
                    else
                    {
                        var cardPictureBox = cardPanel.Controls.OfType<CardPictureBox>().FirstOrDefault();
                        if (cardPictureBox != null)
                        {

                            SendPlayCardPacket(card);
                           
                        }
                    }
                }
                else {
                    
                    
                    ShowErrorDialog("Карта не подходит для хода");
                }

            }

            
            
        }
        private void ShowErrorDialog(string errorMessage)
        {
         
            ErrorDialog errorDialog = new ErrorDialog(errorMessage, this);

           
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowAndCloseErrorDialog(errorDialog)));
            }
            else
            {
                ShowAndCloseErrorDialog(errorDialog);
            }
        }

        private void ShowAndCloseErrorDialog(ErrorDialog errorDialog)
        {
            errorDialog.Show();

          
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1500; 
            timer.Tick += (sender, e) =>
            {
                timer.Stop(); 
                errorDialog.Close(); 
            };
            timer.Start(); 
        }


        public void SetCardSize(int width, int height)
        {
            cardSize = new Size(width, height);


        }
        private float rotationAngle = 0;
        private async Task  MoveCardToCenter(PictureBox cardPictureBox, Panel cardPanel, Panel parent, Dictionary<int, Panel> cardPanels, Card card, string Nickname, bool isPlayer, bool horizontal)
        {
            int centerX = pnlCenterCard.Location.X ; 
            int centerY = pnlCenterCard.Location.Y; 
            cardImage = cardPictureBox.Image;
            Point cardRelativePosition = parent.Location;
            cardPosition = new Point(cardRelativePosition.X, cardRelativePosition.Y);
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 10;
            isMoving = true;
            timer.Tick +=async (sender, e) =>
            {
              
                if (Math.Abs(cardPosition.X - centerX) > 2 || Math.Abs(cardPosition.Y - centerY) > 2)
                {
                  
                    int stepX = (centerX - cardPosition.X) / 2;
                    int stepY = (centerY - cardPosition.Y) / 2;
                    cardPosition = new Point(cardPosition.X + stepX, cardPosition.Y + stepY);
                    cardPanel.Location = cardPosition;

                   
                    /*if (horizontal)
                    {
                        rotationAngle += 90f / (500f / timer.Interval); // Поворачиваем на 90 градусов за 1 секунду
                        if (rotationAngle >= 90) rotationAngle = 90; // Ограничиваем угол до 90 градусов
                    }*/
                    
                    this.Invalidate();
                   
                  
                }
                else
                {
                    pnlCenterCard.Controls.Clear();
                    cardPanel.Location = new Point(0, 0); 
                    var pp = await DrawCard(card, true, false, false,false);
                    pnlCenterCard.Controls.Add(pp);
                    timer.Stop();
                    timer.Dispose();
                    isMoving = false; 
                    parent.Controls.Remove(cardPanel);
                    var cardToRemove = playerHands[Nickname].FirstOrDefault(c => c.Id == card.Id);
                    if (cardToRemove != null)
                    {
                         playerHands[nickName].Remove(cardToRemove);
                    }
                    this.Invalidate();
                    await UpdateCardList(cardPanels, playerHands[Nickname], playerHands[Nickname], parent, isPlayer, horizontal, Nickname);


                }
            };

            timer.Start();
        }



        private  async Task SwapCardPositions(Panel clickedCardPanel, Panel firstCardPanel, Card clickedCard, Card firstCard)
         {
            if (flpPlayerCards.InvokeRequired)
            {
               
                flpPlayerCards.Invoke(new Action(() => SwapCardPositions(clickedCardPanel, firstCardPanel, clickedCard, firstCard)));
                return; 
            }
          
            int indexClicked = flpPlayerCards.Controls.IndexOf(clickedCardPanel);
            int indexFirst = 0;

            playerHands[nickName][indexClicked] = firstCard; 
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
             
            }

           await  BringPanelsToFront( flpPlayerCards,  clickedCardPanel);

        }
       
        private async Task BringPanelsToFront(Panel flpPlayerCards, Panel clickedCardPanel)
        {
            for (int i = 0; i < flpPlayerCards.Controls.Count; i++)
            {
                if (flpPlayerCards.Controls[i] is Panel cardPanelw)
                {
                   
                    cardPanelw.SendToBack(); 
                    flpPlayerCards.Controls.SetChildIndex(cardPanelw, i);

                }
            }
        }




      

        





       

       
    }
}
