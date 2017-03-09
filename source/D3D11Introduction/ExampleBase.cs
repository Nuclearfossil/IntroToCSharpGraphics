using System;

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDriverType = SharpDX.Direct3D.DriverType;
using System.Windows.Forms;

namespace D3D11Introduction
{
    public abstract class ExampleBase
    {
        #region Protected members
        protected RenderForm mRenderForm = null;
        protected SwapChainDescription mSwapChainDescription;
        protected D3DDevice mDevice = null;
        protected SwapChain mSwapChain = null;
        protected DeviceContext mDeviceContext = null;
        protected Factory mFactory = null;
        protected Matrix mView = Matrix.Identity;
        protected Matrix mProj = Matrix.Identity;
        protected Texture2D mBackBuffer = null;
        protected RenderTargetView mRenderView = null;
        protected Texture2D mDepthBuffer = null;
        protected DepthStencilView mDepthView = null;

        protected bool mHasUserResized = true;
        #endregion


        public ExampleBase()
        {
            mRenderForm = new RenderForm("D3D11 Introduction");
        }

        public virtual void Initialize()
        {
            mSwapChainDescription = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = 
                    new ModeDescription(
                        mRenderForm.ClientSize.Width,
                        mRenderForm.ClientSize.Height,
                        new Rational(60, 1),
                        Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = mRenderForm.Handle,
                SampleDescription = new SampleDescription(1, 0),
                Usage = Usage.RenderTargetOutput
            };

            D3DDevice.CreateWithSwapChain(D3DDriverType.Hardware,
                DeviceCreationFlags.None,
                mSwapChainDescription,
                out mDevice,
                out mSwapChain);

            mDeviceContext = mDevice.ImmediateContext;

            // Allows DXGI to monitor an application's message queue for the alt-enter key
            // sequence.
            mFactory = mSwapChain.GetParent<Factory>();
            mFactory.MakeWindowAssociation(mRenderForm.Handle, WindowAssociationFlags.IgnoreAll);

            mView = Matrix.LookAtLH(new Vector3(0, 0, -5), 
                                    new Vector3(0, 0, 0), Vector3.UnitY);
            mProj = Matrix.Identity;
        }

        public virtual void Run()
        {
            RunPhase1();

            mRenderForm.UserResized += (sender, args) => { mHasUserResized = true; };

            mRenderForm.KeyUp += (sender, args) =>
            {
                KeyboardHandler(sender, args);
            };

            RenderLoop.Run(mRenderForm, () =>
            {
                if (mHasUserResized)
                {
                    Resize();
                }

                CustomRender();

                mSwapChain.Present(0, PresentFlags.None);
            });

            EndPhase1();
        }

        protected virtual void RunPhase1() {}

        protected virtual void EndPhase1()
        {
            mDeviceContext.ClearState();
            mDeviceContext.Flush();
            mDevice.Dispose();
            mDeviceContext.Dispose();
            mSwapChain.Dispose();
            mFactory.Dispose();
        }

        protected virtual void KeyboardHandler(object sender, KeyEventArgs args)
        {
            // Do interesting stuff with the keyboard
            if (args.KeyCode == Keys.Escape)
            {
                mRenderForm.Close();
            }
        }

        protected virtual void Resize()
        {
            Utilities.Dispose(ref mBackBuffer);
            Utilities.Dispose(ref mRenderView);
            Utilities.Dispose(ref mDepthBuffer);
            Utilities.Dispose(ref mDepthView);

            mSwapChain.ResizeBuffers(mSwapChainDescription.BufferCount, 
                mRenderForm.ClientSize.Width, 
                mRenderForm.ClientSize.Height, 
                Format.Unknown, 
                SwapChainFlags.None);

            mBackBuffer = Texture2D.FromSwapChain<Texture2D>(mSwapChain, 0);
            mRenderView = new RenderTargetView(mDevice, mBackBuffer);
            mDepthBuffer = new Texture2D(mDevice, new Texture2DDescription()
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = mRenderForm.ClientSize.Width,
                Height = mRenderForm.ClientSize.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            mDepthView = new DepthStencilView(mDevice, mDepthBuffer);

            mDeviceContext.Rasterizer.SetViewport(new Viewport(0, 0, 
                                                    mRenderForm.ClientSize.Width,
                                                    mRenderForm.ClientSize.Height,
                                                    0.0f, 1.0f));
            mDeviceContext.OutputMerger.SetTargets(mDepthView, mRenderView);

            mProj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)mRenderForm.ClientSize.Width / (float)mRenderForm.ClientSize.Height, 0.1f, 1000.0f);

            mHasUserResized = false;
        }

        protected abstract void CustomRender();
    }
}
