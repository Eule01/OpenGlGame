#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using GameCore.Map;
using GameCore.Render.Cameras;
using GameCore.Render.RenderMaterial;
using GameCore.Render.RenderObjects;
using GameCore.Utils;
using OpenGL;

#endregion

namespace GameCore.Render.RenderLayers
{
    public class RenderLayerMapDrawArrays : RenderLayerBase
    {
        private Map.Map theMap;

        private ShaderProgram program;

        private Matrix4 projectionMatrix;

        private readonly SVertex2D[] gQuad = new SVertex2D[]
        {
            SVertex2D.FromArray(new[] {0.0f, 0.0f, 0.0f, 0.0f, 0.0f}),
            SVertex2D.FromArray(new[] {1.0f, 0.0f, 0.0f, 1.0f, 0.0f}),
            SVertex2D.FromArray(new[] {0.0f, 0.0f, 1.0f, 0.0f, 1.0f}),
            SVertex2D.FromArray(new[] {1.0f, 0.0f, 0.0f, 1.0f, 0.0f}),
            SVertex2D.FromArray(new[] {1.0f, 0.0f, 1.0f, 1.0f, 1.0f}),
            SVertex2D.FromArray(new[] {0.0f, 0.0f, 1.0f, 0.0f, 1.0f}),
        };


        private VBO<float> gVertexBuffer;
        private VBO<float> gDrawIdBuffer;

        private int numberOfTiles;
        private int numberOfVerticesPerTile;
        private int stride;
        private uint textureId;

        private uint locationPosition;
        private uint locationDrawid;
        private uint locationTexCoord;

        private TextureArray theTextureArray;
        private Environment theEnvironment;
        private Camera theCamera;
        private bool updateMap;

