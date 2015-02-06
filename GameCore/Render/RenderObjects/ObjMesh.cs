#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GameCore.Render.RenderMaterial;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects
{
    public class ObjMesh : IDisposable
    {
        protected List<ObjObject> objects = new List<ObjObject>();
        private Dictionary<string, ObjMaterial> materials = new Dictionary<string, ObjMaterial>();

        public ShaderProgram defaultProgram;
        private ObjMaterial defaultMaterial;

        public ObjMesh(ShaderProgram program)
        {
            defaultProgram = program;
            Stopwatch watch = Stopwatch.StartNew();
            defaultMaterial = new ObjMaterial(program);

            watch.Stop();
            GameCore.TheGameCore.RaiseMessage(string.Format("Took {0}ms", watch.ElapsedMilliseconds));
//            Console.WriteLine("Took {0}ms", watch.ElapsedMilliseconds);
        }

        public ObjMesh(string filename, ShaderProgram program)
        {
            defaultProgram = program;

            Stopwatch watch = Stopwatch.StartNew();
            defaultMaterial = new ObjMaterial(program);

            using (StreamReader stream = new StreamReader(filename))
            {
                List<string> lines = new List<string>();
                int vertexOffset = 1, vertexCount = 0;
                int uvOffset = 1, uvCount = 0;

                // read the entire file
                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine();
                    if (line.Trim().Length == 0) continue;

                    if ((line[0] == 'o' || line[0] == 'g') && lines.Count != 0)
                    {
                        ObjObject newObject = new ObjObject(lines, materials, vertexOffset, uvOffset);
                        objects.Add(newObject);

                        if (newObject.Material == null) newObject.Material = defaultMaterial;

                        lines.Clear();
                        vertexOffset += vertexCount;
                        uvOffset += uvCount;
                        vertexCount = 0;
                        uvCount = 0;
                    }
                    if (line[0] != '#') lines.Add(line);
                    if (line[0] == 'v')
                    {
                        if (line[1] == ' ') vertexCount++;
                        else uvCount++;
                    }

                    // check if a material file is being used
                    if (line[0] == 'm' && line[1] == 't') LoadMaterials(CreateFixedPath(filename, line.Split(' ')[1]));
                }
            }

            watch.Stop();
            GameCore.TheGameCore.RaiseMessage(string.Format("Took {0}ms", watch.ElapsedMilliseconds));

//            Console.WriteLine("Took {0}ms", watch.ElapsedMilliseconds);
        }

        public void AddObject(ObjObject aObject)
        {
            objects.Add(aObject);
            if (aObject.Material == null) aObject.Material = defaultMaterial;
        }

        public void AddObject(ObjObject aObject, ObjMaterial anObjMaterial)
        {
            aObject.Material = anObjMaterial;
            AddObject(aObject);
        }

        private void LoadMaterials(string filename)
        {
            using (StreamReader stream = new StreamReader(filename))
            {
                List<string> lines = new List<string>();

                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine();
                    if (line.Trim().Length == 0) continue;

                    if (line[0] == 'n' && lines.Count != 0)
                    {
                        // if this is a new material ('newmtl name') then load it
                        ObjMaterial material = new ObjMaterial(lines, defaultProgram);
                        if (!materials.ContainsKey(material.Name)) materials.Add(material.Name, material);
                        lines.Clear();
                    }

                    if (line[0] == 'm')
                    {
                        // try to fix up filenames of texture maps
                        string[] split = line.Split(' ');
                        lines.Add(string.Format("{0} {1}", split[0], CreateFixedPath(filename, split[1])));
                    }
                    else if (line[0] != '#') lines.Add(line); // ignore comments
                }
            }
        }

        private string CreateFixedPath(string objectPath, string filename)
        {
            if (File.Exists(filename)) return filename;

            DirectoryInfo directory = new FileInfo(objectPath).Directory;

            filename = filename.Replace('\\', '/');
            if (filename.Contains("/")) filename = filename.Substring(filename.LastIndexOf('/') + 1);
            filename = directory.FullName + "\\" + filename;

            return filename;
        }

        public void Draw()
        {
            List<ObjObject> transparentObjects = new List<ObjObject>();

            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].Material.Transparency != 1f) transparentObjects.Add(objects[i]);
                else objects[i].Draw();
            }

            for (int i = 0; i < transparentObjects.Count; i++)
            {
                transparentObjects[i].Draw();
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < objects.Count; i++) objects[i].Dispose();
            defaultMaterial.Dispose();
//            if (defaultProgram != null) defaultProgram.Dispose();
        }
    }
}