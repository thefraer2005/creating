

namespace Client4
{
    public class EndGame : Form
    {
        private Button btnRestartGame;
        private Button btnExitGame;
        private Label lblWinner;
        private Label lblScore;
        private GameClient gameClients;
        private Form parentForms;
        public EndGame(string winnerName, int totalPoints, GameClient gameClient, Form parentForm)
        {
            this.gameClients = gameClient;
            this.parentForms = parentForm;
            InitializeComponent(parentForms);

            lblWinner.Text = $"Победитель:{Environment.NewLine}{winnerName}";
            lblScore.Text = $"Очки: {totalPoints}";




        }

        private void InitializeComponent(Form parentForm)
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size((int)(parentForm.ClientSize.Width * 0.5), (int)(parentForm.ClientSize.Height * 0.5));
            this.Location = new Point(
                parentForm.Location.X + (parentForm.ClientSize.Width - this.Width) / 2,
                parentForm.Location.Y + (parentForm.ClientSize.Height - this.Height) / 2
            );
            this.BackColor = Color.Black;
            this.Opacity = 0.8;
            this.ApplyRoundedCorners(30);


            this.Resize += (s, e) => UpdateLayout();



            this.lblWinner = new Label();
            this.lblWinner.Font = new Font("Arial", (float)(this.ClientSize.Height * 0.08), FontStyle.Bold);
            this.lblWinner.AutoSize = false;
            this.lblWinner.Size = new Size((int)(this.ClientSize.Width * 0.8), (int)(parentForm.ClientSize.Height * 0.1));
            this.lblWinner.ForeColor = Color.White;
            this.Controls.Add(this.lblWinner);


            this.lblScore = new Label();
            this.lblScore.Font = new Font("Arial", (float)(this.ClientSize.Height * 0.08), FontStyle.Regular);
            this.lblScore.AutoSize = true;
            this.lblScore.ForeColor = Color.White;

            this.Controls.Add(this.lblScore);

            this.btnExitGame = new Button();
            this.btnExitGame.Name = "btnExitGame";
            this.btnExitGame.Size = new Size((int)(this.ClientSize.Width * 0.5), (int)(this.ClientSize.Height * 0.25));
            this.btnExitGame.TabIndex = 1;
            this.btnExitGame.BackColor = Color.WhiteSmoke;
            this.btnExitGame.ForeColor = Color.Black;
            this.btnExitGame.Font = new Font("Arial", (float)(this.ClientSize.Height * 0.05), FontStyle.Regular);
            this.btnExitGame.Text = "Главное меню";
            btnExitGame.ApplyRoundedCorners(30);


            this.btnExitGame.Click += new EventHandler(this.btnExitGame_Click);
            this.Controls.Add(this.btnExitGame);

            this.Text = "Конец игры";
            this.FormBorderStyle = FormBorderStyle.None;


            UpdateLayout();
        }

        private void UpdateLayout()
        {

            int padding = 10;
            lblWinner.Location = new Point((this.ClientSize.Width - lblWinner.Width) / 2, (int)(this.ClientSize.Height * 0.3));
            lblScore.Location = new Point((this.ClientSize.Width - lblScore.Width) / 2, lblWinner.Bottom + padding);

            btnExitGame.Location = new Point((this.ClientSize.Width - btnExitGame.Width) / 2, lblScore.Bottom + padding);
        }


        private void btnExitGame_Click(object sender, EventArgs e)
        {
            gameClients.Disconnect();

            StartForm startForm = new StartForm();
            startForm.RestartGame();
            startForm.Show();
            this.Hide();
            parentForms.Hide();
        }

    }
}
