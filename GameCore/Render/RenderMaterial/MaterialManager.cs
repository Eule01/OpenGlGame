#region

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

        public ObjMaterial GetFromFile(ShaderProgram program, string aName, string aFileName)
        {
            if (materials.ContainsKey(aName))
            {
                return materials[aName];
            }

            string tempFilePath = Path.Combine(imageDirectory, aFileName);
            if (!File.Exists(tempFilePath))
            {
                return null;
            }

            Texture tempTexture = new Texture(tempFilePath);
            ObjMaterial tempMaterial = new ObjMaterial(program) {DiffuseMap = tempTexture};
            AddMaterial(aName, tempMaterial);

            return tempMaterial;
        }

        public ObjMaterial GetFromFile(ShaderProgram program, string aFileName)
        {
            string aName = Path.GetFileNameWithoutExtension(aFileName);
            return GetFromFile(program, aName, aFileName);
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