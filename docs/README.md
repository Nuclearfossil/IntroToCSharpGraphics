# Documentation for the IntroToCSharpGraphics Github projects

# OpenGL - 2D and Legacy 3D (Fixed Function beginnings)

Welcome to the first part of 3D programming in C# using OpenGL (and OpenTK by extension).

In this series I'll talk a fair bit about some 3D math basics, but the real focus of this article series is to help you build and
understanding of the underlying tech and process that goes on under the hood to get pixels to the screen.

The structure of the `introductory` project is one source file `Program.cs` that you can swap out `ExampleNN` classes to see different
implementations of concepts.

# OpenGL 2D

A few things to note about OpenGL.
+ OpenGL works like a state machine. You set the state of something and it stays set until you change it.
+ There's a "Legacy", fixed function API that doesn't require shaders. We'll start there.
+ There's a new shader based version.
+ There's also a new, Vulkan set of extensions. Vulkan is a 'new' way of rendering, much closer to console style development.

So, where to start? Let's assume a few things.
+ You understand some basics regarding 3D spaces.
+ You have some understanding about texture formats and creating C# applications. + You understand C#, creating applications and windows development.
+ You may have worked with Maya, 3D Max or another DCC package.

Let's put some pixels (in the form of a triangle) onto the screen.

To do that, we first need to create the rendering device. OpenTK handles the majority of that for us. If you really want to see what goes on
at the truly lowest level (binding a window to an OpenGL context, preparing the rendering buffers, etc) there are a great number of C++
tutorials online and this topic is outside of the scope of this series.

All of the examples that I'll show are Console applications - they don't have a typical window that's generated (like a WPF app). OpenTK 
does all of that for you using a class called `GameWindow`. So, I've created a base class called `ExampleBase` that does the majority of the 
setup for you. It initializes the window (at 1024x768, 32 BPP with a 16 bit Depth/Z buffer).

	abstract class ExampleBase : GameWindow
    {
        public ExampleBase() : base(1024, 768, new OpenTK.Graphics.GraphicsMode(32, 16, 0, 0))
        { }

Just in case you aren't aware, let's do a quick review of some things here.

1. Once we create a window, the actual drawing area (what's called the canvas), is where we end up using as the drawable area. That area will have a colour depth, in this case it's set to 32 bits per pixel (2^32 number of colours can be displayed).
2. Next, we define a Depth, or Z-buffer. This allows us to determine the distance of a pixel from the camera. More on that in a bit.
3. We'll talk about the other numbers later.

Next, I've overloaded the `OnLoad` method to update the window title and then do this:

    GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
    
What this does is set the the colour that we use to clear the screen to a specific RGBA value (in this case, full black). This is part of that whole State Machine setup I mentioned earlier; once this value is set, it stays set until it's changed.

As part of the `GameWindow` class, there is a method that is called each frame called `OnRenderFrame`. I overload this method with a call to my own internal method `CustomRenderFrame` where each derived class can do it's own logic. Here's the code:

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        CustomRenderFrame(e.Time);

        base.SwapBuffers();
    }

Finally, we need some simple keyboard handling. So on the `OnUpdateFrame` override, we use the OpenTK library `Input` to get the state of the keyboard (or, alternatively, the mouse or joystick) to process input devices. In this case, we're just checking to see if the escape key has been pressed.

OK, so that's a rapid look at the basics. It's overly simplified, but you can easily research the rest to cover the gaps if need be. You can also ask questions and I can add more detail as need be.

## Lesson 01 - Example 01: Putting a pixel on the screen.

Now that we have the ability to build a window, the drawing area that is made available to us is in a 'normalized' view. The coordinate system of the window ranges from (-1,-1) to (1,1) with (0,0) being the the center of the window. We'll call this the 'Display Coordinate System'.

Let's experiment with that, with `Example01`. In this example, we want to draw one triangle onto the screen.  Why a triangle? That's because outside of lines and points, triangles are the base building blocks of 2D and 3D graphics. OpenGL does define other Primitive types (like quads), but they are collections of triangles.

In the class `Example01` we are going to draw a triangle in the window, using the Display coordinate system. To do this, we're going to use the OpenGL commands `Clear`, `Begin`, `Color` and `Vertex`.

First off, every frame we need to clear the screen (and any other buffer used in the rendering process). There's a couple of buffers that we can clear. We've already defined a 32 bit colour buffer (what we render to) as well as the Depth buffer. That's what the following call does:

    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

Next up, we begin the actual definition of the data to be rendered. We do this by first defining the primitive type we want to define. In this case, we want to define a set of triangles, or a single triangle (in this case). We do this like so:

    GL.Begin(BeginMode.Triangles);

So what we've done is define a state for the data that is coming down the pipe next. We continue feeding that data until we tell OpenGL that we're not sending any more data, with an `End` call.

    GL.End()

In between the `Begin` and `End` calls, that's when we send the triangular data to OpenGL. This is where the commands `Color` and `Vertex` come into play. So, if we do the following:

    { // Not necessary, just in place for clarity
        GL.Color3(1.0f, 0.0f, 0.0f);
        GL.Vertex2(0,0);

        GL.Color3(0.0f, 1.0f, 0.0f);
        GL.Vertex2(1, 0);

        GL.Color3(0.0f, 0.0f, 1.0f);
        GL.Vertex2(0, 1);
    }

I think it's fairly obvious what those commands are doing, but to be safe

`GL.Color3()` defines and RGB colour state that is to be used for every subsequent vertex fed into OpenGL via the `GL.Vertex2` call.  And thus we end up with the following output.

![Example01_01.png]({{site.baseurl}}/docs/Example01_01.png)


## Lesson 01 - Example 02: Getting an idea of sizing

With that simple little example out of the way, let's continue with something a little more - throwing a couple of triangles on the screen to see the real extents of the Display coordinates. Essentially, the rendering code looks like this:

    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    GL.Begin(BeginMode.Triangles);

    { // What happens when you change the vertex positions?
        GL.Color3(1.0f, 0.0f, 0.0f);
        GL.Vertex2(-0.5f, -0.5f);

        GL.Color3(0.0f, 0.0f, 1.0f);
        GL.Vertex2(-0.5f, 0.5f);

        GL.Color3(0.0f, 1.0f, 0.0f);
        GL.Vertex2(0.5f, 0.5f);

        GL.Color3(1.0f, 0.0f, 0.0f);
        GL.Vertex2(0.5f, 0.5f);

        GL.Color3(0.0f, 0.0f, 1.0f);
        GL.Vertex2(0.5f, -0.5f);

        GL.Color3(0.0f, 1.0f, 0.0f);
        GL.Vertex2(-0.5f, -0.5f);
    }

    GL.End();

From this, we end up with the following:

![Example02_01.png]({{site.baseurl}}/docs/Example02_01.png)


## Lesson 01 - Example 03: Blending triangles

Everyone knows what transparency is. Implementing it in OpenGL in the fixed function pipeline is fairly easy.  First off, we need to enable a specific feature in OpenGL - Blend. To do that we call

    GL.Enable(EnableCap.Blend)

That tells OpenGL to enable Blend mode. Now we need to determine what type of blending we're going to use. There's lots of differnt types of blend functions, but for now, let's start with

    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
    
What does that do? The OpenGL blend function is defined [here](https://www.opengl.org/sdk/docs/man/html/glBlendFunc.xhtml). What it tells us is that the first parameter defines the blend function for the source pixel (the triangle we're drawing) vs the second parameter (the destination pixel).

Given the current parameters, the source pixel's Alpha is used in the alpha test. Then the destination's alpha value is used (1-alpha). These are essentially scalars that are multiplied against the color values, both in the source and destination pixels and then the results are added together. I'll leave that as an exercise to the reader to work through the math.

## Lesson 01 - Example 04: Drawing without using the painter's algorithm

If you've been plaing around with rendering your own triangles, you may have noticed that we've only been using `Vector2` functions to draw triangles. We've been doing this because we only needed 2 dimensions worth of data. However, what would happen if we used `Vector3` values? If we go that route, we end up using World Coordinates. From a cartesian standpoint, we end up with the following:

![OpenGL_CoordinateSystem.png]({{site.baseurl}}/docs/OpenGL_CoordinateSystem.png)

Remember that the camera is looking down the Z axis. This means that from the camera perspective Z increments positively towards the camera and negatively away.

So, in our next example, we do the following:

    protected override void CustomRenderFrame(double delta)
    {
        GL.ClearDepth(1);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.Begin(BeginMode.Triangles);
        {
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(-0.75f, -0.75f, 0.5f);

            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(-0.75f, 0.75f, 0.5f);

            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(0.75f, 0.75f, 0.5f);

            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(0.0f, 0.0f, 0.75f);

            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(0.0f, 1.0f, 0.75f);

            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 0.0f, 0.75f);
        }
        GL.End();

    }

First, we clear our depth buffer (it gets cleared to 1). Then we draw two sets of data - one triangle at a Z depth of 0.5f and the other at a depth of 0.75f. And they look like they are displaying correctly (and, of course they are). But what happens if you change the order? Like so:

    GL.Begin(BeginMode.Triangles);
    {
            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(0.0f, 0.0f, 0.75f);

            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(0.0f, 1.0f, 0.75f);

            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 0.0f, 0.75f);

            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(-0.75f, -0.75f, 0.5f);

            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(-0.75f, 0.75f, 0.5f);

            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(0.75f, 0.75f, 0.5f);

    }
    GL.End();

