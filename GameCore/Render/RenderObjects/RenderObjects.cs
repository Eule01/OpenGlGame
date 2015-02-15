#region

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GameCore.GameObjects;
using GameCore.Map;
using GameCore.Render.RenderMaterial;
using GameCore.Utils;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects
{
    public class RenderObjects
    {
        public static MaterialManager TheMaterialManager;

        public RenderObjects()
        {
        }

        public static Dictionary<Tile.TileIds, Bitmap> CreateTileBitmaps(Size aTextureSize)
        {
            Dictionary<Tile.TileIds, TileType> tileList = Tile.GetTileTypes();
            Dictionary<Tile.TileIds, Bitmap> tiletextureList = new Dictionary<Tile.TileIds, Bitmap>();

            SolidBrush tempBrush;
            Bitmap tempBmp;

            foreach (KeyValuePair<Tile.TileIds, TileType> keyValuePair in tileList)
            {
                string tempTiletextureName = "Tile_" + keyValuePair.Key.ToString() + ".png";

                string tempFilePath = Path.Combine(TheMaterialManager.ImageDirectory, tempTiletextureName);
                PlainBmpTexture tempBmpTexture = new PlainBmpTexture(keyValuePair.Value.Name);

                if (!File.Exists(tempFilePath))
                {
                    tempBmpTexture.Color = keyValuePair.Value.Color;
                    tempBrush = new SolidBrush(tempBmpTexture.Color);
                    tempBmp = BitmapHelper.CreatBitamp(aTextureSize, tempBrush);
                 }
                else
                {
                    tempBmp = (Bitmap) Bitmap.FromFile(tempFilePath); //Filename);
                    tempBmp.RotateFlip(RotateFlipType.RotateNoneFlipY); // bitmaps read from bottom up, so flip it
                }
                tiletextureList.Add(keyValuePair.Key, tempBmp);
            }

            return tiletextureList;
        }


        public static Dictionary<Tile.TileIds, PlainBmpTexture> CreateTileTextures(Size aTextureSize,
            ShaderProgram aProgram)
        {
            Dictionary<Tile.TileIds, TileType> tileList = Tile.GetTileTypes();
            Dictionary<Tile.TileIds, PlainBmpTexture> tiletextureList =
                new Dictionary<Tile.TileIds, PlainBmpTexture>();

            SolidBrush tempBrush;
            Bitmap tempBmp;
            ObjMaterial tempMaterial;

            foreach (KeyValuePair<Tile.TileIds, TileType> keyValuePair in tileList)
            {
                string tempTiletextureName = "Tile_" + keyValuePair.Key.ToString() + ".png";

                PlainBmpTexture tempBmpTexture = new PlainBmpTexture(keyValuePair.Value.Name);
                tempMaterial = TheMaterialManager.GetFromFile(aProgram, tempTiletextureName);
                if (tempMaterial == null)
                {
                    tempBmpTexture.Color = keyValuePair.Value.Color;
                    tempBrush = new SolidBrush(tempBmpTexture.Color);
                    tempBmp = BitmapHelper.CreatBitamp(aTextureSize, tempBrush);
                    tempBmpTexture.TextureBmp = tempBmp;
                    tempMaterial = new ObjMaterial(aProgram) {DiffuseMap = new Texture(tempBmp)};
                    tempBmpTexture.Material = tempMaterial;
                }
                else
                {
                    tempBmpTexture.Material = tempMaterial;
                }
                tiletextureList.Add(keyValuePair.Key, tempBmpTexture);
            }

            return tiletextureList;
        }

        public static Dictionary<GameObject.ObjcetIds, PlainBmpTexture> CreateGameObjectsTextures(Size aTextureSize,
            ShaderProgram aProgram)
        {
            Dictionary<GameObject.ObjcetIds, GameObjectType> objTypeList = GameObject.GetObjTypes();
            Dictionary<GameObject.ObjcetIds, PlainBmpTexture> objTextureList =
                new Dictionary<GameObject.ObjcetIds, PlainBmpTexture>();

            SolidBrush tempBrush;
            Bitmap tempBmp;
            ObjMaterial tempMaterial;

            foreach (KeyValuePair<GameObject.ObjcetIds, GameObjectType> keyValuePair in objTypeList)
            {
                PlainBmpTexture tempBmpTexture = new PlainBmpTexture(keyValuePair.Value.Name)
                {
                    Color = keyValuePair.Value.Color
                };
                tempBrush = new SolidBrush(tempBmpTexture.Color);
                tempBmp = BitmapHelper.CreatBitamp(aTextureSize, tempBrush);
                tempBmpTexture.TextureBmp = tempBmp;
                tempMaterial = new ObjMaterial(aProgram) {DiffuseMap = new Texture(tempBmp)};
                tempBmpTexture.Material = tempMaterial;
                objTextureList.Add(keyValuePair.Key, tempBmpTexture);
            }

            return objTextureList;
        }
    }

    public class PlainBmpTexture : TileType
    {
        public PlainBmpTexture(string name) : base(name)
        {
        }

        public Bitmap TextureBmp;

        public ObjMaterial Material;
    }
}