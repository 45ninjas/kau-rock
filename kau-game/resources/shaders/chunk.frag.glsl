#version 330
in vec3 VertexColour;

out vec3 FragColor;

void main() {
    FragColor = VertexColour;
    // FragColor = vec3(1);
}