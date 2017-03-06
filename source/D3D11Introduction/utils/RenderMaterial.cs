using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D3D11Introduction.utils
{
    public class RenderMaterial
    {
        public Vector4 Ambient { get; set; }
        public Vector4 Diffuse { get; set; }
        public Texture DiffuseMap { get; set; }
    }
}
