using Common;


namespace Client3
{
    
    public class ErrorDialog : Form
    {
        private Label lblErrorMessage;
        private System.Windows.Forms.Timer timer;

        public ErrorDialog(string message, Form parentForm)
        {
           
            this.Size = new Size((int)(parentForm.Width * 0.5), (int)(parentForm.Height * 0.3));
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.Opacity = 0.8; 
            this.ApplyRoundedCorners(30);

          
            lblErrorMessage = new Label
            {
                Text = message,
                Location = new Point(20, this.Height / 4),
                Size = new Size(this.Width - 40, this.Height / 2), 
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 15, FontStyle.Bold)
            };

            this.Controls.Add(lblErrorMessage);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                parentForm.Location.X + (parentForm.Width - this.Width) / 2,
                parentForm.Location.Y + (parentForm.Height - this.Height) / 2
            );

           
            this.Owner = parentForm;
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1500;
            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                this.Close();
            };
            timer.Start();
        }

       
    }

   

}
