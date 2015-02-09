#region

using System;
using GameCore.Render.OpenGlHelper;
using GameCore.Render.RenderMaterial;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects
{
    public class ObjObject : IObjObject
    {
        internal VBO<Vector3> vertices;
        internal VBO<Vector3> normals;
        internal VBO<Vector2> uvs;
        internal VBO<int> triangles;

        public string Name { get;  set; }

        public ObjMaterial Material { get; set; }

        public ObjObject(Vector3[] vertexData, int[] elementData)
        {
            vertices = new VBO<Vector3>(vertexData);
            triangles = new VBO<int>(elementData, BufferTarget.ElementArrayBuffer);
            Vector3[] normalData = CalculateNormals(vertexData, elementData);

//            vertices = new VBO<Vector3>(vertexData);
            normals = new VBO<Vector3>(normalData);
            triangles = new VBO<int>(elementData, BufferTarget.ElementArrayBuffer);
        }

        public ObjObject(ObjectVectors anObjectVectors)
        {
            vertices = new VBO<Vector3>(anObjectVectors.Vertex);
            triangles = new VBO<int>(anObjectVectors.ElementData, BufferTarget.ElementArrayBuffer);
            Vector3[] normalData;
            if (anObjectVectors.normalData == null)
            {
                normalData = CalculateNormals(anObjectVectors.Vertex, anObjectVectors.ElementData);
            }
            else
            {
                normalData = anObjectVectors.normalData;
            }

            normals = new VBO<Vector3>(normalData);
            triangles = new VBO<int>(anObjectVectors.ElementData, BufferTarget.ElementArrayBuffer);
            if (anObjectVectors.Uvs != null)
            {
                uvs = new VBO<Vector2>(anObjectVectors.Uvs);
            }
        }

        public static Vector3[] CalculateNormals(Vector3[] vertexData, int[] elementData)
        {
            Vector3 b1, b2, normal;
            Vector3[] normalData = new Vector3[vertexData.Length];

            for (int i = 0; i < elementData.Length/3; i++)
            {
                int cornerA = elementData[i*3];
                int cornerB = elementData[i*3 + 1];
                int cornerC = elementData[i*3 + 2];

                b1 = vertexData[cornerB] - vertexData[cornerA];
                b2 = vertexData[cornerC] - vertexData[cornerA];

                normal = Vector3.Cross(b1, b2).Normalize();

                normalData[cornerA] += normal;
                normalData[cornerB] += normal;
                normalData[cornerC] += normal;
            }

            for (int i = 0; i < normalData.Length; i++) normalData[i] = normalData[i].Normalize();

            return normalData;
        }

        public void Draw()
        {
            if (vertices == null || triangles == null) return;

            Gl.Disable(EnableCap.CullFace);
            if (Material != null) Material.Use();

            Gl.BindBufferToShaderAttribute(vertices, Material.Program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(normals, Material.Program, "vertexNormal");
            if (uvs != null) Gl.BindBufferToShaderAttribute(uvs, Material.Program, "vertexUV");
            Gl.BindBuffer(triangles);

            Gl.DrawElements(BeginMode.Triangles, triangles.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public void Dispose()
        {
            if (vertices != null) vertices.Dispose();
            if (normals != null) normals.Dispose();
            if (triangles != null) triangles.Dispose();
            if (uvs != null) uvs.Dispose();
            if (Material != null) Material.Dispose();
        }

        public override string ToString()
        {
            string outStr = "";
            outStr += Name;
            return outStr;
        }
    }
}