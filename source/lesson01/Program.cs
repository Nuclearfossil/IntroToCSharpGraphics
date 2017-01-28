using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;


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
        {

        }

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
        {
        }

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
        {
        }

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

    class Example03 : ExampleBase
    {
        public Example03() : base()
        {
        }

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

    class Example04 : Example03
    {
        public Example04() : base()
        {
        }

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
    class Program
    {
        static void Main(string[] args)
        {
            using (Example04 game = new Example04() )
            {
                game.Run(30.0);
            }
        }
    }
}
