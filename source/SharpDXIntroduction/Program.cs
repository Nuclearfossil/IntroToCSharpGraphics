using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using System;

using Buffer11 = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace SharpDXIntroduction
{
    public class ExampleBase
    {
        public RenderForm RenderContext { get; private set; }
        
        protected Color4 FillColor { get; set; }

        public virtual void Run()
        {
            if (!SharpDevice.IsDirectX11Supported())
            {
                System.Windows.Forms.MessageBox.Show("DirectX11 Not Supported");
                return;
            }

            //render form
            RenderContext = new RenderForm();
            RenderContext.Text = "Tutorial 2: Init Device (press key from 1 to 8)";

            //background color
            FillColor = Color.CornflowerBlue;
        }
    }

    public class Example01 : ExampleBase
    {
        public override void Run()
        {
            base.Run();

            //keydown event
            RenderContext.KeyDown += (sender, e) =>
            {
                switch (e.KeyCode)
                {
                    case System.Windows.Forms.Keys.D1:

                        FillColor = Color.CornflowerBlue;
                        break;
                    case System.Windows.Forms.Keys.D2:
                        FillColor = Color.Red;
                        break;
                    case System.Windows.Forms.Keys.D3:
                        FillColor = Color.Blue;
                        break;
                    case System.Windows.Forms.Keys.D4:
                        FillColor = Color.Orange;
                        break;
                    case System.Windows.Forms.Keys.D5:
                        FillColor = Color.Yellow;
                        break;
                    case System.Windows.Forms.Keys.D6:
                        FillColor = Color.Olive;
                        break;
                    case System.Windows.Forms.Keys.D7:
                        FillColor = Color.Orchid;
                        break;
                    case System.Windows.Forms.Keys.D8:
                        FillColor = Color.Black;
                        break;
                }
            };

            //main loop
            using (SharpDevice device = new SharpDevice(RenderContext))
            {
                RenderLoop.Run(RenderContext, () =>
                {
                    //resize if form was resized
                    if (device.MustResize)
                    {
                        device.Resize();
                    }

                    //clear color
                    device.Clear(FillColor);

                    //present
                    device.Present();
                });
            }

        }
    }

    public class Example02 : ExampleBase
    {
        public override void Run()
        {
            base.Run();

            //Indices
            int[] indices = new int[]
            {
                0,1,2,0,2,3,
                4,6,5,4,7,6,
                8,9,10,8,10,11,
                12,14,13,12,15,14,
                16,18,17,16,19,18,
                20,21,22,20,22,23
            };


            //Vertices
            ColoredVertex[] vertices = new[]
            {
                ////TOP
                new ColoredVertex(new Vector3(-5,5,5),new Vector4(0,1,0,0)),
                new ColoredVertex(new Vector3(5,5,5),new Vector4(0,1,0,0)),
                new ColoredVertex(new Vector3(5,5,-5),new Vector4(0,1,0,0)),
                new ColoredVertex(new Vector3(-5,5,-5),new Vector4(0,1,0,0)),
                //BOTTOM
                new ColoredVertex(new Vector3(-5,-5,5),new Vector4(1,0,1,1)),
                new ColoredVertex(new Vector3(5,-5,5),new Vector4(1,0,1,1)),
                new ColoredVertex(new Vector3(5,-5,-5),new Vector4(1,0,1,1)),
                new ColoredVertex(new Vector3(-5,-5,-5),new Vector4(1,0,1,1)),
                //LEFT
                new ColoredVertex(new Vector3(-5,-5,5),new Vector4(1,0,0,1)),
                new ColoredVertex(new Vector3(-5,5,5),new Vector4(1,0,0,1)),
                new ColoredVertex(new Vector3(-5,5,-5),new Vector4(1,0,0,1)),
                new ColoredVertex(new Vector3(-5,-5,-5),new Vector4(1,0,0,1)),
                //RIGHT
                new ColoredVertex(new Vector3(5,-5,5),new Vector4(1,1,0,1)),
                new ColoredVertex(new Vector3(5,5,5),new Vector4(1,1,0,1)),
                new ColoredVertex(new Vector3(5,5,-5),new Vector4(1,1,0,1)),
                new ColoredVertex(new Vector3(5,-5,-5),new Vector4(1,1,0,1)),
                //FRONT
                new ColoredVertex(new Vector3(-5,5,5),new Vector4(0,1,1,1)),
                new ColoredVertex(new Vector3(5,5,5),new Vector4(0,1,1,1)),
                new ColoredVertex(new Vector3(5,-5,5),new Vector4(0,1,1,1)),
                new ColoredVertex(new Vector3(-5,-5,5),new Vector4(0,1,1,1)),
                //BACK
                new ColoredVertex(new Vector3(-5,5,-5),new Vector4(0,0,1,1)),
                new ColoredVertex(new Vector3(5,5,-5),new Vector4(0,0,1,1)),
                new ColoredVertex(new Vector3(5,-5,-5),new Vector4(0,0,1,1)),
                new ColoredVertex(new Vector3(-5,-5,-5),new Vector4(0,0,1,1))
                };

            using (SharpDevice device = new SharpDevice(RenderContext))
            {
                //Init Mesh
                SharpMesh mesh = SharpMesh.Create<ColoredVertex>(device, vertices, indices);

                //Create Shader From File and Create Input Layout
                SharpShader shader = new SharpShader(device, "../../HLSL.txt",
                    new SharpShaderDescription() { VertexShaderFunction = "VS", PixelShaderFunction = "PS" },
                    new InputElement[] {
                        new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0)
                    });

                //create constant buffer
                Buffer11 buffer = shader.CreateBuffer<Matrix>();

                //main loop
                RenderLoop.Run(RenderContext, () =>
                {
                    //Resizing
                    if (device.MustResize)
                    {
                        device.Resize();
                    }

                    //apply states
                    device.UpdateAllStates();

                    //clear color
                    device.Clear(Color.CornflowerBlue);

                    //Set matrices
                    float ratio = (float)RenderContext.ClientRectangle.Width / (float)RenderContext.ClientRectangle.Height;
                    Matrix projection = Matrix.PerspectiveFovLH(3.14F / 3.0F, ratio, 1, 1000);
                    Matrix view = Matrix.LookAtLH(new Vector3(0, 10, -50), new Vector3(), Vector3.UnitY);
                    Matrix world = Matrix.RotationY(Environment.TickCount / 1000.0F);
                    Matrix WVP = world * view * projection;

                    //update constant buffer
                    device.UpdateData<Matrix>(buffer, WVP);

                    //pass constant buffer to shader
                    device.DeviceContext.VertexShader.SetConstantBuffer(0, buffer);

                    //apply shader
                    shader.Apply();

                    //draw mesh
                    mesh.Draw();

                    //begin drawing text
                    device.Font.Begin();

                    //flush text to view
                    device.Font.End();
                    //present
                    device.Present();
                });

                //release resources
                mesh.Dispose();
                buffer.Dispose();
            }

        }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            //ExampleBase example = new Example01();
            ExampleBase example = new Example02();

            example.Run();
        }
    }
}
