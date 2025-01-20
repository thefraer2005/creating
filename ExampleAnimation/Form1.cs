namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer timer;
        private PictureBox[] topCards;
        private int[] topCardSpeeds;
        private float[] rotationAngles; // Углы поворота карт
        private bool[] isFlipping; // Состояние переворота
        private Image[] frontImages; // Изображения передней стороны карт
        private Image backImage; // Изображение задней стороны карт
        private bool[] isFrontImage;

        public Form1()
        {
        isFrontImage = new bool[3] { true, true, true };
            this.Width = 800;
            this.Height = 600;
            this.DoubleBuffered = true;

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1; 
            timer.Tick += Timer_Tick;

            topCards = new PictureBox[3];
            topCardSpeeds = new int[3] { 1, 2, 3 };
            rotationAngles = new float[3]; 
            isFlipping = new bool[3]; 

            frontImages = new Image[3];
            for (int i = 0; i < 3; i++)
            {
                frontImages[i] = Image.FromFile($"C:/Users/ASUS/source/repos/Server/WinFormsApp1/img/Blue- {i}.png");
            }
            backImage = Image.FromFile("C:/Users/ASUS/source/repos/Server/WinFormsApp1/img/ggg1.png");

            for (int i = 0; i < 3; i++)
            {
                topCards[i] = new PictureBox
                {
                    Width = 100,
                    Height = 150,
                    Location = new Point(i * 120 + 50, 10),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Image = frontImages[i]
                };
                this.Controls.Add(topCards[i]);
            }

            timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
           
            DrawTopCards(e.Graphics);
        }

        

    private void SetPictureBoxesVisibility(bool visible)
        {
            foreach (var pictureBox in topCards)
            {
                pictureBox.Visible = visible;
            }
        }

        private void DrawTopCards(Graphics g)
        {
            for (int i = 0; i < topCards.Length; i++)
            {
                var cardLocation = topCards[i].Location;

               
                Image cardImage;
                if (rotationAngles[i] < 90 || rotationAngles[i] > 270)
                {
                    cardImage = isFrontImage[i] ? frontImages[i] : backImage;
                }
                else
                {
                    cardImage = isFrontImage[i] ? backImage : frontImages[i];
                }

               
                float scaleX = (float)Math.Abs(Math.Cos(rotationAngles[i] * Math.PI / 180));

               
                g.TranslateTransform(cardLocation.X + topCards[i].Width / 2, cardLocation.Y + topCards[i].Height / 2);
                g.ScaleTransform(scaleX, 1);
                g.TranslateTransform(-topCards[i].Width / 2, -topCards[i].Height / 2);

               
                g.DrawImage(cardImage, new Rectangle(0, 0, topCards[i].Width, topCards[i].Height));

               
                g.ResetTransform();
            }
        }





        private void Timer_Tick(object sender, EventArgs e)
        {
            MoveTopCards();
            UpdateFlipping(); 
            this.Invalidate(); 
        }

        private void MoveTopCards()
        {
            for (int i = 0; i < topCards.Length; i++)
            {
                topCards[i].Left += topCardSpeeds[i];

               
                if (topCards[i].Right >= this.ClientSize.Width || topCards[i].Left <= 0)
                {
                    topCardSpeeds[i] = -topCardSpeeds[i];
                    isFlipping[i] = true; 
                }
            }
        }

        private void UpdateFlipping()
        {
            bool anyFlipping = false;
            for (int i = 0; i < isFlipping.Length; i++)
            {
                if (isFlipping[i])
                {
                    anyFlipping = true;
                    rotationAngles[i] += 1;

                    if (rotationAngles[i] >= 180) 
                    {
                        rotationAngles[i] = 0; 
                        isFlipping[i] = false; 
                        isFrontImage[i] = !isFrontImage[i]; 
                        topCards[i].Image = isFrontImage[i] ? frontImages[i] : backImage;
                    }
                }
            }

            SetPictureBoxesVisibility(!anyFlipping);
        }





    }
}
