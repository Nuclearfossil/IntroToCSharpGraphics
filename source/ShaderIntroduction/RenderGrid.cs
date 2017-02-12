using System;
using System.Drawing;

using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace ShaderIntroduction
{
    public class RenderGrid
    {
        public int[] Delta { get; set; }
        public Color GridColor { get; set; }
        public int Ratio;
        public int CellSize;
        public int GridSize;

        private VertexBuffer mGrid;
        private Shader mShader;

        public RenderGrid()
        {
            Delta = new int[]{ 0, 0 };
        }

        public void Initialize(Color color, float x, float z, int cellSize, int gridSize)
        {
            GridColor = color;
            CellSize = cellSize;
            GridSize = gridSize;

            Delta[0] = (int)Math.Round(x / CellSize) * CellSize;
            Delta[1] = (int)Math.Round(z / CellSize) * CellSize;

            Ratio = GridSize / CellSize;

            mGrid = new VertexBuffer((Ratio + 1) * 4);
            mShader = new Shader(@"shaders/vertex.glsl", @"shaders/fragment.glsl");

            mGrid.DrawMode = BeginMode.Lines;

            uint index = 0;
            for (int i = 0; i < Ratio + 1; i++)
            {
                int current = i * cellSize;
            
                // X line
                mGrid.AddVertex((float)current, 0.0f, 0.0f, color.R, color.G, color.B, color.A);
                mGrid.AddIndex(index++);
                mGrid.AddVertex((float)current, 0.0f, (float)GridSize, color.R, color.G, color.B, color.A);
                mGrid.AddIndex(index++);
            
                // Z Line
                mGrid.AddVertex(0.0f, 0.0f, (float)current, color.R, color.G, color.B, color.A);
                mGrid.AddIndex(index++);
                mGrid.AddVertex((float)GridSize, 0.0f, (float)current, color.R, color.G, color.B, color.A);
                mGrid.AddIndex(index++);
            }

            mGrid.LoadIntoMemory();
        }

        public void Render(ref Matrix4 projection, ref Matrix4 world)
        {
            var ModelViewMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
            Matrix4 ModelViewProjection = ModelViewMatrix * world * projection;

            GL.UseProgram(mShader.Program);
            int mvpLocation = GL.GetUniformLocation(mShader.Program, "mvp_matrix");
            GL.UniformMatrix4(mvpLocation, false, ref ModelViewProjection);
            mGrid.Render(mShader);
            GL.UseProgram(0);
        }
    }
}
