using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;

using D3DBuffer = SharpDX.Direct3D11.Buffer;
using D3DDevice = SharpDX.Direct3D11.Device;

namespace D3D11Introduction.utils
{
    // Not supporting a RenderMesh with multiple components.
    // should be trivial to add - create a collection of D3DBuffers that you add to.
    public class RenderMesh : IDisposable
    {
        #region public structures
        public struct MeshFormat
        {
            public Vector4 position;
            public Vector4 normal;
            public Vector4 color;
            public Vector2 UV;
        }
        #endregion

        public RenderMaterial Material { get; set; }
        public int PrimitiveCount { get; private set; }

        #region protected members
        protected D3DBuffer mVerticesBuffer = null;
        protected int mStride = 0;
        #endregion

        public void AddMesh(D3DDevice device, MeshFormat[] data, int primitiveCount)
        {
            if (mVerticesBuffer != null)
            {
                return;
            }

            mVerticesBuffer = D3DBuffer.Create(device, BindFlags.VertexBuffer, data);

            mStride = Utilities.SizeOf<MeshFormat>();
            PrimitiveCount = primitiveCount;
        }

        public void Draw(D3DDevice device)
        {
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(mVerticesBuffer, mStride, 0));

            device.ImmediateContext.Draw(PrimitiveCount, 0);
        }

        public void Dispose()
        {
            mVerticesBuffer?.Dispose();
        }
    }
}
