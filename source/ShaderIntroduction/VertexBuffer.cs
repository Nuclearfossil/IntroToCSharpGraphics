using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

namespace ShaderIntroduction
{
    public class VertexBuffer
    {
        public int Stride { get; private set; }
        public int AttributeCount { get; private set; }
        public int TriangleCount { get { return mIndexData.Length / 3; } }
        public int VertexCount { get { return mVertexData.Length / AttributeCount; } }
        public bool IsLoaded { get; private set; }
        public BufferUsageHint UsageHint { get; set; }
        public BeginMode DrawMode { get; set; }

        public int VBO { get { return mVertexBufferObjectID; } }
        public int EBO { get { return mElementBufferObjectID; } }

        private int mVertexBufferObjectID;
        private int mElementBufferObjectID;

        private int mVertexPosition;
        private int mIndexPosition;

        protected float[] mVertexData;
        protected uint[] mIndexData;

        public VertexBuffer(int limit = 1024)
        {
            Stride = 28;
            AttributeCount = Stride / sizeof(float);

            UsageHint = BufferUsageHint.StreamDraw;
            DrawMode = BeginMode.Triangles;

            mVertexData = new float[limit * AttributeCount];
            mIndexData = new uint[limit];
        }

        public void Clear()
        {
            mVertexPosition = 0;
            mIndexPosition = 0;
        }

        public void LoadIntoMemory()
        {
            if (IsLoaded)
            {
                return;
            }

            GL.GenBuffers(1, out mVertexBufferObjectID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBufferObjectID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mVertexPosition * sizeof(float)), mVertexData, UsageHint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.GenBuffers(1, out mElementBufferObjectID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mElementBufferObjectID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mIndexPosition * sizeof(uint)), mIndexData, UsageHint);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            IsLoaded = true;
        }

        public void ReloadBuffer()
        {
            if (!IsLoaded)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBufferObjectID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mVertexPosition * sizeof(float)), mVertexData, UsageHint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mElementBufferObjectID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mIndexPosition * sizeof(uint)), mIndexData, UsageHint);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void UnloadBuffer()
        {
            if (!IsLoaded)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBufferObjectID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mVertexPosition * sizeof(float)), IntPtr.Zero, UsageHint);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mElementBufferObjectID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mIndexPosition * sizeof(uint)), IntPtr.Zero, UsageHint);

            GL.DeleteBuffers(1, ref mVertexBufferObjectID);
            GL.DeleteBuffers(1, ref mElementBufferObjectID);

            IsLoaded = false;
        }

        public void Render(Shader shader)
        {
            if (!IsLoaded)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBufferObjectID);

            GL.EnableVertexAttribArray(shader.PositionLocation);
            GL.EnableVertexAttribArray(shader.ColorLocation);
            GL.VertexAttribPointer(shader.PositionLocation, 2, VertexAttribPointerType.Float, false, Stride, 0);
            GL.VertexAttribPointer(shader.ColorLocation, 4, VertexAttribPointerType.Float, false, Stride, 8);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mElementBufferObjectID);
            GL.DrawElements(DrawMode, mIndexPosition, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);
        }

        public void Dispose()
        {
            UnloadBuffer();
            Clear();
            mVertexData = null;
            mIndexData = null;
        }

        public void AddIndex(uint indexA, uint indexB, uint indexC)
        {
            mIndexData[mIndexPosition++] = indexA;
            mIndexData[mIndexPosition++] = indexB;
            mIndexData[mIndexPosition++] = indexC;
        }

        public void AddVertex(float x, float y, float z, float r, float g, float b, float a)
        {
            mVertexData[mVertexPosition++] = x;
            mVertexData[mVertexPosition++] = y;
            mVertexData[mVertexPosition++] = z;
            mVertexData[mVertexPosition++] = r;
            mVertexData[mVertexPosition++] = g;
            mVertexData[mVertexPosition++] = b;
            mVertexData[mVertexPosition++] = a;
        }
    }
}
