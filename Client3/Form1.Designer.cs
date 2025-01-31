
using System.Text;
using System.Windows.Forms;
using Common;
using Newtonsoft.Json;
namespace Client3
{
    using System.Windows.Forms;

    public class CardPictureBox : PictureBox
    {
        public int CardId { get; set; }
    }

    public class RotatedCard : Control
    {
        private Image _image;

        public Image CardImage
        {
            get => _image;
            set
            {
                _image = value;
                Invalidate(); 
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_image != null)
            {
                // Установим центр вращения
                e.Graphics.TranslateTransform(Width / 2, Height / 2);
                e.Graphics.RotateTransform(90);
                e.Graphics.TranslateTransform(-_image.Height / 2, -_image.Width / 2); 
                e.Graphics.DrawImage(_image, new Point(0, 0));
            }
        }

        protected override Size DefaultSize => new Size(100, 150); 
    }
    public class RoundPanel : Panel
    {
        public Color CircleColor { get; set; } = Color.Transparent; 

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

          
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        
            using (Brush brush = new SolidBrush(CircleColor))
            {
                e.Graphics.FillEllipse(brush, 0, 0, Width, Height);
            }

        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate(); 
        }


    }
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
        private System.Windows.Forms.Label player1;
        private System.Windows.Forms.Label player2;
        private System.Windows.Forms.Label player3;
        private System.Windows.Forms.Label player4;
        private System.Windows.Forms.PictureBox pbLoading;

        private System.Windows.Forms.Panel flpPlayerCards;

        private System.Windows.Forms.Panel flpOpponentCards;
        private System.Windows.Forms.Panel flpOpponentCards3;
        private System.Windows.Forms.Panel flpOpponentCards4;
        private System.Windows.Forms.Panel pnlCenterCard;
        private System.Windows.Forms.Panel pnlDeck;
        private System.Windows.Forms.Button btnUno;
        private RoundPanel pnlColorIndicator;

        private Dictionary<int, Panel> playerCardPanels = new Dictionary<int, Panel>();
        private Dictionary<int, Panel> opponentCardPanels = new Dictionary<int, Panel>();
        private Dictionary<int, Panel> opponentCardPanels3 = new Dictionary<int, Panel>();
        private Dictionary<int, Panel> opponentCardPanels4 = new Dictionary<int, Panel>();

        private void InitializeComponent()
        {

           

            this.lblWaiting = new System.Windows.Forms.Label();
            this.lblWaiting.Text = "Ждем подключения остальных...";
            this.lblWaiting.AutoSize = true;
            this.Controls.Add(lblWaiting);

            this.player1 = new System.Windows.Forms.Label();
            this.player1.Text ="";
            this.player1.Visible = false;
            this.player1.AutoSize = true;
            this.player1.ForeColor = Color.Black;
            this.player1.BackColor = Color.Transparent;
            this.Controls.Add(player1);

            this.player2 = new System.Windows.Forms.Label();
            this.player2.Text = "";
            this.player2.AutoSize = true;
            this.player2.BackColor = Color.Transparent;
            this.player2.ForeColor = Color.Black;
            this.player2.Visible = false;
            this.Controls.Add(player2);

            this.player4 = new System.Windows.Forms.Label();
            this.player4.Text = "";
            this.player4.Visible = false; 
            this.player4.ForeColor = Color.Black;
            this.player4.BackColor = Color.Transparent;
            this.player4.AutoSize = true;
            this.Controls.Add(player4);

            this.player3 = new System.Windows.Forms.Label();
            this.player3.Text = "";
            this.player3.ForeColor = Color.Black;
            this.player3.BackColor = Color.Transparent;
            this.player3.Visible = false;
            this.player3.AutoSize = true;
            this.Controls.Add(player3);



            this.pbLoading = new System.Windows.Forms.PictureBox();
            this.pbLoading.Image = Image.FromFile("C:/Users/ASUS/source/repos/Server/Server/img/wait.gif"); 
            this.pbLoading.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(pbLoading);
        

            this.components = new System.ComponentModel.Container();
            this.ClientSize = new System.Drawing.Size(700, 700);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(800, 50);
            this.BackColor = Color.Khaki ;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            this.Text = "Form2";

            this.pnlCenterCard = new System.Windows.Forms.Panel();
            this.pnlCenterCard.Visible = false;
            this.pnlCenterCard.BackColor = Color.Transparent;
            this.Controls.Add(this.pnlCenterCard);

            this.pnlColorIndicator = new RoundPanel();
            this.pnlColorIndicator.Size = new Size(50, 50);
            this.pnlColorIndicator.Visible = true;
            this.pnlColorIndicator.BackColor = Color.Transparent;
            this.Controls.Add(this.pnlColorIndicator);
            this.pnlDeck = new System.Windows.Forms.Panel();
            this.pnlDeck.BackColor = Color.Transparent; 
            this.pnlDeck.Visible = false;
            this.Controls.Add(this.pnlDeck);

        
            this.flpPlayerCards = new System.Windows.Forms.Panel();
            this.flpPlayerCards.Name = "flpPlayerCards";
            this.flpPlayerCards.BackColor = System.Drawing.Color.Transparent;
            this.flpPlayerCards.Visible = false;
            flpPlayerCards.ApplyRoundedCorners(20);
            this.Controls.Add(this.flpPlayerCards);

            
            this.flpOpponentCards = new System.Windows.Forms.Panel();
            this.flpOpponentCards.Name = "flpOpponentCards";
            this.flpOpponentCards.Visible = false;
            flpOpponentCards.ApplyRoundedCorners(20);
            this.flpOpponentCards.BackColor = System.Drawing.Color.Khaki;
            this.Controls.Add(this.flpOpponentCards);

            
            this.flpOpponentCards3 = new System.Windows.Forms.Panel();
            this.flpOpponentCards3.Name = "flpOpponentCards";
            flpOpponentCards3.ApplyRoundedCorners(20);
            this.flpOpponentCards3.Visible = false;
            this.flpOpponentCards3.BackColor = Color.Khaki;
            this.Controls.Add(this.flpOpponentCards3);

            
            this.flpOpponentCards4 = new System.Windows.Forms.Panel();
            this.flpOpponentCards4.BackColor = Color.Khaki;
            flpOpponentCards4.ApplyRoundedCorners(20);
            this.flpOpponentCards4.Name = "flpOpponentCards";
             this.flpOpponentCards4.Visible = false;
            this.Controls.Add(this.flpOpponentCards4);

            this.DoubleBuffered = true;



           

           

        }
        private async void Form1_Resize(object sender, EventArgs e)
        {
           
            if (playerHands.Keys.Count() >= 2)
            {
                await UpdateCardList(playerCardPanels, playerHands[nickName], playerHands[nickName], flpPlayerCards, true, false, nickName);

                switch (otherPlayers.Count)
                {
                    case 2:
                        await UpdateCardList(opponentCardPanels, playerHands[otherPlayers[nextIndex]], playerHands[otherPlayers[nextIndex]], flpOpponentCards, false, false, otherPlayers[nextIndex]); // Обновление списка карт противника
                                                                                                                                                                                                      // Обновление списка карт противника

                        break;
                    case 3:
                        await UpdateCardList(opponentCardPanels3, playerHands[otherPlayers[nextIndex]], playerHands[otherPlayers[nextIndex]], flpOpponentCards3, false, true, otherPlayers[nextIndex]); // Обновление списка карт противника

                        await UpdateCardList(opponentCardPanels4, playerHands[otherPlayers[previousIndex]], playerHands[otherPlayers[previousIndex]], flpOpponentCards4, false, true, otherPlayers[previousIndex]); // Обновление списка карт противника

                        break;
                    case 4:
                        await UpdateCardList(opponentCardPanels3, playerHands[otherPlayers[nextIndex]], playerHands[otherPlayers[nextIndex]], flpOpponentCards3, false, true, otherPlayers[nextIndex]); // Обновление списка карт противника
                        await UpdateCardList(opponentCardPanels, playerHands[otherPlayers[nextNextIndex]], playerHands[otherPlayers[nextNextIndex]], flpOpponentCards, false, false, otherPlayers[nextNextIndex]);
                        await UpdateCardList(opponentCardPanels4, playerHands[otherPlayers[previousIndex]], playerHands[otherPlayers[previousIndex]], flpOpponentCards4, false, true, otherPlayers[previousIndex]); // Обновление списка карт противника

                        break;
                }
            }

            AdjustLayout();
        }


        private void AdjustLayout()
        {
            int formWidth = this.ClientSize.Width;
            int formHeight = this.ClientSize.Height;
            if (this.Controls.Contains(btnUno))
            {
               
                btnUno.Size = new Size((int)(formWidth * 0.2), (int)(formHeight * 0.1));
                btnUno.Location = new Point((int)(formWidth * 0.51), (int)(formHeight * 0.42));
            }

            pnlColorIndicator.Size = new Size((int)(formWidth * 0.05), (int)(formWidth * 0.05));
            pnlColorIndicator.Location = new Point((int)(this.ClientSize.Width * 0.53), (int)(this.ClientSize.Height * 0.525));

            flpPlayerCards.Size = new Size((int)(formWidth * 0.5), (int)(formHeight * 0.2));
            flpPlayerCards.Location = new Point((int)(formWidth * 0.25), (int)(formHeight * 0.8));

           
            player1.Location = new Point((int)(formWidth * 0.25), (int)(formHeight * 0.76));
            player1.Font = new Font("Arial", formHeight * 0.02f, FontStyle.Bold);



            flpOpponentCards.Size = new Size((int)(formWidth * 0.5), (int)(formHeight * 0.2));
            flpOpponentCards.Location = new Point((int)(formWidth * 0.25), (int)(formHeight * 0));

            
            player2.Location = new Point((int)(formWidth * 0.25), (int)(formHeight * 0.2));
            player2.Font = new Font("Arial", formHeight * 0.02f, FontStyle.Bold);

            flpOpponentCards3.Size = new Size((int)(formWidth * 0.2), (int)(formHeight * 0.5));
            flpOpponentCards3.Location = new Point((int)(formWidth * 0.8), (int)(formHeight * 0.25));

            
            player3.Location = new Point((int)(formWidth * 0.8), (int)(formHeight * 0.22));
            player3.Font = new Font("Arial", formHeight * 0.02f, FontStyle.Bold);


            flpOpponentCards4.Size = new Size((int)(formWidth * 0.2), (int)(formHeight * 0.5));
            flpOpponentCards4.Location = new Point((int)(formWidth * 0), (int)(formHeight * 0.25));
            
            player4.Location = new Point((int)(formWidth * 0), (int)(formHeight * 0.22));
            player4.Font = new Font("Arial", formHeight * 0.02f, FontStyle.Bold);

            lblWaiting.Location = new Point((int)(formWidth * 0.25), (int)(formHeight * 0.3));
            int fontSize = Math.Min((int)(this.Height * 0.025), (int)(this.Width * 0.025));
            lblWaiting.Font = new Font("Arial", fontSize, FontStyle.Bold);

            pbLoading.Location = new Point((int)(formWidth * 0.3), (int)(formHeight * 0.35));
            pbLoading.Size = new Size((int)(formWidth * 0.3), (int)(formHeight * 0.3));
            
            pnlDeck.Size = new Size((int)(formWidth * 0.1), (int)(formHeight * 0.15));
            pnlDeck.Location = new Point((int)(formWidth * 0.29), (int)(formHeight * 0.425)); 



            pnlCenterCard.Size = new Size((int)(formWidth * 0.1), (int)(formHeight * 0.15));
            pnlCenterCard.Location = new Point((int)(formWidth * 0.4), (int)(formHeight * 0.425));
            flpPlayerCards.ApplyRoundedCorners(20); 
            flpOpponentCards.ApplyRoundedCorners(20);
            flpOpponentCards3.ApplyRoundedCorners(20);
            flpOpponentCards4.ApplyRoundedCorners(20);
            UpdateCardSizes();

          
        }

        private void UpdateCurrentPlayerIndicator(string currentTurnNickname)
        {
            Panel currentPanel = playerPanels[currentTurnNickname];
            foreach (var panel in playerPanels.Values)
            {

                if(panel== currentPanel)
                {
                    panel.BackColor = Color.Brown ;

                }
                else { panel.BackColor = Color.Khaki; }
                
               

                panel.Invalidate();
            }

        }


      




        private void UpdateCardSizes()
        {
            int cardWidth = (int)(this.ClientSize.Width * 0.10); 
            int cardHeight = (int)(this.ClientSize.Height * 0.15); 


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
            foreach (Control card in flpOpponentCards3.Controls)
            {
                if (card is Panel)
                {
                    card.Size = new Size(cardHeight, cardWidth);
                }
            }
            foreach (Control card in flpOpponentCards4.Controls)
            {
                if (card is Panel)
                {
                    card.Size = new Size(cardHeight, cardWidth);
                }
            }
            

        }
        private Image RotateImage(string imagePath)
        {
            using (Image originalImage = Image.FromFile(imagePath))
            {
               
                Bitmap rotatedImage = new Bitmap(originalImage.Height, originalImage.Width);

                using (Graphics g = Graphics.FromImage(rotatedImage))
                {
                  
                    g.TranslateTransform(rotatedImage.Width / 2, rotatedImage.Height / 2);
                    g.RotateTransform(90);
                    g.TranslateTransform(-originalImage.Width / 2, -originalImage.Height / 2);

                 
                    g.DrawImage(originalImage, new Point(0, 0));
                }

                return rotatedImage;
            }
        }

        private async  Task<Panel> DrawCard(Card card, bool isPlayerCard, bool firstCard, bool main, bool many)
        {
          


            int cardWidth = (int)(this.ClientSize.Width * 0.1);
            int cardHeight = (int)(this.ClientSize.Height * 0.15);

            Panel cardPanel = new Panel
            {
                Size = new Size(cardWidth, cardHeight),
               
            };

            string imagePath = isPlayerCard ? await GetImagePath(card) : await MinioHelper.GetImageUrl("unobucket", "ggg1.png", 600);
            CardPictureBox pictureBox = new CardPictureBox();

            using (HttpClient client = new HttpClient())
            {
               
                if (Uri.IsWellFormedUriString(imagePath, UriKind.Absolute))
                {
                    
                    var response = await client.GetAsync(imagePath);
                    if (response.IsSuccessStatusCode)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            pictureBox.Image = Image.FromStream(stream);
                        }
                    }
                    
                }
               
            }

            
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.CardId = card.Id; 

           
            if (many)
            {
                cardPanel.Size = new Size(cardHeight, cardWidth);
                pictureBox.Image.RotateFlip(RotateFlipType.Rotate90FlipNone); 
            }


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
                       
                        ShowErrorDialog("Сейчас не ваш ход.");
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
                
            }

            return cardPanel;
        }
       


       




        private void SendPlayCardPacket(Card card, CardColor? selectedColor = null)
        {
            if (selectedColor != null)
            {
                card.Color = selectedColor.Value;
            }
           


            string jsonData = JsonConvert.SerializeObject(card);
            byte[] dataBytes = Encoding.UTF8.GetBytes(jsonData);

            

            gameClient.SendMessage(UnoCommand.PLAY_CARD, dataBytes);
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
            if (!IsValidСoloda(playerHands[nickName], centralCard))
            {


                gameClient.SendMessage(UnoCommand.GIVECARD);
            }
            else
            {
               
                ShowErrorDialog("У вас есть подходящие карты.");
            }
        }


       



        private async Task<string?> GetImagePath(Card card)
        {
           
           
            string colorName = card.Color.ToString();

          
            string imageFileName;

            switch (card.Type)
            {
                case CardType.Number:
                    imageFileName = $"{colorName}- {(byte)card.Value}.png";
                    break;

                case CardType.Skip:
                    imageFileName = $"{colorName} Skip- 1.png";
                    break;

                case CardType.Reverse:
                    imageFileName = $"{colorName} Reverse- 1.png";
                    break;

                case CardType.DrawTwo:
                    imageFileName = $"{colorName} Draw2- 1.png";
                    break;

                case CardType.Wild:
                    imageFileName = "Wild- 1.png";
                    break;

                case CardType.WildDrawFour:
                    imageFileName = "Draw4- 1.png";
                    break;

                default:
                    throw new ArgumentException("Неизвестный тип карты");
            }
            string? resultImageFileName = await MinioHelper.GetImageUrl("unobucket", imageFileName, 600);

            
            return resultImageFileName;
        }






        #endregion
    }
}