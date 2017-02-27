using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using System.Diagnostics;

using D3DBuffer = SharpDX.Direct3D11.Buffer;

namespace D3D11Introduction
{
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

                mVertexShaderResult = ShaderBytecode.CompileFromFile("shaders\\example01.fx", "VS", "vs_4_0");
                mVertexShader = new VertexShader(mDevice, mVertexShaderResult);

                mPixelShaderResult = ShaderBytecode.CompileFromFile("shaders\\example01.fx", "PS", "ps_4_0");
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
}
