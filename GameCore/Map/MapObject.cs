#region

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using GameCore.Utils;

#endregion

namespace GameCore.Map
{
    public class MapObject
    {
        public Bitmap TheBitmap;

        public MapDetail TheMapDetail;

        public void SaveToFile(string aFilePath)
        {
            if (TheBitmap != null)
            {
                string bitmapPath = aFilePath + ".png";
                TheBitmap.Save(bitmapPath, ImageFormat.Png);
            }
            if (TheMapDetail != null)
            {
                string objectPath = aFilePath + ".xml";
                SaveObjects.SerializeObject(objectPath, TheMapDetail);
            }
        }

        public static MapObject LoadFromFile(string aFilePath)
        {
            string bitmapPath = aFilePath + ".png";
            string objectPath = aFilePath + ".xml";
            if (!File.Exists(bitmapPath))
            {
            }
            if (!File.Exists(objectPath))
            {
            }

            Bitmap tempBitmap = new Bitmap(bitmapPath);
            if (tempBitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                Bitmap tempBitmap2 = BitmapHelper.ConvertToPixelFormat(tempBitmap, PixelFormat.Format32bppArgb);
                tempBitmap.Dispose();
                tempBitmap = tempBitmap2;
            }

            MapObject tempMapObject = new MapObject
            {
                TheBitmap = tempBitmap,
                TheMapDetail = (MapDetail) SaveObjects.DeserializeObject(objectPath, typeof (MapDetail))
            };

            return tempMapObject;
        }
    }
}