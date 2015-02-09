#region

using GameCore.Render.OpenGlHelper;
using GameCore.Render.RenderMaterial;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects
{
    public class ObjMeshSkyBox : ObjMesh
    {
        public ObjMeshSkyBox(ShaderProgram program)
            : base(program)
        {
            ObjObject tempObj =
                new ObjObject(ObjectPrimitives.CreateSquareWithNormalsYorZ(new Vector3(0.5, 0.5, -0.5),
                    new Vector3(-0.5, 0.5, 0.5),
                    true)); // TOP ?

            ObjMaterial tempMat =
                GameCore.TheGameCore.TheRendererManager.TheRenderer.TheMaterialManager.GetFromFile(program,
                    @".\SkyBoxTest\jajlands1_up.jpg");


            AddObject(tempObj, tempMat);

            tempObj =
                new ObjObject(ObjectPrimitives.CreateSquareWithNormalsYorZ(new Vector3(0.5, -0.5, 0.5),
                    new Vector3(-0.5, -0.5, -0.5),
                    true)); // Bottom 
            tempMat =
                GameCore.TheGameCore.TheRendererManager.TheRenderer.TheMaterialManager.GetFromFile(program,
                    @".\SkyBoxTest\jajlands1_dn.jpg");
            AddObject(tempObj, tempMat);

            tempObj =
                new ObjObject(ObjectPrimitives.CreateSquareWithNormalsYorZ(new Vector3(-0.5, -0.5, -0.5),
                    new Vector3(0.5, 0.5, -0.5),
                    true)); // Back 
            tempMat =
                GameCore.TheGameCore.TheRendererManager.TheRenderer.TheMaterialManager.GetFromFile(program,
                    @".\SkyBoxTest\jajlands1_bk.jpg");
            AddObject(tempObj, tempMat);

            tempObj =
                new ObjObject(ObjectPrimitives.CreateSquareWithNormalsYorZ(new Vector3(0.5, -0.5, 0.5),
                    new Vector3(-0.5, 0.5, 0.5),
                    true)); // Front 
            tempMat =
                GameCore.TheGameCore.TheRendererManager.TheRenderer.TheMaterialManager.GetFromFile(program,
                    @".\SkyBoxTest\jajlands1_ft.jpg");
            AddObject(tempObj, tempMat);

            tempObj =
                new ObjObject(ObjectPrimitives.CreateSquareWithNormalX(new Vector3(-0.5, -0.5, 0.5),
                    new Vector3(-0.5, 0.5, -0.5),
                    true)); // Left 
            tempMat =
                GameCore.TheGameCore.TheRendererManager.TheRenderer.TheMaterialManager.GetFromFile(program,
                    @".\SkyBoxTest\jajlands1_lf.jpg");
            AddObject(tempObj, tempMat);

            tempObj =
                new ObjObject(ObjectPrimitives.CreateSquareWithNormalX(new Vector3(0.5, -0.5, -0.5),
                    new Vector3(0.5, 0.5, 0.5),
                    true)); // Right 
            tempMat =
                GameCore.TheGameCore.TheRendererManager.TheRenderer.TheMaterialManager.GetFromFile(program,
                    @".\SkyBoxTest\jajlands1_rt.jpg");
            AddObject(tempObj, tempMat);
        }
    }
}