# Direct3D - Introduction

In our past examples, we've looked at Fixed Function Pipelines and a quick look at the OpenGLb shader pipeline. This time around, we'll take a look at setting up and using DirectX 11.

SharpDX is the underlying library that we're using for DirectX, as C# doesn't have a native binding/library for accessing lower level graphics libraries.

DirectX isn't just a graphics library; it has support for Audio, Direct2D, Input, Fonts,  video, etc. However all we're looking at in this lesson is the graphics side of things.

SharpDX doesn't abstract out as much as OpenTK does, so we have a fair bit more setup to do in order to be able to just clear a screen. Let's take it in steps:

## Creating the rendering context

SharpDX provides a `RenderForm` class that handles all the basic windows setup and management functionality. 

To initialize D3D, we begin by creating a D3D 11 Device and a D3D Device context. These two interfaces are our abstraction to the video card. So how do we do that?

To create a D3D device, you need to define a swapchain. A swapchain defines:

 1. The number of buffers used in page flipping.
 2. How big the buffers should be.
 3. The format of the backbuffer (color depth).
 4. Refresh rate.
 5. The handle to the window
 6. Windowed/fullscreen

Creating a swapchain looks like this:

```
mSwapChainDescription = new SwapChainDescription()
{
    BufferCount = 1,
    ModeDescription = 
        new ModeDescription(
            mRenderForm.ClientSize.Width,
            mRenderForm.ClientSize.Height,
            new Rational(60, 1),
            Format.R8G8B8A8_UNorm),
    IsWindowed = true,
    OutputHandle = mRenderForm.Handle,
    SampleDescription = new SampleDescription(1, 0),
    Usage = Usage.RenderTargetOutput
};
```

Now that there is a definition of the swapchain, we need to build the D3D device. It's pretty simple:

```
D3DDevice.CreateWithSwapChain(D3DDriverType.Hardware,
    DeviceCreationFlags.None,
    mSwapChainDescription,
    out mDevice,
    out mSwapChain);
```

