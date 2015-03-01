#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;
using GameCore.Utils;

#endregion

namespace GameCore.Map
{
    /// <summary>
    ///     This represents a tile object, the map is made up of tiles.
    /// </summary>
    public class Tile
    {
        public enum TileIds : uint
        {
            Desert = 0,
            Grass = 1,
            Road = 2,
            Water = 3,
            Wall = 4,
        }

        public static Vector Size = new Vector(1, 1);

        private TileIds theTileId = TileIds.Desert;

        public Tile()
        {
        }

        public Tile(TileIds aTileId)
        {
            theTileId = aTileId;
        }


        public TileIds TheTileId
        {
            get { return theTileId; }
            set { theTileId = value; }
        }

        public Vector Location = new Vector(0, 0);

        public override string ToString()
        {
            string outStr = "";
            outStr += Location.ToString();
            outStr += " " + theTileId;

            return outStr;
        }

        public static Dictionary<TileIds, TileType> GetTileTypes()
        {
            Dictionary<TileIds, TileType> tempList = new Dictionary<TileIds, TileType>();

            foreach (TileIds aTileType in (TileIds[]) Enum.GetValues(typeof (TileIds)))
            {
                TileType tempType = new TileType(aTileType.ToString());
                switch (aTileType)
                {
                    case TileIds.Desert:
                        tempType.Color = Color.Wheat;
                        break;
                    case TileIds.Grass:
                        tempType.Color = Color.Green;
                        break;
                    case TileIds.Road:
                        tempType.Color = Color.DarkSlateGray;
                        break;
                    case TileIds.Water:
                         tempType.Color = Color.Blue;
                        break;
                    case TileIds.Wall:
                         tempType.Color = Color.MidnightBlue;
                        break;                 
                    default:
                        tempType.Color = Color.Gray;
                        break;
                }
                tempList.Add(aTileType, tempType);
            }

            return tempList;
        }
    }
}