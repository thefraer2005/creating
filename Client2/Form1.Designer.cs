using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Common;
using Newtonsoft.Json;
namespace Client2
{
    partial class Form1
    {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
      
       
        public int SelectedPlayersCount { get; private set; }
        private System.Windows.Forms.Label lblFirstCard;


        private System.Windows.Forms.Label lblWaiting;
        private System.Windows.Forms.PictureBox pbLoading;

        private System.Windows.Forms.Panel flpPlayerCards;
        private System.Windows.Forms.Panel flpOpponentCards;
        private System.Windows.Forms.Panel pnlCenterCard;
        private System.Windows.Forms.Panel pnlDeck;
        private Panel pnlColorIndicator;
        private Label currentPlayerIndicator;
        private Dictionary<int, Panel> playerCardPanels = new Dictionary<int, Panel>();
        private Dictionary<int, Panel> opponentCardPanels = new Dictionary<int, Panel>();

        private void InitializeComponent()
        {



            this.lblWaiting = new System.Windows.Forms.Label();
            this.lblWaiting.Text = "Ждем подключения...";
            this.lblWaiting.AutoSize = true;
            this.Controls.Add(lblWaiting);

            this.pbLoading = new System.Windows.Forms.PictureBox();
            this.pbLoading.Image = Image.FromFile("C:/Users/ASUS/source/repos/Server/Server/img/wait.gif"); 
            this.pbLoading.SizeMode = PictureBoxSizeMode.StretchImage;
                                                                    

            this.Controls.Add(pbLoading);

            this.components = new System.ComponentModel.Container();
            this.ClientSize = new System.Drawing.Size(700, 700);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(800, 50);
            this.BackColor = Color.Khaki;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            this.Text = "Form2";

           
            this.pnlCenterCard = new System.Windows.Forms.Panel();
            this.pnlCenterCard.Visible = false;
            this.Controls.Add(this.pnlCenterCard);

            this.pnlColorIndicator = new System.Windows.Forms.Panel();

            this.pnlColorIndicator.Visible = false;
            this.Controls.Add(this.pnlColorIndicator);


            this.currentPlayerIndicator = new Label();
            this.currentPlayerIndicator.Visible = false; 
            this.Controls.Add(currentPlayerIndicator);
            this.pnlDeck = new System.Windows.Forms.Panel();

            this.pnlDeck.Visible = false;


            this.Controls.Add(this.pnlDeck);

            this.flpPlayerCards = new System.Windows.Forms.Panel();
            this.flpPlayerCards.Name = "flpPlayerCards";
            this.flpPlayerCards.BackColor = System.Drawing.Color.Khaki;
            this.flpPlayerCards.Visible = false;
            this.Controls.Add(this.flpPlayerCards);

            this.flpOpponentCards = new System.Windows.Forms.Panel();
            this.flpOpponentCards.Name = "flpOpponentCards";
            this.flpOpponentCards.Visible = false;
            this.flpOpponentCards.BackColor = System.Drawing.Color.Khaki;


            this.currentPlayerIndicator.Visible = false;


            this.Controls.Add(this.flpOpponentCards);
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (victoryForm != null && !victoryForm.IsDisposed)
            {
                victoryForm.UpdateSizeAndPosition(this); 
            }
            UpdateCardList(playerCardPanels, playerCards, playerCards, flpPlayerCards, true);
            UpdateCardList(opponentCardPanels, opponentCards, opponentCards, flpOpponentCards, false);
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int formWidth = this.ClientSize.Width;
            int formHeight = this.ClientSize.Height;

            currentPlayerIndicator.Size = new Size((int)(formWidth * 0.04), (int)(formHeight * 0.04));

            pnlColorIndicator.Size = new Size((int)(formWidth * 0.05), (int)(formWidth * 0.05));
            pnlColorIndicator.Location = new Point((int)(this.ClientSize.Width * 0.67), (int)(this.ClientSize.Height * 0.55));


            lblWaiting.Location = new Point((int)(formWidth * 0.35), (int)(formHeight * 0.35));
            int fontSize = Math.Min((int)(this.Height * 0.03), (int)(this.Width * 0.03));
            lblWaiting.Font = new Font("Arial", fontSize, FontStyle.Bold);

            pbLoading.Location = new Point((int)(formWidth * 0.4), (int)(formHeight * 0.4));
            pbLoading.Size = new Size((int)(formWidth * 0.2), (int)(formHeight * 0.2));

            flpPlayerCards.Size = new Size((int)(formWidth * 0.9), (int)(formHeight * 0.225));
            flpPlayerCards.Location = new Point((int)(formWidth * 0.03), (int)(formHeight * 0.725)); 

            flpOpponentCards.Size = new Size((int)(formWidth * 0.9), (int)(formHeight * 0.225));
            flpOpponentCards.Location = new Point((int)(formWidth * 0.03), (int)(formHeight * 0.05));

            pnlDeck.Size = new Size((int)(formWidth * 0.15), (int)(formHeight * 0.225));
            pnlDeck.Location = new Point((int)(formWidth * 0.3), (int)(formHeight * 0.4)); 



            pnlCenterCard.Size = new Size((int)(formWidth * 0.15), (int)(formHeight * 0.225));
            pnlCenterCard.Location = new Point((int)(formWidth * 0.47), (int)(formHeight * 0.4));

            UpdateCardSizes();

            UpdateCurrentPlayerIndicator(currentPlayer);
        }

