using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX.Direct3D11;
using SharpDX.WIC;
using D3DDevice = SharpDX.Direct3D11.Device;

namespace D3D11Introduction.utils
{
    public class Texture
    {
        public ShaderResourceView TextureResource { get; set; }

        public bool Initialize(D3DDevice device, string filename)
        {
            try
            {
                using (Texture2D texture = LoadFromFile(device, new SharpDX.WIC.ImagingFactory(), filename))
                {
                    ShaderResourceViewDescription desc = new ShaderResourceViewDescription()
                    {
                        Format = texture.Description.Format,
                        Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D
                    };
                    desc.Texture2D.MostDetailedMip = 0;
                    desc.Texture2D.MipLevels = -1;

                    TextureResource = new ShaderResourceView(device, texture, desc);
                    device.ImmediateContext.GenerateMips(TextureResource);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public void Shutdown()
        {
            TextureResource?.Dispose();
            TextureResource = null;
        }

        private Texture2D LoadFromFile(D3DDevice device, ImagingFactory imagingFactory, string filename)
        {
            using (var bitmap = LoadBitmap(imagingFactory, filename))
            {
                return CreateTexture2DFromBitmap(device, bitmap);
            }
        }

        private BitmapSource LoadBitmap(ImagingFactory factory, string filename)
        {
            BitmapDecoder decoder = new BitmapDecoder(factory,
                                                      filename,
                                                      DecodeOptions.CacheOnDemand);
            FormatConverter converter = new FormatConverter(factory);

            converter.Initialize(decoder.GetFrame(0),
                                PixelFormat.Format32bppPBGRA,
                                BitmapDitherType.None,
                                null,
                                0.0,
                                BitmapPaletteType.Custom);

            return converter;
        }

        private Texture2D CreateTexture2DFromBitmap(D3DDevice device, BitmapSource bitmapSource)
        {
            int stride = bitmapSource.Size.Width * 4;

            using (var buffer = new SharpDX.DataStream(bitmapSource.Size.Height * stride, true, true))
            {
                bitmapSource.CopyPixels(stride, buffer);
                return new Texture2D(device, new Texture2DDescription()
                {
                    Width = bitmapSource.Size.Width,
                    Height = bitmapSource.Size.Height,
                    ArraySize = 1,
                    BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                    Usage = SharpDX.Direct3D11.ResourceUsage.Default,
                    CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                    Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.GenerateMipMaps,
                    SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                }, new SharpDX.DataRectangle(buffer.DataPointer, stride));
            }
        }
    }
}
