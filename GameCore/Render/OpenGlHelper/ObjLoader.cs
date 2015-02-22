#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using GameCore.Render.RenderMaterial;
using GameCore.Render.RenderObjects;
using GameCore.Render.RenderObjects.ObjGroups;
using OpenGL;

#endregion

namespace GameCore.Render.OpenGlHelper
{
    /// <summary>
    ///     This is from Meshomatic
    ///     XXX: Sources: http://www.opentk.com/files/ObjMeshLoader.cs, OOGL (MS3D), Icarus (Colladia)
    /// </summary>
    public static class ObjLoader
    {
        public static MeshData LoadFile(string file)
        {
            // Silly me, using() closes the file automatically.
            using (FileStream s = File.Open(file, FileMode.Open))
            {
                return LoadStream(s);
            }
        }

        public static ObjGroup LoadObjFileToObjMesh(ShaderProgram aProgram, string filePath)
        {
            string fileDirectory = Path.GetDirectoryName(filePath);
            string fileNameNoExt = Path.GetFileNameWithoutExtension(filePath);
            ObjGroup tempObjGroup = new ObjGroup(aProgram) {Name = fileNameNoExt};

            List<string> lines = ReadAllLinesRemoveComments(filePath);

            List<string> mtlFiles = GetMtlFiles(lines);

            List<MtlData> tempMtlDatas = null;
            if (mtlFiles.Count > 0)
            {
                string mtlFilePath = Path.Combine(fileDirectory, mtlFiles[0]);
                List<string> mtlLines = ReadAllLinesRemoveComments(mtlFilePath);
                tempMtlDatas = GetMtlDatas(mtlLines);
            }

            List<ObjData> tempObjData = GetObjectDatas(lines);

            List<ObjObject> temObjObjects = GetObjObjects(aProgram, tempObjData, tempMtlDatas, fileDirectory);

            tempObjGroup.AddObjects(temObjObjects);

            return tempObjGroup;
        }

        private static List<ObjObject> GetObjObjects(ShaderProgram aProgram, List<ObjData> tempObjData,
            List<MtlData> tempMtlDatas, string fileDirectory)
        {
            List<ObjObject> temObjObjects = new List<ObjObject>();


            int vertexOffset = 0;
            int uVOffset = 0;
            int normalOffset = 0;
            foreach (ObjData anObjData in tempObjData)
            {
                MeshData tempMeshData = ParseFromLines(anObjData.Lines, vertexOffset, uVOffset, normalOffset);
                vertexOffset += tempMeshData.Vertices.Length;
                uVOffset += tempMeshData.TexCoords.Length;
                normalOffset += tempMeshData.Normals.Length;

                ObjObject tempObjObject = tempMeshData.ToObjObject();
                tempObjObject.Name = anObjData.Name;

                // Add the material
                if (tempMtlDatas != null)
                {
                    MtlData tempMtlData = tempMtlDatas.Find(x => x.Name == anObjData.UseMtl);
                    ObjMaterial tempMat = new ObjMaterial(aProgram, tempMtlData);
                    string tempDiffuseTextPath = Path.Combine(fileDirectory, tempMtlData.DiffuseMapFileName);
                    Texture tempTexture = new Texture(tempDiffuseTextPath);
                    tempMat.DiffuseMap = tempTexture;
                    tempObjObject.Material = tempMat;
                }

                temObjObjects.Add(tempObjObject);
            }
            return temObjObjects;
        }

