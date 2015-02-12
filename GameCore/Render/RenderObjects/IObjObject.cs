using System;
using GameCore.Render.RenderMaterial;

namespace GameCore.Render.RenderObjects
{
    public interface IObjObject : IDisposable
    {
        string Name { get; set; }
        ObjMaterial Material { get; set; }
        void Draw();
        string ToString();
    }
}