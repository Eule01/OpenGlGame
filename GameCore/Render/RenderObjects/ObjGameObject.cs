#region

using System;
using GameCore.GameObjects;
using GameCore.Render.OpenGlHelper;
using GameCore.Utils;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects
{
    public class ObjGameObject : ObjObject
    {

        public ObjGameObject(Vector3[] vertexData, int[] elementData) : base(vertexData, elementData)
        {
        }

        public ObjGameObject(ObjectVectors anObjectVectors) : base(anObjectVectors)
        {
        }

        public void Draw(ShaderProgram aProgram)
        {
            if (vertices == null || triangles == null) return;

            Gl.Disable(EnableCap.CullFace);
            if (Material != null) Material.Use();
//            Vector tempLoc = theObjectGame.Location;
            aProgram.Use();
//            if (theObjectGame.TheObjectId == ObjectGame.ObjcetIds.Player)
//            {
//                aProgram["model_matrix"].SetValue(Matrix4.CreateRotationY(-((ObjectPlayer)theObjectGame).Orientation.Angle)*
//                                                  Matrix4.CreateTranslation(new Vector3(tempLoc.X, 0, tempLoc.Y)));
//
//            }
//            else
//            {
//                aProgram["model_matrix"].SetValue(Matrix4.CreateTranslation(new Vector3(tempLoc.X, 0, tempLoc.Y)));
//            }

            Gl.BindBufferToShaderAttribute(vertices, Material.Program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(normals, Material.Program, "vertexNormal");
            if (uvs != null) Gl.BindBufferToShaderAttribute(uvs, Material.Program, "vertexUV");
            Gl.BindBuffer(triangles);

            Gl.DrawElements(BeginMode.Triangles, triangles.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

        }
    }
}