using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Web;

namespace Daftari.Helpers
{
    public class ImageHelper
    {
        public static readonly List<string> ImageExtensions = new List<string> { "JPG", "JPEG", "JPE", "BMP", "GIF", "PNG" };

        private static List<string> _BackgroundColours = new List<string> { "339966", "3366CC", "CC33FF", "FF5050" };
        public static MemoryStream GenerateCircle(string firstName, string lastName)
        {
            var avatarString = string.Format("{0}{1}", firstName[0], lastName[0]).ToUpper();

            var randomIndex = new Random().Next(0, _BackgroundColours.Count - 1);
            var bgColour = _BackgroundColours[randomIndex];

            var bmp = new Bitmap(192, 192);
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            var font = new Font("Arial", 72, FontStyle.Bold, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(bmp);

            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            using (Brush b = new SolidBrush(ColorTranslator.FromHtml("#" + bgColour)))
            {
                graphics.FillEllipse(b, new Rectangle(0, 0, 192, 192));
            }
            graphics.DrawString(avatarString, font, new SolidBrush(Color.WhiteSmoke), 95, 100, sf);
            graphics.Flush();

            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            return ms;
        }

        public static MemoryStream GenerateRactangle(string firstName, string lastName)
        {
            var avatarString = string.Format("{0}{1}", firstName[0], lastName[0]).ToUpper();

            var randomIndex = new Random().Next(0, _BackgroundColours.Count - 1);
            var bgColour = _BackgroundColours[randomIndex];

            var bmp = new Bitmap(192, 192);
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            var font = new Font("Arial", 72, FontStyle.Bold, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(bmp);

            graphics.Clear((Color)new ColorConverter().ConvertFromString("#" + bgColour));
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.DrawString(avatarString, font, new SolidBrush(Color.WhiteSmoke), new RectangleF(0, 0, 192, 192), sf);

            graphics.Flush();

            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);

            return ms;
        }

        public static bool IsValidImage(string Extension)
        {
            return ImageExtensions.Any(x => Extension.ToLower().EndsWith(x.ToLower()));
        }

        // Create a thumbnail in byte array format from the image encoded in the passed byte array.  
        // (RESIZE an image in a byte[] variable.)  
        public static byte[] CreateThumbnail(byte[] PassedImage, int LargestSide)  
        {  
            byte[] ReturnedThumbnail;  
              
            using (MemoryStream StartMemoryStream = new MemoryStream(),  
                                NewMemoryStream = new MemoryStream())  
            {  
                // write the string to the stream  
                StartMemoryStream.Write(PassedImage, 0, PassedImage.Length);  
 
                // create the start Bitmap from the MemoryStream that contains the image  
                Bitmap startBitmap = new Bitmap(StartMemoryStream);  
 
                // set thumbnail height and width proportional to the original image.  
                int newHeight;  
                int newWidth;  
                double HW_ratio;  
                if (startBitmap.Height > startBitmap.Width)  
                {  
                    newHeight = LargestSide;  
                    HW_ratio = (double)((double)LargestSide / (double)startBitmap.Height);  
                    newWidth = (int)(HW_ratio * (double)startBitmap.Width);  
                }  
                else 
                {  
                    newWidth = LargestSide;  
                    HW_ratio = (double)((double)LargestSide / (double)startBitmap.Width);  
                    newHeight = (int)(HW_ratio * (double)startBitmap.Height);  
                }  
 
                // create a new Bitmap with dimensions for the thumbnail.  
                Bitmap newBitmap = new Bitmap(newWidth, newHeight);  
 
                // Copy the image from the START Bitmap into the NEW Bitmap.  
                // This will create a thumnail size of the same image.  
                newBitmap = ResizeImage(startBitmap, newWidth, newHeight);  
                  
                // Save this image to the specified stream in the specified format.  
                newBitmap.Save(NewMemoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);  
 
                // Fill the byte[] for the thumbnail from the new MemoryStream.  
                ReturnedThumbnail = NewMemoryStream.ToArray();  
            }  
 
            // return the resized image as a string of bytes.  
            return ReturnedThumbnail;  
        }  
 
        // Resize a Bitmap  
        private static Bitmap ResizeImage(Bitmap image, int width, int height)  
        {  
            Bitmap resizedImage = new Bitmap(width, height);  
            using (Graphics gfx = Graphics.FromImage(resizedImage))  
            {  
                gfx.DrawImage(image, new Rectangle(0, 0, width, height),   
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);  
            }  
            return resizedImage;  
        }
    }
}