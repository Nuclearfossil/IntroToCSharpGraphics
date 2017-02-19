using System;
using System.Windows.Forms;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using SharpDX.Direct3D;
using System.Diagnostics;

using D3DBuffer = SharpDX.Direct3D11.Buffer;
using D3DDevice = SharpDX.Direct3D11.Device;
using D3DDriverType = SharpDX.Direct3D.DriverType;

namespace D3D11Introduction
{
    internal static class Program
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
                    ModeDescription = new ModeDescription(mRenderForm.ClientSize.Width, mRenderForm.ClientSize.Height,
                                                          new Rational(60, 1),
                                                          Format.R8G8B8A8_UNorm),
                    IsWindowed = true,
                    OutputHandle = mRenderForm.Handle,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = Usage.RenderTargetOutput
                };

                D3DDevice.CreateWithSwapChain(D3DDriverType.Hardware, DeviceCreationFlags.None, mSwapChainDescription, out mDevice, out mSwapChain);
                mDeviceContext = mDevice.ImmediateContext;

                mFactory = mSwapChain.GetParent<Factory>();
                mFactory.MakeWindowAssociation(mRenderForm.Handle, WindowAssociationFlags.IgnoreAll);

                mView = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
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

                mSwapChain.ResizeBuffers(mSwapChainDescription.BufferCount, mRenderForm.ClientSize.Width, mRenderForm.ClientSize.Height, Format.Unknown, SwapChainFlags.None);

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

                mDeviceContext.Rasterizer.SetViewport(new Viewport(0, 0, mRenderForm.ClientSize.Width, mRenderForm.ClientSize.Height, 0.0f, 1.0f));
                mDeviceContext.OutputMerger.SetTargets(mDepthView, mRenderView);

                mProj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)mRenderForm.ClientSize.Width / (float)mRenderForm.ClientSize.Height, 0.1f, 100.0f);

                mHasUserResized = false;
            }

            protected abstract void CustomRender();
        }

        public class Example01 : ExampleBase
        {
            #region Protected members
            protected ShaderSignature mSignature = null;
            protected InputLayout mLayout = null;
            protected D3DBuffer mVertices = null;
            protected D3DBuffer mConstantBuffer = null;
            protected CompilationResult mVertexShaderResult = null;
            protected CompilationResult mPixelShaderResult = null;
            protected VertexShader mVertexShader = null;
            protected PixelShader mPixelShader = null;
            protected Stopwatch mClock = null;
            #endregion

            protected override void CustomRender()
            {
                Matrix viewProj = Matrix.Multiply(mView, mProj);

                mDeviceContext.ClearDepthStencilView(mDepthView, DepthStencilClearFlags.Depth, 1.0f, 0);
                mDeviceContext.ClearRenderTargetView(mRenderView, Color.Black);

                float currentTime = mClock.ElapsedMilliseconds / 1000.0f;
                Matrix worldViewProj = Matrix.RotationX(currentTime) * Matrix.RotationY(currentTime * 2.0f) * Matrix.RotationZ(currentTime * 0.7f) * viewProj;
                worldViewProj.Transpose();
                mDeviceContext.UpdateSubresource(ref worldViewProj, mConstantBuffer);

                mDeviceContext.Draw(36, 0);
            }

            protected override void EndPhase1()
            {
                base.EndPhase1();

                mSignature.Dispose();
                mVertexShaderResult.Dispose();
                mVertexShader.Dispose();
                mPixelShaderResult.Dispose();
                mPixelShader.Dispose();
                mVertices.Dispose();
                mLayout.Dispose();
                mConstantBuffer.Dispose();
                mDepthBuffer.Dispose();
                mRenderView.Dispose();
                mBackBuffer.Dispose();
            }

            protected override void RunPhase1()
            {
                base.RunPhase1();

                mVertexShaderResult = ShaderBytecode.CompileFromFile("example01.fx", "VS", "vs_4_0");
                mVertexShader = new VertexShader(mDevice, mVertexShaderResult);

                mPixelShaderResult = ShaderBytecode.CompileFromFile("example01.fx", "PS", "ps_4_0");
                mPixelShader = new PixelShader(mDevice, mPixelShaderResult);

                mSignature = ShaderSignature.GetInputSignature(mVertexShaderResult);
                mLayout = new InputLayout(mDevice, mSignature, new[]
                                {
                                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                                    new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                                });

                mVertices = D3DBuffer.Create(mDevice, BindFlags.VertexBuffer, new[]
                                      {
                                          new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Front
                                          new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                          new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                          new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                          new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                          new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),

                                          new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f), // BACK
                                          new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                                          new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                                          new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                                          new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                                          new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),

                                          new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f), // Top
                                          new Vector4(-1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                                          new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                                          new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                                          new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                                          new Vector4( 1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),

                                          new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f), // Bottom
                                          new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                                          new Vector4(-1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                                          new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                                          new Vector4( 1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                                          new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),

                                          new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f), // Left
                                          new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                                          new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                                          new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                                          new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                                          new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),

                                          new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f), // Right
                                          new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                                          new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                                          new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                                          new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                                          new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                                });

                mConstantBuffer = new D3DBuffer(mDevice,
                                        Utilities.SizeOf<Matrix>(),
                                        ResourceUsage.Default,
                                        BindFlags.ConstantBuffer,
                                        CpuAccessFlags.None,
                                        ResourceOptionFlags.None, 0);

                mDeviceContext.InputAssembler.InputLayout = mLayout;
                mDeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                mDeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(mVertices, Utilities.SizeOf<Vector4>() * 2, 0));
                mDeviceContext.VertexShader.SetConstantBuffer(0, mConstantBuffer);
                mDeviceContext.VertexShader.Set(mVertexShader);
                mDeviceContext.PixelShader.Set(mPixelShader);

                mClock = new Stopwatch();
                mClock.Start();
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ExampleBase example = new Example01();

            example.Initialize();
            example.Run();
        }
    }
}
