#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using GameCore.Map;
using GameCore.Render.Cameras;
using GameCore.Render.RenderMaterial;
using GameCore.Render.RenderObjects;
using GameCore.UserInterface;
using GameCore.Utils;
using OpenGL;
using PixelFormat = OpenGL.PixelFormat;

#endregion

namespace GameCore.Render.RenderLayers
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SVertex2D
    {
        public float x, y, z; //Position
        public float u, v; //Uv

        public float[] ToArray()
        {
            return new[] {x, y, z, u, v};
        }

        public static SVertex2D FromArray(float[] anArray)
        {
            if (anArray.Length != 5)
            {
                throw new ArgumentException("Needs to be an array of length 5");
            }
            SVertex2D tempSVertex2D = new SVertex2D()
            {
                x = anArray[0],
                y = anArray[1],
                z = anArray[2],
                u = anArray[3],
                v = anArray[4]
            };
            return tempSVertex2D;
        }

        public override string ToString()
        {
            return "" + x + "," + y + "," + z + "," + u + "," + v;
        }
    };


    [StructLayout(LayoutKind.Sequential)]
    internal struct SDrawElementsCommand
    {
        public UInt32 vertexCount;
        public UInt32 instanceCount;
        public UInt32 firstIndex;
        public UInt32 baseVertex;
        public UInt32 baseInstance;


        public UInt32[] ToArray()
        {
            return new[] {vertexCount, instanceCount, firstIndex, baseVertex, baseInstance};
        }

        public override string ToString()
        {
            return "" + vertexCount + "," + instanceCount + "," + firstIndex + "," + baseVertex + "," + baseInstance;
        }
    };



    public class RenderLayerMapMultiDrawElementsIndirect : RenderLayerBase
    {
        private Camera theCamera;
        private Map.Map theMap;

        private ShaderProgram program;

        /// <summary>
        ///     The near clipping distance.
        /// </summary>
        private const float ZNear = 0.1f;

        /// <summary>
        ///     The far clipping distance.
        /// </summary>
        private const float ZFar = 1000f;

        /// <summary>
        ///     Field of view of the camera
        /// </summary>
        private const float Fov = 0.45f;

        private Matrix4 projectionMatrix;

        private readonly SVertex2D[] gQuad = new SVertex2D[]
        {
            SVertex2D.FromArray(new[] {0.0f, 0.0f, 0.0f, 0.0f, 0.0f}),
            SVertex2D.FromArray(new[] {1.0f, 0.0f, 0.0f, 1.0f, 0.0f}),
            SVertex2D.FromArray(new[] {0.0f, 0.0f, 1.0f, 0.0f, 1.0f}),
            SVertex2D.FromArray(new[] {1.0f, 0.0f, 1.0f, 1.0f, 1.0f})
        };

        private readonly int[] gIndex = new int[] {0, 1, 2, 1, 3, 2};

        private VBO<float> gVertexBuffer;
        private VBO<int> gElementBuffer;
        private VBO<int> gIndirectBuffer;
        private VBO<float> gDrawIdBuffer;

        private int numberOfTiles;
        private int stride;
        private uint textureId;

       
        public override void OnLoad()
        {
            theMap = TheGameStatus.TheMap;

            // create our shader program
            program = new ShaderProgram(VertexShader, FragmentShader);

            // set up the projection and view matrix
            program.Use();
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(Fov, (float) Width/Height, ZNear,
                ZFar);
            program["projection_matrix"].SetValue(projectionMatrix);
//            program["model_matrix"].SetValue(Matrix4.Identity);
//            program["light_direction"].SetValue(lightDirection);
//            program["enable_lighting"].SetValue(lighting);
//            program["ambient"].SetValue(ambientLighting);

//            Gl.Uniform1i(0,0);

            GenerateGeometry();
            GenerateArrayTexture();

            Gl.UseProgram(0);
            Gl.BindBuffer(BufferTarget.DrawIndirectBuffer, 0);
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }


        private void GenerateGeometry()
        {
            Stopwatch watch = Stopwatch.StartNew();


            Dictionary<Tile.TileIds, Bitmap> tempTiletypeList =
                RenderObjects.RenderObjects.CreateTileBitmaps(new Size(256, 256));
            List<ObjObject> tempObjList = new List<ObjObject>();
            List<Tile> tempTiles = theMap.Tiles;

            numberOfTiles = tempTiles.Count();
            int numberOfTextures = tempTiletypeList.Count;
            // Number of values in SVertex2D
            stride = 5;
            SVertex2D[] vVertex = new SVertex2D[numberOfTiles*4];

            // n tiles
            // 4 SVertex2D vectors per tile
            // 5 (stride) entries per SVertex2D

            int index = 0;
            foreach (Tile tempTile in tempTiles)
            {
                Vector tempLoc = tempTile.Location;
                for (int k = 0; k != 4; ++k)
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
            float[] verteses = new float[numberOfTiles*stride*4];

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

            gElementBuffer = new VBO<int>(gIndex, BufferTarget.ElementArrayBuffer);
            Gl.BindBuffer(gElementBuffer);

            //Generate draw commands
            SDrawElementsCommand[] vDrawCommand = new SDrawElementsCommand[numberOfTiles];
            for (uint i = 0; i < numberOfTiles; ++i)
            {
                vDrawCommand[i].vertexCount = 6;
                vDrawCommand[i].instanceCount = 1;
                vDrawCommand[i].firstIndex = 0;
                vDrawCommand[i].baseVertex = (uint) (i*4); //****
                vDrawCommand[i].baseInstance = i;
            }


            // Copy into float array
            //Generate draw commands
            int vDrawCommanStride = 5;
            int[] vDrawCommandArray = new int[numberOfTiles*vDrawCommanStride];

            index = 0;
            for (int i = 0; i < vDrawCommand.Count(); i++)
            {
                vDrawCommandArray[index] = (int) vDrawCommand[i].vertexCount;
                vDrawCommandArray[index + 1] = (int) vDrawCommand[i].instanceCount;
                vDrawCommandArray[index + 2] = (int) vDrawCommand[i].firstIndex;
                vDrawCommandArray[index + 3] = (int) vDrawCommand[i].baseVertex;
                vDrawCommandArray[index + 4] = (int) vDrawCommand[i].baseInstance;
                index += vDrawCommanStride;
            }

            gIndirectBuffer = new VBO<int>(vDrawCommandArray, BufferTarget.DrawIndirectBuffer,
                BufferUsageHint.StaticDraw);
            Gl.BindBuffer(gIndirectBuffer);


            //Generate an instanced vertex array to identify each draw call in the shader
            float[] vDrawId = new float[numberOfTiles];

            index = 0;
            foreach (Tile aTile in tempTiles)
            {
                vDrawId[index] = (int) aTile.TheTileId;
                index++;
            }


            //            for (UInt32 i = 0; i < numberOfTiles; i++)
            //            {
            //                vDrawId[i] = i;
            //            }

            gDrawIdBuffer = new VBO<float>(vDrawId, VertexAttribPointerType.Float, BufferTarget.ArrayBuffer,
                BufferUsageHint.StaticDraw);
            Gl.BindBuffer(gDrawIdBuffer);

            locationDrawid = (uint) Gl.GetAttribLocation(program.ProgramID, "drawTexId");

            watch.Stop();
            GameCore.TheGameCore.RaiseMessage(string.Format("CreateTiles() took {0}ms", watch.ElapsedMilliseconds));
        }

        private void GenerateArrayTexture()
        {
            //Generate an array texture
            //            Gl.GenTextures(1, gArrayTexture);
            textureId = Gl.GenTexture();

            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2DArray, textureId);

            //Generate an array texture
            //  glGenTextures( 1, &gArrayTexture );
            //  glActiveTexture(GL_TEXTURE0);
            //  glBindTexture(GL_TEXTURE_2D_ARRAY, gArrayTexture);

            Size textureSize = new Size(256, 256);

            Dictionary<Tile.TileIds, Bitmap> tempTiletypeList =
                RenderObjects.RenderObjects.CreateTileBitmaps(new Size(256, 256));

            //            int numberOfTiles = tempTiles.Count();
            int numberOfTextures = tempTiletypeList.Count;

            // replace glTexStorage3D with following code
            // https://www.opengl.org/sdk/docs/man/html/glTexStorage3D.xhtml
            //http://stackoverflow.com/questions/17760193/correct-storage-allocation-for-textures-in-gl-texture-2d-array
            //No mipmaps as textures are 1x1
            int levels = 1; // Specify the number of texture levels

            int width = textureSize.Width;
            int height = textureSize.Height;

            for (int i = 0; i < levels; i++)
            {
                Gl.TexImage3D(TextureTarget.Texture2DArray, i, PixelInternalFormat.Rgba8, width, height,
                    numberOfTextures,
                    0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
                width = Math.Max(1, (width/2));
                height = Math.Max(1, (height/2));
            }
            //Create storage for the texture. (100 layers of 1x1 texels)
            //  glTexStorage3D( GL_TEXTURE_2D_ARRAY,
            //                  1,                    //No mipmaps as textures are 1x1
            //                  GL_RGB8,              //Internal format
            //                  1, 1,                 //width,height
            //                  100                   //Number of layers
            //                );

            for (int i = 0; i != numberOfTextures; ++i)
            {
                Bitmap tempBmp = tempTiletypeList.Values.ToArray()[i];


                Size tempSize = tempBmp.Size;

                // must be Format32bppArgb file format, so convert it if it isn't in that format
                BitmapData bitmapData = tempBmp.LockBits(new Rectangle(0, 0, tempSize.Width, tempSize.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);


                Gl.TexSubImage3D(TextureTarget.Texture2DArray,
                    0, //Mipmap number
                    0, 0, i, //xoffset, yoffset, zoffset
                    textureSize.Width, textureSize.Height, 1, //width, height, depth
                    PixelFormat.Bgra, //format
                    PixelType.UnsignedByte, //type
                    bitmapData.Scan0 //pointer to data
                    );


                tempBmp.UnlockBits(bitmapData);

                //    glTexSubImage3D( GL_TEXTURE_2D_ARRAY,
                //                     0,                     //Mipmap number
                //                     0,0,i,                 //xoffset, yoffset, zoffset
                //                     1,1,1,                 //width, height, depth
                //                     GL_RGB,                //format
                //                     GL_UNSIGNED_BYTE,      //type
                //                     color);                //pointer to data
            }
//
//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);//(int)TextureParam.Linear);   // linear filter
//            Gl.BindTexture(TextureTarget.Texture2DArray, 0);


            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter,
                TextureParameter.Linear);
            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter,
                TextureParameter.Linear);
            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS,
                TextureParameter.ClampToEdge);
            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT,
                TextureParameter.ClampToEdge);
        }


        public override void OnDisplay()
        {
        }

        public override void OnRenderFrame(float deltaTime)
        {
            // apply our camera view matrix to the shader view matrix (this can be used for all objects in the scene)
            //            Gl.UseProgram(program);
            program.Use();
            program["view_matrix"].SetValue(theCamera.ViewMatrix);
            program["projection_matrix"].SetValue(projectionMatrix);
//            program["textureArray"].SetValue(textureId);


            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2DArray, textureId);

            int tempStride = sizeof (Single)*stride; // *****

            Gl.EnableVertexAttribArray(locationPosition);
            Gl.BindBuffer(gVertexBuffer);
            Gl.VertexAttribPointer(locationPosition, 3, VertexAttribPointerType.Float, false, tempStride,
                IntPtr.Zero);

            Gl.EnableVertexAttribArray(locationTexCoord);
            Gl.VertexAttribPointer(locationTexCoord, 2, VertexAttribPointerType.Float, false, tempStride,
                new IntPtr(3*sizeof (Single)));


            Gl.BindBuffer(gIndirectBuffer);


            Gl.EnableVertexAttribArray(locationDrawid);
            Gl.BindBuffer(gDrawIdBuffer);
