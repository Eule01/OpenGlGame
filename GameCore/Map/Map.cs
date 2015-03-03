#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml.Serialization;
using GameCore.Utils;
using OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

#endregion

namespace GameCore.Map
{
    [XmlType(AnonymousType = true)]
    public class Map
    {
        /// <summary>
        ///     The size of the map.
        /// </summary>
        private Rectangle theBoundingBox = Rectangle.Empty;

        private int width;
        private int height;

        private Tile[] theTileArray;
        private int offsetX;
        private int offsetY;


        public Map()
        {
        }

        [XmlIgnore]
        public RectangleF TheBoundingBox
        {
            get { return theBoundingBox; }
        }


        [XmlIgnore]
        public int Width
        {
            get { return width; }
        }

        [XmlIgnore]
        public int Height
        {
            get { return height; }
        }

        [XmlElement("TheTileArray")]
        public Tile[] TheTileArray
        {
            get { return theTileArray; }
            set
            {
                theTileArray = value;
                theBoundingBox = GetBoundingBox(theTileArray);
                width = theBoundingBox.Width;
                height = theBoundingBox.Height;
                offsetX = theBoundingBox.Location.X;
                offsetY = theBoundingBox.Location.Y;
            }
        }

        [XmlIgnore]
        public Tile this[int x, int y]
        {
            get { return theTileArray[(x-offsetX) + (y-offsetY)*width]; }
        }

        private static Rectangle GetBoundingBox(Tile[] tempTiles)
        {
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;

            foreach (Tile aTile in tempTiles)
            {
                Vector aLoc = aTile.Location;
                if (aLoc.X > maxX)
                {
                    maxX = (int) aLoc.X;
                }
                if (aLoc.X < minX)
                {
                    minX = (int) aLoc.X;
                }
                if (aLoc.Y > maxY)
                {
                    maxY = (int) aLoc.Y;
                }
                if (aLoc.Y < minY)
                {
                    minY = (int) aLoc.Y;
                }
            }
            return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

        /// <summary>
        ///     Closes this object and disposes everything.
        /// </summary>
        public void Close()
        {
        }

        internal static Map CreateTestMap()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Size aSize = new Size(500, 500);
            Map aMap = new Map {TheTileArray = CreatTestTiles(new Point(-250, -250), aSize)};
            watch.Stop();
            GameCore.TheGameCore.RaiseMessage(string.Format("CreateTestMap() took {0}ms", watch.ElapsedMilliseconds));
            return aMap;
        }

