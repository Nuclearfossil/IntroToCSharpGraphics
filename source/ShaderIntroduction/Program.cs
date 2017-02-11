using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace ShaderIntroduction
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Example01 currentGame = new Example01())
            {
                currentGame.Run(30);
            }
        }
    }
}
