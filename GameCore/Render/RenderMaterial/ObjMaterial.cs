using System;
using System.Collections.Generic;
using System.IO;
using OpenGL;

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

        public ObjMaterial(List<string> lines, ShaderProgram program)
        {
            if (!lines[0].StartsWith("newmtl")) return;

            Name = lines[0].Substring(7);
            Transparency = 1f;

            for (int i = 1; i < lines.Count; i++)
            {
                string[] split = lines[i].Split(' ');

                switch (split[0])
                {
                    case "Ns":
                        SpecularCoefficient = float.Parse(split[1]);
                        break;
                    case "Ka":
                        Ambient = new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
                        break;
                    case "Kd":
                        Diffuse = new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
                        break;
                    case "Ks":
                        Specular = new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
                        break;
                    case "d":
                        Transparency = float.Parse(split[1]);
                        break;
                    case "illum":
                        Illumination = (IlluminationMode) int.Parse(split[1]);
                        break;
                    case "map_Kd":
                        if (File.Exists(split[1])) DiffuseMap = new Texture(split[1]);
                        break;
                }
            }

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