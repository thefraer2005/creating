using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client2
{
    public partial class StartForm : Form
    {
        private GameClient gameClient;
        private GameClient gameClientRestart;
        private int selectedPlayersCount;
        private Button btnStartGame;
        private TextBox txtNickname; // Поле ввода для ника
        private Label lblErrorMessage;

        private string nickname;
        public  StartForm()
        {

            InitializeComponent();
            this.gameClient = new GameClient();
            gameClient.Error += HandleGameStarted;




        }

        private void InitializeComponent()
        {
            this.ClientSize = new System.Drawing.Size(700, 700);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(800, 50);
            this.Name = "FormStartGame";
            this.Text = "Начало игры1";
            this.btnStartGame = new System.Windows.Forms.Button();
            this.btnStartGame.Location = new System.Drawing.Point(
             (this.ClientSize.Width - this.btnStartGame.Width) / 2,
             (this.ClientSize.Height - this.btnStartGame.Height) / 2
         );
            this.btnStartGame.Name = "btnStartGame";
            this.btnStartGame.Size = new System.Drawing.Size(100, 30);
            this.btnStartGame.TabIndex = 0;
            this.btnStartGame.Text = "Начать игру2";
            this.btnStartGame.UseVisualStyleBackColor = true;
            this.btnStartGame.Click += new System.EventHandler(this.btnStartGame_Click);
            this.Controls.Add(this.btnStartGame);

            this.txtNickname = new System.Windows.Forms.TextBox();
            this.txtNickname.Location = new System.Drawing.Point(
                (this.ClientSize.Width - 200) / 2,
                (this.ClientSize.Height - this.txtNickname.Height) / 2 - 20 // Смещение вверх
            );
            this.txtNickname.Name = "txtNickname";
            this.txtNickname.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(this.txtNickname);

            // Инициализация метки для сообщений об ошибках
            this.lblErrorMessage = new System.Windows.Forms.Label();
            this.lblErrorMessage.Location = new System.Drawing.Point(
                (this.ClientSize.Width - 300) / 2,
                (this.ClientSize.Height - this.lblErrorMessage.Height) / 2 + 90 // Смещение вниз
            );
            this.lblErrorMessage.Name = "lblErrorMessage";
            this.lblErrorMessage.Size = new System.Drawing.Size(300, 30);
            this.lblErrorMessage.ForeColor = Color.Red; // Красный цвет для ошибок
            this.lblErrorMessage.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(this.lblErrorMessage);



        }

        private async void btnStartGame_Click(object sender, EventArgs e)
        {
            nickname = txtNickname.Text.Trim(); // Получаем ник из текстового поля

            if (string.IsNullOrEmpty(nickname)) // Проверка на пустое поле
            {
                lblErrorMessage.Text = "Пожалуйста, введите ник."; // Сообщение об ошибке
                return;
            }

            byte[] nicknameBytes = Encoding.UTF8.GetBytes(nickname); // Кодируем ник
          
             gameClient.Connect("127.0.0.1");
            await gameClient.SendMessage(UnoCommand.CONNECT, nicknameBytes);

            lblErrorMessage.Text = ""; // Очищаем сообщение об ошибке
            this.btnStartGame.Visible = false;
            this.txtNickname.Visible = false;

            CreatePlayerSelectionButtons();



            // Метод для создания кнопок выбора игроков
        }

        private void CreatePlayerSelectionButtons()
        {
            int buttonWidth = 100;
            int buttonHeight = 30;
            int spacing = 10;

            // Кнопка для 2 игроков
            var btnTwoPlayers = new Button();
            btnTwoPlayers.Text = "2 игрока";
            btnTwoPlayers.Size = new Size(buttonWidth, buttonHeight);
            btnTwoPlayers.Location = new Point(
                (this.ClientSize.Width - buttonWidth) / 2,
                (this.ClientSize.Height - (3 * buttonHeight + 2 * spacing)) / 2
            );
            btnTwoPlayers.Click += (s, e) => SelectPlayers(2);

            // Кнопка для 3 игроков
            var btnThreePlayers = new Button();
            btnThreePlayers.Text = "3 игрока";
            btnThreePlayers.Size = new Size(buttonWidth, buttonHeight);
            btnThreePlayers.Location = new Point(
                (this.ClientSize.Width - buttonWidth) / 2,
                btnTwoPlayers.Location.Y + buttonHeight + spacing
            );
            btnThreePlayers.Click += (s, e) => SelectPlayers(3);

            // Кнопка для 4 игроков
            var btnFourPlayers = new Button();
            btnFourPlayers.Text = "4 игрока";
            btnFourPlayers.Size = new Size(buttonWidth, buttonHeight);
            btnFourPlayers.Location = new Point(
                (this.ClientSize.Width - buttonWidth) / 2,
                btnThreePlayers.Location.Y + buttonHeight + spacing
            );
            btnFourPlayers.Click += (s, e) => SelectPlayers(4);

            this.Controls.Add(btnTwoPlayers);
            this.Controls.Add(btnThreePlayers);
            this.Controls.Add(btnFourPlayers);
        }
        private void SelectPlayers(int playerCount)
        {


            selectedPlayersCount = playerCount;
            StartGame();
        }

        private void HandleGameStarted(byte[] responseContent)
        {
            // Здесь вы можете проверить количество игроков в responseContent
            int serverPlayerCount = BitConverter.ToInt32(responseContent, 0); // Предполагаем, что сервер отправляет количество игроков

            if (serverPlayerCount != selectedPlayersCount)
            {
                MessageBox.Show("Количество игроков не совпадает. Пожалуйста, выберите снова.");
                CreatePlayerSelectionButtons();
            }
            else
            {
                Form1 gameForm = new Form1(this, gameClient, selectedPlayersCount, nickname);
                gameForm.Show();
                this.Hide();

            }

        }


        private async void StartGame()
        {
            try
            {


                if (selectedPlayersCount > 0)
                {
                    byte[] data = BitConverter.GetBytes(selectedPlayersCount);
                    await gameClient.SendMessage(UnoCommand.START, data);

                   

                  


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}
