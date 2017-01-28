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

    class Program
    {
        static void Main(string[] args)
        {
            using (Example02 game = new Example02() )
            {
                game.Run(30.0);
            }
        }
    }
}
