﻿using System;

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
            // ExampleBase example = new Example01();
            // ExampleBase example = new Example02();
            ExampleBase example = new Example03();

            example.Initialize();
            example.Run();
        }
    }
}
