using System;

using D3D11Introduction.utils;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using System.Diagnostics;

namespace D3D11Introduction
{
    public class Example02 : ExampleBase
    {
        #region protected members
        protected Shader mShader = null;
        protected Cube mCube = null;

        protected string mShaderData = string.Empty;
        protected InputLayout mLayout = null;
        #endregion

        #region private members
        private Stopwatch mClock = null;
        #endregion

        public override void Initialize()
        {
            base.Initialize();
            mCube = new Cube(mDevice);
            mShader = new Shader();

            mShaderData = System.IO.File.ReadAllText(@"shaders\example01.fx");

            mClock = new Stopwatch();
            mClock.Start();
        }

        protected override void RunPhase1()
        {
            base.RunPhase1();

            mShader.Load(mDevice, mShaderData);
            mShader.Compile(mDevice);

            mLayout = new InputLayout(mDevice, mShader.VertexShaderSignature, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
            });

            mDeviceContext.InputAssembler.InputLayout = mLayout;
        }

        protected override void CustomRender()
        {
            Matrix viewProj = Matrix.Multiply(mView, mProj);

            mDeviceContext.ClearDepthStencilView(mDepthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            mDeviceContext.ClearRenderTargetView(mRenderView, Color.Black);

            float currentTime = mClock.ElapsedMilliseconds / 1000.0f;
            Matrix worldViewProj = Matrix.RotationX(currentTime) * Matrix.RotationY(currentTime * 2.0f) * Matrix.RotationZ(currentTime * 0.7f) * viewProj;
            worldViewProj.Transpose();

            mShader.SetShaderParam(mDevice, worldViewProj, Vector3.Zero, Vector4.Zero, Vector4.Zero);
            mShader.Apply(mDevice);

            mCube.Draw(mDevice);
        }
    }
}
