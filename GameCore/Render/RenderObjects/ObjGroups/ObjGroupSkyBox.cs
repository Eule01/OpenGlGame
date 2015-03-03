#region

using System.Collections.Generic;
using System.Drawing;
using GameCore.Render.OpenGlHelper;
using GameCore.Render.RenderMaterial;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects.ObjGroups
{
    public class ObjGroupSkyBox : ObjGroup
    {
        public ObjGroupSkyBox(ShaderProgram program)
            : base(program)
        {
        }


        public static ObjGroupSkyBox GetTexturedBox3(ShaderProgram program,
            Dictionary<RenderObjects.BoxSides, Bitmap> aBmpList)
        {
            ObjGroupSkyBox tempObjGroupSkyBox = new ObjGroupSkyBox(program);
            float dist = 0.5f;
            float distz = 0.5f;
            float distz0 = -0.5f;


            ObjectVectors tempVectors = new ObjectVectors(); // TOP ?
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(-dist, distz, -dist),
                    new Vector3(dist, distz, -dist),
                    new Vector3(-dist, distz, dist),
                    new Vector3(dist, distz, dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(0.0, 1.0),
                new Vector2(1.0, 1.0),
                new Vector2(0.0, 0.0),
                new Vector2(1.0, 0.0),
            };

            ObjMaterial tempMat = GetObjMaterial(program, aBmpList[RenderObjects.BoxSides.Top]);

            ObjObject tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);


            tempVectors = new ObjectVectors(); // Left
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(-dist, distz0, -dist),
                    new Vector3(-dist, distz, -dist),
                    new Vector3(-dist, distz0, dist),
                    new Vector3(-dist, distz, dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(1.0, 1.0),
                new Vector2(1.0, 0.0),
                new Vector2(0.0, 1.0),
                new Vector2(0.0, 0.0),
            };

            tempMat = GetObjMaterial(program, aBmpList[RenderObjects.BoxSides.Left]);

            tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);

            tempVectors = new ObjectVectors(); // Back
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(-dist, distz0, -dist),
                    new Vector3(-dist, distz, -dist),
                    new Vector3(dist, distz0, -dist),
                    new Vector3(dist, distz, -dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(0.0, 1.0),
                new Vector2(0.0, 0.0),
                new Vector2(1.0, 1.0),
                new Vector2(1.0, 0.0),
            };

            tempMat = GetObjMaterial(program, aBmpList[RenderObjects.BoxSides.Back]);


            tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);

            tempVectors = new ObjectVectors(); // Right
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(dist, distz0, -dist),
                    new Vector3(dist, distz, -dist),
                    new Vector3(dist, distz0, dist),
                    new Vector3(dist, distz, dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(0.0, 1.0),
                new Vector2(0.0, 0.0),
                new Vector2(1.0, 1.0),
                new Vector2(1.0, 0.0),
            };

            tempMat = GetObjMaterial(program, aBmpList[RenderObjects.BoxSides.Right]);

            tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);

            tempVectors = new ObjectVectors(); // Front
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(-dist, distz0, dist),
                    new Vector3(-dist, distz, dist),
                    new Vector3(dist, distz0, dist),
                    new Vector3(dist, distz, dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(1.0, 1.0),
                new Vector2(1.0, 0.0),
                new Vector2(0.0, 1.0),
                new Vector2(0.0, 0.0),
            };


            tempMat = GetObjMaterial(program, aBmpList[RenderObjects.BoxSides.Front]);


            tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);

            tempVectors = new ObjectVectors(); // Bottom ?
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(-dist, distz0, -dist),
                    new Vector3(dist, distz0, -dist),
                    new Vector3(-dist, distz0, dist),
                    new Vector3(dist, distz0, dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(0.0, 0.0),
                new Vector2(1.0, 0.0),
                new Vector2(0.0, 1.0),
                new Vector2(1.0, 1.0),
            };

            tempMat = GetObjMaterial(program, aBmpList[RenderObjects.BoxSides.Bottom]);

            tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);

            return tempObjGroupSkyBox;
        }

        private static ObjMaterial GetObjMaterial(ShaderProgram program, Bitmap bitmapImage)
        {
            ObjMaterial tempMat = new ObjMaterial(program);

            tempMat.DiffuseMap = new Texture(bitmapImage, false);

            Gl.BindTexture(tempMat.DiffuseMap);

            Gl.TexParameteri(tempMat.DiffuseMap.TextureTarget, TextureParameterName.TextureMinFilter,
                TextureParameter.Linear);
            Gl.TexParameteri(tempMat.DiffuseMap.TextureTarget, TextureParameterName.TextureMagFilter,
                TextureParameter.Linear);
            Gl.TexParameteri(tempMat.DiffuseMap.TextureTarget, TextureParameterName.TextureWrapS,
                TextureParameter.ClampToEdge);
            Gl.TexParameteri(tempMat.DiffuseMap.TextureTarget, TextureParameterName.TextureWrapT,
                TextureParameter.ClampToEdge);
            return tempMat;
        }


        /// <summary>
        ///     Loads a single image texture into the skybox this will probably always result in visibale seams.
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static ObjGroupSkyBox GetNewSkyBoxTypeT(ShaderProgram program, string aFilePath)
        {
            BoxIndex[] anBoxIndix = BoxIndex.GetTypeT();

            Dictionary<RenderObjects.BoxSides, Bitmap> tempBmpList =
//                RenderObjects.GetBoxTextures(@".\SkyBoxes\Above_The_Sea.jpg", 3, 4, anBoxIndix);
//                RenderObjects.GetBoxTextures(@".\SkyBoxes\2226.jpg", 3, 4, anBoxIndix);
//            RenderObjects.GetBoxTextures(@".\SkyBoxes\interstellar_large.jpg", 3, 4, anBoxIndix);
//            RenderObjects.GetBoxTextures(@".\SkyBoxes\grimmnight_large.jpg", 3, 4, anBoxIndix);
                RenderObjects.GetBoxTextures(aFilePath, 3, 4, anBoxIndix);

            ObjGroupSkyBox tempObjGroupSkyBox = GetTexturedBox3(program, tempBmpList);

            return tempObjGroupSkyBox;
        }

        /// <summary>
        ///     Loads a single image texture into the skybox this will probably always result in visibale seams.
        ///     Image 4x3
        ///     |       |       |
        ///     |  Top  |       |
        ///     -------------------------------
        ///     Left   | Front | Right | Back
        ///     |       |       |
        ///     -------------------------------
        ///     |Bottom |       |
        ///     |       |       |
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static ObjGroupSkyBox GetNewSkyBox2(ShaderProgram program)
        {
            ObjGroupSkyBox tempObjGroupSkyBox = new ObjGroupSkyBox(program);
            tempObjGroupSkyBox.ClearObjects();


//            ObjMaterial tempMat =
//                GameCore.TheGameCore.TheRendererManager.TheRenderer.theResourceManager.GetFromFile(program,
//                    @".\SkyBoxes\Day_Skybox.png", false);
            ObjMaterial tempMat =
                GameCore.TheGameCore.TheRendererManager.TheRenderer.TheResourceManager.GetFromFile(program,
                    @".\SkyBoxes\Above_The_Sea.jpg", false);

            float dist = 0.5f;
            float distz = 0.5f;
            float distz0 = -0.5f;


            ObjectVectors tempVectors = new ObjectVectors(); // TOP ?
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(-dist, distz, -dist),
                    new Vector3(dist, distz, -dist),
                    new Vector3(-dist, distz, dist),
                    new Vector3(dist, distz, dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            double aThird = 0.33;
            double twoThird = 0.66;
            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(0.25, aThird),
                new Vector2(0.5, aThird),
                new Vector2(0.25, 0.0),
                new Vector2(0.5, 0.0),
            };


            ObjObject tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);


            tempVectors = new ObjectVectors(); // Left
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(-dist, distz0, -dist),
                    new Vector3(-dist, distz, -dist),
                    new Vector3(-dist, distz0, dist),
                    new Vector3(-dist, distz, dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(0.25, twoThird),
                new Vector2(0.25, aThird),
                new Vector2(0.0, twoThird),
                new Vector2(0.0, aThird),
            };


            tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);

            tempVectors = new ObjectVectors(); // Back
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(-dist, distz0, -dist),
                    new Vector3(-dist, distz, -dist),
                    new Vector3(dist, distz0, -dist),
                    new Vector3(dist, distz, -dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(0.25, twoThird),
                new Vector2(0.25, aThird),
                new Vector2(0.5, twoThird),
                new Vector2(0.5, aThird),
            };

            tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);


            tempVectors = new ObjectVectors(); // Right
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(dist, distz0, -dist),
                    new Vector3(dist, distz, -dist),
                    new Vector3(dist, distz0, dist),
                    new Vector3(dist, distz, dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(0.5, twoThird),
                new Vector2(0.5, aThird),
                new Vector2(0.75, twoThird),
                new Vector2(0.75, aThird),
            };

            tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);


            tempVectors = new ObjectVectors(); // Front
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(-dist, distz0, dist),
                    new Vector3(-dist, distz, dist),
                    new Vector3(dist, distz0, dist),
                    new Vector3(dist, distz, dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(1, twoThird),
                new Vector2(1, aThird),
                new Vector2(0.75, twoThird),
                new Vector2(0.75, aThird),
            };

            tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);

            tempVectors = new ObjectVectors(); // Bottom ?
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(-dist, distz0, -dist),
                    new Vector3(dist, distz0, -dist),
                    new Vector3(-dist, distz0, dist),
                    new Vector3(dist, distz0, dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(0.25, twoThird),
                new Vector2(0.5, twoThird),
                new Vector2(0.25, 1.0),
                new Vector2(0.5, 1.0),
            };


            tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);


            return tempObjGroupSkyBox;
        }

        public static ObjGroupSkyBox GetNewSkyBox1(ShaderProgram program)
        {
            ObjGroupSkyBox tempObjGroupSkyBox = new ObjGroupSkyBox(program);
            tempObjGroupSkyBox.ClearObjects();


            ObjMaterial tempMat =
                GameCore.TheGameCore.TheRendererManager.TheRenderer.TheResourceManager.GetFromFile(program,
                    @".\SkyBoxes\SkyBox-Clouds-Few-Noon.png", false);

            float dist = 0.5f;
            float distz = 0.45f;
            float distz0 = -0.05f;


            ObjectVectors tempVectors = new ObjectVectors(); // TOP ?
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(-dist, distz, -dist),
                    new Vector3(dist, distz, -dist),
                    new Vector3(-dist, distz, dist),
                    new Vector3(dist, distz, dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(0.25, 0.25),
                new Vector2(0.75, 0.25),
                new Vector2(0.25, 0.75),
                new Vector2(0.75, 0.75),
            };

            ObjObject tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);

            tempVectors = new ObjectVectors(); // Left
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(-dist, distz0, -dist),
                    new Vector3(-dist, distz, -dist),
                    new Vector3(-dist, distz0, dist),
                    new Vector3(-dist, distz, dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(0.0, 0.25),
                new Vector2(0.25, 0.25),
                new Vector2(0.0, 0.75),
                new Vector2(0.25, 0.75),
            };

            tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);

            tempVectors = new ObjectVectors(); // Back
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(-dist, distz0, -dist),
                    new Vector3(-dist, distz, -dist),
                    new Vector3(dist, distz0, -dist),
                    new Vector3(dist, distz, -dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(0.25, 0.0),
                new Vector2(0.25, 0.25),
                new Vector2(0.75, 0.0),
                new Vector2(0.75, 0.25),
            };

            tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);


            tempVectors = new ObjectVectors(); // Right
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(dist, distz0, -dist),
                    new Vector3(dist, distz, -dist),
                    new Vector3(dist, distz0, dist),
                    new Vector3(dist, distz, dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(1.0, 0.25),
                new Vector2(0.75, 0.25),
                new Vector2(1.0, 0.75),
                new Vector2(0.75, 0.75),
            };

            tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);


            tempVectors = new ObjectVectors(); // Front
            tempVectors.Vertex =
                tempVectors.Vertex = new[]
                {
                    new Vector3(-dist, distz0, dist),
                    new Vector3(-dist, distz, dist),
                    new Vector3(dist, distz0, dist),
                    new Vector3(dist, distz, dist),
                };

            tempVectors.ElementData = new[]
            {
                0, 1, 2,
                1, 3, 2,
            };

            tempVectors.Uvs = new Vector2[]
            {
                new Vector2(0.25, 1.0),
                new Vector2(0.25, 0.75),
                new Vector2(0.75, 1.0),
                new Vector2(0.75, 0.75),
            };

            tempObj = new ObjObject(tempVectors) {Material = tempMat};
            tempObjGroupSkyBox.AddObject(tempObj);

            return tempObjGroupSkyBox;
        }

//        public override void Draw()
//        {
//            if (modelMatrixOld)
//            {
//                UpdateModelMatrix();
//            }
//            defaultProgram["model_matrix"].SetValue(modelMatrix);
//
//            List<IObjObject> transparentObjects = new List<IObjObject>();
//
//            foreach (IObjObject anObj in objects)
//            {
//                if (anObj.Material.Transparency < 1f) transparentObjects.Add(anObj);
//                else anObj.Draw();
//            }
//
//            foreach (IObjObject anObj in transparentObjects)
//            {
//                anObj.Draw();
//            }
//
//
//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter,
//    TextureParameter.Linear);
//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter,
//                TextureParameter.Linear);
//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS,
//                TextureParameter.ClampToEdge);
//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT,
//                TextureParameter.ClampToEdge);
//
//
//        }
    }
}