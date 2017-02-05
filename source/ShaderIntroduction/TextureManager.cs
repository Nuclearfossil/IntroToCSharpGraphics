using System.Collections.Generic;
using System.Drawing;

using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;

namespace ShaderIntroduction
{
    public class TextureManager
    {
        private static Dictionary<string, int> TextureResources = new Dictionary<string, int>();

        public static int LoadTexture(string filename)
        {
            // Have we already loaded the bitmap? if so, return the Texture ID
            if (TextureResources.ContainsKey(filename))
            {
                return TextureResources[filename];
            }

            Bitmap image = new Bitmap(filename);
            int textureID = LoadImage(image);
            TextureResources.Add(filename, textureID);
            return textureID;
        }

        private static int LoadImage(Bitmap image)
        {
            int texID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texID);
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                            ImageLockMode.ReadOnly,
                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D,
                        0,
                        PixelInternalFormat.Rgba,
                        data.Width, data.Height,
                        0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType.UnsignedByte,
                        data.Scan0);
            image.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texID;
        }
    }
}
