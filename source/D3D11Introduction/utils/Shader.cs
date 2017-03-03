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
    public class Shader : IDisposable
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
        #endregion

        #region private members
        private CompilationResult mVertexShaderResult;
        private CompilationResult mPixelShaderResult;
        #endregion


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

        public Shader()
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

            mVertexShaderResult = ShaderBytecode.Compile(vsData, "VS", VertexShaderVersion);
            mPixelShaderResult = ShaderBytecode.Compile(psData, "PS", PixelShaderVersion);

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

        public void SetShaderParam(D3DDevice device, Matrix wvp, Vector3 lightDirection, Vector4 ambientColor, Vector4 diffuseColour)
        {
            LightBuffer lightBuffer = new LightBuffer()
            {
                ambientColor = ambientColor,
                diffuseColor = diffuseColour,
                lightDirection = lightDirection,
                padding = 0
            };

            device.ImmediateContext.UpdateSubresource(ref wvp, mWVPConstantBuffer);
            device.ImmediateContext.VertexShader.SetConstantBuffer(0, mWVPConstantBuffer);

            DataStream mappedResourceLight = default(DataStream);
            device.ImmediateContext.MapSubresource(mLightConstantBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResourceLight);
            mappedResourceLight.Write(lightBuffer);
            device.ImmediateContext.UnmapSubresource(mLightConstantBuffer, 0);

            device.ImmediateContext.PixelShader.SetConstantBuffer(0, mLightConstantBuffer);
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
