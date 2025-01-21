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
namespace Client1
{

    public partial class Form1 : Form
    {

        private GameClient gameClient;
        private int selectPLayers;
        List<string> otherPlayers = new List<string>();
        private WinnerRound victoryForm;
        private EndGame endForm;
        private StartForm startForm;
        private string nickName;

        public Form1(StartForm startForm, GameClient gameClient, int selectPlayers,string nickName)
        {
            this.nickName=nickName;
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
        private void InitCards(byte[] data)
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
            this.flpOpponentCards.Visible = true;
            this.pnlColorIndicator.Visible = true;
            this.flpPlayerCards.Visible = true;




            var gameStartInfo = JsonConvert.DeserializeObject<GameStartInfo>(Encoding.UTF8.GetString(data));
            currentPlayer = gameStartInfo.FirstPlayerNickname;
            pnlCenterCard.Controls.Clear();
            centralCard = gameStartInfo.FirstCard;
            UpdateColorIndicator(centralCard);
            DrawCard(centralCard, true, true, false); 
            DrawCard(centralCard, false, false, true); 
            playerCards.Clear();
            opponentCards.Clear();

            flpPlayerCards.Controls.Clear();
            flpOpponentCards.Controls.Clear();
            playerCardPanels.Clear();
            opponentCardPanels.Clear();
            otherPlayers.Clear();

            foreach (var player in gameStartInfo.PlayerHands)
            {
                var playerName = player.Key;
                var cards = player.Value;

                var playerLabel = new Label
                {
                    Text = $"{playerName}:",
                    AutoSize = true,
                    Font = new Font(FontFamily.GenericSansSerif, 4, FontStyle.Bold),
                    Margin = new Padding(1)
                };

                bool isPlayer = playerName.Equals(nickName); 

                if (isPlayer)
                {
                    flpPlayerCards.Controls.Add(playerLabel);
                    playerCards.AddRange(cards);
                }
                else
                {
                    flpOpponentCards.Controls.Add(playerLabel);
                    opponentCards.AddRange(cards);
                    otherPlayers.Add(playerName);
                }

                if (isPlayer)
                {
                    UpdateCardList(playerCardPanels, playerCards, cards, flpPlayerCards, true);
                }
                else
                {
                    UpdateCardList(opponentCardPanels, opponentCards, cards, flpOpponentCards, false);
                }
                UpdateCurrentPlayerIndicator(currentPlayer);
            }
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
        private List<Card> playerCards = new List<Card>();
        private List<Card> opponentCards = new List<Card>();


        private Card centralCard;
        
       

        private void UpdateCardsState(byte[] data)
        {
            if (flpPlayerCards.InvokeRequired || flpOpponentCards.InvokeRequired)
            {
                flpPlayerCards.Invoke(new Action(() => UpdateCardsState(data)));
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
          
            UpdateColorIndicator(centralCard);
            pnlCenterCard.Controls.Clear();
            DrawCard(centralCard, true, true, false);

            var newPlayerCards = updateInfo.PlayerHands[nickName];
            var newOpponentCards = updateInfo.PlayerHands[otherPlayers[0]];

            UpdateCardList(playerCardPanels, playerCards, newPlayerCards, flpPlayerCards,true);
            UpdateCardList(opponentCardPanels, opponentCards, newOpponentCards, flpOpponentCards,false);

            playerCards = newPlayerCards;
            opponentCards = newOpponentCards; 

            UpdateCurrentPlayerIndicator(currentPlayer);

            if (playerCards.Count == 1)
            {
                ShowUnoButton();
            }
            else
            {
                HideUnoButton(currentPlayer);
            }
        }

        private void UpdateCardList(Dictionary<int, Panel> cardPanels, List<Card> currentCards, List<Card> newCards, Panel targetPanel, bool isPlayer)
        {
            foreach (var card in currentCards.ToList())
            {
                if (!newCards.Contains(card))
                {
                    if (cardPanels.TryGetValue(card.Id, out var cardPanel))
                    {
                        targetPanel.Controls.Remove(cardPanel);
                        cardPanels.Remove(card.Id); 
                    }
                }
            }

            int currentXPosition = 0; 

            foreach (var card in currentCards)
            {
                if (cardPanels.TryGetValue(card.Id, out var cardPanel))
                {
                    cardPanel.Location = new Point(currentXPosition, cardPanel.Location.Y);
                    UpdatePictureBoxPosition(cardPanel);
                    currentXPosition += cardPanel.Width; 
                }
            }

            foreach (var card in newCards)
            {
                if (!cardPanels.ContainsKey(card.Id))
                {
                    var newCardPanel = DrawCard(card, isPlayer, false, false);
                    newCardPanel.Location = new Point(currentXPosition, newCardPanel.Location.Y);
                    UpdatePictureBoxPosition(newCardPanel); 
                    targetPanel.Controls.Add(newCardPanel);
                    cardPanels[card.Id] = newCardPanel; 

                    currentXPosition += newCardPanel.Width;
                }
            }

            if (currentXPosition > targetPanel.Width)
            {
                int overlapOffset = (currentXPosition - targetPanel.Width) / newCards.Count;
                currentXPosition = 0;

                foreach (var card in newCards)
                {
                    if (cardPanels.TryGetValue(card.Id, out var cardPanel))
                    {
                        cardPanel.Location = new Point(currentXPosition, cardPanel.Location.Y);
                        UpdatePictureBoxPosition(cardPanel); 
                        currentXPosition += cardPanel.Width - overlapOffset;
                    }
                }
            }

            targetPanel.Invalidate();
        }

        private void UpdatePictureBoxPosition(Panel cardPanel)
        {
            if (cardPanel.Controls.Count > 0 && cardPanel.Controls[0] is PictureBox pictureBox)
            {
                pictureBox.Location = new Point(0, 0);
                pictureBox.Size = cardPanel.Size;
            }
        }



        void ShowUnoButton()
        {
            Button btnUno = new Button();
            btnUno.Text = "Uno";
            btnUno.Location = new System.Drawing.Point(750, 350);
            btnUno.Size = new System.Drawing.Size(100, 80);

            btnUno.Click += (sender, e) =>
            {
                var packet = new Packet
                {
                    Command = Protocol.UNO,

                };

                byte[] packetData = packet.ToBytes();

                gameClient.SendMessage(packet.Command);

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
