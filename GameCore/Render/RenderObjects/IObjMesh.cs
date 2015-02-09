using System;
using System.Collections.Generic;
using GameCore.Render.RenderMaterial;

namespace GameCore.Render.RenderObjects
{
    public interface IObjMesh : IDisposable
    {
        void AddObjects(List<ObjObject> aObjObjects);
        void AddObject(ObjObject aObject);
        void AddObject(ObjObject aObject, ObjMaterial anObjMaterial);
        void Draw();
        void Dispose();
    }
}