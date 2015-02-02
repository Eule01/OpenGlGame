#region

using System;
using GameCore.GameObjects;
using GameCore.OpenGlHelper;
using GameCore.Utils;
using OpenGL;

#endregion

namespace GameCore.DrawingObjects
{
    public class RenderGameObject : ObjObject
    {

        public GameObject TheGameObject;

        public RenderGameObject(Vector3[] vertexData, int[] elementData) : base(vertexData, elementData)
        {
        }

        public RenderGameObject(ObjectVectors anObjectVectors) : base(anObjectVectors)
        {
        }

        public void Draw(ShaderProgram aProgram)
        {
            if (vertices == null || triangles == null) return;

            Gl.Disable(EnableCap.CullFace);
            if (Material != null) Material.Use();
            Vector tempLoc = TheGameObject.Location;
            aProgram.Use();
            if (TheGameObject.TheObjectId == GameObject.ObjcetIds.Player)
            {
                aProgram["model_matrix"].SetValue(Matrix4.CreateRotationZ(((ObjectPlayer)TheGameObject).Orientation.Angle)*
                                                  Matrix4.CreateTranslation(new Vector3(tempLoc.X, tempLoc.Y, 0)));

            }
            else
            {
                aProgram["model_matrix"].SetValue(Matrix4.CreateTranslation(new Vector3(tempLoc.X, tempLoc.Y, 0)));
            }
//            aProgram["model_matrix"].SetValue(CreateTranslation(new Vector3(tempLoc.X, tempLoc.Y, 0)));

            Gl.BindBufferToShaderAttribute(vertices, Material.Program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(normals, Material.Program, "vertexNormal");
            if (uvs != null) Gl.BindBufferToShaderAttribute(uvs, Material.Program, "vertexUV");
            Gl.BindBuffer(triangles);

            Gl.DrawElements(BeginMode.Triangles, triangles.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}