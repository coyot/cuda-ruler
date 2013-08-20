using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace aCudaResearch.Cuda
{
    public class BitmapWrapper
    {
        public Bitmap Bitmap { get; private set; }
        public int Stride { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public byte[] RgbValues { get; private set; }

        private BitmapWrapper(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }

        public static BitmapWrapper ConvertBitmap(Bitmap bitmap)
        {
            var bitmapWrapper = new BitmapWrapper(bitmap);
            bitmapWrapper.Convert();

            return bitmapWrapper;
        }

        private void Convert()
        {
            var bitmapData = Bitmap.LockBits(
                new Rectangle(Point.Empty, Bitmap.Size),
                ImageLockMode.ReadOnly,
                Bitmap.PixelFormat);

            int bytes = bitmapData.Stride * bitmapData.Height;

            Stride = bitmapData.Stride;
            Width = bitmapData.Width;
            Height = bitmapData.Height;
            RgbValues = new byte[bytes];

            Marshal.Copy(bitmapData.Scan0, RgbValues, 0, bytes);
            Bitmap.UnlockBits(bitmapData);
        }


        public void ConvertBack()
        {
            var bitmapData = Bitmap.LockBits(
                new Rectangle(Point.Empty, Bitmap.Size),
                ImageLockMode.ReadWrite,
                Bitmap.PixelFormat);
            Marshal.Copy(RgbValues, 0, bitmapData.Scan0, RgbValues.Length);
            Bitmap.UnlockBits(bitmapData);
        }
    }
}