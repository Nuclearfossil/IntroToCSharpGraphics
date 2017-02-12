using System;

using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace ShaderIntroduction
{
    class Example01 : ExampleBase
    {
        private Shader mBasicShader;
        private VertexBuffer mVertexBuffer01;
        private VertexBuffer mVertexBuffer02;

        private RenderGrid mGrid;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthClamp);

            GL.DepthMask(true);

            mBasicShader = new Shader(@"shaders/vertex.glsl", @"shaders/fragment.glsl");

            #region Setup the vertex buffer [vbo]
            mVertexBuffer01 = new VertexBuffer(6);

            //just a triangle with full r g b a
            mVertexBuffer01.AddVertex(0.0f, 0.0f, 0.0f,        // X, Y, Z
                                      1.0f, 0.0f, 0.0f, 1.0f); // R, G, B, A
            mVertexBuffer01.AddVertex(1.0f, 0.0f, 0.0f,        // X, Y, Z
                                      0.0f, 0.8f, 0.0f, 1.0f); // R, G, B, A
            mVertexBuffer01.AddVertex(1.0f, 1.0f, 0.0f,        // X, Y, Z
                                      0.0f, 0.0f, 1.0f, 1.0f); // R, G, B, A
            mVertexBuffer01.AddTriangle(0, 1, 2); // Indices into the VBO

            mVertexBuffer01.AddVertex(1.0f, 1.0f, 0.0f,        // X, Y, Z
                                      0.0f, 0.0f, 1.0f, 1.0f); // R, G, B, A
            mVertexBuffer01.AddVertex(0.0f, 1.0f, 0.0f,        // X, Y, Z
                                      0.0f, 0.8f, 0.0f, 1.0f); // R, G, B, A
            mVertexBuffer01.AddVertex(0.0f, 0.0f, 0.0f,        // X, Y, Z
                                      1.0f, 0.0f, 0.0f, 1.0f); // R, G, B, A
            mVertexBuffer01.AddTriangle(3, 4, 5); // Indices into the VBO

            mVertexBuffer01.LoadIntoMemory();
            #endregion

            #region Setting up the second Vertex buffer
            mVertexBuffer02 = new VertexBuffer(6);
            mVertexBuffer02.AddVertex(0.0f, 0.0f, 1.0f,        // X, Y, Z
                                      1.0f, 0.0f, 1.0f, 1.0f); // R, G, B, A
            mVertexBuffer02.AddVertex(1.0f, 0.0f, 1.0f,        // X, Y, Z
                                      1.0f, 0.0f, 1.0f, 1.0f); // R, G, B, A
            mVertexBuffer02.AddVertex(1.0f, 1.0f, 1.0f,        // X, Y, Z
                                      1.0f, 0.0f, 1.0f, 1.0f); // R, G, B, A
            mVertexBuffer02.AddTriangle(0, 1, 2); // Indices into the VBO

            mVertexBuffer02.AddVertex(1.0f, 1.0f, 1.0f,        // X, Y, Z
                                      1.0f, 0.0f, 1.0f, 1.0f); // R, G, B, A
            mVertexBuffer02.AddVertex(0.0f, 1.0f, 1.0f,        // X, Y, Z
                                      1.0f, 0.0f, 1.0f, 1.0f); // R, G, B, A
            mVertexBuffer02.AddVertex(0.0f, 0.0f, 1.0f,        // X, Y, Z
                                      1.0f, 0.0f, 1.0f, 1.0f); // R, G, B, A
            mVertexBuffer02.AddTriangle(3, 4, 5); // Indices into the VBO

            mVertexBuffer02.LoadIntoMemory();
            #endregion

            mGrid = new RenderGrid();
            mGrid.Initialize(System.Drawing.Color.Green, 0, 0, 10, 100);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            CurrentCamera.Update();

            var ProjectionMatrix = CurrentCamera.Projection;
            var WorldMatrix = CurrentCamera.View;
            var ModelViewMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);

            Matrix4 ModelViewProjection = ModelViewMatrix * WorldMatrix * ProjectionMatrix;

            GL.UseProgram(mBasicShader.Program);
            int mvpLocation = GL.GetUniformLocation(mBasicShader.Program, "mvp_matrix");
            GL.UniformMatrix4(mvpLocation, false, ref ModelViewProjection);
            GL.UseProgram(0);
        }

        protected override void CustomRenderFrame(double delta)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var ProjectionMatrix = CurrentCamera.Projection;
            var WorldMatrix = CurrentCamera.View;

            mGrid.Render(ref ProjectionMatrix, ref WorldMatrix);

            mVertexBuffer01.Render(mBasicShader);
            mVertexBuffer02.Render(mBasicShader);
        }

    }
}
