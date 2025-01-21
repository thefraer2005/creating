using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client1
{
    public enum CardColor
    {
        Red,
        Green,
        Blue,
        Yellow
    }

    public class ColorSelectionForm : Form
    {
        public CardColor SelectedColor { get; private set; }

        public ColorSelectionForm()
        {
            this.Text = "Выберите цвет";
            this.Size = new Size(300, 200);

            foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
            {
                Button colorButton = new Button
                {
                    Text = color.ToString(),
                    Dock = DockStyle.Top,
                    Tag = color
                };
                colorButton.Click += ColorButton_Click;
                this.Controls.Add(colorButton);
            }
        }

        private void ColorButton_Click(object sender, EventArgs e)
        {
            SelectedColor = (CardColor)((Button)sender).Tag;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }

}
