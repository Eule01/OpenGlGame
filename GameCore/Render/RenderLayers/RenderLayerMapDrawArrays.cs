#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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
    public class RenderLayerMapDrawArrays : RenderLayerBase
    {
        public Camera Camera;
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

        private readonly SVertex2D[] gQuadIndexed = new SVertex2D[]
        {
            SVertex2D.FromArray(new[] {0.0f, 0.0f, 0.0f, 0.0f, 0.0f}),
            SVertex2D.FromArray(new[] {1.0f, 0.0f, 0.0f, 1.0f, 0.0f}),
            SVertex2D.FromArray(new[] {0.0f, 0.0f, 1.0f, 0.0f, 1.0f}),
            SVertex2D.FromArray(new[] {1.0f, 0.0f, 1.0f, 1.0f, 1.0f})
        };

        private readonly int[] gIndex = new int[] {0, 1, 2, 1, 3, 2};

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
//        private VBO<int> gDrawIdBuffer;

        private int numberOfTiles;
        private int stride;
        private uint textureId;


        public RenderLayerMapDrawArrays(int width, int height, GameStatus theGameStatus, UserInputPlayer theUserInputPlayer,
            KeyBindings theKeyBindings, MaterialManager theMaterialManager)
            : base(width, height, theGameStatus, theUserInputPlayer, theKeyBindings, theMaterialManager)
        {
        }

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
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        /// <summary>
        /// http://forum.lwjgl.org/index.php?topic=5374.0
        /// </summary>
//        private void createVBO()
//        {
//            FloatBuffer texCoords = BufferUtils.createFloatBuffer(2 * 4 * numColMap * numRowMap);
//            FloatBuffer vertices = BufferUtils.createFloatBuffer(2 * 4 * numColMap * numRowMap);
//
//            for (int row = 0; row < numRowMap; row++)
//            {
//                for (int col = 0; col < numColMap; col++)
//                {
//
//                    currentTile = map[row][col];
//
//                    if (currentTile == 0) continue;
//
//
//                    int index = Math.abs(currentTile);
//
//
//
//                    if (currentTile < 0)
//                        flip = true;
//                    else
//                        flip = false;
//
//
//
//
//                    texCoords.put(tilset[index].getTexCoords());
//
//                    vertices.put(new float[]
//                        {
//                        (float) (x + col * tileSize + (flip?tileSize:0)), (float) (y + row * tileSize),
//                        (float) (x + col * tileSize + tileSize * (flip?-1:1) + (flip?tileSize:0)), (float) (y + row * tileSize),
//                        (float) (x + col * tileSize + tileSize * (flip?-1:1) + (flip?tileSize:0)), (float) (y + row * tileSize + tileSize),
//                        (float) (x + col * tileSize + (flip?tileSize:0)), (float) (y + row * tileSize + tileSize)
//                        });
//                }
//            }
//            texCoords.rewind();
//            vertices.rewind();
//
//            //indexBuffer.rewind();
//
//            vboVertexID = glGenBuffers();
//            glBindBuffer(GL_ARRAY_BUFFER, vboVertexID);
//            glBufferData(GL_ARRAY_BUFFER, vertices, GL_STATIC_DRAW);
//            glBindBuffer(GL_ARRAY_BUFFER, 0);
//
//            vboTextureID = glGenBuffers();
//            glBindBuffer(GL_ARRAY_BUFFER, vboTextureID);
//            glBufferData(GL_ARRAY_BUFFER, texCoords, GL_STATIC_DRAW);
//            glBindBuffer(GL_ARRAY_BUFFER, 0);
//        }
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
            SVertex2D[] vVertex = new SVertex2D[numberOfTiles*6];

            // n tiles
            // 4 SVertex2D vectors per tile
            // 5 (stride) entries per SVertex2D

            int index = 0;
            foreach (Tile tempTile in tempTiles)
            {
                Vector tempLoc = tempTile.Location;
                for (int k = 0; k != 6; ++k)
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
            float[] verteses = new float[numberOfTiles*stride*6];

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
            //Generate an instanced vertex array to identify each draw call in the shader
            float[] vDrawId = new float[numberOfTiles*6];
//            int[] vDrawId = new int[numberOfTiles*6];

//            index = 0;
//            foreach (Tile aTile in tempTiles)
//            {
//                vDrawId[index] = (int) aTile.TheTileId;
//                index++;
//            }


            for (int i = 0; i < numberOfTiles*6; i++)
            {
                Tile aTile = tempTiles[i/6];
                vDrawId[i] = (int) aTile.TheTileId;
            }

            gDrawIdBuffer = new VBO<float>(vDrawId, VertexAttribPointerType.Float, BufferTarget.ArrayBuffer,
                BufferUsageHint.StaticDraw);
//            gDrawIdBuffer = new VBO<int>(vDrawId, VertexAttribPointerType.Int, BufferTarget.ArrayBuffer,
//                BufferUsageHint.StaticDraw);
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

            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter,
                TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter,
                TextureParameter.Nearest); //(int)TextureParam.Linear);   // linear filter
//            Gl.BindTexture(TextureTarget.Texture2DArray, 0);


//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter,
//                TextureParameter.Linear);
//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter,
//                TextureParameter.Linear);
//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS,
//                TextureParameter.ClampToEdge);
//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT,
//                TextureParameter.ClampToEdge);
        }


        public override void OnDisplay()
        {
        }

        public override void OnRenderFrame(float deltaTime)
        {
            // apply our camera view matrix to the shader view matrix (this can be used for all objects in the scene)
            //            Gl.UseProgram(program);
            program.Use();
            program["view_matrix"].SetValue(Camera.ViewMatrix);
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


//            Gl.BindBuffer(gIndirectBuffer);


            Gl.EnableVertexAttribArray(locationDrawid);
            Gl.BindBuffer(gDrawIdBuffer);
//            Gl.VertexAttribPointer(locationDrawid, 1, VertexAttribPointerType.Int, false, Marshal.SizeOf(typeof(int)),
//                IntPtr.Zero);


            Gl.VertexAttribPointer(locationDrawid, 1, VertexAttribPointerType.Float, false, sizeof(Single), IntPtr.Zero);
//            Gl.VertexAttribPointer(locationDrawid, 1, VertexAttribPointerType.Int, false, 4, IntPtr.Zero);


//            Gl.VertexAttribDivisor(locationDrawid, 1);
            // This needs to be implemented.
            // https://www.opengl.org/wiki/GLAPI/glMultiDrawElementsIndirect
            //  glClear( GL_COLOR_BUFFER_BIT );


            //https://www.opengl.org/sdk/docs/man/html/glMultiDrawElementsIndirect.xhtml
//             GLsizei n;
//    for (n = 0; n < drawcount; n++) {
//        const DrawElementsIndirectCommand *cmd;
//        if (stride != 0) {
//            cmd = (const DrawElementsIndirectCommand  *)((uintptr)indirect + n * stride);
//        } else {
//            cmd = (const DrawElementsIndirectCommand  *)indirect + n;
//        }
//
//        glDrawElementsInstancedBaseVertexBaseInstance(mode,
//                                                      cmd->count,
//                                                      type,
//                                                      cmd->firstIndex + size-of-type,
//                                                      cmd->instanceCount,
//                                                      cmd->baseVertex,
//                                                      cmd->baseInstance);
//    }


            Gl.DrawArrays(BeginMode.Triangles, 0, numberOfTiles*6);

//            Gl.DisableVertexAttribArray(0);
//            Gl.Finish();
//            Gl.MultiDrawElementsIndirect(BeginMode.Triangles, DrawElementsType.UnsignedInt, IntPtr.Zero, numberOfTiles,0);
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
            if (gDrawIdBuffer != null) gDrawIdBuffer.Dispose();
            Gl.DeleteTextures(1, new uint[] {textureId});
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
//    in int drawTexId;
    uniform mat4 view_matrix;
    uniform mat4 projection_matrix;

    out vec2 uv;
    flat out float drawID;
//    flat out int drawID;
    void main(void)
    {
//        int tempDrawId = int(drawTexId);
//        drawID = tempDrawId;
        drawID = drawTexId;
     //   drwaID = float(tempDrawId);
        gl_Position = projection_matrix *view_matrix * vec4(position,1.0);
        uv = texCoord;
     // int tempDrawId = drawTexId/1000000000;
     //int actual_layer = max(0, min(4 - 1, floor(drawTexId​ + 0.5)) )
     //drwaID = actual_layer;
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
//    flat in int drawID;
    flat in float drawID;
    layout (binding=0) uniform sampler2DArray textureArray;

    void main(void)
    {
      //float actual_layer = max(0.0, min(4.0 - 1.0, floor(float(drawID)​ + 0.5)));
        float actual_layer = max(0.0, min(4.0 - 1.0,floor(float(drawID) + 0.5)));
    color = texture(textureArray, vec3(uv.x,uv.y,actual_layer) );
    //  color = texture(textureArray, vec3(uv.x,uv.y,drawID) );
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