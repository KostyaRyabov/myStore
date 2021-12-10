
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace myStore.converters
{
    public static class ImageConvertor
    {
        public static BitmapSource Bmp2BmpSource(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            return bitmapSource;
        }

        public static byte[] BmpSource2bArray(BitmapSource bitmap_source)
        {
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmap_source));
                enc.Save(outStream);
                return outStream.ToArray();
            }
        }

        public static Bitmap bArray2Bitmap(byte[] bitmap)
        {
            using (var ms = new MemoryStream(bitmap))
                return new Bitmap(ms);
        }

        public static BitmapSource bArray2BmpSource(byte[] bitmap)
        {
            var bmp = bArray2Bitmap(bitmap);
            return Bmp2BmpSource(bmp);
        }
    }
}
