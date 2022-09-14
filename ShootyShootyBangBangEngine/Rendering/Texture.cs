using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ShootyShootyBangBangEngine.Rendering
{
    public class Texture
    {
        int m_handle;
        int m_width;
        int m_height;

        public int GetWidth() { return m_width; }
        public int GetHeight() { return m_height; }

        public Texture(string path)
        {
            m_handle = GL.GenTexture();
            Use();

            //Load the image
            Image<Rgba32> image = (Image<Rgba32>)Image.Load(path);

            //ImageSharp loads from the top-left pixel, whereas OpenGL loads from the bottom-left, causing the texture to be flipped vertically.
            //This will correct that, making the texture display properly.
            image.Mutate(x => x.Flip(FlipMode.Vertical));
            m_width = image.Width;
            m_height = image.Height;

            //Get an array of the pixels, in ImageSharp's internal format.
            Rgba32[] tempPixels = image.GetPixelSpan().ToArray();

            //Convert ImageSharp's format into a byte array, so we can use it with OpenGL.
            List<byte> pixels = new List<byte>();

            foreach (Rgba32 p in tempPixels)
            {
                pixels.Add(p.R);
                pixels.Add(p.G);
                pixels.Add(p.B);
                pixels.Add(p.A);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());
        }

        public void Use()
        {
            GL.BindTexture(TextureTarget.Texture2D, m_handle);
        }
    }
}
