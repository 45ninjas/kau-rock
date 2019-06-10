using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace KauRock {
	public class GenericSky : IDisposable{
		private float[] verts  = {
			-1.0f, -1.0f,
			-1.0f, 1.0f,
			1.0f, 1.0f,
			1.0f, -1.0f                
		};

		private uint[] tris = {
			0, 1, 2,
			0, 2, 3
		};

		ShaderProgram shader;

		int vertexBufferObject;
		int elementBufferObject;
		int vertexArrayObject;

		public GenericSky() {
			// Load the shader.
			using(var loader = new KauRock.Loaders.Shader()) {
				loader.AddRaw("sky.frag", fragShader, ShaderType.FragmentShader);
				loader.AddRaw("sky.vert", vertShader, ShaderType.VertexShader);

				shader = loader.Load("sky.frag", "sky.vert");
			}
			vertexArrayObject = GL.GenVertexArray();
			vertexBufferObject = GL.GenBuffer();
			elementBufferObject = GL.GenBuffer();

			GL.BindVertexArray(vertexArrayObject);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * verts.Length, verts, BufferUsageHint.StaticDraw);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
			GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * tris.Length, tris, BufferUsageHint.StaticDraw);

			GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
			GL.EnableVertexAttribArray(0);

			GL.BindVertexArray(0);
		}

		public void Dispose()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			GL.DeleteBuffer(vertexBufferObject);

			GL.DeleteVertexArray(vertexArrayObject);
			GL.DeleteBuffer(elementBufferObject);
		}

		public void Render() {
			shader.UseProgram();

			// GL.Disable(EnableCap.DepthTest);

			GL.BindVertexArray(vertexArrayObject);
			GL.DrawElements(PrimitiveType.Triangles, tris.Length, DrawElementsType.UnsignedInt, 0);
			GL.BindVertexArray(0);

			GL.Clear(ClearBufferMask.DepthBufferBit);

			// GL.Enable(EnableCap.DepthTest);
		}

		const string fragShader = @"#version 330 core
out vec4 FragColor;

uniform float height = 720.0;

uniform float Stop1=0.4;
uniform vec4 C1 = vec4(0.3, 0.4, 0.9, 1);it 

uniform float Stop2=0.5;
uniform vec4 C2 = vec4(0.9);

uniform float Stop3=0.8;
uniform vec4 C3 = vec4(0.2, 0.8, 1, 1) * 0.8;

in vec4 gl_FragCoord;

void main() {
	float Y = (gl_FragCoord.y / height);
	FragColor = C1;
	FragColor = mix(FragColor, C2, smoothstep(Stop1, Stop2, Y));
	FragColor = mix(FragColor, C3, smoothstep(Stop2, Stop3, Y));
	// FragColor.x = Y;
}
";
		const string vertShader = @"#version 330 core
layout (location = 0) in vec2 aPosition;

void main() {
	gl_Position = vec4(aPosition.xy, 0.0, 1);
}
";
	}
}