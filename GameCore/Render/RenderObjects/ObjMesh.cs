#region

using System.Collections.Generic;
using System.Diagnostics;
using GameCore.Render.RenderMaterial;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects
{
    public class ObjMesh : IObjMesh
    {
        private List<IObjObject> objects = new List<IObjObject>();
        private Dictionary<string, ObjMaterial> materials = new Dictionary<string, ObjMaterial>();

        private ShaderProgram defaultProgram;
        private ObjMaterial defaultMaterial;

        public string Name;

        public ObjMesh(ShaderProgram program)
        {
            defaultProgram = program;
            Stopwatch watch = Stopwatch.StartNew();
            defaultMaterial = new ObjMaterial(program);

            watch.Stop();
            GameCore.TheGameCore.RaiseMessage(string.Format("Took {0}ms", watch.ElapsedMilliseconds));
        }

        public void AddObjects(List<ObjObject> aObjObjects)
        {
            foreach (ObjObject aObjObject in aObjObjects)
            {
                AddObject(aObjObject);
            }
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

        public void Draw()
        {
            List<ObjObject> transparentObjects = new List<ObjObject>();

            foreach (ObjObject anObj in objects)
            {
                if (anObj.Material.Transparency >= 1f) transparentObjects.Add(anObj);
                else anObj.Draw();
            }

            foreach (ObjObject anObj in transparentObjects)
            {
                anObj.Draw();
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