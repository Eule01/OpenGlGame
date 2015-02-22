#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenGL;
using PixelFormat = OpenGL.PixelFormat;

#endregion

namespace GameCore.Render.RenderMaterial
{
    public class TextureArray
    {
        public uint TextureID { get; private set; }

        public Size Size { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="aListOfBitmaps"></param>
        public static TextureArray CreateFromBitmaps(List<Bitmap> aListOfBitmaps)
        {
            TextureArray tempTextureArray = new TextureArray();

            //Generate an array texture
            tempTextureArray.TextureID = Gl.GenTexture();

            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2DArray, tempTextureArray.TextureID);

            tempTextureArray.Size = aListOfBitmaps[0].Size;

            int numberOfTextures = aListOfBitmaps.Count;

            // replace glTexStorage3D with following code
            // https://www.opengl.org/sdk/docs/man/html/glTexStorage3D.xhtml
            //http://stackoverflow.com/questions/17760193/correct-storage-allocation-for-textures-in-gl-texture-2d-array
            //No mipmaps as textures are 1x1
            int levels = 4; // Specify the number of texture levels (mipmaps)
//            int levels = 1; // Specify the number of texture levels (mipmaps)

            int width = tempTextureArray.Size.Width;
            int height = tempTextureArray.Size.Height;

            for (int i = 0; i < levels; i++)
            {
                Gl.TexImage3D(TextureTarget.Texture2DArray, i, PixelInternalFormat.Rgba8, width, height,
                    numberOfTextures,
                    0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
                width = Math.Max(1, (width/2));
                height = Math.Max(1, (height/2));
            }

            for (int i = 0; i != numberOfTextures; ++i)
            {
                Bitmap tempBmp = aListOfBitmaps[i];


                Size tempSize = tempBmp.Size;

                // must be Format32bppArgb file format, so convert it if it isn't in that format
                BitmapData bitmapData = tempBmp.LockBits(new Rectangle(0, 0, tempSize.Width, tempSize.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Gl.TexSubImage3D(TextureTarget.Texture2DArray,
                    0, //Mipmap number
                    0, 0, i, //xoffset, yoffset, zoffset
                    tempTextureArray.Size.Width, tempTextureArray.Size.Height, 1, //width, height, depth
                    PixelFormat.Bgra, //format
                    PixelType.UnsignedByte, //type
                    bitmapData.Scan0 //pointer to data
                    );

                tempBmp.UnlockBits(bitmapData);
            }

            Gl.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);

            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter,
                TextureParameter.NearestMipMapLinear);
            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter,
                TextureParameter.NearestMipMapLinear); 
            
            //(int)TextureParam.Linear);   // linear filter
//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter,
//                TextureParameter.NearestMipMapNearest);
//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter,
//                TextureParameter.NearestMipMapNearest); //(int)TextureParam.Linear);   // linear filter
//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter,
//                TextureParameter.Nearest);
//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter,
//                TextureParameter.Nearest); //(int)TextureParam.Linear);   // linear filter

//
//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter,
//                TextureParameter.Linear);
//            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter,
//                TextureParameter.Linear);
            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS,
                TextureParameter.ClampToEdge);
            Gl.TexParameteri(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT,
                TextureParameter.ClampToEdge);
            Gl.BindTexture(TextureTarget.Texture2DArray, 0);

            return tempTextureArray;
        }

        public void Use()
        {
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2DArray, TextureID);
        }
    }
}