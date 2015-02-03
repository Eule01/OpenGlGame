#region

using System;
using System.Collections.Generic;
using GameCore.Render.OpenGlHelper;
using GameCore.Render.RenderMaterial;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects
{
    public class ObjObject : IDisposable
    {
        internal VBO<Vector3> vertices;
        internal VBO<Vector3> normals;
        internal VBO<Vector2> uvs;
        internal VBO<int> triangles;

        public string Name { get; internal set; }

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
            Vector3[] normalData = CalculateNormals(anObjectVectors.Vertex, anObjectVectors.ElementData);

//            vertices = new VBO<Vector3>(anObjectVectors.Vertex);
            normals = new VBO<Vector3>(normalData);
            triangles = new VBO<int>(anObjectVectors.ElementData, BufferTarget.ElementArrayBuffer);
            if (anObjectVectors.Uvs != null)
            {
                uvs = new VBO<Vector2>(anObjectVectors.Uvs);
            }
        }


        public ObjObject(List<string> lines, Dictionary<string, ObjMaterial> materials, int vertexOffset, int uvOffset)
        {
            // we need at least 1 line to be a valid file
            if (lines.Count == 0) return;

            // the first line should contain 'o'
            if (lines[0][0] != 'o' && lines[0][0] != 'g') return;
            Name = lines[0].Substring(2);

            List<Vector3> vertexList = new List<Vector3>();
            List<Vector2> uvList = new List<Vector2>();
            List<int> triangleList = new List<int>();
            List<Vector2> unpackedUvs = new List<Vector2>();
            List<int> normalsList = new List<int>();

            // now we read the lines
            for (int i = 1; i < lines.Count; i++)
            {
                string[] split = lines[i].Split(' ');

                switch (split[0])
                {
                    case "v":
                        vertexList.Add(
                            new Vector3(double.Parse(split[1]), double.Parse(split[2]), double.Parse(split[3]))*0.025f);
                        break;
                    case "vt":
                        uvList.Add(new Vector2(double.Parse(split[1]), double.Parse(split[2])));
                        break;
                    case "f":
                        string[] indices = new string[] {split[1], split[2], split[3]};

                        if (split[1].Contains("/"))
                        {
                            indices[0] = split[1].Substring(0, split[1].IndexOf("/"));
                            indices[1] = split[2].Substring(0, split[2].IndexOf("/"));
                            indices[2] = split[3].Substring(0, split[3].IndexOf("/"));

                            // TODO there was something wrong here.
//                            string[] uvs = new string[3];
                            string[] tempUvs = new string[3];
                            tempUvs[0] = split[1].Substring(split[1].IndexOf("/") + 1);
                            tempUvs[1] = split[2].Substring(split[2].IndexOf("/") + 1);
                            tempUvs[2] = split[3].Substring(split[3].IndexOf("/") + 1);

                            int[] triangle = new int[]
                                {
                                    int.Parse(indices[0]) - vertexOffset, int.Parse(indices[1]) - vertexOffset,
                                    int.Parse(indices[2]) - vertexOffset
                                };

                            if (unpackedUvs.Count == 0)
                                for (int j = 0; j < vertexList.Count; j++) unpackedUvs.Add(Vector2.Zero);
                            normalsList.Add(triangle[0]);
                            normalsList.Add(triangle[1]);
                            normalsList.Add(triangle[2]);

                            if (unpackedUvs[triangle[0]] == Vector2.Zero)
                                unpackedUvs[triangle[0]] = uvList[int.Parse(tempUvs[0]) - uvOffset];
                            else
                            {
                                unpackedUvs.Add(uvList[int.Parse(tempUvs[0]) - uvOffset]);
                                vertexList.Add(vertexList[triangle[0]]);
                                triangle[0] = unpackedUvs.Count - 1;
                            }

                            if (unpackedUvs[triangle[1]] == Vector2.Zero)
                                unpackedUvs[triangle[1]] = uvList[int.Parse(tempUvs[1]) - uvOffset];
                            else
                            {
                                unpackedUvs.Add(uvList[int.Parse(tempUvs[1]) - uvOffset]);
                                vertexList.Add(vertexList[triangle[1]]);
                                triangle[1] = unpackedUvs.Count - 1;
                            }

                            if (unpackedUvs[triangle[2]] == Vector2.Zero)
                                unpackedUvs[triangle[2]] = uvList[int.Parse(tempUvs[2]) - uvOffset];
                            else
                            {
                                unpackedUvs.Add(uvList[int.Parse(tempUvs[2]) - uvOffset]);
                                vertexList.Add(vertexList[triangle[2]]);
                                triangle[2] = unpackedUvs.Count - 1;
                            }

                            triangleList.Add(triangle[0]);
                            triangleList.Add(triangle[1]);
                            triangleList.Add(triangle[2]);
                        }
                        else
                        {
                            triangleList.Add(int.Parse(indices[0]) - vertexOffset);
                            triangleList.Add(int.Parse(indices[1]) - vertexOffset);
                            triangleList.Add(int.Parse(indices[2]) - vertexOffset);
                        }
                        break;
                    case "usemtl":
                        if (materials.ContainsKey(split[1])) Material = materials[split[1]];
                        break;
                }
            }

            // calculate the normals (if they didn't exist)
            Vector3[] vertexData = vertexList.ToArray();
            int[] elementData = triangleList.ToArray();
            Vector3[] normalData = CalculateNormals(vertexData, elementData);

            // now convert the lists over to vertex buffer objects to be rendered by OpenGL
            vertices = new VBO<Vector3>(vertexData);
            normals = new VBO<Vector3>(normalData);
            if (unpackedUvs.Count != 0) uvs = new VBO<Vector2>(unpackedUvs.ToArray());
            triangles = new VBO<int>(elementData, BufferTarget.ElementArrayBuffer);
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