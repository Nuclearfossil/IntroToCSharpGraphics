using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using System.Drawing;
using System.Collections.Generic;
using OpenTK.Input;

using ImGuiNET;
using System.Numerics;

/// This first example project illustrates simple GL graphics primitives in screen space.
///
/// We don't talk about initializing windowing systems (that's taken care by OpenTK). However
/// we are using the outdated, fixed function pipeline to illustrate basic rendering concepts.
/// 

namespace BBIU_CSharp_Native
{

    abstract class ExampleBase : GameWindow
    {
        public ExampleBase() : base(1024, 768, new OpenTK.Graphics.GraphicsMode(32, 16, 0, 0))
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

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
            {
                Exit();
            }
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
            //GL.Enable(EnableCap.DepthTest);
            //GL.DepthFunc(DepthFunction.Lequal);
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

                GL.Color3(1.0f, 1.0f, 1.0f);
                GL.Vertex3(0.0f, 0.0f, 0.75f);

                GL.Color3(1.0f, 1.0f, 1.0f);
                GL.Vertex3(0.0f, 1.0f, 0.75f);

                GL.Color3(1.0f, 1.0f, 1.0f);
                GL.Vertex3(1.0f, 0.0f, 0.75f);
            }
            GL.End();

        }
    }

    /// <summary>
    /// Very simple texture example
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

    abstract class ExampleBase3D : ExampleBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthClamp);

            GL.DepthMask(true);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);
            double aspectRatio = Width / (double)Height;
            float fov = 1.00899694f;
            float nearPlane = 1.0f;
            float farPlane = 100.0f;

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(fov, (float)aspectRatio, nearPlane, farPlane);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
        }
    }

    class Camera
    {
        // Standard Cartesian co-ordinate frame
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        // Euler angles for rotation (yes, gimble lock)
        public float Yaw { get; set; }

        public Camera(float x=0.0f, float y=0.0f, float z=0.0f)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public void Update()
        {
            // Simple rotation about the Y axis
            float z = Z;
            Z = z * (float)Math.Cos(Yaw) - X * (float)Math.Sin(Yaw);
            X = z * (float)Math.Sin(Yaw) + X * (float)Math.Cos(Yaw);

            Matrix4 lookat = Matrix4.LookAt(X, Y, Z, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            Yaw += Yaw;
        }
    }

    class Example06 : ExampleBase3D
    {
        private Camera Camera3D = new Camera(5, 5, 5);

        protected override void CustomRenderFrame(double delta)
        {
            Camera3D.Yaw = 0.01f;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DrawGrid(Color.Green, 0.0f, 0.0f, 1);

            Camera3D.Update();

            GL.Begin(BeginMode.Quads);
            {
                // Front face
                GL.Color3(System.Drawing.Color.Red);
                GL.Vertex3(1, 1, 0);
                GL.Vertex3(-1, 1, 0);
                GL.Vertex3(-1, -1, 0);
                GL.Vertex3(1, -1, 0);

                // Side face
                GL.Color3(System.Drawing.Color.Blue);
                GL.Vertex3(1, 1, 0);
                GL.Vertex3(1, -1, 0);
                GL.Vertex3(1, -1, -2);
                GL.Vertex3(1, 1, -2);

                // Top face
                GL.Color3(System.Drawing.Color.Green);
                GL.Vertex3(1, 1, 0);
                GL.Vertex3(1, 1, -2);
                GL.Vertex3(-1, 1, -2);
                GL.Vertex3(-1, 1, 0);

                // Other Side face
                GL.Color3(System.Drawing.Color.White);
                GL.Vertex3(-1, 1, 0);
                GL.Vertex3(-1, 1, -2);
                GL.Vertex3(-1, -1, -2);
                GL.Vertex3(-1, -1, 0);

                GL.Color3(System.Drawing.Color.Red);
                GL.Vertex3(1, 1, -2);
                GL.Vertex3(1, -1, -2);
                GL.Vertex3(-1, -1, -2);
                GL.Vertex3(-1, 1, -2);

                // Bottom missing on purpose
            }
            GL.End();

        }

        public void DrawGrid(System.Drawing.Color color, float X, float Z, int cellSize = 16, int gridSize = 256)
        {
            int dX = (int)Math.Round(X / cellSize) * cellSize;
            int dZ = (int)Math.Round(Z / cellSize) * cellSize;

            int cellCount = gridSize / cellSize;

            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.LineWidth(1.5f);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);

            GL.PushMatrix();

            GL.Translate(dX - gridSize / 2, 0, dZ - gridSize / 2);

            int i;

            GL.Color3(color);
            GL.Begin(BeginMode.Lines);

            for (i = 0; i < cellCount + 1; i++)
            {
                int current = i * cellSize;

                GL.Vertex3(current, 0, 0);
                GL.Vertex3(current, 0, gridSize);

                GL.Vertex3(0, 0, current);
                GL.Vertex3(gridSize, 0, current);
            }

            GL.End();

            GL.PopMatrix();

            GL.Disable(EnableCap.LineSmooth);
            GL.Disable(EnableCap.Blend);

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
            using (Example01 game = new Example01() )
            //using (Example02 game = new Example02())
            //using (Example03 game = new Example03())
            //using (Example04 game = new Example04())
            //using (Example05 game = new Example05())
            //using (Example06 game = new Example06())
            {
                game.Run(30.0);
            }
        }
    }
}
