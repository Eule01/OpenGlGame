#region

using System.Drawing;
using System.Drawing.Imaging;

#endregion

namespace GameCore.Utils
{
    public static class BitmapHelper
    {
        public static Bitmap CreatBitamp(Size aSize, SolidBrush aBrush)
        {
            Bitmap newBmp = new Bitmap(aSize.Width, aSize.Height,
                                       PixelFormat.Format32bppPArgb);
            using (Graphics gfx = Graphics.FromImage(newBmp))
            {
                gfx.FillRectangle(aBrush, 0, 0, aSize.Width, aSize.Height);
            }
            return newBmp;
        }
    }
}