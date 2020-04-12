#version 330

in  vec3 vPosition;
in vec2 texcoord;
out vec2 f_texcoord;

uniform mat4 modelView;

void
main()
{
    gl_Position = modelView * vec4(vPosition, 1.0);
    f_texcoord = texcoord;
}