        private static List<string> ReadAllLinesRemoveComments(string filePath)
        {
            List<string> lines = new List<string>();
            using (FileStream s = File.Open(filePath, FileMode.Open))
            {
                StreamReader reader = new StreamReader(s);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.StartsWith("#"))
                    {
                        lines.Add(line);
                    }
                }
            }
            return lines;
        }

        #region Obj

        public static MeshData ParseFromLines(List<string> lines, int aVertexOffset = 0,
            int anUvOffset = 0, int aNormalOffset = 0)
        {
            List<Vector3> points = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> texCoords = new List<Vector2>();
            List<Tri> tris = new List<Tri>();
            char[] splitChars = {' '};
            string line;

            foreach (string aLine in lines)
            {
                line = aLine.Trim(splitChars);
                line = line.Replace("  ", " ");

                string[] parameters = line.Split(splitChars);

                switch (parameters[0])
                {
                    case "p":
                        // Point
                        break;

                    case "v":
                        // Vertex
                        float x = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        float y = float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat);
                        float z = float.Parse(parameters[3], CultureInfo.InvariantCulture.NumberFormat);
                        points.Add(new Vector3(x, y, z));
                        break;

                    case "vt":
                        // TexCoord
                        float u = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        float v = float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat);
                        texCoords.Add(new Vector2(u, v));
                        break;

                    case "vn":
                        // Normal
                        float nx = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        float ny = float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat);
                        float nz = float.Parse(parameters[3], CultureInfo.InvariantCulture.NumberFormat);
                        normals.Add(new Vector3(nx, ny, nz));
                        break;

                    case "f":
                        // Face
                        tris.AddRange(ParseFace(parameters, aVertexOffset, anUvOffset, aNormalOffset));
                        break;
                }
            }

            Vector3[] p = points.ToArray();
            Vector2[] tc = texCoords.ToArray();
            Vector3[] n = normals.ToArray();
            Tri[] f = tris.ToArray();

            // If there are no specified texcoords or normals, we add a dummy one.
            // That way the Points will have something to refer to.
            if (tc.Length == 0)
            {
                tc = new Vector2[1];
                tc[0] = new Vector2(0, 0);
            }
            if (n.Length == 0)
            {
                n = new Vector3[1];
                n[0] = new Vector3(1, 0, 0);
            }

            return new MeshData(p, n, tc, f);
        }

        public static MeshData LoadStream(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            List<Vector3> points = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> texCoords = new List<Vector2>();
            List<Tri> tris = new List<Tri>();
            string line;
            char[] splitChars = {' '};
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim(splitChars);
                line = line.Replace("  ", " ");

                string[] parameters = line.Split(splitChars);

                switch (parameters[0])
                {
                    case "p":
                        // Point
                        break;

                    case "v":
                        // Vertex
                        float x = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        float y = float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat);
                        float z = float.Parse(parameters[3], CultureInfo.InvariantCulture.NumberFormat);
                        points.Add(new Vector3(x, y, z));
                        break;

                    case "vt":
                        // TexCoord
                        float u = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        float v = float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat);
                        texCoords.Add(new Vector2(u, v));
                        break;

                    case "vn":
                        // Normal
                        float nx = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        float ny = float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat);
                        float nz = float.Parse(parameters[3], CultureInfo.InvariantCulture.NumberFormat);
                        normals.Add(new Vector3(nx, ny, nz));
                        break;

                    case "f":
                        // Face
                        tris.AddRange(ParseFace(parameters));
                        break;
                }
            }

            Vector3[] p = points.ToArray();
            Vector2[] tc = texCoords.ToArray();
            Vector3[] n = normals.ToArray();
            Tri[] f = tris.ToArray();

            // If there are no specified texcoords or normals, we add a dummy one.
            // That way the Points will have something to refer to.
            if (tc.Length == 0)
            {
                tc = new Vector2[1];
                tc[0] = new Vector2(0, 0);
            }
            if (n.Length == 0)
            {
                n = new Vector3[1];
                n[0] = new Vector3(1, 0, 0);
            }

            return new MeshData(p, n, tc, f);
        }

        private static List<ObjData> GetObjectDatas(List<string> lines)
        {
            List<ObjData> objDatas = new List<ObjData>();
            char[] splitChars = {' '};
            string line;
            ObjData tempData = null;
            foreach (string aLine in lines)
            {
                line = aLine.Trim(splitChars);
                line = line.Replace("  ", " ");

                string[] parameters = line.Split(splitChars);
                // New Object starts
                if (parameters[0] == "o")
                {
                    tempData = new ObjData(parameters[1]);
                    objDatas.Add(tempData);
                }
                else if (tempData != null && parameters[0] == "usemtl")
                {
                    tempData.UseMtl = parameters[1];
                }
                else if (tempData != null)
                {
                    tempData.Lines.Add(aLine);
                }
            }


            return objDatas;
        }

        private static Tri[] ParseFace(string[] indices, int aVertexOffset = 0,
            int anUvOffset = 0, int aNormalOffset = 0)
        {
            Point[] p = new Point[indices.Length - 1];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = ParsePoint(indices[i + 1], aVertexOffset, anUvOffset, aNormalOffset);
            }
            return Triangulate(p);
            //return new Face(p);
        }

        // Takes an array of points and returns an array of triangles.
        // The points form an arbitrary polygon.
        private static Tri[] Triangulate(Point[] ps)
        {
            List<Tri> ts = new List<Tri>();
            if (ps.Length < 3)
            {
                throw new Exception("Invalid shape!  Must have >2 points");
            }

            Point lastButOne = ps[1];
            Point lastButTwo = ps[0];
            for (int i = 2; i < ps.Length; i++)
            {
                Tri t = new Tri(lastButTwo, lastButOne, ps[i]);
                lastButOne = ps[i];
                lastButTwo = ps[i - 1];
                ts.Add(t);
            }
            return ts.ToArray();
        }

        private static Point ParsePoint(string s, int aVertexOffset = 0,
            int anUvOffset = 0, int aNormalOffset = 0)
        {
            char[] splitChars = {'/'};
            string[] parameters = s.Split(splitChars);
            int vert, tex, norm;
            vert = tex = norm = 0;
            vert = int.Parse(parameters[0]) - 1;
            // Texcoords and normals are optional in .obj files
            if (parameters[1] != "")
            {
                tex = int.Parse(parameters[1]) - 1;
            }
            if (parameters[2] != "")
            {
                norm = int.Parse(parameters[2]) - 1;
            }
            return new Point(vert - aVertexOffset, norm - aNormalOffset, tex - anUvOffset);
        }

        #endregion

        #region Mtl

        private static List<string> GetMtlFiles(List<string> lines)
        {
            List<string> mtlFiles = new List<string>();
            char[] splitChars = {' '};
            string line;
            foreach (string aLine in lines)
            {
                line = aLine.Trim(splitChars);
                line = line.Replace("  ", " ");

                string[] parameters = line.Split(splitChars);
                if (parameters[0] == "mtllib")
                {
                    mtlFiles.Add(parameters[1]);
                }
            }
            return mtlFiles;
        }

        private static List<MtlData> GetMtlDatas(List<string> mtlLines)
        {
            List<MtlData> mtlDatas = new List<MtlData>();
            char[] splitChars = {' '};
            string line;
            MtlData tempData = null;
            foreach (string aLine in mtlLines)
            {
                line = aLine.Trim(splitChars);
                line = line.Replace("  ", " ");

                string[] parameters = line.Split(splitChars);

                switch (parameters[0])
                {
                    case "newmtl":
                        tempData = new MtlData(parameters[1]);
                        mtlDatas.Add(tempData);
                        break;
                    case "Ns":
                        if (tempData != null)
                        {
                            tempData.Ns = double.Parse(parameters[1]);
                        }
                        break;
                    case "Ni":
                        if (tempData != null)
                        {
                            tempData.Ni = double.Parse(parameters[1]);
                        }
                        break;
                    case "d":
                        if (tempData != null)
                        {
                            tempData.d = double.Parse(parameters[1]);
                        }
                        break;
                    case "illum":
                        if (tempData != null)
                        {
                            tempData.illum = int.Parse(parameters[1]);
                        }
                        break;
                    case "Ka":
                        if (tempData != null)
                        {
                            tempData.Ka = new Vector3(float.Parse(parameters[1]), float.Parse(parameters[2]),
                                float.Parse(parameters[3]));
                        }
                        break;
                    case "Kd":
                        if (tempData != null)
                        {
                            tempData.Kd = new Vector3(float.Parse(parameters[1]), float.Parse(parameters[2]),
                                float.Parse(parameters[3]));
                        }
                        break;
                    case "Ks":
                        if (tempData != null)
                        {
                            tempData.Ks = new Vector3(float.Parse(parameters[1]), float.Parse(parameters[2]),
                                float.Parse(parameters[3]));
                        }
                        break;
                    case "map_Kd":
                        if (tempData != null)
                        {
                            tempData.DiffuseMapFileName = parameters[1];
                        }
                        break;
                }
            }

            return mtlDatas;
        }

        #endregion
    }

    public class ObjData
    {
        public ObjData(string name)
        {
            Name = name;
            Lines = new List<string>();
        }

        public string Name;

        public string UseMtl;

        public List<string> Lines;
    }

    public class MtlData
    {
        public MtlData(string name)
        {
            Name = name;
        }

        public string Name;

        /// <summary>
        ///     SpecularCoefficient
        /// </summary>
        public double Ns;

        /// <summary>
        ///     Ambient
        /// </summary>
        public Vector3 Ka;

        /// <summary>
        ///     Diffuse
        /// </summary>
        public Vector3 Kd;

        /// <summary>
        ///     Specular
        /// </summary>
        public Vector3 Ks;


        public double Ni;

        /// <summary>
        ///     Transparency
        /// </summary>
        public double d;

        /// <summary>
        ///     Illumination = (IlluminationMode)
        /// </summary>
        public int illum;

        /// <summary>
        ///     DiffuseMap Texture
        /// </summary>
        public string DiffuseMapFileName;
    }
}