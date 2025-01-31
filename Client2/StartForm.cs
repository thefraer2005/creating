using Common;

using System.Drawing.Drawing2D;

using System.Text;

namespace Client2
{
    public static class ControlExtensions
    {
        public static void ApplyRoundedCorners(this Control control, int cornerRadius)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.StartFigure();
                path.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
                path.AddArc(control.Width - cornerRadius - 1, 0, cornerRadius, cornerRadius, 270, 90);
                path.AddArc(control.Width - cornerRadius - 1, control.Height - cornerRadius - 1, cornerRadius, cornerRadius, 0, 90);
                path.AddArc(0, control.Height - cornerRadius - 1, cornerRadius, cornerRadius, 90, 90);
                path.CloseFigure();
                control.Region = new Region(path);
            }
        }
    }
   

    public partial class StartForm : Form
    {
        private Label lblNicknamePrompt;
        private Label titleLabel;
        private ImageHandler playBtnHandler;
        private ImageHandler select2;
        private ImageHandler select3;
        private ImageHandler select4;
        private GameClient gameClient;
        private GameClient gameClientRestart;
        private int selectedPlayersCount;
        private Button btnStartGame;
        private TextBox txtNickname; 
        private Label lblErrorMessage;
        private ImageHandler backGroundImg;
        private string nickname;
        private PictureBox picStartGame;
        private PictureBox picTwoPlayers;
        private PictureBox picThreePlayers;
        private PictureBox picFourPlayers;
        public StartForm()
        {
            select2 = new ImageHandler("222.png");
            select3 = new ImageHandler("333.png");
            select4 = new ImageHandler("444.png");
            playBtnHandler = new ImageHandler("play-button.png");
            backGroundImg= new ImageHandler("main_img.jpg");
            InitializeComponent();

            InitializeGameClient();
            this.Resize += Form_Resize;


        }
        private void Form_Resize(object sender, EventArgs e)
        {
            UpdateControlSizes(); 
        }

       
        private async void LoadInitialImage(PictureBox pictureBox,string nameImg,ImageHandler imgHandler)
        {
            await imgHandler.LoadImageAsync(pictureBox, "unobucket", nameImg);
        }


        private void InitializeGameClient()
        {
            gameClient = new GameClient();
            gameClient.Error += HandleGameStarted; 
        }
        private void picStartGame_Click(object sender, EventArgs e)
        {
            nickname = txtNickname.Text.Trim(); 

            if (string.IsNullOrEmpty(nickname)) 
            {
                lblErrorMessage.Visible = true;
                lblErrorMessage.Text = "Пожалуйста, введите ник."; 
                return;
            }

            byte[] nicknameBytes = Encoding.UTF8.GetBytes(nickname); 
            this.gameClient.Connect("127.0.0.1");
            this.gameClient.SendMessage(UnoCommand.CONNECT, nicknameBytes);

            this.lblErrorMessage.Visible = false;
            this.picStartGame.Visible = false;
            this.txtNickname.Visible = false;
            lblNicknamePrompt.Visible = false;
            UpdateControlSizes();
            picTwoPlayers.Visible = true;
             picThreePlayers.Visible = true;
            picFourPlayers.Visible = true;
            titleLabel.Visible = true;

        }
        public void RestartGame()
        {
            InitializeGameClient();
            txtNickname.Visible = true; 
            picStartGame.Visible = true; 
            lblErrorMessage.Text = ""; 
        }





        private async void FormStartGame_Load(object sender, EventArgs e)
        {
            Image? backgroundImage = await backGroundImg.LoadImageFromMinio("unobucket", "main_img.jpg");

            if (backgroundImage != null)
            {
                this.BackgroundImage = backgroundImage;
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }

        private void InitializeComponent()
        {
             
            ImageHandler backGroundImg = new ImageHandler("main_img.jpg");
           
            this.ClientSize = new System.Drawing.Size(400, 400);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(500, 10);
            this.Name = "FormStartGame";
            this.Text = "Начало игры1";
            this.Load += new System.EventHandler(this.FormStartGame_Load);

            int panelWidth = this.Width;
            int panelHeight = this.Height;
            int imageWidth = (int)(this.Width * 0.3);
            int imageHeight = (int)(this.Height * 0.3);

            this.picStartGame = new PictureBox();
            picStartGame.Size = new System.Drawing.Size((int)(this.Width * 0.3), (int)(this.Height * 0.3)); 
            picStartGame.SizeMode = PictureBoxSizeMode.StretchImage; 
            picStartGame.Location = new System.Drawing.Point(
                (this.ClientSize.Width - picStartGame.Width) / 2,
                (this.ClientSize.Height - picStartGame.Height) / 2
            );
            picStartGame.BackColor = Color.Transparent;

            picStartGame.Click += new System.EventHandler(this.picStartGame_Click);
            picStartGame.MouseLeave += playBtnHandler.PictureBox_MouseLeave;
            picStartGame.MouseDown += playBtnHandler.PictureBox_MouseDown;
            picStartGame.MouseUp += playBtnHandler.PictureBox_MouseUp;
            picStartGame.MouseEnter += playBtnHandler.PictureBox_MouseEnter;
            LoadInitialImage(picStartGame,"play-button.png", playBtnHandler);
            this.Controls.Add(picStartGame);

            this.txtNickname = CreateTextBox((int)(panelWidth * 0.35), (int)(panelHeight * 0.25),
                                                  (int)(panelWidth * 0.3), (int)(panelHeight * 0.1));
            txtNickname.ApplyRoundedCorners(20);
            this.Controls.Add(this.txtNickname);
            this.lblNicknamePrompt = CreateLabel("Кто ты воин?",
                                          (int)(panelWidth * 0.2), (int)(panelHeight * 0.1),
                                          (int)(panelWidth * 0.6), (int)(panelHeight * 0.1),
                                           new Font("Arial", 30, FontStyle.Bold));

            this.Controls.Add(this.lblNicknamePrompt);

          

          
            this.lblErrorMessage = CreateLabel("",
                                                (this.ClientSize.Width - 300) / 2,
                                                (this.ClientSize.Height - this.txtNickname.Height) / 2 + 150,
                                                300, 30,
                                                new Font("Arial", 14, FontStyle.Bold));
            lblErrorMessage.Visible = false; 
            this.Controls.Add(this.lblErrorMessage);

            this.titleLabel = CreateLabel($"На сколько человек будет вечеринка {nickname}?)",
                                (int)(panelWidth * 0.05), (int)(panelHeight * 0.2), 
                                 (int)(panelWidth * 0.9), (int)(panelHeight * 0.1),
                                 new Font("Arial", 16, FontStyle.Bold));
            titleLabel.Visible = false;
            this.Controls.Add(this.titleLabel);

           
            int spacing = 20;


            picTwoPlayers = CreatePictureBox(select2, imageWidth, imageHeight,
                                     (int)(this.Width * 0.005),
                                     (int)(this.Height * 0.35), 2);
            picTwoPlayers.Visible = false;
            picTwoPlayers.ApplyRoundedCorners(20);
            picThreePlayers = CreatePictureBox(select3, imageWidth, imageHeight,
                                     (int)(this.Width * 0.34),
                                     (int)(this.Height * 0.35), 3);
            picThreePlayers.ApplyRoundedCorners(20);
            picThreePlayers.Visible = false;
            picFourPlayers = CreatePictureBox(select4, imageWidth, imageHeight,
                                     (int)(this.Width * 0.66),
                                     (int)(this.Height * 0.35), 4);
            picFourPlayers.ApplyRoundedCorners(20);
            picFourPlayers.Visible = false;
            LoadInitialImage(picTwoPlayers, "222.png", select2);
            LoadInitialImage(picThreePlayers, "333.png", select3);
            LoadInitialImage(picFourPlayers, "444.png", select4);

            this.Controls.Add(picTwoPlayers);
            this.Controls.Add(picThreePlayers);
            this.Controls.Add(picFourPlayers);


        }


        private Label CreateLabel(string text, int x, int y, int width, int height, Font font)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = font
            };
        }

        private TextBox CreateTextBox(int x, int y, int width, int height)
        {
            TextBox textBox = new TextBox();
            textBox.Location = new Point(x, y);
            textBox.Size = new Size(width, height);
            textBox.BackColor = Color.Black;
            textBox.ForeColor = Color.White;
            textBox.Font = new Font("Arial", 14, FontStyle.Bold);
            return textBox;
        }



        private void UpdateControlSizes()
        {
            int panelWidth = this.Width;
            int panelHeight = this.Height;
            int imageWidth = (int)(this.Width * 0.25);
            int imageHeight = (int)(this.Height * 0.25);
            txtNickname.ApplyRoundedCorners(20);
            this.txtNickname.Size = new Size((int)(panelWidth * 0.3), (int)(panelHeight * 0.1));
            this.txtNickname.Location = new Point((int)(panelWidth * 0.35), (int)(panelHeight * 0.25));
            this.txtNickname.Font = new Font("Arial", panelHeight * 0.02f, FontStyle.Bold);

            lblNicknamePrompt.Size = new Size((int)(panelWidth * 0.6), (int)(panelHeight * 0.1));
            lblNicknamePrompt.Location = new Point((int)(panelWidth * 0.2), (int)(panelHeight * 0.1));
            lblNicknamePrompt.Font = new Font(lblNicknamePrompt.Font.FontFamily, panelHeight * 0.05f, FontStyle.Bold);
            picTwoPlayers.SizeMode = PictureBoxSizeMode.StretchImage;
            picThreePlayers.SizeMode = PictureBoxSizeMode.StretchImage;
            picFourPlayers.SizeMode = PictureBoxSizeMode.StretchImage;
            picStartGame.SizeMode = PictureBoxSizeMode.StretchImage;


            lblErrorMessage.Size = new Size((int)(panelWidth * 0.6), (int)(panelHeight * 0.1));
            lblErrorMessage.Location = new Point((int)(panelWidth * 0.2), (int)(panelHeight * 0.7));
            lblErrorMessage.Font = new Font(lblErrorMessage.Font.FontFamily, panelHeight * 0.02f, FontStyle.Bold);

            picTwoPlayers.Size = new Size(imageWidth, imageHeight);
            picTwoPlayers.Location = new Point((int)(panelWidth * 0.005), (int)(panelHeight * 0.35));
            picTwoPlayers.ApplyRoundedCorners(20);
            picThreePlayers.Size = new Size(imageWidth, imageHeight);
            picThreePlayers.Location = new Point((int)(panelHeight * 0.34), (int)(panelHeight * 0.35));
            picThreePlayers.ApplyRoundedCorners(20);
            picFourPlayers.Size = new Size(imageWidth, imageHeight);
            picFourPlayers.Location = new Point((int)(panelHeight * 0.66), (int)(panelHeight * 0.35));
            picFourPlayers.ApplyRoundedCorners(20);


            titleLabel.Size = new Size((int)(panelWidth * 0.9), (int)(panelHeight * 0.1));
            titleLabel.Location = new Point((int)(panelWidth * 0.05), (int)(panelHeight * 0.2));
            titleLabel.Font = new Font(titleLabel.Font.FontFamily, panelHeight * 0.03f, FontStyle.Bold);

            picStartGame.Size = new Size(imageWidth, imageHeight);
            picStartGame.Location = new Point((int)(panelWidth * 0.35), (int)(panelHeight * 0.35));



        }
        private PictureBox CreatePictureBox(ImageHandler handler, int width, int height, int x, int y,int selectCount)
        {
            var pictureBox = new PictureBox();
            pictureBox.Size = new Size(width, height);
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Location = new Point(x, y);

            pictureBox.MouseEnter += handler.PictureBox_MouseEnter;
            pictureBox.MouseLeave += handler.PictureBox_MouseLeave;
            pictureBox.MouseDown += handler.PictureBox_MouseDown;
            pictureBox.MouseUp += handler.PictureBox_MouseUp;
            pictureBox.Click += (s, e) => SelectPlayers(selectCount);
           

            return pictureBox;
        }
        private void SelectPlayers(int playerCount)
        {


            selectedPlayersCount = playerCount;
            StartGame();
        }
        private void HandleGameStarted(byte[] responseContent)
        {
           
            int serverPlayerCount = BitConverter.ToInt32(responseContent, 0); 

            if (serverPlayerCount != selectedPlayersCount)
            {
                ErrorDialog errorDialog = new ErrorDialog("Количество игроков не совпадает.Пожалуйста, выберите снова.", this);
                errorDialog.Show();

            }
            else
            {
                Form1 gameForm = new Form1(this, gameClient, selectedPlayersCount, nickname);
                gameForm.Show();
                this.Hide();

            }

        }


        private void StartGame()
        {
            try
            {


                if (selectedPlayersCount > 0)
                {
                    byte[] data = BitConverter.GetBytes(selectedPlayersCount);
                    this.gameClient.SendMessage(UnoCommand.START, data);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}
