#region

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GameCore.GameObjects;
using GameCore.Map;
using GameCore.Render.RenderMaterial;
using GameCore.Utils;
using OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

#endregion

namespace GameCore.Render.RenderObjects
{
    public class RenderObjects
    {
        public static ResourceManager TheResourceManager;

        public RenderObjects()
        {
        }


        public enum BoxSides
        {
            Top,
            Front,
            Left,
            Right,
            Back,
            Bottom
        }

        public static Dictionary<BoxSides, Bitmap> GetBoxTextures(string anImagePath,int rows, int cols, BoxIndex[] anBoxIndix)
        {
            Dictionary<BoxSides, Bitmap> bitmapsList = new Dictionary<BoxSides, Bitmap>();
            string tempFilePath = Path.Combine(TheResourceManager.ImageDirectory, anImagePath);
            Bitmap tempMainImage = (Bitmap) Bitmap.FromFile(tempFilePath);
//            tempMainImage.RotateFlip(RotateFlipType.RotateNoneFlipY); // bitmaps read from bottom up, so flip it
            if (tempMainImage.PixelFormat != PixelFormat.Format32bppArgb)
            {
                Bitmap clone = new Bitmap(tempMainImage.Width, tempMainImage.Height,
                    System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                using (Graphics gr = Graphics.FromImage(clone))
                {
                    gr.DrawImage(tempMainImage, new Rectangle(0, 0, clone.Width, clone.Height));
                }
                tempMainImage = clone;
            }
            int width = tempMainImage.Width;
            int height = tempMainImage.Height;

            int detlaWidth = width/cols;
            int detlaHeight = height/rows;

            // X row
            // Y col
            // index = X + Y * cols;  
            // X = index%cols
            // Y = index/cols
            //

            int X;
            int Y;
            for (int i = 0; i < anBoxIndix.Length; i++)
            {
                BoxIndex tempBoxSide = anBoxIndix[i];
                X = tempBoxSide.Index%cols;
                Y = tempBoxSide.Index/cols;

                Rectangle tempRectangle = new Rectangle(X * detlaWidth, Y * detlaHeight, detlaWidth, detlaHeight);
                Bitmap tempBitmap = tempMainImage.Clone(tempRectangle, PixelFormat.Format32bppArgb);

                bitmapsList.Add(tempBoxSide.BoxSide,tempBitmap);
            }
            tempMainImage.Dispose();
            tempMainImage = null;

            return bitmapsList;
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

                string tempFilePath = Path.Combine(TheResourceManager.TileDirectory, tempTiletextureName);
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
                    if (tempBmp.PixelFormat != PixelFormat.Format32bppArgb)
                    {
                        Bitmap clone = new Bitmap(tempBmp.Width, tempBmp.Height,
                            System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                        using (Graphics gr = Graphics.FromImage(clone))
                        {
                            gr.DrawImage(tempBmp, new Rectangle(0, 0, clone.Width, clone.Height));
                        }
                        tempBmp = clone;
                    }
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
                tempMaterial = TheResourceManager.GetFromFile(aProgram, tempTiletextureName,false,ResourceManager.ResourceTypes.Tile);
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

        public static Dictionary<ObjectGame.ObjcetIds, PlainBmpTexture> CreateGameObjectsTextures(Size aTextureSize,
            ShaderProgram aProgram)
        {
            Dictionary<ObjectGame.ObjcetIds, GameObjectType> objTypeList = ObjectGame.GetObjTypes();
            Dictionary<ObjectGame.ObjcetIds, PlainBmpTexture> objTextureList =
                new Dictionary<ObjectGame.ObjcetIds, PlainBmpTexture>();

            SolidBrush tempBrush;
            Bitmap tempBmp;
            ObjMaterial tempMaterial;

            foreach (KeyValuePair<ObjectGame.ObjcetIds, GameObjectType> keyValuePair in objTypeList)
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

    public class BoxIndex
    {
        public int Index;
        public RenderObjects.BoxSides BoxSide;


        public static BoxIndex[] GetTypeT()
        {
            BoxIndex[] anBoxIndix = new BoxIndex[]
            {
                new BoxIndex() {Index = 1, BoxSide = RenderObjects.BoxSides.Top},
                new BoxIndex() {Index = 4, BoxSide = RenderObjects.BoxSides.Left},
                new BoxIndex() {Index = 5, BoxSide = RenderObjects.BoxSides.Back},
                new BoxIndex() {Index = 6, BoxSide = RenderObjects.BoxSides.Right},
                new BoxIndex() {Index = 7, BoxSide = RenderObjects.BoxSides.Front},
                new BoxIndex() {Index = 9, BoxSide = RenderObjects.BoxSides.Bottom},
            };
            return anBoxIndix;
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