//            Gl.VertexAttribPointer(locationDrawid, 1, VertexAttribPointerType.Int, false, Marshal.SizeOf(typeof(int)),
//                IntPtr.Zero);
            Gl.VertexAttribPointer(locationDrawid, 1, VertexAttribPointerType.Float, false, sizeof(Single), IntPtr.Zero);
            Gl.VertexAttribDivisor(locationDrawid, 1);
            // This needs to be implemented.
            // https://www.opengl.org/wiki/GLAPI/glMultiDrawElementsIndirect
            //  glClear( GL_COLOR_BUFFER_BIT );


            Gl.MultiDrawElementsIndirect(BeginMode.Triangles, DrawElementsType.UnsignedInt, IntPtr.Zero, numberOfTiles,0);
            //  glMultiDrawElementsIndirect( GL_TRIANGLES, 
            //			       GL_UNSIGNED_INT, 
            //			       (GLvoid*)0, 
            //			       100, 
            //			       0 );

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
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(Fov, (float) Width/Height, ZNear,
                ZFar);
            program["projection_matrix"].SetValue(projectionMatrix);
        }

        public override void OnClose()
        {
            if (gVertexBuffer != null) gVertexBuffer.Dispose();
            if (gElementBuffer != null) gElementBuffer.Dispose();
            if (gIndirectBuffer != null) gIndirectBuffer.Dispose();
            if (gDrawIdBuffer != null) gDrawIdBuffer.Dispose();
            Gl.DeleteTextures(1, new uint[] {textureId});
        }

        public override void ReInitialize()
        {
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

        public static string VertexShader = @"
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
      gl_Position = projection_matrix *view_matrix * vec4(position,1.0);
      uv = texCoord;
     // drwaID = drawTexId;
     // int tempDrawId = drawTexId/1000000000;
     drawID = drawTexId;
    }";


        //              int tempDrawId = drawTexId;
