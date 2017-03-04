using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using D3D11Introduction.utils;

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Diagnostics;

namespace D3D11Introduction
{
    public class Example04 : ExampleBase
    {
        #region
        protected ShaderMatrixCB mShader = null;
        protected InputLayout mLayout = null;
        protected Texture mTexture = null;
        protected SamplerState mSampler = null;
        protected CubeTexturedNormals mCube = null;
        protected Stopwatch mClock = null;
        #endregion

        public override void Initialize()
        {
            base.Initialize();

            mShader = new ShaderMatrixCB();
            mTexture = new utils.Texture();
            mTexture.Initialize(mDevice, @"textures/crate01.jpg");

            // Create a texture sampler state description.
            SamplerStateDescription samplerDesc = new SamplerStateDescription()
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                MipLodBias = 0,
                MaximumAnisotropy = 1,
                ComparisonFunction = Comparison.Always,
                BorderColor = new Color4(0, 0, 0, 0),  // Black Border.
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            };

            // Create the texture sampler state.
            mSampler = new SamplerState(mDevice, samplerDesc);
        }

        protected override void RunPhase1()
        {
            base.RunPhase1();

            SamplerStateDescription desc = new SamplerStateDescription()
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                MipLodBias = 0,
                MaximumAnisotropy = 1,
                ComparisonFunction = Comparison.Always,
                BorderColor = new Color4(0, 0, 0, 0),
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            };

            mSampler = new SamplerState(mDevice, desc);

            string vsData = System.IO.File.ReadAllText("shaders\\example04.vs");
            string psData = System.IO.File.ReadAllText("shaders\\example04.ps");

            mShader.Load(mDevice, vsData, psData);
            mShader.Compile(mDevice);

            mLayout = new InputLayout(mDevice, mShader.VertexShaderSignature, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 32, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 48, 0)
            });

            mDeviceContext.InputAssembler.InputLayout = mLayout;

            mCube = new CubeTexturedNormals(mDevice);

            mClock = new Stopwatch();
            mClock.Start();
        }

        protected override void CustomRender()
        {
            Matrix world = Matrix.Identity;
            Matrix viewProj = Matrix.Multiply(mView, mProj);

            mDeviceContext.ClearDepthStencilView(mDepthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            mDeviceContext.ClearRenderTargetView(mRenderView, Color.Black);

            mDeviceContext.PixelShader.SetSampler(0, mSampler);

            // float currentTime = 0.1f;
            float currentTime = mClock.ElapsedMilliseconds / 1000.0f;
            world = Matrix.RotationX(currentTime) * Matrix.RotationY(currentTime * 2.0f) * Matrix.RotationZ(currentTime * 0.7f);
            Matrix worldViewProj = world * viewProj;
            worldViewProj.Transpose();

            mShader.SetShaderParam(mDevice, new Vector3(0.0f, 5.0f, 5.0f), mTexture.TextureResource, new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(0.1f, 0.1f, 0.1f, 1.0f), ref world, ref viewProj);
            mShader.Apply(mDevice);

            mCube.Draw(mDevice);
        }
    }
}
