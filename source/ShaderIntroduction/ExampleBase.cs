using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using System.Drawing;
using System.Collections.Generic;
using System;

namespace ShaderIntroduction
{
    abstract class ExampleBase : GameWindow
    {
        protected Camera CurrentCamera;

        public ExampleBase() :
            base(1024, 768, new OpenTK.Graphics.GraphicsMode(32, 16, 0, 0))
        {
            CurrentCamera = new Camera(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Title = "Shader Introduction";
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            // Enable Blending
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // Depth Testing
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthClamp);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.DepthMask(true);

            // Face Culling
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
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
}
