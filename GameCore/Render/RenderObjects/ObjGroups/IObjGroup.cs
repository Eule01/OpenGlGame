using System;
using System.Collections.Generic;
using GameCore.Render.RenderMaterial;
using OpenGL;

namespace GameCore.Render.RenderObjects.ObjGroups
{
    public interface IObjGroup : IDisposable
    {
        void AddObjects(List<ObjObject> aObjObjects);
        void AddObject(ObjObject aObject);
        void AddObject(ObjObject aObject, ObjMaterial anObjMaterial);
        void Draw();
        void Dispose();
        Vector3 Location { get; set; }
        Quaternion Orientation { get; set; }
        Vector3 Scale { get; set; }
    }
}