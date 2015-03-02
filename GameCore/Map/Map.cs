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
        private RectangleF theBoundingBox = RectangleF.Empty;


//        private Tile[,] theTiles;

        private Dictionary<Vector, Tile> theTiles;
        private List<Tile> tiles;

        public Map()
        {
            theTiles = new Dictionary<Vector, Tile>();
            tiles = new List<Tile>();
//            Init();
        }

        /// <summary>
        ///     Initialize the map object.
        /// </summary>
        private void Init()
        {
            Tiles = CreatTestTiles(new Point(-10, -10), new Size(20, 20));
            theBoundingBox = GetBoundingBox(theTiles);
        }

        [XmlIgnore]
        public RectangleF TheBoundingBox
        {
            get { return theBoundingBox; }
        }

        [XmlElement("Tiles")]
        public List<Tile> Tiles
        {
            get { return tiles; }
            set
            {
                tiles = value;
                theTiles.Clear();
                foreach (Tile aTile in tiles)
                {
                    theTiles.Add(aTile.Location, aTile);
                }
                theBoundingBox = GetBoundingBox(theTiles);
            }
        }


        [XmlIgnore]
        public Tile this[int x, int y]
        {
            get
            {
                Vector loc = new Vector(x, y);
                if (theTiles.ContainsKey(loc))
                {
                    return theTiles[loc];
                }
                else
                {
                    return null;
                }
            }
        }

        private static RectangleF GetBoundingBox(Dictionary<Vector, Tile> tempTiles)
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (Vector aLoc in tempTiles.Keys)
            {
                if (aLoc.X > maxX)
                {
                    maxX = aLoc.X;
                }
                if (aLoc.X < minX)
                {
                    minX = aLoc.X;
                }
                if (aLoc.Y > maxY)
                {
                    maxY = aLoc.Y;
                }
                if (aLoc.Y < minY)
                {
                    minY = aLoc.Y;
                }
            }
            return new RectangleF(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

        internal static void SaveMap(string aFilePath, Map aMap)
        {
            SaveObjects.SerializeObject(aFilePath, aMap);
        }

        internal static Map LoadMap(string aFilePath)
        {
            Map aMap = (Map) SaveObjects.DeserializeObject(aFilePath, typeof (Map));
            return aMap;
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

            Map aMap = new Map {Tiles = CreatTestTiles(new Point(-250, -250), new Size(500, 500))};
            watch.Stop();
            GameCore.TheGameCore.RaiseMessage(string.Format("CreateTestMap() took {0}ms", watch.ElapsedMilliseconds));


            return aMap;
        }


        private static List<Tile> CreatTestTiles(Point startPos, Size aSize)
        {
            List<Tile> tempTiles = new List<Tile>();
            Tile tempTile = null;
            int x0 = startPos.X;
            int x1 = startPos.X + aSize.Width;

            int y0 = startPos.Y;
            int y1 = startPos.Y + aSize.Height;


            for (int x = x0; x < x1; x++)
            {
                for (int y = y0; y < y1; y++)
                {
                    if (x == 8)
                    {
                        tempTile = new Tile(Tile.TileIds.Grass) {Location = new Vector(x, y)};
                    }
                    else if (x == 5)
                    {
                        tempTile = new Tile(Tile.TileIds.Road) {Location = new Vector(x, y)};
                    }
                    else if (x == 2)
                    {
                        tempTile = new Tile(Tile.TileIds.Water) {Location = new Vector(x, y)};
                    }
                    else if (x == 12)
                    {
                        tempTile = new Tile(Tile.TileIds.Wall) {Location = new Vector(x, y)};
                    }
                    else
                    {
                        tempTile = new Tile(Tile.TileIds.Desert) {Location = new Vector(x, y)};
                    }
                    tempTiles.Add(tempTile);
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

        public static void TestFromMapObject(Map aMap)
        {
//            MapObject tempMapObject = ConvertToMapObject(aMap);
            SaveToMapObject(aMap, @".\testMapSave");
            Map tempMap = LoadFromMapObjectFiles(@".\testMapSave");
//            Map tempMap = FromMapObject(tempMapObject);
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
            List<Tile> tempTiles = new List<Tile>();
            // lock source bitmap data
            BitmapData srcData = tempBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                aPixelFormat);
            int srcOffset = (srcData.Stride - width*4)/4;

            int mapOffsetX = (int) aMapObject.TheMapDetail.TheBoundingBox.Location.X;
            int mapOffsetY = (int) aMapObject.TheMapDetail.TheBoundingBox.Location.Y;

            unsafe
            {
                int* src = (int*) srcData.Scan0.ToPointer();
//                byte* src = (byte*) srcData.Scan0.ToPointer();
                int x;
                // for each row
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
//                    for (int x = 0; x < width; x++, src += 4)
                    for (x = 0; x < width; x++, src++)
                    {
                        int color = *src;
                        Tile tempTile = new Tile(colorToTileIds[color])
                        {
                            Location = new Vector(x + mapOffsetX, y + mapOffsetY)
                        };
                        tempTiles.Add(tempTile);
                    }
                    src += srcOffset;
                }
            }
            tempMap.Tiles = tempTiles;

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

            unsafe
            {
//                byte* src = (byte*) srcData.Scan0.ToPointer();
                int* src = (int*) srcData.Scan0.ToPointer();

                // for each row
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
//                    for (int x = 0; x < width; x++, src += 4)
                    for (int x = 0; x < width; x++, src++)
                    {
//                        Tile tempTile = aMap[x + mapOffsetX, y + mapOffsetY];
//                        Color tempColor = tileTypes[tempTile.TheTileId].Color;
//                        Color tempColor = tileTypes[aMap[x + mapOffsetX, y + mapOffsetY].TheTileId].Color;
//                        Color tempColor = tileIdToColor[aMap[x + mapOffsetX, y + mapOffsetY].TheTileId];
                        *src = tileIdToColor[aMap[x + mapOffsetX, y + mapOffsetY].TheTileId];

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

        public override string ToString()
        {
            string outStr = "";
            outStr += "BoudingBox: " + theBoundingBox;
            outStr += " with " + tiles.Count + " tiles";
            return outStr;
        }
    }

    /// ARGB components
    /// Note: PixelFormat.Format24bppRgb actually means BGRA format
    /// </summary>
    public class ARGB
    {
        public const short AIndex = 3;
        public const short RIndex = 2;
        public const short GIndex = 1;
        public const short BIndex = 0;

        /// <summary>
        ///     [0...255]
        /// </summary>
        public byte Alpha;

        /// <summary>
        ///     [0...255]
        /// </summary>
        public byte Red;

        /// <summary>
        ///     [0...255]
        /// </summary>
        public byte Green;

        /// <summary>
        ///     [0...255]
        /// </summary>
        public byte Blue;

        // Color property
        public Color Color
        {
            get { return Color.FromArgb(Alpha, Red, Green, Blue); }
            set
            {
                Alpha = value.A;
                Red = value.R;
                Green = value.G;
                Blue = value.B;
            }
        }

        // Constructors
        public ARGB()
        {
        }

        public ARGB(byte alpha, byte red, byte green, byte blue)
        {
            Alpha = alpha;
            Red = red;
            Green = green;
            Blue = blue;
        }

        public override string ToString()
        {
            return "A: " + Alpha + "R: " + Red + " G: " + Green + " B: " + Blue;
        }
    }

    /// RGB components
    /// Note: PixelFormat.Format24bppRgb actually means BGR format
    /// </summary>
    public class RGB
    {
        public const short RIndex = 2;
        public const short GIndex = 1;
        public const short BIndex = 0;

        /// <summary>
        ///     [0...255]
        /// </summary>
        public byte Red;

        /// <summary>
        ///     [0...255]
        /// </summary>
        public byte Green;

        /// <summary>
        ///     [0...255]
        /// </summary>
        public byte Blue;

        // Color property
        public Color Color
        {
            get { return Color.FromArgb(Red, Green, Blue); }
            set
            {
                Red = value.R;
                Green = value.G;
                Blue = value.B;
            }
        }

        // Constructors
        public RGB()
        {
        }

        public RGB(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public override string ToString()
        {
            return "R: " + Red + " G: " + Green + " B: " + Blue;
        }
    }
}