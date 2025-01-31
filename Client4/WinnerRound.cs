using Common;


namespace Client4
{
    public partial class WinnerRound : Form
    {
        private Label lblWinner;
        private Label lblPoints;
        private int selectPLayers;
        private Button btnContinue;
        private GameClient gameClient;
        private Form parentForm;
        private StartForm start;
        private string nick;
        private System.Windows.Forms.Timer closeTimer;

        public WinnerRound(string winnerNickname, int winnerPoints, GameClient gameClient, Form parentForm, int selectPlayer, StartForm start, string nick)
        {
            this.start = start;
            this.nick = nick;
            this.parentForm = parentForm;
            this.gameClient = gameClient;


            this.selectPLayers = selectPlayer;
            InitializeComponent(parentForm);
            parentForm.LocationChanged += (s, e) => UpdatePosition(parentForm);
            this.lblWinner.Text = $"Раунд выйграл: {Environment.NewLine}{winnerNickname}";
            this.lblPoints.Text = $"Очки: {winnerPoints}";
        }
        private void NewRound()
        {
            parentForm.Show();

            gameClient.SendMessage(UnoCommand.NEW_ROUND);
            this.Hide();
        }


        public void UpdatePosition(Form parentForm)
        {
            this.Location = new Point(
                parentForm.Location.X + (parentForm.Width - this.Width) / 2,
                parentForm.Location.Y + (parentForm.Height - this.Height) / 2
            );
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
            this.lblWinner.Font = new Font("Arial", (float)(this.ClientSize.Height * 0.07), FontStyle.Bold);
            this.lblWinner.AutoSize = false;
            this.lblWinner.Size = new Size((int)(this.ClientSize.Width * 0.8), (int)(parentForm.ClientSize.Height * 0.1));
            this.lblWinner.ForeColor = Color.White;
            this.Controls.Add(this.lblWinner);

            this.lblPoints = new Label();
            this.lblPoints.Font = new Font("Arial", (float)(this.ClientSize.Height * 0.08), FontStyle.Regular);
            this.lblPoints.AutoSize = true;
            this.lblPoints.ForeColor = Color.White;

            this.Controls.Add(this.lblPoints);

            this.btnContinue = new Button();
            this.btnContinue.Name = "btnExitGame";
            this.btnContinue.Size = new Size((int)(this.ClientSize.Width * 0.5), (int)(this.ClientSize.Height * 0.25));
            this.btnContinue.TabIndex = 1;
            this.btnContinue.BackColor = Color.WhiteSmoke;
            this.btnContinue.ForeColor = Color.Black;
            this.btnContinue.Font = new Font("Arial", (float)(this.ClientSize.Height * 0.05), FontStyle.Regular);
            this.btnContinue.Text = "Продолжить";
            btnContinue.ApplyRoundedCorners(30);

            this.btnContinue.Click += (sender, e) => this.NewRound();

            this.Controls.Add(this.btnContinue);

            this.Text = "Конец игры";
            this.FormBorderStyle = FormBorderStyle.None;


            UpdateLayout();
        }

        private void UpdateLayout()
        {

            int padding = 10;
            lblWinner.Location = new Point((this.ClientSize.Width - lblWinner.Width) / 2, (int)(this.ClientSize.Height * 0.3));
            lblPoints.Location = new Point((this.ClientSize.Width - lblPoints.Width) / 2, lblWinner.Bottom + padding);

            btnContinue.Location = new Point((this.ClientSize.Width - btnContinue.Width) / 2, lblPoints.Bottom + padding);
        }

    }
}
