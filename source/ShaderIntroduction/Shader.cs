using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;

namespace ShaderIntroduction
{
    public class Shader
    {
        public string VertexShaderSource { get; private set; }
        public string FragmentShaderSource { get; private set; }

        public int VertexShaderID { get; private set; }
        public int FragmentShaderID { get; private set; }

        public int Program { get; private set; }

        public int PositionLocation { get; set; }
        public int NormalLLocation { get; set; }
        public int TexCoordinateLocation { get; set; }
        public int ColorLocation { get; set; }

        private const string kPostionLocation = "vertex_position";
        private const string kNormalLocation = "vertex_normal";
        private const string kTexCoordLocation = "vertex_texcoord";
        private const string kColorLocation = "vertex_color";


        public Shader(ref string vs, ref string fs)
        {
            VertexShaderSource = vs;
            FragmentShaderSource = fs;

            Build();
        }

        public Shader(string vsPath, string fsPath)
        {
            VertexShaderSource = File.ReadAllText(vsPath);
            FragmentShaderSource = File.ReadAllText(fsPath);

            Build();
        }

        private void Build()
        {
            int statusCode;
            string info;

            VertexShaderID = GL.CreateShader(ShaderType.VertexShader);
            FragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);

            // Compile each shader
            // Each shader is independant. You don't need to pair compilation of VS/FS together
            GL.ShaderSource(VertexShaderID, VertexShaderSource);
            GL.CompileShader(VertexShaderID);
            GL.GetShaderInfoLog(VertexShaderID, out info);
            GL.GetShader(VertexShaderID, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
            {
                Console.Write(info);
                throw new ApplicationException(info);
            }

            GL.ShaderSource(FragmentShaderID, FragmentShaderSource);
            GL.CompileShader(FragmentShaderID);
            GL.GetShaderInfoLog(FragmentShaderID, out info);
            GL.GetShader(FragmentShaderID, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
            {
                Console.Write(info);
                throw new ApplicationException(info);
            }

            Program = GL.CreateProgram();
            GL.AttachShader(Program, FragmentShaderID);
            GL.AttachShader(Program, VertexShaderID);

            GL.LinkProgram(Program);
            GL.UseProgram(Program);

            // Layout the structure of the data going in
            PositionLocation = GL.GetAttribLocation(Program, kPostionLocation);
            NormalLLocation = GL.GetAttribLocation(Program, kNormalLocation);
            TexCoordinateLocation = GL.GetAttribLocation(Program, kTexCoordLocation);
            ColorLocation = GL.GetAttribLocation(Program, kColorLocation);

            // Actual binding code
            if (PositionLocation >= 0)
            {
                GL.BindAttribLocation(Program, PositionLocation, kPostionLocation);
            }

            if (NormalLLocation >= 0)
            {
                GL.BindAttribLocation(Program, NormalLLocation, kColorLocation);
            }

            if (TexCoordinateLocation >= 0)
            {
                GL.BindAttribLocation(Program, TexCoordinateLocation, kTexCoordLocation);
            }

            if (ColorLocation >= 0)
            {
                GL.BindAttribLocation(Program, ColorLocation, kColorLocation);
            }

            GL.UseProgram(0);
        }

        public void Dispose()
        {
            if (Program != 0)
            {
                GL.DeleteProgram(Program);
            }

            if (FragmentShaderID != 0)
            {
                GL.DeleteShader(FragmentShaderID);
            }

            if (VertexShaderID != 0)
            {
                GL.DeleteShader(VertexShaderID);
            }
        }
    }
}