We end up (hopefully) with different results as before. what we end up seeing is that the order in which we present the data to the renderer matters. This is commonly known as the 'Painters Algorithm'. We can't always assume that we can sort the triangles before sending them to OpenGL. So we use the Depth buffer to determine if the pixel to be rendered should be consumed or discarded.  In order to enable that, we need to enable the Depth test. That's fairly simple to do:

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Lequal);
    }

Adding this code and reverting your original changes should result in the triangles being rendered correctly.

When we get into more 'proper' 3D, we'll discuss how the depth buffer works in conjunction with the projection matrix.

## Lesson 01 - Example 05: Drawing using textures

This will cover a bit more content including:

+ How to load a texture and OpenGL's Texture functions
+ Mipmapping
+ UV coordinates

How do we load up a texture to feed into OpenGL? We can use the standard bitmap class to read in the data. The problem is, how do we pull the bits in the bitmap into OpenGL? We can access the bits of a bitmap with the `LockBits` method. This gives us a `BitmapData` that we can use to copy the image data into OpenGL.

Normally we keep a reference or pointer to reuse later. In OpenGL we use numerical IDs to identify resources. We'll create a TextureID using the `GL.GenTexture()` method. We then use the `GL.TexImage2D()` method to copy the image data into the Texture resource.

The last thing we want to do is set up our texture filtering and generate mipmaps. What are Mipmaps? It's a way to increase speed when you're using a textured triangle at a distance.

Think about it, when you are rending a texture where the pixels of the triangle are smaller than what they would be in the source image, we don't need all that extra data. So we would want to use a smaller version of the image. Which is what a mipmap functionally is. But not just one image, but a cascading pyramid of scaled textures.

See the [wikipedia](https://en.wikipedia.org/wiki/Mipmap) article for more information.

That's what my `LoadTexture()` and `LoadImage()` methods do. Once we have the texture loaded, we generate the mipmaps for that texture with the `GL.GenerateMpimaps()` method.

The last thing is that I've created a `ContentPipeline` class that holds a dictionary of strings and IDs. The goal here is to keep from re-creating the same texture resource - we simply key a resource to the filename.

Finally, when we want to draw a textured polygon we need to initialize OpenGL to use textures. In `Example05` I've updated the class to enable textures like so:

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        GL.Enable(EnableCap.Texture2D);

        // Load up any resources we need
        mSampleImageTextureID = ContentPipeline.LoadTexture("resources/SampleImage01.png");
    }

Now that we have a Texture ID in the `mSampleImageTextureID` variable, we can use that when rendering our next triangle. In order to render a textured triangle, instead of (or in addition to) using `GL.Color4()`, we use `GL.TexCoord2()` to define the texture coordinate for the vertex.

So what does that mean, Texture Coordinate lookup? Essentially for every Vertex that we have, we can look up a color value for it in a texture. Much like the colors that you saw in the previous examples, a linear look up is done on that texture as well.

So, this is the source image:

![Example05_01.png]({{site.baseurl}}/docs/Example05_01.png)


So, when we want to render triangles with a texture, we need to provide a lookup into the image. This is what the UV coordinate are used for. The coordinate set ranged from (0, 0) to (1, 1). That's the top left of the image and to the bottom right of the image.

So, if we draw a square (two triangles sharing an edge), like so:

    GL.ClearDepth(1);
    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    GL.BindTexture(TextureTarget.Texture2D, mSampleImageTextureID);

    GL.Begin(PrimitiveType.Triangles);
    {
        GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);

        GL.TexCoord2(0.0f, 0.0f);
        GL.Vertex2(-0.5f, 0.5f);

        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex2(0.5f, 0.5f);

        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex2(-0.5f, -0.5f);

        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex2(0.5f, 0.5f);

        GL.TexCoord2(1.0f, 1.0f);
        GL.Vertex2(0.5f, -0.5f);

        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex2(-0.5f, -0.5f);

    }
    GL.End();
    }

