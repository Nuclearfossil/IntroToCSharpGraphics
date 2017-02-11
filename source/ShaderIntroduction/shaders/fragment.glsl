#version 400

layout (location = 0) out vec4 frag_color;

in vec4 color;

void main(void)
{
    frag_color = color;
}
