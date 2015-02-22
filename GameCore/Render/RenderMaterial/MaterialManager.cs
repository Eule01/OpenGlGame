#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GameCore.Utils;
using OpenGL;

#endregion

namespace GameCore.Render.RenderMaterial
{
    /// <summary>
    ///     This class manages all the materials (textures) used
    /// </summary>
    public class MaterialManager
    {
        private Dictionary<string, ObjMaterial> materials = new Dictionary<string, ObjMaterial>();

        private string imageDirectory = @"./Resources/Images/";

        private string tileDirectory = @"Tiles";
        private string skyBoxDirectory = @"SkyBoxes";

        public enum ResourceTypes
        {
            Tile,
            Skybox,
            Unknown
        }


        public string ImageDirectory
        {
            get { return imageDirectory; }
        }

        public string TileDirectory
        {
            get { return Path.Combine(imageDirectory,tileDirectory); }
        }

        public string SkyBoxDirectory
        {
            get { return Path.Combine(imageDirectory,skyBoxDirectory); }
        }


        private void AddMaterial(string aName, ObjMaterial anObjMaterial)
        {
            anObjMaterial.Name = aName;
            materials.Add(aName, anObjMaterial);
        }

        public ObjMaterial Get(string aName)
        {
            if (materials.ContainsKey(aName))
            {
                return materials[aName];
            }
            return null;
        }

        public ObjMaterial GetFromFile(ShaderProgram program, string aName, string aFileName, bool FlipY = true, ResourceTypes aResourceTypes = ResourceTypes.Unknown)
        {
            if (materials.ContainsKey(aName))
            {
                return materials[aName];
            }
            string tempFilePath;
            switch (aResourceTypes)
            {
                case ResourceTypes.Tile:
                    tempFilePath = TileDirectory;              
                    break;
                case ResourceTypes.Skybox:
                    tempFilePath = SkyBoxDirectory;
                    break;
                case ResourceTypes.Unknown:
                    tempFilePath = ImageDirectory;
                   break;
                default:
                    throw new ArgumentOutOfRangeException("aResourceTypes");
            }
            tempFilePath = Path.Combine(tempFilePath, aFileName);
            if (!File.Exists(tempFilePath))
            {
                GameCore.TheGameCore.RaiseMessage(string.Format("MaterialManager.GetFromFile() file does not exist: "+ tempFilePath));
                return null;
            }

            Texture tempTexture = new Texture(tempFilePath,FlipY);
            ObjMaterial tempMaterial = new ObjMaterial(program) {DiffuseMap = tempTexture};
            AddMaterial(aName, tempMaterial);

            return tempMaterial;
        }

        public ObjMaterial GetFromFile(ShaderProgram program, string aFileName, bool FlipY = true, ResourceTypes aResourceTypes = ResourceTypes.Unknown)
        {
            string aName = Path.GetFileNameWithoutExtension(aFileName);
            return GetFromFile(program, aName, aFileName, FlipY, aResourceTypes);
        }

        public ObjMaterial GetPlainColor(ShaderProgram program, string aName, Color aColor)
        {
            if (materials.ContainsKey(aName))
            {
                return materials[aName];
            }

            SolidBrush tempBrush = new SolidBrush(aColor);
            Bitmap tempBmp = BitmapHelper.CreatBitamp(new Size(20, 20), tempBrush);
            ObjMaterial tempMaterial = new ObjMaterial(program) {DiffuseMap = new Texture(tempBmp)};
            AddMaterial(aName, tempMaterial);
            return tempMaterial;
        }

        public void Close()
        {
            foreach (KeyValuePair<string, ObjMaterial> keyValuePair in materials)
            {
                keyValuePair.Value.Dispose();
            }
        }
    }
}