A couple of new commands there. `GL.BindTexture` takes the texture we had previously loaded as `mSampleImageTextureID`. This preps the OpenGL state machine to use the texture (and does all the underlying texture loading). Now, when we providing a vertex, we must first provide a UV coordinate. Remember, it's a state machine - so we have to set the UV coordinate first. That's what the `GL_TexCoord2()` method does. The other thing to note is that we also set the vertex color as well - `GL.Color4(1.0f, 1.0f, 1.0f, 1.0f)`. That's shared across all the vertices. Again, that comes from it being a state machine. That gives us the following:

![Example05_02.png]({{site.baseurl}}/docs/Example05_02.png)

Please notice that the texture hasn't maintained the aspect ratio of the source pixels. It's compressed to fit inside the rectangle.

Play around with the UV coordinates to see what happens when you change them.


# OpenGL 3D

OK, now we migrate away from 2D (although there's much more that can be covered) and into 3D. It starts getting more complex at this point, so we'll start with a simpler example (no texturing, just simple triangular objects).

Remeber from Lesson01 - Example 04, we described the coordinate system OpenGL uses for 3D. So let's say we want to draw a cube. What does the coordinate set look like? Well, we can draw it out on paper first.

![OpenGL3D_CubePrimitive.jpg]({{site.baseurl}}/docs/OpenGL3D_CubePrimitive.jpg)


Pretty straightforward, nothing crazy complex there. So, how do we do all that crazy 3D stuff?

## Lesson 01 - Example 06: Holy Moley 3D

In order to do 3d, I'm going to make a new base class called `ExampleBase3D`, derived from `ExampleBase`. And now we introduce a few new concepts.

### Face Culling

Face culling is all about performance. Drawing a triangle is expensive. Drawing over the same area on screen is expensive. So if we can reject a triangle from drawing, we can get a fair bit of a performance boost. So, how does culling work? It's based on the order in which the vertices of a triangle are described when it faces the viewer.  In our previous examples, the position of the viewer is, surprise, surprise, where you sit. Starting with the first vertex of the triangle, the order can either be clockwise or counter-clockwise. OpenGL is smart enough to be able to cull either way. Or not cull at all. So, in our overriden `OnLoad()` method we have the following:

    base.OnLoad(e);

    GL.Enable(EnableCap.CullFace);
    GL.CullFace(CullFaceMode.Back);
    GL.Enable(EnableCap.DepthTest);
    GL.Enable(EnableCap.DepthClamp);

    GL.DepthMask(true);

In order:

+ `GL.Enable(EnableCap.CullFace)` enabled triangle culling.
+ `GL.CullFace(CullFaceMode.Back)` enable Back-face culling.

The other bits we've already covered in other examples.

In the next section we need to talk about the transformation pipeline. We've already alluded to it in the past (when talking about 2D rendering) but now it's time to go into more detail. I love this site's explaination of it:

[Songho.ca](http://www.songho.ca/opengl/gl_transform.html)

![TransformPipeline]({{site.baseurl}}/docs/Example06_01.png)

Looking at the image, we have been dealing with Normalized Device Coordinates > Viewport Transform > Windows Coordinates.

We're dealing with a 3D object (a set of triangles) that needs to be projected onto a 2D surface (the monitor). We do that (and a few other things) through the use of a projection matrix. 

What does the projection matrix do? It does a couple of things. First, it defines the aspect ration of the 'window' that the 3D objects will be rendered to. Why is that important? Well, assume that we don't have a square window - that's very possible. If we don't into account the non-square-ness of the window, you can end up with stretch or squashed transformed triangles. Additionally, we need to take into account the near and far planes of the rendering 'Frustum' - the Frustum is a conical shape, a pyramid, with the top point cut off. Where the top is cut off is your monitor's screen. The bottom of the pyramid is the furthest that you will want to render. Additionally, you want to track the Field of View (vertical) for rendering. I may go into more detail on this in a later update if need be.

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, Width, Height);
        double aspectRatio = Width / (double)Height;
        float fov = 1.00899694f;
        float nearPlane = 1.0f;
        float farPlane = 100.0f;

        Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(fov, (float)aspectRatio, nearPlane, farPlane);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadMatrix(ref perspective);
    }

So what we have now is a matrix that we can pass all vertices throught to project them onto the screen from world space. If a triangle falls partially outside of the frustum, it's clipped. If the triangle is completely outside of the frustum, it's rejected. 

Also note that we only update the projection matrix when we resize the window; The projection matrix does not take into account the position of the camera. The reason for that is that the camera doesn't actually rotate; objects rotate into the camera's view. More on that later.

If you look at the `Camera` class, you'll see that it's fairly straight forward.

 - The constructor takes an eye point.
 - The Update rotates around the Y axis
 - We feed the rotated eye position into `Matrix4.LookAt` to build the modelview matrix. We then feed that into the appropriate matrix slot.


The only other thing to describe is the grid that we draw. This is nothing more than a set of lines, rendered with a bit of antialiasing `GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);` and it's own modelview matrix `GL.PushMatrix();`, `GL.Translate(dX - grid_size / 2, 0, dZ - grid_size / 2);` and `GL.PopMatrix();`. As always, with the OpenGL state machine, if we need to bracket each state: `GL.Enables` must have a matching `GL.Disable`, `GL.PushMatrix` must have a corresponding `GL.PopMatrix`.

## Summary

That's about it for this project. I'll update it as I come across missing bits, clarify more things that I've only superficially covered. However, this really was only meant as a refresher in Legacy OpenGL. The next project will go into more 'modern' OpenGL (shaders).

