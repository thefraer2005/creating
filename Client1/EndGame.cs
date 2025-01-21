using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client1
{
    public class EndGame : Form
    {
        private Button btnRestartGame;
        private Button btnExitGame;
        private Label lblWinner;
        private Label lblScore;
        private GameClient gameClient;
        private Form parentForm;
        public EndGame(string winnerName, int totalPoints, GameClient gameClient, Form parentForm)
        {
            InitializeComponent(parentForm);
            this.gameClient = gameClient;
            this.parentForm = parentForm;
            lblWinner.Text = $"Победитель: {winnerName}";
            lblScore.Text = $"Очки: {totalPoints}";

            lblWinner.Visible = true;
            lblScore.Visible = true;


        }

        private void InitializeComponent(Form parentForm)
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size((int)(parentForm.ClientSize.Width * 0.5), (int)(parentForm.ClientSize.Height * 0.5));
            this.Location = new Point(
                parentForm.Location.X + (parentForm.ClientSize.Width - this.Width) / 2,
                parentForm.Location.Y + (parentForm.ClientSize.Height - this.Height) / 2
            );
            this.btnRestartGame = new Button();
            this.btnRestartGame.Location = new Point((this.ClientSize.Width - 100) / 2, (int)(this.ClientSize.Height * 0.75));
            this.btnRestartGame.Name = "btnRestartGame";
            this.btnRestartGame.Size = new Size(100, 30);
            this.btnRestartGame.TabIndex = 2;



            this.lblWinner = new Label();
            this.lblWinner.Font = new Font("Arial", 24, FontStyle.Bold);
            this.lblWinner.Location = new Point((this.ClientSize.Width - 300) / 2, (int)(this.ClientSize.Height * 0.5) - 50);
            this.lblWinner.AutoSize = true;
            this.Controls.Add(this.lblWinner);

            this.lblScore = new Label();
            this.lblScore.Font = new Font("Arial", 18, FontStyle.Regular);
            this.lblScore.Location = new Point((this.ClientSize.Width - 300) / 2, (int)(this.ClientSize.Height * 0.5));
            this.lblScore.AutoSize = true;
            this.Controls.Add(this.lblScore);

            this.btnExitGame = new Button();
            this.btnExitGame.Location = new Point((this.ClientSize.Width - 100) / 2, (int)(this.ClientSize.Height * 0.75) + 50);
            this.btnExitGame.Name = "btnExitGame";
            this.btnExitGame.Size = new Size(100, 30);
            this.btnExitGame.TabIndex = 1;

            this.btnExitGame.Text = "Выйти из игры";
            this.btnExitGame.UseVisualStyleBackColor = true;
            this.btnExitGame.Click += new EventHandler(this.btnExitGame_Click);
            this.Controls.Add(this.btnExitGame);

            this.Text = "Конец игры1";
            this.FormBorderStyle = FormBorderStyle.None;
        }



        private void btnExitGame_Click(object sender, EventArgs e)
        {
            this.gameClient = new GameClient();
            StartForm startForm = new StartForm();
            startForm.Show();
            this.Hide();
            parentForm.Hide();


            gameClient.Disconnect();

        }
    }
}
