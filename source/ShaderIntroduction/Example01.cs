﻿using System;

using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace ShaderIntroduction
{
    class Example01 : ExampleBase
    {
        private Shader BasicShader;

        private VertexBuffer vertexBuffer;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            BasicShader = new Shader(@"shaders/vertex.glsl", @"shaders/fragment.glsl");

            //setup the vertex buffer [vbo]
            vertexBuffer = new VertexBuffer(3);

            //just a triangle with full r g b
            vertexBuffer.AddVertex(0.0f, 0.0f, 0.0f,        // X, Y, Z
                                   1.0f, 0.0f, 0.0f, 1.0f); // R, G, B, A
            vertexBuffer.AddVertex(0.5f, 1.0f, 0.0f,        // X, Y, Z
                                   0.0f, 1.0f, 0.0f, 1.0f); // R, G, B, A
            vertexBuffer.AddVertex(1.0f, 0.0f, 0.0f,        // X, Y, Z
                                   0.0f, 0.0f, 1.0f, 1.0f); // R, G, B, A

            vertexBuffer.AddIndex(0, 1, 2); // Indices into the VBO

            vertexBuffer.LoadIntoMemory();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var CameraPosition = new Vector3(0.5f, 0.5f, 0);
            var ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 
                                                                        Width / (float)Height,
                                                                        0.5f,
                                                                        1000.0f);
            var WorldMatrix = Matrix4.CreateTranslation(-CameraPosition);
            var ModelviewMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, -2.0f);

            Matrix4 ModelViewProjection = ModelviewMatrix * WorldMatrix * ProjectionMatrix;

            GL.UseProgram(BasicShader.Program);
            int mvpLocation = GL.GetUniformLocation(BasicShader.Program, "mvp_matrix");
            GL.UniformMatrix4(mvpLocation, false, ref ModelViewProjection);
            GL.UseProgram(0);
        }

        protected override void CustomRenderFrame(double delta)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(BasicShader.Program);
            vertexBuffer.Render(BasicShader);
            GL.UseProgram(0);
        }

    }
}