I've really glossed over a lot of details in this article (OpenGL matrices, Cameras). Rather than overtly ignore it, I've included links to, IMO, some great articles on those topics:

 - (OpenGL Matrices)[http://www.opengl-tutorial.org/beginners-tutorials/tutorial-3-matrices/]
 - (Transformations)[https://open.gl/transformations]
 - (OpenGL FAQ)[https://www.opengl.org/archives/resources/faq/technical/transformations.htm]
 
 ## Todo
  [ ] an example of lighting?
  [ ] shadows?


# OpenGL - An introduction to a more modern approach

In the previous section, we discussed the legacy OpenGL programming model (more commonly refered to as the 'Fixed Function Pipeline'). In this next section, we're going to investigate a more modern approach, that utilizes shaders, Vertex buffers, Index buffers and how to send data to shaders from your program.

## To start - a more proper framework

In the Introduction project, a lot of things were put together hapdash. I've corrected that in the `ShaderIntroduction` project:

![Initial Class Layout]({{site.baseurl}}/docs/InitialClassLayout.png)

A fair bit has changed with this codebase. I've been a bit more regimented in how these classes were defined. They're also a bit more generalized and properly reusable. Some things to note:

 1. The `ExampleBase` class Contains a `Camera` by default. It is never Updated (I may change this) so it is up to the derived class to update the camera. 
 2. The `Camera` class is a fairly rough FPS-style camera. I will be cleaning this up as we go. However, the `Camera` class also maintains the Projection matrix, as well as the View matrix. Traditionally, cameras maintain that information.
 3. `TextureManager` is extracted into it's own class. This will not be used in this lesson (currently), but is going to be used in later lessons.
 4. The same goes for the `RenderGrid` class.
 
In our previous codebase, we were using a more 'legacy' based version of OpenGL; this is the fixed function version of OpenGL that does not support shaders.  In this example, we're going to start talking about using proper shaders. We're not going to touch on Texturing in this introduction. What we are going to cover, however consists of:

 1. Vertex and Index buffers.
 2. Setting up data.
 3. Setting up and creating simple shaders.
 4. Sending data to shaders.
 5. Rendering.
 
This is a surprising amount of material to cover and our once-simple applications from the first lesson are going to become significantly more complex.

## What is a Vertex buffer?

In our Legacy pipeline (from lesson01), we defined geometry data through using a set of OpenGL state calls to

```
    GL.Begin(primitve)
       GL.Color()
       GL.VertexN()
    GL.End()
```

This if far from optimal, considering that for a large amount of data, it never changes. So we can defien a 'buffer' of data - in this case a buffer of Vertex data. That goes into what OpenGL calls a `Vertex Buffer Object` or `VBO`.  Wikipedia defines a VBO as

> A Vertex Buffer Object (VBO) is an OpenGL feature that provides methods for uploading vertex data (position, normal vector, color, etc.) to the video device for non-immediate-mode rendering. VBOs offer substantial performance gains over immediate mode rendering primarily because the data resides in the video device memory rather than the system memory and so it can be rendered directly by the video device. These are equivalent to vertex buffers in Direct3D.

However, in modern OpenGL, it is just a *buffer* of data. It has no inherent structure as far as OpenGL is concerned; you can put whatever the heck you want into it. It is up to a shader program to evaluate the contents of that buffer.

If there is no inherent structure to the data, then how does it work? Let's walk through a simple example (which is what we use in the current codebase). Let's assume that right now the only data we want to work with is triangle data. A triangle consists of three vertices: X, Y and Z. We also want to colourized the triangle, so each vertex will have a color component (R, G, B, A). So for each vertex element, we are going to have something that looks like this:

![VBO layout]({{site.baseurl}}/docs/VBOIntrospection01.png)

What we're saying here is that we're using 12 bytes (3 floats @ 4 bytes/float) to define the positional data of a vertex and 16 bytes (4 floats @ 4 bytes/float) for the color component. This results in a total of 28 bytes for the total data per vertex. This is called the `Stride` of the vertex data. What we have done here is created an `interleaved` data format. We end up with data that looks like this:

> [V][C][V][C][V][C][V][C] ...

The stride defines how far we have to jump to get to the next block of data (eg: once we've read the first vertex, to get at the next vertex, we have to jump N bytes to read the next vertex ... if we skip the color data).

We can also store the vertex attribute data in 'blocks', and thus not interleaving the data. Like so:

> [VVVV...][CCC...]

Using an interleaved format is probably the most performant, but there are always exceptions to the rule.  So why is it the most performant overall?  From Wikipedia:

> Interleaved data formats cause less GPU cache pressure, because the vertex coordinate and attributes of a single vertex aren't scattered all over in memory. They fit consecutively into few cache lines, whereas scattered attributes could cause more cache updates and therefore evictions. The worst case scenario could be one (attribute) element per cache line at a time because of distant memory locations, while vertices get pulled in a non-deterministic/non-contiguous manner, where possibly no prediction and prefetching kicks in. GPUs are very similar to CPUs in this matter.

It's also not just VBOs that you're interested in. You will also want to create an Index Buffer (OpenGL calls these Element Buffer Object or EBOs).  So, what is an EBO? What do we need these for?  Let's do a simple example to illustrate:

![EBO layout]({{site.baseurl}}/docs/EBOIntrospection01.png)

So, what we've done is essentially reduce the amount of data that we need to represent a mesh.  If we store the data as pure triangles, with duplication, we end up with `2 Triangles x 3 vertices x 3 floats(x,y,z) x 4 bytes/vertex` (no color information in this case) - 72 bytes (168 bytes if we include color). However, using an index buffer, we reduce that to `4 vertices x 3 floats x 4 bytes/vertex` - 48 bytes. Adding the index buffer into the mix is `6 indices x 4 bytes` - 24 bytes. But that's assuming 4 bytes/index. Which we really, REALLY don't need to use. Remember, a `uint` gives us a grand total of 4,294,967,295 / 3 (1,431,655,765 triangles). I fear the day we need almost 1.5 billion triangles for games. So we could use half that (a `ushort` for instance) to get 65,000 triangles.

However, just in case you think it isn't that much of a saving. Let's assume you have a 50,000 triangle mesh. And you have the standard 28 byes/vertex.

```
    50000 x 3 x 28 = 4,200,000 bytes
```

It's really hard to say what kind of loss we'd get because it really depends on the mesh geometry, but let's assume a fully connected mesh where each triangle adds only one additional vertex.


```
   ( 50000 + 3 ) * 28 = 1,400,084
   50000 * 3 indices * 2 bytes/index = 300,000
   Total: 1,700,084
```


The less data we need to transfer, the better.  The math may be off, but you can see a couple of additional improvements here as well - we don't have to duplicate vertices! Instead of an additional 3 floats to represent 1 vertex, we simply add 1 uint to point the an existing vertex in the VBO.

How do you go about creating a `VBO`? I've encapsulated the core functionality of a VBO in the `VertexBuffer` class. The process can be broken down into the following:

 1. Determine the layout of your vertex data. In this case, I've already done that.
 2. Create a data store to transiently hold the data. This is the data your application will track. You'll also promote this to OpenGL. In the case of our example, we create a `float` array that can hold all the data. If your data is not of a consistent data type (ie: you may use a mix of doubles, floats, even ints), you may want to use a struct array instead.
 3. Create a data store for your EBO.
 4. You'll need to have OpenGL create a VBO and EBO buffer for you. Like all buffers in OpenGL, a buffer is represented by an integer value. That ID is written into `mVertexBufferObjectID`.
 5. Populate the VBO and EBO with data.
 6. Do Render Stuff (Purposely left high level at this point).
 
## Generating VBOs and EBOs
 
First off, we need a place to store the data. in `VertexBuffer`, we have two members:
 
```
    protected float[] mVertexData;
    protected uint[] mIndexData;
```
 
This is just a way to temporarily hold the data until we can push it up to the graphics card. Populating this array is nothing fancy - check out `AddTriangle`, `AddVertex` and `AddIndex`.

So, to create a VBO and EBO:

```
    GL.GenBuffers( numBuffersToGenerate, buffersGenerated)
```

But all that does is create a buffer. Now that we have a buffer, we can start working with the data. Remember, OpenGL still works like a state machine, even though it's 'Modern' OpenGL. Now that we have a buffer ID (the Buffer ID is called a `buffer object name`), we can start working with it.

```
    GL.BindBuffer(bufferTargetType, bufferName)
    GL.BufferData(bufferTargetType, size, data, usage)
    GL.BindBuffer(bufferTargetType, bufferName)
```

What do we have now? If done right, we have a buffer for Vertex data and a buffer for index data.

How do we render this? That's where shaders come in.

## Vertex Shaders

I'm going to start off with the technical side of loading up a shader first. After that, more details on the shaders themselves and how we end up getting data into the shaders.

First off, we need some way of creating shaders. Instead of hard-coding it, we're going to load it from a text file. It's part of the project, so it gets copied into the output folder.

Loading the shaders is fairly straightforward, we use the `File.ReadAllText()` function to pull in all the text from the shader.

Now that we have the shader text, we need to compile the shader. Just like the VBOs and EBOs, we need to create a Shader Named object.


```
    VertexShaderID = GL.CreateShader(ShaderType.VertexShader);
    FragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);
```

Now we have shader ID to work with. It's time to compile the shader:


```
   GL.ShaderSource(VertexShaderID, VertexShaderSource);
   GL.CompileShader(VertexShaderID);
   GL.GetShaderInfoLog(VertexShaderID, out info);
   GL.GetShader(VertexShaderID, ShaderParameter.CompileStatus, out statusCode);

   if (statusCode != 1)
   {
      Console.Write(info);
      throw new ApplicationException(info);
   }
```

We have to compile two shaders, one for the vertex shader and one for the fragment shader. The shaders, working together constitutes a `Program`. So we need to create a program and attach shaders to it:

```
    Program = GL.CreateProgram();
    GL.AttachShader(Program, FragmentShaderID);
    GL.AttachShader(Program, VertexShaderID);

    GL.LinkProgram(Program);
    GL.UseProgram(Program);
```

Now that we have the shader program created and the shaders loaded, we can now render the data. However, we now need to talk about shaders.

### Vertex Shaders

Let's take a look at the shader itself.

```
    #version 400
    layout (location = 0) in vec3 vertex_position;
    layout (location = 1) in vec4 vertex_color;

    uniform mat4 mvp_matrix;

    out vec4 color;

    void main(void)
    {
        color = vertex_color;
        //ref line 124
        gl_Position = mvp_matrix * vec4(vertex_position, 1.0);
    }
```

OK, a couple of things to dig into here.  Aside from the GLSL version decorator (the `#version 400`), the next two lines define the data that we are expecting to get from the vertex buffer. So the first bit, the `layout (location = 0) in vec3 vertex_position;` reads like this

> `layout(qualifier1, qualifier2 = value, ...) variable definition`

What the layout qualifier does is allow the C# code (CPU code) to not have to directly bind the vertex attribute to the item in the stream (it's done with a call to `GL.GetAttributeLocation()`and `GL.BindAttrLocation()`). Otherwise, without those calls, the GLSL compiler assumes that the vertex layout matches what's defined in the shader - the first attribute in the vertex layout specification is the position, the second attribute in the vertex layout is the color (remember, it's a 0 based index).

The next line defines the Model-View-Projection matrix. This is sent in from the CPU side (C# code). The code looks like this:

```
    Matrix4 ModelViewProjection = ModelViewMatrix *
                                  WorldMatrix *
                                  ProjectionMatrix;

    GL.UseProgram(shaderProgram);
    int mvpLocation = GL.GetUniformLocation(mBasicShader.Program, "mvp_matrix");
    GL.UniformMatrix4(mvpLocation, false, ref ModelViewProjection);
    GL.UseProgram(0);
```

So we use a specifc shader program with `GL.UseProgram(ProgramID)` and disable it by calling the same method, but with a `0` arguement.

Sending data to the shader program, in this case the Model-View-projection matrix is done through two functions: the first one to get a variable ID from the program (in this case the `mvp_matrix` variable) through `GL.GetUniformLocation()`. Then, once we have the variable ID, we can set the data (in this case, it's a uniform matrix) through `GL.UniformMatrix4()`. Whatever we set in this variable is the value the shader will use.

At this point, it's all on the shader:

 - We define an output, that is used in the next shader stage. In this case, the `out vec4 color;`
 - We define the shader entry point, the `main` in C-language speak.
 - We set the output color that we just defined to the color passed in by the vertex buffer
 - we then modify the vertex postion (`vertex_position`) passed in on the vertex buffer why the MVP matrix and assign it to a variable called `gl_Position`.
 
Where did that `gl_Position` variable come from? It's actually defined as part of OpenGL's vertex, tesselation evaluation and geometry languages. It's part of a global instance of the `gl_PerVertex` named block. It is an output that receives the homogeneous vertex position; the position of the vertex in screen space. See [khronos.org](https://www.khronos.org/registry/OpenGL-Refpages/gl4/html/gl_Position.xhtml) for a deeper breakdown. In other words, the GLSL vertex shader doesn't output any data, but it does allow us to populate global data in each shader stage.

OK. What do I mean by 'each shader stage'. We've danced around this a bit. This shader we've just looked at? I've repeatedly called it the 'vertex shader'. That's arguably the first stage in the shading pipeline. However there's a lot more to it than that.

Personally, I love this diagram from (http://antongerdelan.net):

![OpenGL 4 Hardware Pipeline](http://antongerdelan.net/opengl/images/hwpipe2.png)

In all honesty, Anton's site does a fantastic job talking about the broad aspects of shaders. I'll try to do justice and dig a bit deeper into them and how we're using them.

## Fragment/Pixel shaders

As we've seen in the above diagarm, we've only touched on Vertex shaders. There's one last piece of the puzzle, and that's Pixel/Fragment shaders. I'll probably end up slipping and call them Pixel shaders more often than Fragment shaders, but I'm using them interchangibly.

If you look at our C# code, you'll notice that we don't do anything with sending data to Pixel shaders. That's because all the data we're sending right now is generated in the Vertex shader. In a later article, I'll introduce how we would do that.

So, the entire purpose of a pixel shader is, in all honesty, exactly that - how to shade pixels. Adding a color value to a pixel on the screen.

From the vertex shader, we saw that we set the color output value to that of the vertex color:

```
  color = vertex_color;
```

This color is fed into the pixel shader (interpolated across the triangle).

In the Pixel shader, we don't actually do a lot:

```
    #version 400

    layout (location = 0) out vec4 frag_color;

    in vec4 color;

    void main(void)
    {
        frag_color = color;
    }
```

It's incredibly simple - we set an output value bound to the pixel. In fragment shaders, you can think of this as the index of the output buffer that we are writing the pixel to. If we don't define one in out CPU code (C# code), and we weren't to explicitly add the `(location=0)` bit to our shader, if there is only ont `out` variable, the compiler will normally generate one for us. But don't do that.

Thus, we're only writing out the interpolated color from the fragment shader.  That's it. Nothing terribly fancy. In a later lesson, I'll go over more.

I think that's enough for this lesson. It's been a lot to write, as we're covering a lot of fundamentals.  The rest of the code is fairly self-explanitory.

Until next time!

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

The size of the constant buffer, in this example, is defined by the size of a Matrix class - which is the Model-View-projection matrix.

Also note, that in D3D, it's more common to refer to it as a World-View-Projection matrix, rather than Model-View-Projection. For the rest of this document we will use that notation.

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
} 
```

Breaking it down line by line, we're seeing the function signature for the Vertex shader defined as:

`PS_IN` is the output from the function (the definition is defined earlier), the name of the function, `VS` and the input arguments into the Vertex Shader `VS_IN input`.

In the body of the function, we define the output of the function as the variable `output` - which is of type `PS_IN`. and we initialize it all to 0.

the next two lines populate both the position and color of the output.

And, like all functions, we return a value, the `output` variable.

That's the Vertex Shader. Now, looking at the Pixel shader, it should be fairly obvious what each is doing (to a point).

The function `PS` returns a `float4` - that represents a color of the pixel that we're trying to render. The input is the result out of the `VS` function (ergo why it's called `PS_IN` - Pixel Shader INput).

So, that only leaves the `: SV_Target` bit on the pixel shader function definition.

`SV_Target` is a type of 'System-Value' semantics; it's defined as part of the HSLS compiler. When used in a pixel shader, it describes the pixel location.

To summarize, the vertex shader does nothing more than transform the vertex (and thus the triangle) into screen space and then interpolate the color passed in to the vertex shader across the triangle.

HLSL shader sematics can be found [here](https://goo.gl/3N8AnL).

# Building a better library
## The Shader Class
We want to simplify out coding process by creating several new classes that will allow us to more rapidly create applications. We'll start out by building out a shader class to work with.

We've created a new file, `Example02.cs`. We still load the same shader file (shaders\example01.fx) but that is now loaded into a `Shader` class: `mShader`.

`Shader` is and IDisposable, so make sure you dispose of it correctly when you use it.  There's a buch of things in there that we aren't going to be talking about just yet, but patience, we will get to them!

We have multiple `Load` methods. One expecting an 'fx' file, and one that separates out the Pixel and Vertex shaders. I prefer to use separate files for shaders, but that is totally optional. So going forward with other programs, I'll be using separate files.

The process for using a Shader class is this - create the textual version of the shader (either as a hard-coded string or, as in this method, load the program off disk), compile the shader, set the parameters on the shader, and then call `Apply` on the shader.

## The Cube class
I think we're all tired of seeing the same code copied and pasted for the sample cube we use, so I've generated a `Cube` class that encapsulates all this. Yes, it's just a copy/paste of the code that handles all the relevant data and functionality of preparing a cube for rendering. But it will also be the inspiration for other classes later on.

# Extending the Cube class
I expect that everyone is familiar with normals. Let's extend the cube class to contain normals in it's data. Looking at the class `CubeNormals`, you can now see that we have vectors that contain the additional normal data.

There's no special reason why the vertex layout is in [position, normal, color] arrangement. We could easily have set up the orientation in any format we choose. We do, however, have to make sure that the *stride* of the layout is set correctly. In this case, we've added one more `Vector4` into the mix, so our stride need to be updated appropriately. I think you can see that updating the stride manually like this is a pain. Once your data format is fairly concrete, you don't have to worry as much about changing the format. However, it is highly possible that different meshes may have different data requirements (binormals, multiple UV and Color sets ...) so you may want to take that into consideration with how you calculate your vertex stride.

We also keep the Input Layout of the vertex data separate from the Cube structure. I'm currently torn on that - it arguably makes more sense to bind it elsewhere; since it uses the VertexShaderSignature, perhaps it should reside in the shader itself? I may change that in a later revision of the `Shader` class.

Looking at the method for setting the parameters on the shader, you now can see that we're setting a few more data points - the last three, to be more specific. In this case, we're looking at a light direction, an ambient color and a diffuse color. This example shows us how to properly set a Lighting 'Constant buffer' in D3D. Following into the `Shader` class, you can see that we have a shader that contains a struct called `LightBuffer`. It contains 2 `Vector4` elements as well as a `Vector3` element and a final `float` called `padding`.

Why the `padding` element? And why the `[StructLayout(Layoutkind.Sequential)]` decorator? Hitting up the MSDN docs gives us a great hint as to why:
link: (https://msdn.microsoft.com/en-us/library/system.runtime.interopservices.layoutkind(v=vs.110).aspx)

> Controls the layout of an object when exported to unmanaged code.

Aha. But no, not really. It actually it more related to the attribute `LayoutKind.Sequential`

> The members of the object are laid out sequentially, in the order in which they appear when exported to unmanaged memory. The members are laid out according to the packing specified in StructLayoutAttribute.Pack, and can be noncontiguous.

We don't want noncontiguous when sending data to our GPU. OK, that's all fine and dandy, but why the `padding`?

Rules for packing constant buffers can be a little odd. Constant buffers are expected to align on a 16 byte boundary; the start of a variable in a constant buffer needs to be aligned on a 16 byte boundary. So we add a float into the mix to ensure that if we add another variable at the end of this struct, it comes after the padding and ensures that we're properly aligned.

The other way of solving this is to be more explicit in how we define our elements in the structure:

```
[StructLayout(LayoutKind.Explicit)]
public struct LightBuffer
{
    [FieldOffset(0)]
    public Vector4 ambientColor;

    [FieldOffset(16)]
    public Vector4 diffuseColor;

    [FieldOffset(32)]
    public Vector3 lightDirection;
} 
```

Both have their pros and cons. I'll leave it up to the reader to decide which is best for their needs.

Now that we have the layout of the constant buffer, we need a `BufferDescription` for that buffer:

```
BufferDescription lightBufferDesc = new BufferDescription()
{
    Usage = ResourceUsage.Dynamic,
    SizeInBytes = Utilities.SizeOf<LightBuffer>(),
    BindFlags = BindFlags.ConstantBuffer,
    CpuAccessFlags = CpuAccessFlags.Write,
    OptionFlags = ResourceOptionFlags.None,
    StructureByteStride = 0
};

mLightConstantBuffer = new D3DBuffer(device, lightBufferDesc);
```

and to put data into this buffer for the shader to use:

```
DataStream mappedResourceLight = default(DataStream);
device.ImmediateContext.MapSubresource(mLightConstantBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResourceLight);
mappedResourceLight.Write(lightBuffer);
device.ImmediateContext.UnmapSubresource(mLightConstantBuffer, 0);

device.ImmediateContext.PixelShader.SetConstantBuffer(0, mLightConstantBuffer);
```

# Textureing
Rendering stuff isn't very useful if you can't texture it. This next bit covers some fairly simple aspects of loading and using textures in D3D

First thing first, let's look at our new model class, `CubeTextureNormals`. From it's name, you can probably tell what it contains: positions, normals, and ... textures?

Well, not really, 'textures', but texture UV coordinates. Simply put, UV coordinate sets define where, in 'texture space' a vertex would fall on a texture. Texture space is defined between 0 and 1 and starts at the top left of a texture.

For example:

![UV coordinates]({{site.baseurl}}/docs/uv_coordinates.png)

For our needs, we are only using 2D coordinates (yes, you can have 3D textures). Applying a triangle onto the texture can be visualized like so:

![Triangle mapped to UV]({{site.baseurl}}/docs/triangle_uv_coordinates.png)

Points a, b and c would have coresponding UV values mappeed against the texture. Nothing crazy there. I've coded them by hand (you're welcome) but you'd want to use a proper 3D DCC package to do it right. There are also various projections you could use to automatically generate the UV coordinates onto regular shapes, but that's beyond the scope of this article.

Now that you have UV data for your content, we need to load some textures to use!  To that end, I've created a `Texture` class we can use. There's really nothing surprising here. SharpDX has access to the 'Windows Imaging Component'; a Microsoft API that gives us access to some fairly low level functionality for reading/writing image data. (link: https://msdn.microsoft.com/en-us/library/windows/desktop/ee719902.aspx). The big takeaway here is that WIC allows us to read pretty much any image data type.

Note: as an exercise to the reader, there is an intentional bug here. Where is it? What does it do? How do you fix it?

The takeaway from this class is that we want to provide to D3D a `ShaderResourceView` that maps a `Texture2D`. That resource is then fed into a 'Sampler' that can do many, many things to a texture. I won't go over all the grubby details of that, aside from the fact that allows for interpolation between pixels of an image, or combination of pixels in an image, depending upon how close/far from a triangle the viewer is.

You create a sampler using a `SamplerStateDescription` and a `SamplerState`. The `SamplerStateDescription` is just that, a description object that says how to create the `SamplerState`. From the MSDN, the `SamplerState`:

> Sampler state determines how texture data is sampled using texture addressing modes, filtering, and level of detail.  Sampling is done each time a texture pixel, or texel, is read from a texture. A texture contains an array of texels, or texture pixels. The position of each texel is denoted by (u,v), where u is the width and v is the height, and is mapped between 0 and 1 based on the texture width and height. The resulting texture coordinates are used to address a texel when sampling a texture.

link: (https://msdn.microsoft.com/en-us/library/ff604998.aspx)

I do want to go into much more detail on this, but Jay is looking to talk more about SamplerStates and different types of filtering. I'm going to park this topic here until he has a chance to talk about it.

Just like everything else we've done, we need to let the D3D pipeline know what's going on. We enable a sampler like so:

```
 mDeviceContext.PixelShader.SetSampler(0, mSampler);
```

And we update the shader to now take a `ShaderViewResource` that maps to our texture:

```
mShader.SetShaderParam(mDevice, new Vector3(0.0f, 5.0f, 5.0f), mTexture.TextureResource, new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new Vector4(0.1f, 0.1f, 0.1f, 1.0f), ref world, ref viewProj);
```

And in the shader, we take that `ShaderResourceView` and apply it to the pixel shader (the vertex shader doesn't use it at all)

```
public void SetShaderParam(D3DDevice device, Vector3 lightDirection, ShaderResourceView texture, Vector4 ambientColor, Vector4 diffuseColour, ref Matrix world, ref Matrix viewproj)
{
...
   device.ImmediateContext.PixelShader.SetShaderResource(0, texture);
}
```

## But that's not all!

One other thing that we do in `Example04` is break down the transformation matrices passed into the vertex shader. Previously we've been combining all the matrices into a world-view-projection matrix. However that may not be useful in the long run. When we have multiple objects, you may want to pre-computer the view-projection matrix, but the world matrix will change from object to objet (they all won't be transformed into world space - the GPU is better at doing that).

So, we have a `MatrixBuffer`, laid out very similarly to the LightBuffer!

```
[StructLayout(LayoutKind.Sequential)]
internal struct MatrixBuffer
{
    public Matrix world;
    public Matrix viewproj;
}
```

No padding necesary here.

Sending this across to the GPU is, again, fairly straightforward:

```
device.ImmediateContext.MapSubresource(mMatrixConstantBuffer, 
                                       MapMode.WriteDiscard, 
                                       MapFlags.None,
                                       out mMappedResourceMatrix);
mMappedResourceMatrix.Write(matrixBuffer);
device.ImmediateContext.UnmapSubresource(mMatrixConstantBuffer, 0);

device.ImmediateContext.VertexShader.SetConstantBuffer(1, mMatrixConstantBuffer);
```

However, we do have to have a bit of glue code on the shader side of things. The definition of that buffer in the vertex shader looks like so:

```
cbuffer MatrixBuffer : register(b1)
{
    matrix world;
    matrix viewproj;
}
```

The `register(b1)` tells the HLSL shader compiler what constant buffer register (or 'slot') to use for reading the data. In the C# side, that maps to the '1' in

```
device.ImmediateContext.VertexShader.SetConstantBuffer(1, mMatrixConstantBuffer);
```

The layout of the `register` keyword can be found here: (https://msdn.microsoft.com/en-us/library/windows/desktop/dd607359(v=vs.85).aspx)

Shader model reference link: (https://msdn.microsoft.com/en-us/library/windows/desktop/bb509638(v=vs.85).aspx)

# Something sort of resembling a framework (Example05.cs)
No, it's not *really* a framework now, but it's starting to look more an more like one. Yes, it's still another spinning cube, but the cube, this time, was generated in Maya, not by hand. Thus we now have a rudimentary `RenderMesh` class that holds a renderable object.

So, a `RenderableMesh` has a D3DBuffer for vertex data, only reads Triangles (no index buffer at this point), and also contains a `RenderMaterial` class instance.  For a lot of you, this is old hat, especially if you've worked in any 3D DCC, but a Material in this case defines color information that should be applied to a mesh. Typically that includes an ambient color, specular, diffuse ... textures ... it really depends on your shading model.  For this case, tho, we're going to use a fairly simple shading model; ambient color, diffuse color and a diffuse map.  In a later example, we're going to do much, much more complex shaders.

However, we need to be able to load data from an intermediate file format. In this case, .fbx (although other formats are supported). Are we going to write our own import library? Hell no. There's a fantastics Open Source library out there for both C++ and C# called Open Asset Import library: (https://github.com/assimp)

Yes, it's called AssImp. Shut up.

What I've done is created a static class called `MeshManager` that tracks loaded meshes (so you don't re-load an existing mesh) and provides a `RenderMesh` for you. It uses the AssImp library to do all the heavy lifting. What we do with that library is access all the vertex and material data and generate the `RenderMesh` and `RenderMaterial` from it. There are a few convenience functions in there (building a proper path to the assets, including textures), as well as some data validation.

But there is a lot more it can (and will) do. Loading an asset from an fbx, dae or other file format is not a quick process. It's fairly complex. Also, intermediate file formats are *large*. They don't have to be. Especially considering what we want as data. So the actual goal of this tool class will be to eventually generate a 'transform' of an intermediate data file into a highly compact and potentially streamable version of the data, in binary form.  But more on that later (much later).

To that end, I've also updated the Shader as well - the constant buffer for the `LightBuffer` hasn't changed, but we've broken the `MatrixBuffer` down even further to contain the workd, view and projection matrices. This is far from optimal, but there's a reason for this madness.

See, as I was building `Example05`, I was having a heck of a time getting the object transformation *just* right. So I broke the matrices down into their individual components so I could better debug them - make sure that what I was sending in was what I was expecting.

"But wait!" you say, with baited breath. "It's a shader. How can you debug shaders! In C#!".

Well, you don't debug them. In C#. You debug them in the graphics debugger. And in Windows 10. if you're on Windows 7, you may be SOL.

If you're in dev studio and start up the 'Graphics Debugger', you're in for some win! 

![Graphics Debugging]({{site.baseurl}}/docs/graphic_debugging_01.png)

And that starts up the graphics debugger! (BTW - totally available in the community edition of VS!)

![graphic_debugging_02.png]({{site.baseurl}}/docs/graphic_debugging_02.png)

Grabbing a sample of what's going on? Press the 'Capture Frame' button:

![graphic_debugging_03.png]({{site.baseurl}}/docs/graphic_debugging_03.png)

Once you've snagged some frames, you can now start debugging what's going on. There's a lot in here, and I won't cover it all here. But I'll hit the highlights.

Let's inspect a captured frame:

![graphic_debugging_04.png]({{site.baseurl}}/docs/graphic_debugging_04.png)

That opens up a whole other debugger!

![graphic_debugging_05.png]({{site.baseurl}}/docs/graphic_debugging_05.png)

In the 'Event List' window, you can see all the D3D calls that have been invoked.  Expand the 'Draw' item in it:

![graphic_debugging_06.png]({{site.baseurl}}/docs/graphic_debugging_06.png)

and then click on, say, the Input Layout item:

![graphic_debugging_07.png]({{site.baseurl}}/docs/graphic_debugging_07.png)

The arrows pointing to the two buttons? Those are the vertex and pixel shader debuggers. When you click on them, they simulate what happens in the appropriate shader. Go ahead and click on the vertex shader 'play' button.

![graphic_debugging_08.png]({{site.baseurl}}/docs/graphic_debugging_08.png)

That is an honest to god debugger for your vertex and pixel shader. You can set breakpoints, inspect variables. You can't change values to see what happens, but it is a great way to figure out what's where and what values are being processed.

![graphic_debugging_09.png]({{site.baseurl}}/docs/graphic_debugging_09.png)

You can also inspect and see what are in constant buffers

![graphic_debugging_10.png]({{site.baseurl}}/docs/graphic_debugging_10.png)

![graphic_debugging_11.png]({{site.baseurl}}/docs/graphic_debugging_11.png)

![graphic_debugging_12.png]({{site.baseurl}}/docs/graphic_debugging_12.png)

So, as you can see, we've actually got some decent debugging tools with D3D and Visual Studio, out of the box. We'll explore these tools more as we progress along our merry little way.

## A couple of notes
The shaders that we use here are fairly lightweight.  They're also unoptimized.

I don't mean, I haven't witten them optimally (I haven't, but that's because I'm being purposely verbose). I've disabled compiliation optimizations on the shaders so that the above debugger has better data for being able to debug them.

I've done this in the shader compiler:

```
public bool Load(D3DDevice device, string vsData, string psData)
{
    if (device == null || vsData == string.Empty || psData == string.Empty)
    {
        return false;
    }

    mVertexShaderResult = ShaderBytecode.Compile(vsData, "VS", VertexShaderVersion, ShaderFlags.Debug | ShaderFlags.SkipOptimization);
    mPixelShaderResult = ShaderBytecode.Compile(psData, "PS", PixelShaderVersion, ShaderFlags.Debug | ShaderFlags.SkipOptimization);

    return (mVertexShaderResult.ResultCode == Result.Ok) &&
            (mPixelShaderResult.ResultCode == Result.Ok);
}
```

Specifically, using the following flags:

`ShaderFlags.Debug | ShaderFlags.SkipOptimization`

In production code, you'll want to remove them or replace them with something more appropriate.

# And a camera to round out this mess
In `Example06` I finally introduce a camera class into the mix. It encapsulated the View and Projection matrices, exposes accessors for them and updates the camera based on input.

I'm cheating a fair bit here and using OpenTK's input library to get the caemra up and running. I will eventually move away from this and use DirectInput (or whatever D3D 11 is calling it these days).  This camera is essentially a near-verbatim copyu of the previous OpenGL camera I created, replacing all the matrix operations with the comparable D3D calls.

Also note that I'm using a Left Handed co-ordinate system. Who says you need to use a Right Handed co-ordinate system!

#Summary
That's pretty much it for an intro. We'll dig further into D3D in a future lesson, exploring lighting models and different material systems.  And with that, I'm out!
## Additional links

 - [DirectX 11 Website](https://goo.gl/5kHKFz)