        private static Tile[] CreatTestTiles(Point startPos, Size aSize)
        {
            int width = aSize.Width;
            int height = aSize.Height;
            int mapOffsetX = startPos.X;
            int mapOffsetY = startPos.Y;


            Tile[] tempTiles = new Tile[width*height];


            Tile tempTile = null;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector tempLoc = new Vector(x + mapOffsetX, y + mapOffsetY);
                    if (x + mapOffsetX == 8)
                    {
                        tempTile = new Tile(Tile.TileIds.Grass) {Location = tempLoc};
                    }
                    else if (x + mapOffsetX == 5)
                    {
                        tempTile = new Tile(Tile.TileIds.Road) {Location = tempLoc};
                    }
                    else if (x + mapOffsetX == 2)
                    {
                        tempTile = new Tile(Tile.TileIds.Water) {Location = tempLoc};
                    }
                    else if (x + mapOffsetX == 12)
                    {
                        tempTile = new Tile(Tile.TileIds.Wall) {Location = tempLoc};
                    }
                    else
                    {
                        tempTile = new Tile(Tile.TileIds.Desert) {Location = tempLoc};
                    }
                    tempTiles[x+ y*width] = tempTile;
                }
            }
            return tempTiles;
        }

        public void SelectTile(Vector3 mouseWorld)
        {
            Tile tempSelectedTile = this[(int) Math.Floor(mouseWorld.x), (int) Math.Floor(mouseWorld.z)];
            if (tempSelectedTile != null)
            {
                GameCore.TheGameCore.OnGameEventHandler(new GameEventArgs(GameEventArgs.Types.MapTileSelected)
                {
                    TheTile = tempSelectedTile
                });
            }
        }

        #region Load and Save

        internal static void SaveMap(string aFilePath, Map aMap)
        {
            SaveObjects.SerializeObject(aFilePath, aMap);
        }

        internal static Map LoadMap(string aFilePath)
        {
            Map aMap = (Map) SaveObjects.DeserializeObject(aFilePath, typeof (Map));
            return aMap;
        }

        public static void SaveToMapObject(Map aMap, string aFileNamePath)
        {
            MapObject tempMapObject = ConvertToMapObject(aMap);

            tempMapObject.SaveToFile(aFileNamePath);
        }

        public static Map LoadFromMapObjectFiles(string aFileNamePath)
        {
            MapObject tempMapObject = MapObject.LoadFromFile(aFileNamePath);
            Map tempMap = FromMapObject(tempMapObject);
            return tempMap;
        }


        public static MapObject ConvertToMapObject(Map aMap)
        {
            MapObject tempMapObject = new MapObject
            {
                TheMapDetail = new MapDetail {TheBoundingBox = aMap.theBoundingBox},
                TheBitmap = ConvertToBitmap(aMap)
            };
            return tempMapObject;
        }

        public static Map FromMapObject(MapObject aMapObject)
        {
            Stopwatch watch = Stopwatch.StartNew();

            int width = (int) aMapObject.TheMapDetail.TheBoundingBox.Width;
            int height = (int) aMapObject.TheMapDetail.TheBoundingBox.Height;

            Dictionary<Tile.TileIds, TileType> tileTypes = Tile.GetTileTypes();
            Dictionary<int, Tile.TileIds> colorToTileIds = new Dictionary<int, Tile.TileIds>();
            foreach (Tile.TileIds aTileIds in tileTypes.Keys)
            {
                TileType temptype = tileTypes[aTileIds];
                Color tempColor = temptype.Color;
                colorToTileIds.Add(tempColor.ToArgb(), aTileIds);
            }


            const PixelFormat aPixelFormat = PixelFormat.Format32bppArgb;
            Bitmap tempBitmap = aMapObject.TheBitmap;
            if (tempBitmap.PixelFormat != aPixelFormat)
            {
                throw new Exception("Bitmap is of wrong pixel format Bitmap.PixelFormat: " + tempBitmap.PixelFormat +
                                    " required: " + aPixelFormat);
            }
            Map tempMap = new Map();
            Tile[] tempTiles = new Tile[width*height];
            // lock source bitmap data
            BitmapData srcData = tempBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                aPixelFormat);
            int srcOffset = (srcData.Stride - width*4)/4;

            int mapOffsetX = (int) aMapObject.TheMapDetail.TheBoundingBox.Location.X;
            int mapOffsetY = (int) aMapObject.TheMapDetail.TheBoundingBox.Location.Y;

            int index = 0;
            unsafe
            {
                int* src = (int*) srcData.Scan0.ToPointer();
//                byte* src = (byte*) srcData.Scan0.ToPointer();
                // for each row
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
//                    for (int x = 0; x < width; x++, src += 4)
                    for (int x = 0; x < width; x++, src++, index++)
                    {
                        int color = *src;
                        Tile tempTile = new Tile(colorToTileIds[color])
                        {
                            Location = new Vector(x + mapOffsetX, y + mapOffsetY)
                        };
                        tempTiles[index] = tempTile;
                    }
                    src += srcOffset;
                }
            }
            tempMap.TheTileArray = tempTiles;

            // unlock image
            tempBitmap.UnlockBits(srcData);
            watch.Stop();
            GameCore.TheGameCore.RaiseMessage("FromMapObject() took " + watch.ElapsedMilliseconds + "ms, MapObject: " +
                                              aMapObject);

            return tempMap;
        }

        public static Bitmap ConvertToBitmap(Map aMap)
        {
            Stopwatch watch = Stopwatch.StartNew();

            int width = (int) aMap.theBoundingBox.Width;
            int height = (int) aMap.theBoundingBox.Height;
            Tile[] tempTiles = aMap.TheTileArray;
            Dictionary<Tile.TileIds, TileType> tileTypes = Tile.GetTileTypes();
            Dictionary<Tile.TileIds, int> tileIdToColor = new Dictionary<Tile.TileIds, int>();
            foreach (Tile.TileIds aTileIds in tileTypes.Keys)
            {
                TileType temptype = tileTypes[aTileIds];
                Color tempColor = temptype.Color;
                tileIdToColor.Add(aTileIds, tempColor.ToArgb());
            }


            const PixelFormat aPixelFormat = PixelFormat.Format32bppArgb;
            Bitmap tempBitmap = new Bitmap(width, height, aPixelFormat);

            // lock source bitmap data
            BitmapData srcData = tempBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                aPixelFormat);
//            int srcOffset = srcData.Stride - width*4;
            int srcOffset = (srcData.Stride - width*4)/4;

            int mapOffsetX = (int) aMap.theBoundingBox.Location.X;
            int mapOffsetY = (int) aMap.theBoundingBox.Location.Y;
            int index = 0;
            unsafe
            {
//                byte* src = (byte*) srcData.Scan0.ToPointer();
                int* src = (int*) srcData.Scan0.ToPointer();

                // for each row
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
//                    for (int x = 0; x < width; x++, src += 4)
                    for (int x = 0; x < width; x++, src++, index++)
                    {
//                        Tile tempTile = aMap[x + mapOffsetX, y + mapOffsetY];
//                        Color tempColor = tileTypes[tempTile.TheTileId].Color;
//                        Color tempColor = tileTypes[aMap[x + mapOffsetX, y + mapOffsetY].TheTileId].Color;
//                        Color tempColor = tileIdToColor[aMap[x + mapOffsetX, y + mapOffsetY].TheTileId];
                        *src = tileIdToColor[tempTiles[index].TheTileId];

//                        src[ARGB.AIndex] = tempColor.A;
//                        src[ARGB.RIndex] = tempColor.R;
//                        src[ARGB.GIndex] = tempColor.G;
//                        src[ARGB.BIndex] = tempColor.B;
                    }
                    src += srcOffset;
                }
            }
            // unlock image
            tempBitmap.UnlockBits(srcData);
            watch.Stop();
            GameCore.TheGameCore.RaiseMessage("ConvertToBitmap() took " + watch.ElapsedMilliseconds + "ms, Map: " + aMap);

            return tempBitmap;
        }

        #endregion

        public override string ToString()
        {
            string outStr = "";
            outStr += "BoudingBox: " + theBoundingBox;
            outStr += " with " + theTileArray.Length + " tiles";
            return outStr;
        }
    }
}