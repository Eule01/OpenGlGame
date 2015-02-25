#region

using System.Collections.Generic;
using System.Diagnostics;
using GameCore.Render.RenderMaterial;
using OpenGL;

#endregion

namespace GameCore.Render.RenderObjects.ObjGroups
{
    public class ObjGroup : IObjGroup
    {
        protected List<IObjObject> Objects = new List<IObjObject>();

        protected internal ShaderProgram defaultProgram;
        private ObjMaterial defaultMaterial;

        public string Name;

        /// <summary>
        ///     The location of the Mesh object.
        /// </summary>
        internal Vector3 location = Vector3.Zero;

        /// <summary>
        ///     The orientation of the Mesh object.
        ///     This migh be faster implemented by three vectors Right, Up and Dir
        /// </summary>
        private Quaternion orientation = new Quaternion(0, 0, 0, 1);

//        private Quaternion orientation = Quaternion.Zero;

        /// <summary>
        ///     The scale in all three dimensions.
        /// </summary>
        internal Vector3 scale = Vector3.UnitScale;

        /// <summary>
        ///     The model matrix of the mesh object. Contains the Location, Orientation and Scale.
        /// </summary>
        protected Matrix4 modelMatrix = Matrix4.Identity;

        /// <summary>
        ///     True if the model matrix needs updating.
        /// </summary>
        protected bool modelMatrixOld = false;

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

        protected void UpdateModelMatrix()
        {
            modelMatrix = orientation.Matrix4*Matrix4.CreateScaling(scale)*Matrix4.CreateTranslation(location);
            modelMatrixOld = false;
        }


        protected void ClearObjects()
        {
            Objects.Clear();
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
            Objects.Add(aObject);
            if (aObject.Material == null) aObject.Material = defaultMaterial;
        }

        public void AddObject(ObjObject aObject, ObjMaterial anObjMaterial)
        {
            aObject.Material = anObjMaterial;
            AddObject(aObject);
        }

        public virtual void Draw()
        {
            if (modelMatrixOld)
            {
                UpdateModelMatrix();
            }
            defaultProgram["model_matrix"].SetValue(modelMatrix);


            if (false)
            {
                foreach (IObjObject anObj in Objects)
                {
                    anObj.Draw();
                }
            }
            else
            {
                // Make sure that the transparent objects are drawn last.
                List<IObjObject> transparentObjects = new List<IObjObject>();

                foreach (IObjObject anObj in Objects)
                {
                    if (anObj.Material.Transparency < 1f) transparentObjects.Add(anObj);
                    else anObj.Draw();
                }

                foreach (IObjObject anObj in transparentObjects)
                {
                    anObj.Draw();
                }
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < Objects.Count; i++) Objects[i].Dispose();
            defaultMaterial.Dispose();
//            if (defaultProgram != null) defaultProgram.Dispose();
        }

        public List<IObjObject> GetObjects()
        {
            return Objects;
        }
    }
}