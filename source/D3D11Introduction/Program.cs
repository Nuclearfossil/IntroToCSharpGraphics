using System;

namespace D3D11Introduction
{
    internal static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ExampleBase example = new Example01();

            example.Initialize();
            example.Run();
        }
    }
}
