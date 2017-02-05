using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace ShaderIntroduction
{
    class Example : ExampleBase
    {
        private RenderGrid DrawGrid = new RenderGrid();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            DrawGrid = new RenderGrid();
            DrawGrid.Initialize(System.Drawing.Color.Green, 0, 0, 10, 500);

            CurrentCamera.Position = new OpenTK.Vector3(10.0f, 10.0f, 10.0f);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            CurrentCamera.Update();
        }

        protected override void CustomRenderFrame(double delta)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var projection = CurrentCamera.Projection;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);

            var modelview = CurrentCamera.View;
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            DrawGrid.Render();

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
    }
    class Program
    {
        static void Main(string[] args)
        {
            using (Example currentGame = new Example())
            {
                currentGame.Run(30);
            }
        }
    }
}
