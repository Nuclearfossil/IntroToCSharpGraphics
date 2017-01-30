# OpenGL - 2D and Legacy 3D (Fixed Function beginnings)

Welcome to the first part of 3D programming in C# using OpenGL (and OpenTK by extension).

In this series I'll talk a fair bit about some 3D math basics, but the real focus of this article series is to help you build and
understanding of the underlying tech and process that goes on under the hood to get pixels to the screen.

The structure of the `introductory` project is one source file `Program.cs` that you can swap out `ExampleNN` classes to see different
implementations of concepts.

# OpenGL

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

![Example01](docresources\Example01_01.png)

# Lesson01 - Example 02: Getting an idea of sizing

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

![Example02](docresources\Example02_01.png)

# Lesson01 - Example 03: Blending triangles

Everyone knows what transparency is. Implementing it in OpenGL in the fixed function pipeline is fairly easy.  First off, we need to enable a specific feature in OpenGL - Blend. To do that we call

    GL.Enable(EnableCap.Blend)

That tells OpenGL to enable Blend mode. Now we need to determine what type of blending we're going to use. There's lots of differnt types of blend functions, but for now, let's start with

    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
    
What does that do? The OpenGL blend function is defined [here](https://www.opengl.org/sdk/docs/man/html/glBlendFunc.xhtml). What it tells us is that the first parameter defines the blend function for the source pixel (the triangle we're drawing) vs the second parameter (the destination pixel).

Given the current parameters, the source pixel's Alpha is used in the alpha test. Then the destination's alpha value is used (1-alpha). These are essentially scalars that are multiplied against the color values, both in the source and destination pixels and then the results are added together. I'll leave that as an exercise to the reader to work through the math.

# Lesson01 - Example 04: Drawing without using the painter's algorithm

If you've been plaing around with rendering your own triangles, you may have noticed that we've only been using `Vector2` functions to draw triangles. We've been doing this because we only needed 2 dimensions worth of data. However, what would happen if we used `Vector3` values? If we go that route, we end up using World Coordinates. From a cartesian standpoint, we end up with the following:

![GLCoordinate](docresources\OpenGL_CoordinateSystem.png)

Remember that the camera is looking down the Z axis. This means that from the camera perspective Z increments positively towards the camera and negatively away. 
