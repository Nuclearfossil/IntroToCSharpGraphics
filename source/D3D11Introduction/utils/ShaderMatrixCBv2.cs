using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;

using D3DDevice = SharpDX.Direct3D11.Device;
using D3DBuffer = SharpDX.Direct3D11.Buffer;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace D3D11Introduction.utils
{
    public class ShaderMatrixCBv2 : IDisposable
    {
        #region public members
        public ShaderSignature VertexShaderSignature
        {
            get;
            protected set;
        }
        #endregion

        #region protected members
        protected VertexShader mVertexShader = null;
        protected PixelShader mPixelShader = null;
        protected D3DBuffer mWVPConstantBuffer = null;
        protected D3DBuffer mLightConstantBuffer = null;
        protected D3DBuffer mMatrixConstantBuffer = null;
        #endregion

        #region private members
        private CompilationResult mVertexShaderResult;
        private CompilationResult mPixelShaderResult;
        private DataStream mMappedResourceMatrix;
        private DataStream mMappedResourceLight;
        #endregion

        [StructLayout(LayoutKind.Sequential)]
        internal struct MatrixBuffer
        {
            public Matrix world;
            public Matrix view;
            public Matrix proj;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LightBuffer
        {
            public Vector4 ambientColor;
            public Vector4 diffuseColor;
            public Vector3 lightDirection;
            public float padding; // Added extra padding so structure is a multiple of 16.
        }

        public string VertexShaderVersion { get; set; }
        public string PixelShaderVersion { get; set; }

        public ShaderMatrixCBv2()
        {
            VertexShaderVersion = "vs_4_0";
            PixelShaderVersion = "ps_4_0";
        }

        public bool Load(D3DDevice device, string data)
        {
            if (data == null || data == string.Empty)
            {
                return false;
            }

            mVertexShaderResult = ShaderBytecode.Compile(data, "VS", VertexShaderVersion);
            mPixelShaderResult = ShaderBytecode.Compile(data, "PS", PixelShaderVersion);

            return (mVertexShaderResult.ResultCode == Result.Ok)  && 
                    (mPixelShaderResult.ResultCode == Result.Ok);
        }

        public bool Load(D3DDevice device, string vsData, string psData)
        {
            if (device == null || vsData == string.Empty || psData == string.Empty)
            {
                return false;
            }

            mVertexShaderResult = ShaderBytecode.Compile(vsData, "VS", VertexShaderVersion, ShaderFlags.Debug | ShaderFlags.SkipOptimization);
            mPixelShaderResult = ShaderBytecode.Compile(psData, "PS", PixelShaderVersion, ShaderFlags.Debug | ShaderFlags.SkipOptimization);

            return (mVertexShaderResult.ResultCode == Result.Ok) &&
                    (mPixelShaderResult.ResultCode == Result.Ok);
        }

        public bool Compile(D3DDevice device)
        {
            if (mVertexShaderResult.ResultCode == Result.Ok && 
                mPixelShaderResult.ResultCode == Result.Ok)
            {
                mVertexShader = new VertexShader(device, mVertexShaderResult);
                mPixelShader = new PixelShader(device, mPixelShaderResult);

                VertexShaderSignature = ShaderSignature.GetInputOutputSignature(mVertexShaderResult);

                mWVPConstantBuffer = new D3DBuffer(device,
                                                Utilities.SizeOf<Matrix>(),
                                                ResourceUsage.Default,
                                                BindFlags.ConstantBuffer,
                                                CpuAccessFlags.None,
                                                ResourceOptionFlags.None, 0);

                BufferDescription matrixBufferDesc = new BufferDescription()
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<MatrixBuffer>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                mMatrixConstantBuffer = new D3DBuffer(device, matrixBufferDesc);

                BufferDescription lightBufferDesc = new BufferDescription()
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<LightBuffer>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                mLightConstantBuffer = new D3DBuffer(device, lightBufferDesc);
            }

            return (mVertexShader != null) && 
                    (mPixelShader != null) && 
                    (VertexShaderSignature != null);
        }

        public void SetShaderParam(D3DDevice device, Vector3 lightDirection, ShaderResourceView texture, Vector4 ambientColor, Vector4 diffuseColour, ref Matrix world, Matrix view, Matrix proj)
        {
            LightBuffer lightBuffer = new LightBuffer()
            {
                ambientColor = ambientColor,
                diffuseColor = diffuseColour,
                lightDirection = lightDirection,
                padding = 0
            };

            MatrixBuffer matrixBuffer = new MatrixBuffer()
            {
                world = world,
                view = view,
                proj = proj
            };

            device.ImmediateContext.MapSubresource(mMatrixConstantBuffer, MapMode.WriteDiscard, MapFlags.None, out mMappedResourceMatrix);
            mMappedResourceMatrix.Write(matrixBuffer);
            device.ImmediateContext.UnmapSubresource(mMatrixConstantBuffer, 0);

            device.ImmediateContext.VertexShader.SetConstantBuffer(1, mMatrixConstantBuffer);

            device.ImmediateContext.MapSubresource(mLightConstantBuffer, MapMode.WriteDiscard, MapFlags.None, out mMappedResourceLight);
            mMappedResourceLight.Write(lightBuffer);
            device.ImmediateContext.UnmapSubresource(mLightConstantBuffer, 0);

            device.ImmediateContext.PixelShader.SetConstantBuffer(0, mLightConstantBuffer);

            device.ImmediateContext.PixelShader.SetShaderResource(0, texture);
        }

        public bool Apply(D3DDevice device)
        {
            bool result = false;

            if ((mVertexShader != null) &&
                (mPixelShader != null))
            {
                device.ImmediateContext.VertexShader.Set(mVertexShader);
                device.ImmediateContext.PixelShader.Set(mPixelShader);
                result = true;
            }
            return result;
        }

        public void Dispose()
        {
            mVertexShaderResult?.Dispose();
            mPixelShaderResult?.Dispose();

            mVertexShader?.Dispose();
            mPixelShader?.Dispose();

            mWVPConstantBuffer?.Dispose();
            mLightConstantBuffer?.Dispose();
        }

    }
}