//      if (tempDrawId > 3)
//         drawID = 3;
//      else if (tempDrawId < 0)
//         drawID = 0;
//      else
//        drawID = tempDrawId;


        // drawid = 2;
        //   int tempDrawId = drawid;
        //   int tempDrawId = drawid/1000000000;
        //    if (tempDrawId > 3)
        //        drawID = 3;
        //     else
        //        drawID = tempDrawId;
        // drawID = 1;


        //       public static string vertexShaderSource = @"
        //#version 430 core
        //    in vec3 position;
        //    in vec2 texCoord;
        //    in uint drawid;
        //    uniform mat4 view_matrix;
        //    uniform mat4 projection_matrix;
        //
        //    out vec2 uv;
        //    flat out uint drawID;
        //    void main(void)
        //    {
        //      gl_Position = projection_matrix *view_matrix * vec4(position,1.0);
        //      uv = texCoord;
        //      drawID = drawid;
        //    }";
        //        public static string vertexShaderSource = @"
        //#version 430 core
        //    layout (location = 0 ) in vec3 position;
        //    layout (location = 1 ) in vec2 texCoord;
        //    layout (location = 2 ) in uint drawid;
        //    uniform mat4 view_matrix;
        //    uniform mat4 projection_matrix;
        //
        //    out vec2 uv;
        //    flat out uint drawID;
        //    void main(void)
        //    {
        //      gl_Position = projection_matrix *view_matrix * vec4(position,1.0);
        //      uv = texCoord;
        //      drawID = drawid;
        //    }";

        public static string FragmentShader = @"
#version 430 core
    out vec4 color;
    in vec2 uv;
    flat in float drawID;
//    in int drawID;
    layout (binding=0) uniform sampler2DArray textureArray;
    void main(void)
    {
    //    int actual_layer = max(0, min(d​ - 1, floor(drawID​ + 0.5)) )
    //  color = texture(textureArray, vec3(uv.x,uv.y,actual_layer) );
      color = texture(textureArray, vec3(uv.x,uv.y,drawID) );
    }";
        private uint locationPosition;
        private uint locationDrawid;
        private uint locationTexCoord;
        //
        //        public static string fragmentShaderSource = @"
        //#version 430 core
        //    out vec4 color;
        //    in vec2 uv;
        //    flat in uint drawID;
        //    layout (binding=0) uniform sampler2DArray textureArray;
        //    void main(void)
        //    {
        //      color = texture(textureArray, vec3(uv.x,uv.y,drawID) );
        //    }";

        #endregion
    }
}