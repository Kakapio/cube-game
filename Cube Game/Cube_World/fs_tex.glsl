#version 330

in vec2 f_texcoord;
out vec4 outputColor;

uniform sampler2D mainTexture;

void
main()
{
 vec2 flipped_texcoord = vec2(f_texcoord.x, 1.0 - f_texcoord.y);
    outputColor = texture(mainTexture, flipped_texcoord);
}