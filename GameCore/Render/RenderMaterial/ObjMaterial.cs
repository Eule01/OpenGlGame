#region

using System;
using GameCore.Render.OpenGlHelper;
using OpenGL;

#endregion

namespace GameCore.Render.RenderMaterial
{
    public class ObjMaterial : IDisposable
    {
        public string Name { get; set; }
        public Vector3 Ambient { get; private set; }
        public Vector3 Diffuse { get; private set; }
        public Vector3 Specular { get; private set; }
        public float SpecularCoefficient { get; private set; }
        public float Transparency { get; private set; }
        public IlluminationMode Illumination { get; private set; }

        public Texture DiffuseMap { get; set; }
        public ShaderProgram Program { get; private set; }

        public enum IlluminationMode
        {
            ColorOnAmbientOff = 0,
            ColorOnAmbientOn = 1,
            HighlightOn = 2,
            ReflectionOnRaytraceOn = 3,
            TransparencyGlassOnReflectionRayTraceOn = 4,
            ReflectionFresnelOnRayTranceOn = 5,
            TransparencyRefractionOnReflectionFresnelOffRayTraceOn = 6,
            TransparencyRefractionOnReflectionFresnelOnRayTranceOn = 7,
            ReflectionOnRayTraceOff = 8,
            TransparencyGlassOnReflectionRayTraceOff = 9,
            CastsShadowsOntoInvisibleSurfaces = 10
        }

        public ObjMaterial(ShaderProgram program)
        {
            Name = "opengl-default-project";
            Transparency = 1f;
            Ambient = Vector3.UnitScale;
            Diffuse = Vector3.UnitScale;
            Program = program;
        }

        public ObjMaterial(ShaderProgram program, MtlData aMtlData)
        {
            Name = aMtlData.Name;
            Transparency = (float) aMtlData.d;
            Ambient = aMtlData.Ka;
            Diffuse = aMtlData.Kd;
            Specular = aMtlData.Ks;
            SpecularCoefficient = (float) aMtlData.Ns;
            Illumination = (IlluminationMode) aMtlData.illum;
            Program = program;
        }

        public void Use()
        {
            if (DiffuseMap != null)
            {
                Gl.ActiveTexture(TextureUnit.Texture0);
                Gl.BindTexture(DiffuseMap);
                Program["useTexture"].SetValue(true);
            }
            else Program["useTexture"].SetValue(false);

            Program.Use();

            Program["diffuse"].SetValue(Diffuse);
            Program["texture"].SetValue(0);
            Program["transparency"].SetValue(Transparency);
        }

        public void Dispose()
        {
            if (DiffuseMap != null) DiffuseMap.Dispose();
            if (Program != null)
            {
                Program.DisposeChildren = true;
                Program.Dispose();
            }
        }

        public override string ToString()
        {
            string outStr = "";
            outStr += Name;
            return outStr;
        }
    }
}