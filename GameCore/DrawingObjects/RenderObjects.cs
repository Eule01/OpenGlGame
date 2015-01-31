#region

using System.Collections.Generic;
using System.Drawing;
using GameCore.GameObjects;
using GameCore.Map;
using GameCore.Utils;
using OpenGL;

#endregion

namespace GameCore.DrawingObjects
{
    public class RenderObjects
    {
        public RenderObjects()
        {
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
                PlainBmpTexture tempBmpTexture = new PlainBmpTexture(keyValuePair.Value.Name)
                    {
                        Color = keyValuePair.Value.Color
                    };
                tempBrush = new SolidBrush(tempBmpTexture.Color);
                tempBmp = BitmapHelper.CreatBitamp(aTextureSize, tempBrush);
                tempBmpTexture.TextureBmp = tempBmp;
                tempMaterial = new ObjMaterial(aProgram) {DiffuseMap = new Texture(tempBmp)};
                tempBmpTexture.Material = tempMaterial;
                tiletextureList.Add(keyValuePair.Key, tempBmpTexture);
            }

            return tiletextureList;
        }


        public static ObjMaterial CreatPlainMaterial(Size aTextureSize, ShaderProgram aProgram, Color aColor)
        {
                ObjMaterial tempMaterial;
                SolidBrush tempBrush = new SolidBrush(aColor);
                Bitmap tempBmp = BitmapHelper.CreatBitamp(aTextureSize, tempBrush);
                tempMaterial = new ObjMaterial(aProgram) { DiffuseMap = new Texture(tempBmp) };

            return tempMaterial;
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