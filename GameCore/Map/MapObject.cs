using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using GameCore.Utils;

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

            MapObject tempMapObject = new MapObject
            {
                TheBitmap = new Bitmap(bitmapPath),
                TheMapDetail = (MapDetail)SaveObjects.DeserializeObject(objectPath, typeof(MapDetail))
            };

            return tempMapObject;
        }


    
    }
}