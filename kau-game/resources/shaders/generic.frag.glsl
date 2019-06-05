#version 330

uniform vec4 tintColor;
uniform sampler2D PrimaryTexture;

in vec2 TexCoord;
out vec4 FragColor;

void main() {
    FragColor = texture(PrimaryTexture, TexCoord) * tintColor;
}