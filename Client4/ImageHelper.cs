using Common;

using System.Drawing.Imaging;


namespace Client4
{
    

    public class ImageHandler
    {
        private string playButtonImagePath;
      

        public ImageHandler(string playButtonImage)
        {
            playButtonImagePath = playButtonImage;
           
        }
        public void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            if (sender is PictureBox pictureBox && pictureBox.Image != null)
            {
              
                pictureBox.Image = DarkenImage(pictureBox.Image);
            }
        }
       
        public Image DarkenImage(Image original)
        {
            Bitmap darkenedBitmap = new Bitmap(original.Width, original.Height);

            using (Graphics g = Graphics.FromImage(darkenedBitmap))
            {
                ColorMatrix colorMatrix = new ColorMatrix();
                colorMatrix.Matrix00 = 0.5f; 
                colorMatrix.Matrix11 = 0.5f; 
                colorMatrix.Matrix22 = 0.5f; 

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);

                g.DrawImage(original, new Rectangle(0, 0, darkenedBitmap.Width, darkenedBitmap.Height),
                            0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }

            return darkenedBitmap;
        }
        public async Task LoadImageAsync(PictureBox pictureBox, string bucketName, string imageFileName)
        {
            Image? image = await LoadImageFromMinio(bucketName, imageFileName);
            if (image != null)
            {
                pictureBox.Image = image; 
            }
        }

        public async void PictureBox_MouseLeave(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            if (pic != null)
            {
                await LoadImageAsync(pic, "unobucket", playButtonImagePath); 
            }
        }

        public void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            if (pic != null)
            {
               
                pic.Size = new Size((int)(pic.Width * 0.9), (int)(pic.Height * 0.9)); 
            }
        }

        public void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            if (pic != null)
            {
              
                pic.Size = new Size((int)(pic.Width / 0.9), (int)(pic.Height / 0.9));
            }
        }

        public async Task<Image?> LoadImageFromMinio(string bucketName, string imageFileName)
        {
            string? imageUrl = await MinioHelper.GetImageUrl(bucketName, imageFileName, 900); 

            if (imageUrl != null)
            {
                using (var client = new HttpClient())
                {
                    var imageBytes = await client.GetByteArrayAsync(imageUrl);
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        return Image.FromStream(stream);
                    }
                }
            }
            return null;
        }
    }

}
