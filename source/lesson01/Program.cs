using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using System.Drawing;
using System.Collections.Generic;

/// This first example project illustrates simple GL graphics primitives in screen space.
///
/// We don't talk about initializing windowing systems (that's taken care by OpenTK). However
/// we are using the outdated, fixed function pipeline to illustrate basic rendering concepts.
/// 

namespace BBIU_CSharp_Native
{

    abstract class ExampleBase : GameWindow
    {
        public ExampleBase() : base(1024, 768, new OpenTK.Graphics.GraphicsMode(32, 8, 0, 0))
        { }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Title = "Intro to OpenGL Graphics";

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            CustomRenderFrame(e.Time);

            base.SwapBuffers();

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        protected abstract void CustomRenderFrame(double delta);
    }

    /// <summary>
    ///  Rendering a simple triangle into screen space
    /// </summary>
    class Example01 : ExampleBase
    {
        public Example01() : base()
        { }

        protected override void CustomRenderFrame(double delta)
        {            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Begin(BeginMode.Triangles);

            { // Not necessary, just in place for clarity
                GL.Color3(1.0f, 0.0f, 0.0f);
                GL.Vertex2(0,0);

                GL.Color3(0.0f, 1.0f, 0.0f);
                GL.Vertex2(1, 0);

                GL.Color3(0.0f, 0.0f, 1.0f);
                GL.Vertex2(0, 1);
            }

            GL.End();

        }
    }

    /// <summary>
    /// Not a particularly exciting program, but it illustrates how screen space works.
    /// Try changing the vertex positions to see what happens!
    /// </summary>
    class Example02 : ExampleBase
    {
        public Example02() : base()
        { }

        protected override void CustomRenderFrame(double delta)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Begin(BeginMode.Triangles);

            { // What happens when you change the vertex positions?
                GL.Color3(1.0f, 0.0f, 0.0f);
                GL.Vertex2(-0.5f, -0.5f);

                GL.Color3(0.0f, 0.0f, 1.0f);
                GL.Vertex2(-0.5f, 0.5f);

                GL.Color3(0.0f, 1.0f, 0.0f);
                GL.Vertex2(0.5f, 0.5f);

                GL.Color3(1.0f, 0.0f, 0.0f);
                GL.Vertex2(0.5f, 0.5f);

                GL.Color3(0.0f, 0.0f, 1.0f);
                GL.Vertex2(0.5f, -0.5f);

                GL.Color3(0.0f, 1.0f, 0.0f);
                GL.Vertex2(-0.5f, -0.5f);
            }

            GL.End();
        }
    }

    /// <summary>
    /// Blending example.
    ///  Nothing too crazy here. Illustrating how one goes about setting up transparencies.
    /// </summary>
    class Example03 : ExampleBase
    {
        public Example03() : base()
        { }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        protected override void CustomRenderFrame(double delta)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Begin(BeginMode.Triangles);
            {
                GL.Color3(1.0f, 0.0f, 0.0f);
                GL.Vertex2(-0.75f, -0.75f);

                GL.Color3(0.0f, 0.0f, 1.0f);
                GL.Vertex2(-0.75f, 0.75f);

                GL.Color3(0.0f, 1.0f, 0.0f);
                GL.Vertex2(0.75f, 0.75f);

                GL.Color3(1.0f, 0.0f, 0.0f);
                GL.Vertex2(0.75f, 0.75f);

                GL.Color3(0.0f, 0.0f, 1.0f);
                GL.Vertex2(0.75f, -0.75f);

                GL.Color3(0.0f, 1.0f, 0.0f);
                GL.Vertex2(-0.75f, -0.75f);

                GL.Color4(1.0f, 1.0f, 1.0f, .5f);
                GL.Vertex2(0.0f, 0.0f);

                GL.Color4(1.0f, 1.0f, 1.0f, .5f);
                GL.Vertex2(0.0f, 1.0f);

                GL.Color4(1.0f, 1.0f, 1.0f, .5f);
                GL.Vertex2(1.0f, 0.0f);
            }
            GL.End();
        }
    }

    /// <summary>
    /// Depth testing example
    /// </summary>
    class Example04 : Example03
    {
        public Example04() : base()
        { }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
        }

        protected override void CustomRenderFrame(double delta)
        {
            GL.ClearDepth(1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Begin(BeginMode.Triangles);
            {
                GL.Color3(1.0f, 0.0f, 0.0f);
                GL.Vertex3(-0.75f, -0.75f, 0.5f);

                GL.Color3(0.0f, 0.0f, 1.0f);
                GL.Vertex3(-0.75f, 0.75f, 0.5f);

                GL.Color3(0.0f, 1.0f, 0.0f);
                GL.Vertex3(0.75f, 0.75f, 0.5f);

                GL.Color4(1.0f, 1.0f, 1.0f, .5f);
                GL.Vertex3(0.0f, 0.0f, 0.75f);

                GL.Color4(1.0f, 1.0f, 1.0f, .5f);
                GL.Vertex3(0.0f, 1.0f, 0.75f);

                GL.Color4(1.0f, 1.0f, 1.0f, .5f);
                GL.Vertex3(1.0f, 0.0f, 0.75f);
            }
            GL.End();

        }
    }

    /// <summary>
    /// Texture Example
    /// </summary>
    class Example05 : ExampleBase
    {
        private int mSampleImageTextureID = 0;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.Enable(EnableCap.Texture2D);

            // Load up any resources we need
            mSampleImageTextureID = ContentPipeline.LoadTexture("resources/SampleImage01.png");
        }


        protected override void CustomRenderFrame(double delta)
        {
            GL.ClearDepth(1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindTexture(TextureTarget.Texture2D, mSampleImageTextureID);

            GL.Begin(PrimitiveType.Triangles);
            {
                GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);

                GL.TexCoord2(0.0f, 0.0f);
                GL.Vertex2(-0.5f, 0.5f);

                GL.TexCoord2(1.0f, 0.0f);
                GL.Vertex2(0.5f, 0.5f);

                GL.TexCoord2(0.0f, 1.0f);
                GL.Vertex2(-0.5f, -0.5f);

                GL.TexCoord2(1.0f, 0.0f);
                GL.Vertex2(0.5f, 0.5f);

                GL.TexCoord2(1.0f, 1.0f);
                GL.Vertex2(0.5f, -0.5f);

                GL.TexCoord2(0.0f, 1.0f);
                GL.Vertex2(-0.5f, -0.5f);

            }
            GL.End();
        }

    }

    class ContentPipeline
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
    class Program
    {
        static void Main(string[] args)
        {
            using (Example05 game = new Example05() )
            {
                game.Run(30.0);
            }
        }
    }
}