For reference: [Creating the D3D Device](https://goo.gl/hIOU7z)

Next up, we create a device context. From Microsoft:

> A device context contains the circumstance or setting in which a device is used. More specifically, a device context is used to set pipeline state and generate rendering commands using the resources owned by a device. Direct3D 11 implements two types of device contexts, one for immediate rendering and the other for deferred rendering.

For this example, we're going to be working with Immediate contexts.

We also need buffers for the Backbuffer, front buffer, depth buffer. We build the buffers when we resize the window (or are in fullscreen mode and want to change resolutions).

Each buffer has a specific way to be generated:

### BackBuffer is acquired from the Swap Chain

```
mBackBuffer = Texture2D.FromSwapChain<Texture2D>(mSwapChain, 0);
```

### Render Target is created based of the Back Buffer

```
mRenderView = new RenderTargetView(mDevice, mBackBuffer);
```

### The Depth buffer

```
mDepthBuffer = new Texture2D(mDevice, new Texture2DDescription()
{
    Format = Format.D32_Float_S8X24_UInt,
    ArraySize = 1,
    MipLevels = 1,
    Width = mRenderForm.ClientSize.Width,
    Height = mRenderForm.ClientSize.Height,
    SampleDescription = new SampleDescription(1, 0),
    Usage = ResourceUsage.Default,
    BindFlags = BindFlags.DepthStencil,
    CpuAccessFlags = CpuAccessFlags.None,
    OptionFlags = ResourceOptionFlags.None
});
```
### The Depth Stencil View buffer

```
    mDepthView = new DepthStencilView(mDevice, mDepthBuffer);
```    

Why do they call it a Render Target and Depth Stencil 'view'? Microsoft separates the concepts of resources (blocks of memory) and views. A view is just that, a 'view' into the data.

Now that all the buffers/views have been created, we need to finalize the binding:

```
mDeviceContext.Rasterizer.SetViewport(
    new Viewport(0, 0, 
        mRenderForm.ClientSize.Width,
        mRenderForm.ClientSize.Height,
        0.0f, 1.0f));
mDeviceContext.OutputMerger.SetTargets(mDepthView, mRenderView);
```

## Loading Shaders

Loading shaders in D3D is actually pretty easy:

```
mVertexShaderResult = ShaderBytecode.CompileFromFile("example01.fx", "VS", "vs_4_0");
mVertexShader = new VertexShader(mDevice, mVertexShaderResult);

mPixelShaderResult = ShaderBytecode.CompileFromFile("example01.fx", "PS", "ps_4_0");
mPixelShader = new PixelShader(mDevice, mPixelShaderResult);
```

The input and output of shaders are defined as part of a Shader 'Signature'. We define that like so:

```
mSignature = ShaderSignature.GetInputSignature(mVertexShaderResult);
```

Defining the vertex layout is a little different than what we've seen in OpenGL:

```
mLayout = new InputLayout(
    mDevice, 
    mSignature,
    new[]
    {
        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
    });
```

In the Shader file, this binds to the following:

```
struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
};
```

You can see how the two correlate to each other this way.

Each `InputElement` defines each element in the input layout - in this case, we're defining the Position and Color, the format of each position and color are 4 floats. In order, the arguments are:

 - The HLSL semantic associated with this element in a shader input-signature.
 - The semantic index for the element. A semantic index modifies a semantic, with an integer index number. A semantic index is only needed in a case where there is more than one element with the same semantic. For example, a 4x4 matrix would have four components each with the semantic name matrix, however each of the four component would have different semantic indices (0, 1, 2, and 3).
 - The data type of the element data.
 - Offset (in bytes) between each element. Use AppendAligned for convenience to define the current element directly after the previous one, including any packing if necessary.
 - An integer value that identifies the input-assembler. Valid values are between 0 and 15.


## Definint a Vertex Buffer

Again, pretty simple - Use `D3DBuffer` to create the data:

```
mVertices = D3DBuffer.Create(
    mDevice,
    BindFlags.VertexBuffer,
    new[]
    {
      new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), // Front Vertex
      new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Front Color

      new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), // Vertex
      new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Color

      ...

      });
```

## And a constant buffer

We talked about constant buffers in the OpenGL shader tutorial, but to reiterate, a Shader Constant is input from the CPU to a shader. For example, a shader constant for the Model-View-Projection matrix is defined through a constant buffer.

```
mConstantBuffer = new D3DBuffer(
                    mDevice,
                    Utilities.SizeOf<Matrix>(),
                    ResourceUsage.Default,
                    BindFlags.ConstantBuffer,
                    CpuAccessFlags.None,
                    ResourceOptionFlags.None,
                    0);
```

## Preppring for Rendering

Time to set up the Device context for rendering:

- Set the input layout
- Set the topology
- Set the Vertex buffer binding (tell the VB what to draw)
- Set the Constant buffer (the World-View-Projection Matrix) to the appropriate constant in the shader.
- Set the Vertex and Pixel shaders

```
mDeviceContext.InputAssembler.InputLayout = mLayout;
mDeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
mDeviceContext.InputAssembler.SetVertexBuffers(
    0, 
    new VertexBufferBinding(mVertices, Utilities.SizeOf<Vector4>() * 2, 0));
mDeviceContext.VertexShader.SetConstantBuffer(0, mConstantBuffer);
mDeviceContext.VertexShader.Set(mVertexShader);
mDeviceContext.PixelShader.Set(mPixelShader);
```

## Finally Rendering the data

Building matrices if fairly straightforward:

`Matrix viewProj = Matrix.Multiply(mView, mProj);`

Clearing buffers before rendering:

```
mDeviceContext.ClearDepthStencilView(
    mDepthView, 
    DepthStencilClearFlags.Depth, 
    1.0f, 
    0);
mDeviceContext.ClearRenderTargetView(mRenderView, Color.Black);
```

Updating the constant buffer with data:

```
mDeviceContext.UpdateSubresource(ref worldViewProj, mConstantBuffer);
```

And then to draw:

`mDeviceContext.Draw(36, 0);`

## And the shader

Shader semantics are fairly similar between GLSL and HLSL. The differences really come down to syntax, more than anything else. For the Vertex Shader, we've already seen the format for the input data (the position and the color per vertex) defined. We also see a definition for the input for the Pixel Shader defined like so:

```
struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
};
```

And then there's the constant in the shader file, defined for the World-View-Projection matrix:

`float4x4 worldViewProj;`

Now that we have the data defined, let's see a vertex shader in action:

```
PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;
	
	output.pos = mul(input.pos, worldViewProj);
	output.col = input.col;
	
	return output;
}```

Breaking it down line by line, we're seeing the function signature for the Vertex shader defined as:

`PS_IN` is the output from the function (the definition is defined earlier), the name of the function, `VS` and the input arguments into the Vertex Shader `VS_IN input`.

In the body of the function, we define the output of the function as the variable `output` - which is of type `PS_IN`. and we initialize it all to 0.

the next two lines populate both the position and color of the output.

And, like all functions, we return a value, the `output` variable.

That's the Vertex Shader. Now, looking at the Pixel shader, it should be fairly obvious what each is doing (to a point).

The function `PS` returns a `float4` - that represents a color of the pixel that we're trying to render. The input is the result out of the `VS` function (ergo why it's called `PS_IN` - Pixel Shader INput).

So, that only leaves the `: SV_Target` bit on the pixel shader function definition.

`SV_Target` is a type of 'System-Value' semantics; 
## Additional links

 - [DirectX 11 Website](https://goo.gl/5kHKFz)
