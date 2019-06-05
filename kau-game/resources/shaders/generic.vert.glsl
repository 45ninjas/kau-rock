#version 330

// aPosition is the 1st VertexAttrib of each stride
layout(location = 0) in vec3 aPosition;
// aTexCoord is the 2nd VertexAttrib of each stride
layout(location = 1) in vec2 aTexCoord;

out vec2 TexCoord;

void main() {
    gl_Position = vec4(aPosition, 1.0);
    TexCoord = aTexCoord;
}