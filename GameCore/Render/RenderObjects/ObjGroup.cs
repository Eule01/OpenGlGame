#region

using System.Collections.Generic;
using System.Diagnostics;
using GameCore.Render.RenderMaterial;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects
{
    public class ObjGroup : IObjGroup
    {
        private List<IObjObject> objects = new List<IObjObject>();
        private Dictionary<string, ObjMaterial> materials = new Dictionary<string, ObjMaterial>();

        private ShaderProgram defaultProgram;
        private ObjMaterial defaultMaterial;

        public string Name;

        /// <summary>
        ///     The location of the Mesh object.
        /// </summary>
        private Vector3 location = Vector3.Zero;

        /// <summary>
        ///     The orientation of the Mesh object.
        /// This migh be faster implemented by three vectors Right, Up and Dir
        /// </summary>
        private Quaternion orientation = new Quaternion(0,0,0,1);
//        private Quaternion orientation = Quaternion.Zero;

        /// <summary>
        ///     The scale in all three dimensions.
        /// </summary>
        private Vector3 scale = Vector3.UnitScale;

        /// <summary>
        ///     The model matrix of the mesh object. Contains the Location, Orientation and Scale.
        /// </summary>
        private Matrix4 modelMatrix = Matrix4.Identity;

        /// <summary>
        /// True if the model matrix needs updating.
        /// </summary>
        private bool modelMatrixOld = false;

        public ObjGroup(ShaderProgram program)
        {
            defaultProgram = program;
            Stopwatch watch = Stopwatch.StartNew();
            defaultMaterial = new ObjMaterial(program);

            watch.Stop();
            GameCore.TheGameCore.RaiseMessage(string.Format("Took {0}ms", watch.ElapsedMilliseconds));
        }

        public Vector3 Location
        {
            get { return location; }
            set
            {
                location = value;
                modelMatrixOld = true;
            }
        }

        public Quaternion Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                modelMatrixOld = true;
            }
        }

        public Vector3 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                modelMatrixOld = true;
            }
        }


        private void UpdateModelMatrix()
        {
//            modelMatrix = Matrix4.CreateTranslation(location) * Matrix4.CreateScaling(scale) * orientation.Matrix4;
            modelMatrix = orientation.Matrix4 * Matrix4.CreateScaling(scale) * Matrix4.CreateTranslation(location);
            modelMatrixOld = false;

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
            if (modelMatrixOld)
            {
                UpdateModelMatrix();
            }

            defaultProgram["model_matrix"].SetValue(modelMatrix);

            // Make sure that the transparent objects are drawn last.
            List<ObjObject> transparentObjects = new List<ObjObject>();

            foreach (ObjObject anObj in objects)
            {
                if (anObj.Material.Transparency < 1f) transparentObjects.Add(anObj);
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