using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

using SharpDX.DXGI;

using System;
using System.Runtime.InteropServices;

using D3DBuffer = SharpDX.Direct3D11.Buffer;
using D3DDevice = SharpDX.Direct3D11.Device;

namespace D3D11Introduction.utils
{
    public class CubeTexturedNormals : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MeshFormat
        {
            public Vector4 position;
            public Vector4 normal;
            public Vector4 color;
            public Vector2 UV;
        }

        #region Protected Members
        protected D3DBuffer mVerticesBuffer = null;
        protected int mStride = 0;
        protected MeshFormat[] mVertices = null;
        #endregion


        public CubeTexturedNormals(D3DDevice device)
        {
            mVertices = new MeshFormat[36];
            // Postion (vector4), Normal (vector4), Color (vector4), UV (vector2)
            mVertices[0].position = new Vector4(-1.0f, -1.0f, -1.0f, 1.0f); mVertices[0].normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f); mVertices[0].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[0].UV = new Vector2(0.0f, 1.0f); // FRONT
            mVertices[1].position = new Vector4(-1.0f, 1.0f, -1.0f, 1.0f);  mVertices[1].normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f); mVertices[1].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[1].UV = new Vector2(0.0f, 0.0f);
            mVertices[2].position = new Vector4(1.0f, 1.0f, -1.0f, 1.0f);   mVertices[2].normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f); mVertices[2].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[2].UV = new Vector2(1.0f, 0.0f);
            mVertices[3].position = new Vector4(-1.0f, -1.0f, -1.0f, 1.0f); mVertices[3].normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f); mVertices[3].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[3].UV = new Vector2(0.0f, 1.0f);
            mVertices[4].position = new Vector4(1.0f, 1.0f, -1.0f, 1.0f);   mVertices[4].normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f); mVertices[4].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[4].UV = new Vector2(1.0f, 0.0f);
            mVertices[5].position = new Vector4(1.0f, -1.0f, -1.0f, 1.0f);  mVertices[5].normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f); mVertices[5].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[5].UV = new Vector2(1.0f, 1.0f);

            mVertices[6].position = new Vector4(-1.0f, -1.0f, 1.0f, 1.0f); mVertices[6].normal = new Vector4(0.0f, 0.0f, -1.0f, 1.0f);  mVertices[6].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);  mVertices[6].UV =  new Vector2(0.0f, 1.0f); // BACK
            mVertices[7].position = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);   mVertices[7].normal = new Vector4(0.0f, 0.0f, -1.0f, 1.0f);  mVertices[7].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);  mVertices[7].UV =  new Vector2(1.0f, 0.0f);
            mVertices[8].position = new Vector4(-1.0f, 1.0f, 1.0f, 1.0f);  mVertices[8].normal = new Vector4(0.0f, 0.0f, -1.0f, 1.0f);  mVertices[8].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);  mVertices[8].UV =  new Vector2(0.0f, 0.0f);
            mVertices[9].position = new Vector4(-1.0f, -1.0f, 1.0f, 1.0f); mVertices[9].normal = new Vector4(0.0f, 0.0f, -1.0f, 1.0f);  mVertices[9].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);  mVertices[9].UV =  new Vector2(0.0f, 1.0f);
            mVertices[10].position = new Vector4(1.0f, -1.0f, 1.0f, 1.0f); mVertices[10].normal = new Vector4(0.0f, 0.0f, -1.0f, 1.0f); mVertices[10].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[10].UV = new Vector2(1.0f, 1.0f);
            mVertices[11].position = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);  mVertices[11].normal = new Vector4(0.0f, 0.0f, -1.0f, 1.0f); mVertices[11].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[11].UV = new Vector2(1.0f, 0.0f);

            mVertices[12].position = new Vector4(-1.0f, 1.0f, -1.0f, 1.0f); mVertices[12].normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f); mVertices[12].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[12].UV = new Vector2(0.0f, 1.0f); // Top
            mVertices[13].position = new Vector4(-1.0f, 1.0f, 1.0f, 1.0f);  mVertices[13].normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f); mVertices[13].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[13].UV = new Vector2(0.0f, 0.0f);
            mVertices[14].position = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);   mVertices[14].normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f); mVertices[14].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[14].UV = new Vector2(1.0f, 0.0f);
            mVertices[15].position = new Vector4(-1.0f, 1.0f, -1.0f, 1.0f); mVertices[15].normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f); mVertices[15].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[15].UV = new Vector2(0.0f, 1.0f);
            mVertices[16].position = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);   mVertices[16].normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f); mVertices[16].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[16].UV = new Vector2(1.0f, 0.0f);
            mVertices[17].position = new Vector4(1.0f, 1.0f, -1.0f, 1.0f);  mVertices[17].normal = new Vector4(0.0f, 0.0f, 1.0f, 1.0f); mVertices[17].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[17].UV = new Vector2(1.0f, 1.0f);

            mVertices[18].position = new Vector4(-1.0f, -1.0f, -1.0f, 1.0f); mVertices[18].normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f); mVertices[18].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[18].UV = new Vector2(0.0f, 1.0f); // Bottom
            mVertices[19].position = new Vector4(1.0f, -1.0f, 1.0f, 1.0f);   mVertices[19].normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f); mVertices[19].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[19].UV = new Vector2(1.0f, 0.0f);
            mVertices[20].position = new Vector4(-1.0f, -1.0f, 1.0f, 1.0f);  mVertices[20].normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f); mVertices[20].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[20].UV = new Vector2(0.0f, 0.0f);
            mVertices[21].position = new Vector4(-1.0f, -1.0f, -1.0f, 1.0f); mVertices[21].normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f); mVertices[21].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[21].UV = new Vector2(0.0f, 1.0f);
            mVertices[22].position = new Vector4(1.0f, -1.0f, -1.0f, 1.0f);  mVertices[22].normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f); mVertices[22].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[22].UV = new Vector2(1.0f, 1.0f);
            mVertices[23].position = new Vector4(1.0f, -1.0f, 1.0f, 1.0f);   mVertices[23].normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f); mVertices[23].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[23].UV = new Vector2(1.0f, 0.0f);

            mVertices[24].position = new Vector4(-1.0f, -1.0f, -1.0f, 1.0f); mVertices[24].normal = new Vector4(0.0f, 1.0f, 0.0f, 1.0f); mVertices[24].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[24].UV = new Vector2(0.0f, 1.0f); // Left
            mVertices[25].position = new Vector4(-1.0f, -1.0f, 1.0f, 1.0f);  mVertices[25].normal = new Vector4(0.0f, 1.0f, 0.0f, 1.0f); mVertices[25].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[25].UV = new Vector2(0.0f, 0.0f);
            mVertices[26].position = new Vector4(-1.0f, 1.0f, 1.0f, 1.0f);   mVertices[26].normal = new Vector4(0.0f, 1.0f, 0.0f, 1.0f); mVertices[26].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[26].UV = new Vector2(1.0f, 0.0f);
            mVertices[27].position = new Vector4(-1.0f, -1.0f, -1.0f, 1.0f); mVertices[27].normal = new Vector4(0.0f, 1.0f, 0.0f, 1.0f); mVertices[27].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[27].UV = new Vector2(0.0f, 1.0f);
            mVertices[28].position = new Vector4(-1.0f, 1.0f, 1.0f, 1.0f);   mVertices[28].normal = new Vector4(0.0f, 1.0f, 0.0f, 1.0f); mVertices[28].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[28].UV = new Vector2(1.0f, 0.0f);
            mVertices[29].position = new Vector4(-1.0f, 1.0f, -1.0f, 1.0f);  mVertices[29].normal = new Vector4(0.0f, 1.0f, 0.0f, 1.0f); mVertices[29].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[29].UV = new Vector2(1.0f, 1.0f);

            mVertices[30].position = new Vector4(1.0f, -1.0f, -1.0f, 1.0f); mVertices[30].normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f); mVertices[30].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[30].UV = new Vector2(0.0f, 1.0f); // Right
            mVertices[31].position = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);   mVertices[31].normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f); mVertices[31].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[31].UV = new Vector2(1.0f, 0.0f);
            mVertices[32].position = new Vector4(1.0f, -1.0f, 1.0f, 1.0f);  mVertices[32].normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f); mVertices[32].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[32].UV = new Vector2(0.0f, 0.0f);
            mVertices[33].position = new Vector4(1.0f, -1.0f, -1.0f, 1.0f); mVertices[33].normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f); mVertices[33].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[33].UV = new Vector2(0.0f, 1.0f);
            mVertices[34].position = new Vector4(1.0f, 1.0f, -1.0f, 1.0f);  mVertices[34].normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f); mVertices[34].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[34].UV = new Vector2(1.0f, 1.0f);
            mVertices[35].position = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);   mVertices[35].normal = new Vector4(0.0f, -1.0f, 0.0f, 1.0f); mVertices[35].color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f); mVertices[35].UV = new Vector2(1.0f, 0.0f);

            mVerticesBuffer = D3DBuffer.Create(device, BindFlags.VertexBuffer, mVertices);

            mStride = Utilities.SizeOf<MeshFormat>();
        }

        public void Draw(D3DDevice device)
        {
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(mVerticesBuffer, mStride, 0));

            device.ImmediateContext.Draw(36, 0);
        }

        public void Dispose()
        {
            mVerticesBuffer?.Dispose();
        }
    }
}