        private void UpdateCurrentPlayerIndicator(string currentTurnNickname)
        {
           
            int formWidth = this.ClientSize.Width;
            int formHeight = this.ClientSize.Height;

            if (currentTurnNickname == nickName)
            {
                currentPlayerIndicator.Location = new Point(flpPlayerCards.Location.X + flpPlayerCards.Width + 10,
                    flpPlayerCards.Location.Y + (flpPlayerCards.Height / 2) - (currentPlayerIndicator.Height / 2));
                currentPlayerIndicator.Visible = true; 
            }
            else
            {
                currentPlayerIndicator.Location = new Point(flpOpponentCards.Location.X + flpOpponentCards.Width + 10,
                    flpOpponentCards.Location.Y + (flpOpponentCards.Height / 2) - (currentPlayerIndicator.Height / 2));
                currentPlayerIndicator.Visible = true;
            }
        }


        private void UpdateCardSizes()
        {
            int cardWidth = (int)(this.ClientSize.Width * 0.15); 
            int cardHeight = (int)(this.ClientSize.Height * 0.225); 


            if (pnlDeck.Controls.Count > 0)
            {
                foreach (Control card in pnlDeck.Controls)
                {
                    card.Size = new Size(cardWidth, cardHeight);

                }
            }
            if (pnlCenterCard.Controls.Count > 0)
            {
                foreach (Control card in pnlCenterCard.Controls)
                {
                    card.Size = new Size(cardWidth, cardHeight);

                }
            }

            foreach (Control card in flpPlayerCards.Controls)
            {
                if (card is Panel)
                {
                    card.Size = new Size(cardWidth, cardHeight);
                }
            }

            foreach (Control card in flpOpponentCards.Controls)
            {
                if (card is Panel)
                {
                    card.Size = new Size(cardWidth, cardHeight);
                }
            }
        }

        private Panel DrawCard(Card card, bool isPlayerCard, bool firstCard, bool main)
        {
            int cardWidth = (int)(this.ClientSize.Width * 0.15);
            int cardHeight = (int)(this.ClientSize.Height * 0.225);
            Panel cardPanel = new Panel
            {
                Size = new Size(cardWidth, cardHeight)

            };

            string imagePath = isPlayerCard ? GetImagePath(card) : GetFaceDownImagePath();
            if (main)
            {
                imagePath = GetFaceDownImagePath();
            }

            PictureBox pictureBox = new PictureBox
            {
                Image = Image.FromFile(imagePath),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };

            cardPanel.Controls.Add(pictureBox); 

            if (main)
            {
                pictureBox.Click += (sender, e) =>
                {
                    if (currentPlayer.Equals(nickName)) 
                    {
                        OnColodaClick();
                    }
                    else
                    {
                        MessageBox.Show("Сейчас не ваш ход!");
                    }
                };
                pnlDeck.Controls.Add(cardPanel);
            }
            else
            {
                if (firstCard)
                {
                    pnlCenterCard.Controls.Add(cardPanel);
                }
                else
                {
                    pictureBox.Click += (sender, e) =>
                    {
                        if (currentPlayer.Equals(nickName)) 
                        {
                            OnCardClick(card);
                        }
                        else
                        {
                            MessageBox.Show("Сейчас не ваш ход!");
                        }
                    };
                }
            }

            return cardPanel; 
        }

