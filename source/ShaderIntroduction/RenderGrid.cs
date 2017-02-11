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
        }

        public void Render()
        {
        }
    }
}
