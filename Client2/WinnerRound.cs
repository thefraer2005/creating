using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client2
{
    public partial class WinnerRound : Form
    {
        private Label lblWinner;
        private Label lblPoints;
        private int selectPLayers;
        private Form parentForm;
        private Button btnContinue;
        private GameClient gameClient;
        private System.Windows.Forms.Timer closeTimer;

        public WinnerRound(string winnerNickname, int winnerPoints, GameClient gameClient, Form parentForm,int selectPlayer)
        {
            this.parentForm = parentForm;
            this.gameClient = gameClient;
            this.selectPLayers = selectPlayer;
            InitializeComponent(winnerNickname, winnerPoints, parentForm);
            parentForm.LocationChanged += (s, e) => UpdatePosition(parentForm);
            parentForm.SizeChanged += (s, e) => UpdateSizeAndPosition(parentForm);
        }
        private void NewRound()
        {
            parentForm.Show();
          
            gameClient.SendMessage(UnoCommand.NEW_ROUND);
            this.Close();
        }
        private void SetPositionAndSize(Form parentForm)
        {
            UpdateSizeAndPosition(parentForm); 
        }

        public void UpdateSizeAndPosition(Form parentForm)
        {
            
        }

        public void UpdatePosition(Form parentForm)
        {
            this.Location = new Point(
                parentForm.Location.X + (parentForm.Width - this.Width) / 2,
                parentForm.Location.Y + (parentForm.Height - this.Height) / 2
            );
        }
        private void InitializeComponent(string winnerNickname, int winnerPoints, Form parentForm)
        {
            this.Text = "Результаты раунда1";
            this.StartPosition = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size((int)(parentForm.Width * 0.5), (int)(parentForm.Height * 0.5));

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                parentForm.Location.X + (parentForm.Width - this.Width) / 2,
                parentForm.Location.Y + (parentForm.Height - this.Height) / 2
            );


            lblWinner = new Label();
            lblWinner.Text = $"Раунд выиграл: {winnerNickname}";
            lblWinner.Location = new Point(30, 30);
            lblWinner.AutoSize = true;
            lblWinner.Font = new Font(lblWinner.Font.FontFamily, 12, FontStyle.Bold);

            lblPoints = new Label();
            lblPoints.Text = $"Очки: {winnerPoints}";
            lblPoints.Location = new Point(30, 70);
            lblPoints.AutoSize = true;
            lblPoints.Font = new Font(lblPoints.Font.FontFamily, 12);

            btnContinue = new Button();
            btnContinue.Text = "Продолжить";
            btnContinue.Location = new Point(100, 120);
            btnContinue.Click += (s, e) => NewRound();

            this.Controls.Add(lblWinner);
            this.Controls.Add(lblPoints);
            this.Controls.Add(btnContinue);

            
        }
    }
}
