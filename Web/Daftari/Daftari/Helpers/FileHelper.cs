using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace Daftari.Helpers
{
    /// <summary>
    /// Helper for file manipulations
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Convert image file to bytes
        /// </summary>
        /// <param name="file">image file</param>
        /// <returns>Bytes of the image file</returns>
        public static byte[] GetImageFileBytes(HttpPostedFileBase file)
        {
            byte[] bytes = new byte[file.InputStream.Length + 10];
            int numBytesToRead = (int)file.InputStream.Length;
            int numBytesRead = 0;
            do
            {
                // Read may return anything from 0 to 10.
                int n = file.InputStream.Read(bytes, numBytesRead, 10);
                numBytesRead += n;
                numBytesToRead -= n;
            } while (numBytesToRead > 0);
            file.InputStream.Close();
            return bytes;
        }

        public static byte[] GetImageFileBytes(string FileName)
        {
            using (FileStream file = new FileStream(FileName, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[file.Length + 10];
                int numBytesToRead = (int)file.Length;
                int numBytesRead = 0;
                do
                {
                    // Read may return anything from 0 to 10.
                    int n = file.Read(bytes, numBytesRead, 10);
                    numBytesRead += n;
                    numBytesToRead -= n;
                } while (numBytesToRead > 0);
                file.Close();
                return bytes;
            }
        }

        public static byte[] ImageResize(byte[] fullImage, int newPixels)
        {
            MemoryStream myMemStream = new MemoryStream(fullImage);
            Image fullsizeImage = Image.FromStream(myMemStream);
            ImageFormat contentType = GetContentType(fullImage);

            int maxSize = fullsizeImage.Width > fullsizeImage.Height ? fullsizeImage.Width : fullsizeImage.Height;

            if (maxSize > newPixels)
            {
                //resize
                int scaleDiff = maxSize - newPixels;
                Image newImage = fullsizeImage.GetThumbnailImage(fullsizeImage.Width - scaleDiff, fullsizeImage.Height - scaleDiff, null, System.IntPtr.Zero);
                MemoryStream myResult = new MemoryStream();
                newImage.Save(myResult, contentType);  //Or whatever format you want.
                return myResult.ToArray();  //Returns a new byte array.
            }
            return fullImage;
        }

        public static ImageFormat GetContentType(byte[] imageBytes)
        {
            MemoryStream ms = new MemoryStream(imageBytes);

            using (BinaryReader br = new BinaryReader(ms))
            {
                int maxMagicBytesLength = imageFormatDecoders.Keys.OrderByDescending(x => x.Length).First().Length;

                byte[] magicBytes = new byte[maxMagicBytesLength];

                for (int i = 0; i < maxMagicBytesLength; i += 1)
                {
                    magicBytes[i] = br.ReadByte();

                    foreach (var kvPair in imageFormatDecoders)
                    {
                        if (magicBytes.StartsWith(kvPair.Key))
                        {
                            return kvPair.Value;
                        }
                    }
                }
            }
            return null;
        }

        private static bool StartsWith(this byte[] thisBytes, byte[] thatBytes)
        {
            for (int i = 0; i < thatBytes.Length; i += 1)
            {
                if (thisBytes[i] != thatBytes[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static Dictionary<byte[], ImageFormat> imageFormatDecoders = new Dictionary<byte[], ImageFormat>()
        {
            { new byte[]{ 0x42, 0x4D }, ImageFormat.Bmp},
            { new byte[]{ 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, ImageFormat.Gif },
            { new byte[]{ 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }, ImageFormat.Gif },
            { new byte[]{ 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, ImageFormat.Png },
            { new byte[]{ 0xff, 0xd8 }, ImageFormat.Jpeg },
        };

        public static string GetSizeInMemory(this long bytesize)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = Convert.ToDouble(bytesize);
            int order = 0;
            while (len >= 1024D && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            return string.Format(CultureInfo.CurrentCulture, "{0:0.#} {1}", len, sizes[order]);
        }

        public static string ToBase64(this byte[] byteArray, string contentType)
        {
            string base64String = Convert.ToBase64String(byteArray, 0, byteArray.Length);
            return $"data:{contentType};base64,{base64String}";
        }
    }
}