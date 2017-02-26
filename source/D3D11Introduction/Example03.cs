using System;

using D3D11Introduction.utils;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using System.Diagnostics;

namespace D3D11Introduction
{
    public class Example03 : ExampleBase
    {
        #region protected members
        protected Shader mShader = null;
        protected CubeNormals mCube = null;

        protected string mVSData = string.Empty;
        protected string mPSData = string.Empty;
        protected InputLayout mLayout = null;
        #endregion

        #region private members
        private Stopwatch mClock = null;
        #endregion

        public override void Initialize()
        {
            base.Initialize();
            mCube = new CubeNormals(mDevice);
            mShader = new Shader();

            mVSData = System.IO.File.ReadAllText("example03.vs");
            mPSData = System.IO.File.ReadAllText("example03.ps");

            mClock = new Stopwatch();
            mClock.Start();
        }

        protected override void RunPhase1()
        {
            base.RunPhase1();

            mShader.Load(mDevice, mVSData, mPSData);
            mShader.Compile(mDevice);

            mLayout = new InputLayout(mDevice, mShader.VertexShaderSignature, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 32, 0)
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

            mShader.SetShaderParam(mDevice, worldViewProj, new Vector3(0.0f, 0.5f, 0.5f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f));
            mShader.Apply(mDevice);

            mCube.Draw(mDevice);
        }
    }
}
