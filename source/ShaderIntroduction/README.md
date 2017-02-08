# OpenGL - An introduction to a more modern approach

In the previous section, we discussed the legacy OpenGL programming model (more commonly refered to as the 'Fixed Function Pipeline'). In this next section, we're going to investigate a more modern approach, that utilizes shaders, Vertex buffers, Index buffers and how to send data to shaders from your program.

## To start - a more proper framework

In the Introduction project, a lot of things were put together hapdash. I've corrected that in the `ShaderIntroduction` project:

![Initial Class layout](docresources\InitialClassLayout.png)

A fair bit has changed with this codebase. I've been a bit more regimented in how these classes were defined. They're also a bit more generalized and properly reusable. Some things to note:

 1 The `ExampleBase` class Contains a `Camera` by default. It is never Updated (I may change this) so it is up to the derived class to update the camera.
 2 The `Camera` class is a fairly rough FPS-style camera. I will be cleaning this up as we go. However, the `Camera` class also maintains the Projection matrix, as well as the View matrix. Traditionally, cameras maintain that information.
 3 `TextureManager` is extracted into it's own class.
 4 The same goes for the `RenderGrid` class.