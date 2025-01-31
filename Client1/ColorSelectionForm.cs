using Common;

namespace Client1
{


    public class ColorSelectionForm : Form
    {
        public CardColor? SelectedColor { get; private set; } 

        public ColorSelectionForm(Form parentForm)
        {
            this.Text = "Выберите цвет карты";
            this.Size = new Size((int)(parentForm.Width*0.3), (int)(parentForm.Height*0.3));
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(parentForm.Location.X + (parentForm.Width - this.Width) / 2,
                                       parentForm.Location.Y + (parentForm.Height - this.Height) / 2);

           
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black; 
            this.Opacity = 0.8; 
            this.ApplyRoundedCorners(30);

            foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
            {
                Button colorButton = new Button
                {
                    Text = color.ToString(),
                    Dock = DockStyle.Top,
                    Tag = color,
                    BackColor = GetColorFromCardColor(color), 
                    ForeColor = Color.Black 
                };
                colorButton.Size = new Size((int)(parentForm.Width * 0.3), (int)(parentForm.Height * 0.07));
                colorButton.Click += ColorButton_Click;
                colorButton.Margin = new Padding((int)(parentForm.Height * 0.005), 0, 0,0 );
                colorButton.ApplyRoundedCorners(15);
                colorButton.TextAlign = ContentAlignment.MiddleCenter;

                colorButton.Font = new Font("Arial", parentForm.Height * 0.01f);
                this.Controls.Add(colorButton);
            }
        }

        private void ColorButton_Click(object sender, EventArgs e)
        {
            SelectedColor = (CardColor)((Button)sender).Tag;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private Color GetColorFromCardColor(CardColor cardColor)
        {
            return cardColor switch
            {
                CardColor.Red => Color.Red,
                CardColor.Green => Color.Green,
                CardColor.Blue => Color.Blue,
                CardColor.Yellow => Color.Yellow
                
            };
        }
    }


}
