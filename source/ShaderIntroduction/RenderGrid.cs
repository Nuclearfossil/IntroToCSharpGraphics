using System;
using System.Drawing;

using OpenTK.Graphics.OpenGL;
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
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.LineWidth(1.5f);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);

            GL.PushMatrix();

            GL.Translate(Delta[0] - GridSize / 2, 0, Delta[1] - GridSize / 2);

            GL.Color3(GridColor);
            GL.Begin(BeginMode.Lines);

            for (int i = 0; i < Ratio + 1; i++)
            {
                int currentLine = i * CellSize;

                GL.Vertex3(currentLine, 0, 0);
                GL.Vertex3(currentLine, 0, GridSize);

                GL.Vertex3(0, 0, currentLine);
                GL.Vertex3(GridSize, 0, currentLine);
            }

            GL.End();

            GL.PopMatrix();

            GL.Disable(EnableCap.LineSmooth);
            GL.Disable(EnableCap.Blend);

        }
    }
}
