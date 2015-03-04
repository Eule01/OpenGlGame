#region

using System;
using System.Collections.Generic;
using System.Drawing;
using GameCore.Map;
using OpenGL;

#endregion

namespace GameCore.Utils.PathFinder.FirstAStar
{
    public class SearchHelpers
    {
        public static bool[,] GetSearchBoolMap(Map.Map aMap)
        {
            int width = aMap.Width;
            int height = aMap.Height;
            Tile[] tempTiles = aMap.TheTileArray;

            bool[,] tempMap = new bool[width, height];
            int index = 0;
            unsafe
            {
                // for each row
                for (int y = 0; y < height; y++)
                {
                    // for each pixel
                    //                    for (int x = 0; x < width; x++, src += 4)
                    for (int x = 0; x < width; x++,  index++)
                    {
                        switch (tempTiles[index].TheTileId)
                        {
                            case Tile.TileIds.Desert:
                                tempMap[x, y] = true;
                                break;
                            case Tile.TileIds.Grass:
                                tempMap[x, y] = true;
                                break;
                            case Tile.TileIds.Road:
                                tempMap[x, y] = true;
                                break;
                            case Tile.TileIds.Water:
                                tempMap[x, y] = false;
                                break;
                            case Tile.TileIds.Wall:
                                tempMap[x, y] = false;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            return tempMap;
        }

        public static Vector3[] PathToVector3Array(Map.Map aMap, List<Point> aPath, float aHeith)
        {
            int offestX = aMap.TheBoundingBox.Location.X;
            int offestY = aMap.TheBoundingBox.Location.Y;


            Vector3[] temVector3s = new Vector3[aPath.Count];

            for (int index = 0; index < aPath.Count; index++)
            {
                Point aPoint = aPath[index];
                temVector3s[index] = new Vector3(aPoint.X + offestX, aHeith, aPoint.Y + offestY);
            }
            return temVector3s;
        }

    }
}