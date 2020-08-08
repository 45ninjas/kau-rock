#version 330

layout(location = 0) in vec3 aColour;
layout(location = 1) in vec3 aPosition;

uniform mat4 transform;
uniform mat4 view;
uniform mat4 projection;

out vec3 VertexColour;

vec3 hueShift(vec3 color, float hueAdjust);

void main() {
    gl_Position = vec4(aPosition, 1.0) * transform * view * projection;

    vec4 position = vec4(aPosition, 1.0) * transform;
    // VertexColour = aColour;
    VertexColour = hueShift(vec3(1, 0, 0), position.y / 6.28319f);
}

// This is a copy pasta from stack overflow. Don't bother shaming, I already feel it.
vec3 hueShift( vec3 color, float hueAdjust ){

    const vec3  kRGBToYPrime = vec3 (0.299, 0.587, 0.114);
    const vec3  kRGBToI      = vec3 (0.596, -0.275, -0.321);
    const vec3  kRGBToQ      = vec3 (0.212, -0.523, 0.311);

    const vec3  kYIQToR     = vec3 (1.0, 0.956, 0.621);
    const vec3  kYIQToG     = vec3 (1.0, -0.272, -0.647);
    const vec3  kYIQToB     = vec3 (1.0, -1.107, 1.704);

    float   YPrime  = dot (color, kRGBToYPrime);
    float   I       = dot (color, kRGBToI);
    float   Q       = dot (color, kRGBToQ);
    float   hue     = atan (Q, I);
    float   chroma  = sqrt (I * I + Q * Q);

    hue += hueAdjust;

    Q = chroma * sin (hue);
    I = chroma * cos (hue);

    vec3    yIQ   = vec3 (YPrime, I, Q);

    return vec3( dot (yIQ, kYIQToR), dot (yIQ, kYIQToG), dot (yIQ, kYIQToB) );
}