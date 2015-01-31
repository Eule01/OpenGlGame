#region

using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;
using GameCore.Utils;

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
            Tiles = CreatTestTiles(new Size(20, 20));
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
            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
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

        internal static Map GetTestMap()
        {
            Map aMap = new Map {Tiles = CreatTestTiles(new Size(20, 20))};
            return aMap;
        }


        private static List<Tile> CreatTestTiles(Size aSize)
        {
            List<Tile> tempTiles = new List<Tile>();
            Tile tempTile = null;
            for (int x = 0; x < aSize.Width; x++)
            {
                for (int y = 0; y < aSize.Height; y++)
                {
                    if (x == 5)
                    {
                        tempTile = new Tile(Tile.TileIds.Road) {Location = new Vector(x, y)};
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
    }
}