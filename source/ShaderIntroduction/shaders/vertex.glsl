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