        public override void OnLoad()
        {
            GameCore.TheGameCore.TheGameEventHandler += TheGameCore_TheGameEventHandler;

            theMap = TheGameStatus.TheMap;

            // create our shader program
            program = new ShaderProgram(VertexShader, FragmentShader);

            // set up the projection and view matrix
            program.Use();
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(TheRenderStatus.Fov, (float) Width/Height,
                TheRenderStatus.ZNear,
                TheRenderStatus.ZFar);
            program["projection_matrix"].SetValue(projectionMatrix);
//            program["model_matrix"].SetValue(Matrix4.Identity);
            program["light_direction"].SetValue(theEnvironment.LightDirection);
            program["enable_lighting"].SetValue(theEnvironment.Lighting);
            program["ambient"].SetValue(theEnvironment.LightAmbient);

            GenerateGeometry();
            GenerateArrayTexture();

            Gl.UseProgram(0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void TheGameCore_TheGameEventHandler(object sender, GameEventArgs args)
        {
            switch (args.TheType)
            {
                case GameEventArgs.Types.StatusGameEngine:
                    break;
                case GameEventArgs.Types.Message:
                    break;
                case GameEventArgs.Types.MapLoaded:
                    updateMap = true;
                    break;
                case GameEventArgs.Types.MapSaved:
                    break;
                case GameEventArgs.Types.RendererExited:
                    break;
                case GameEventArgs.Types.MapTileSelected:
                    break;
                case GameEventArgs.Types.MapTileChanged:
                  ChangeTileType(args.TheTile);
                  break;
            }
        }

        private void ChangeTileType(Tile theTile)
        {
            Tile[] tempTiles = theMap.TheTileArray;

            CreateTileIdVbo(tempTiles);
        }

        private void GenerateGeometry()
        {
            numberOfVerticesPerTile = gQuad.Length;

            Stopwatch watch = Stopwatch.StartNew();


            Dictionary<Tile.TileIds, Bitmap> tempTiletypeList =
                RenderObjects.RenderObjects.CreateTileBitmaps(new Size(256, 256));
            List<ObjObject> tempObjList = new List<ObjObject>();
            Tile[] tempTiles = theMap.TheTileArray;

            numberOfTiles = tempTiles.Length;
            int numberOfTextures = tempTiletypeList.Count;
            // Number of values in SVertex2D
            stride = 5;
            SVertex2D[] vVertex = new SVertex2D[numberOfTiles*numberOfVerticesPerTile];

            // n tiles
            // 6 SVertex2D vectors per tile
            // 5 (stride) entries per SVertex2D

            int index = 0;
            foreach (Tile tempTile in tempTiles)
            {
                Vector tempLoc = tempTile.Location;
                for (int k = 0; k != numberOfVerticesPerTile; ++k)
                {
                    vVertex[index].x = gQuad[k].x + tempLoc.X;
                    vVertex[index].y = gQuad[k].y;
                    vVertex[index].z = gQuad[k].z + tempLoc.Y;
                    vVertex[index].u = gQuad[k].u;
                    vVertex[index].v = gQuad[k].v;
                    index++;
                }
            }

            // Copy into float array
            float[] verteses = new float[numberOfTiles*stride*numberOfVerticesPerTile];

            index = 0;
            for (int i = 0; i < vVertex.Count(); i++)
            {
                verteses[index] = vVertex[i].x;
                verteses[index + 1] = vVertex[i].y;
                verteses[index + 2] = vVertex[i].z;
                verteses[index + 3] = vVertex[i].u;
                verteses[index + 4] = vVertex[i].v;
                index += stride;
            }

            // Bind the buffers
            gVertexBuffer = new VBO<float>(verteses, BufferTarget.ArrayBuffer);
            Gl.BindBuffer(gVertexBuffer);

            locationPosition = (uint) Gl.GetAttribLocation(program.ProgramID, "position");
            locationTexCoord = (uint) Gl.GetAttribLocation(program.ProgramID, "texCoord");

//            int tempStride = sizeof(Single) * stride; // *****
//
//            Gl.EnableVertexAttribArray(locationPosition);
//            Gl.VertexAttribPointer(locationPosition, 3, VertexAttribPointerType.Float, false, tempStride,
//                IntPtr.Zero);
//
//            Gl.EnableVertexAttribArray(locationTexCoord);
//            Gl.VertexAttribPointer(locationTexCoord, 2, VertexAttribPointerType.Float, false, tempStride,
//                new IntPtr(3 * sizeof(Single)));
//
//
            CreateTileIdVbo(tempTiles);

            watch.Stop();
            GameCore.TheGameCore.RaiseMessage(string.Format("CreateTiles() took {0}ms", watch.ElapsedMilliseconds));
        }

        private void CreateTileIdVbo(Tile[] tempTiles)
        {
//Generate an instanced vertex array to identify each draw call in the shader
            float[] vDrawId = new float[numberOfTiles*numberOfVerticesPerTile];
//            int[] vDrawId = new int[numberOfTiles*6];

//            index = 0;
//            foreach (Tile aTile in tempTiles)
//            {
//                vDrawId[index] = (int) aTile.TheTileId;
//                index++;
//            }


            for (int i = 0; i < numberOfTiles*numberOfVerticesPerTile; i++)
            {
                Tile aTile = tempTiles[i/numberOfVerticesPerTile];
                vDrawId[i] = (int) aTile.TheTileId;
            }

            gDrawIdBuffer = new VBO<float>(vDrawId, VertexAttribPointerType.Float, BufferTarget.ArrayBuffer,
                BufferUsageHint.StaticDraw);
//            gDrawIdBuffer = new VBO<int>(vDrawId, VertexAttribPointerType.Int, BufferTarget.ArrayBuffer,
//                BufferUsageHint.StaticDraw);
            Gl.BindBuffer(gDrawIdBuffer);

            locationDrawid = (uint) Gl.GetAttribLocation(program.ProgramID, "drawTexId");
        }

        private void GenerateArrayTexture()
        {
            Dictionary<Tile.TileIds, Bitmap> tempTileList =
                RenderObjects.RenderObjects.CreateTileBitmaps(new Size(256, 256));
            theTextureArray = TextureArray.CreateFromBitmaps(new List<Bitmap>(tempTileList.Values));
        }


        public override void OnDisplay()
        {

        }

        public override void OnRenderFrame(float deltaTime)
        {
            if (updateMap)
            {
                gVertexBuffer.Dispose();
                gDrawIdBuffer.Dispose();
                theMap = TheGameStatus.TheMap;
                GenerateGeometry();
                GenerateArrayTexture();

                Gl.UseProgram(0);
                Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

                updateMap = false;
            }

            // apply our camera view matrix to the shader view matrix (this can be used for all objects in the scene)
            program.Use();
            program["view_matrix"].SetValue(theCamera.ViewMatrix);
            program["projection_matrix"].SetValue(projectionMatrix);
            program["enable_lighting"].SetValue(theEnvironment.Lighting);
            if (theEnvironment.LightMove)
            {
                program["light_direction"].SetValue(theEnvironment.LightDirection);
            }


            theTextureArray.Use();

            int tempStride = sizeof (Single)*stride;

            Gl.EnableVertexAttribArray(locationPosition);
            Gl.BindBuffer(gVertexBuffer);
            Gl.VertexAttribPointer(locationPosition, 3, VertexAttribPointerType.Float, false, tempStride,
                IntPtr.Zero);

            Gl.EnableVertexAttribArray(locationTexCoord);
            Gl.VertexAttribPointer(locationTexCoord, 2, VertexAttribPointerType.Float, false, tempStride,
                new IntPtr(3*sizeof (Single)));

            Gl.EnableVertexAttribArray(locationDrawid);
            Gl.BindBuffer(gDrawIdBuffer);

            Gl.VertexAttribPointer(locationDrawid, 1, VertexAttribPointerType.Float, false, sizeof (Single), IntPtr.Zero);


            Gl.DrawArrays(BeginMode.Triangles, 0, numberOfTiles*6);

//            Gl.BindTexture(TextureTarget.Texture2DArray, 0);
//
//            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
//            Gl.BindBuffer(BufferTarget.DrawIndirectBuffer, 0);
//            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
//            Gl.UseProgram(0);
        }

        public override void OnReshape(int width, int height)
        {
            Width = width;
            Height = height;


            Gl.UseProgram(program.ProgramID);
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(TheRenderStatus.Fov,
                (float) TheRenderStatus.Width/TheRenderStatus.Height, TheRenderStatus.ZNear,
                TheRenderStatus.ZFar);
            program["projection_matrix"].SetValue(projectionMatrix);
        }

        public override void OnClose()
        {
            if (gVertexBuffer != null) gVertexBuffer.Dispose();
            if (gDrawIdBuffer != null) gDrawIdBuffer.Dispose();
            Gl.DeleteTextures(1, new uint[] {textureId});
        }

        public override void ReInitialize()
        {
            theEnvironment = TheGameStatus.TheEnvironment;
            theMap = TheGameStatus.TheMap;
            theCamera = TheGameStatus.TheCamera;
        }

        /// <summary>
        /// </summary>
        /// <param name="button" />
        /// <param name="state" />
        /// <param name="x" />
        /// <param name="y" />
        /// <returns>
        ///     true if the mouse event has been processesd. In that case its not passed on
        /// </returns>
        public override bool OnMouse(int button, int state, int x, int y)
        {
            return false;
        }

        public override void OnMove(int x, int y)
        {
        }

        public override void OnSpecialKeyboardDown(int key, int x, int y)
        {
        }

        public override void OnSpecialKeyboardUp(int key, int x, int y)
        {
        }

        public override void OnKeyboardDown(byte key, int x, int y)
        {
        }

        public override void OnKeyboardUp(byte key, int x, int y)
        {
        }

        #region Sample Shader

        public const string VertexShader = @"
#version 430 core
    in vec3 position;
    in vec2 texCoord;
    in float drawTexId;
    uniform mat4 view_matrix;
    uniform mat4 projection_matrix;

    out vec2 uv;
    flat out float drawID;
    void main(void)
    {
        drawID = drawTexId;
        gl_Position = projection_matrix *view_matrix * vec4(position,1.0);
        uv = texCoord;
    }";

        //int actual_layer = max(0, min(numerOfTextures - 1, floor(drawTexId​ + 0.5)) )
        //drwaID = actual_layer;


        public const string FragmentShader = @"
#version 430 core
    out vec4 fragment;
    in vec2 uv;
    flat in float drawID;
    uniform float ambient;
    uniform bool enable_lighting;
    uniform vec3 light_direction;
    bool useTexture = true;
    float transparency = 1.0;

    layout (binding=0) uniform sampler2DArray textureArray;

    void main(void)
    {
        float light = (enable_lighting ? max(ambient, dot(vec3(0, 1, 0), light_direction)) : 1.0);
        vec4 sampleTex = texture(textureArray, vec3(uv.x,uv.y,drawID) );
        fragment = vec4(light * sampleTex.xyz, transparency * sampleTex.a);
    }";

        #endregion
    }
}