        public void OnCardClick(Card card)
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
                    if (HasNoMatchingCard(playerCards, centralCard))
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
                    else
                    {
                        MessageBox.Show("У вас есть  карта, соответствующая центральной. Вы не можете сыграть эту карту.");
                    }
                }
                else
                {
                    SendPlayCardPacket(card);
                }
            }
            else
            {
                MessageBox.Show("Неверный ход! Карта не соответствует центральной.");
            }
        }
        private void SendPlayCardPacket(Card card, CardColor? selectedColor = null)
        {
            if (selectedColor != null)
            {
                card.Color = selectedColor.ToString();
            }
           


            string jsonData = JsonConvert.SerializeObject(card);
            byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);

            var packet = new Packet
            {
                Command = Protocol.PlayCard,
                Data = dataBytes
            };

            byte[] packetData = packet.ToBytes();

            gameClient.SendMessage(packet.Command, packet.Data);
        }
        private bool HasNoMatchingCard(List<Card> hand, Card centralCard)
        {
            foreach (var handCard in hand)
            {

                if (handCard.Color == centralCard.Color)
                {
                    if (handCard.Type == CardType.Wild || handCard.Type == CardType.WildDrawFour)
                    { return true; }
                    else
                    {
                        return false;
                    }
                }

            }
            return true;

        }




        private bool IsValidMove(Card card, Card centralCard)
        {
           
            if (card.Type == CardType.Wild || card.Type == CardType.WildDrawFour)
            {
              
                return true;
            }
           
            // Проверка на соответствие по цвету или значению
            return card.Color == centralCard.Color || card.Value == centralCard.Value;
        }
        private bool IsValidСoloda(List<Card> playerCards, Card centralCard)
        {
            foreach (var card in playerCards)
            {
                if (IsValidMove(card, centralCard))
                {
                    return true;
                }
            }

            return false;
        }
        private void OnColodaClick()
        {
            if (!IsValidСoloda(playerCards, centralCard))
            {

                var packet = new Packet
                {
                    Command = Protocol.GiveCard,

                };

                byte[] packetData = packet.ToBytes();

                gameClient.SendMessage(packet.Command);
            }
            else
            {
                MessageBox.Show("У вас есть подходящие карты");
            }
        }


        private string GetFaceDownImagePath()
        {
            return Path.Combine("C:/Users/ASUS/source/repos/Server/Server", "img", "ggg1.png");
        }



        private string GetImagePath(Card card)
        {
            string baseImagePath = Path.Combine("C:/Users/ASUS/source/repos/Server/Server", "img");

            string imageFileName;

            switch (card.Type)
            {
                case CardType.Number:
                    imageFileName = $"{card.Color}- {card.Value}.png";
                    break;

                case CardType.Skip:
                    imageFileName = $"{card.Color} Skip- 1.png";
                    break;

                case CardType.Reverse:
                    imageFileName = $"{card.Color} Reverse- 1.png";
                    break;

                case CardType.DrawTwo:
                    imageFileName = $"{card.Color} Draw2- 1.png";
                    break;

                case CardType.Wild:
                    imageFileName = "Wild- 1.png";
                    break;

                case CardType.WildDrawFour:
                    imageFileName = "Draw4- 1.png";
                    break;

                default:
                    imageFileName = "Draw4- 1.png";
                    break;
            }

            return Path.Combine(baseImagePath, imageFileName);
        }






        #endregion
    }
}