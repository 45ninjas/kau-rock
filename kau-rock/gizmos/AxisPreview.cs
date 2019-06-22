using System;
using KauRock;
using KauRock.Utilities;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace KauRock.Gizmos {

	public class AxisPreview : IGizmo, System.IDisposable {
		float[] verts = {
		//x, y, z,		r, g, b

		//ZY
			0, 0, 0,		0, 0, 1,
			0, 0, 1,		0, 0, 1,
			0, 1, 0,		0, 0, 0,
		//YX
			0, 0, 0,		0, 1, 0,
			0, 1, 0,		0, 1, 0,
			1, 0, 0,		0, 0, 0,
		//ZX
			0, 0, 0,		1, 0, 0,
			1, 0, 0,		1, 0, 0,
			0, 0, 1,		0, 0, 0,
		};

		uint[] tris = {

			// Front 			Back
			0, 1, 2,			1, 0, 2,		// ZY
			6, 7, 8,			7, 6, 8,		// YX
			3, 4, 5,			4, 3, 5,		// XZ

			0, 1, 5,		  1, 0, 5,		// ZY Alternative
			6, 7, 2,		  7, 6, 2,		// YX Alternative
			3, 4, 8,		  4, 3, 8,		// XZ Alternative
		};

		int size;
		int border;

		Vector3 translation;
		Vector3 scale;

		int vertexBuffer;
		int vertexArray;
		int elementBuffer;

		ShaderProgram shader;

		public AxisPreview(KauWindow window, int size = 60, int border = 25) {

			this.size = size;
			this.border = border;
			Events.WindowResize += onResize;
			onResize(window);

			using (var loader = new KauRock.Loaders.Shader()) {
				loader.AddRaw("axis-preview.vert", vertShader, ShaderType.VertexShader);
				loader.AddRaw("axis-preview.frag", fragShader, ShaderType.FragmentShader);
				shader = loader.Load("axis-preview.vert", "axis-preview.frag");
			}

			vertexArray = GL.GenVertexArray();
			vertexBuffer = GL.GenBuffer();
			elementBuffer = GL.GenBuffer();
			GL.BindVertexArray(vertexArray);

			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * verts.Length, verts, BufferUsageHint.StaticDraw);
			
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBuffer);
			GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * tris.Length, tris, BufferUsageHint.StaticDraw);

			int stride = sizeof(float) * 6;

			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, stride / 2);
			GL.EnableVertexAttribArray(1);
		}

		private void onResize(KauWindow window) {
			float aspect = (float)window.Width / (float)window.Height;

			scale = new Vector3(size / (float)window.Width, size / (float)window.Height, 1f);
			translation = new Vector3(
				(window.Width - (size + border))  / ((float)window.Width),
				(window.Height - (size + border)) / ((float)window.Height),
			0);
		}

		public void Render(Matrix4 view, Matrix4 projection) {

			view = view.ClearTranslation();
			view = view.ClearScale();
			view *= Matrix4.CreateScale(scale);
			view *= Matrix4.CreateTranslation(translation);

			shader.UseProgram(Matrix4.Identity, view, Matrix4.Identity);
			
			GL.BindVertexArray(vertexArray);
			GL.DrawElements(PrimitiveType.Triangles, tris.Length, DrawElementsType.UnsignedInt, 0);
		}

		public void Dispose() {
			GL.DeleteBuffer(vertexBuffer);
			GL.DeleteVertexArray(vertexArray);
		}

		const string fragShader = @"#version 330 core
out vec3 FragmentColour;
in vec3 vertexColour;

void main() {
	if(length(vertexColour) < 0.98) {
		discard;
	}
	FragmentColour = vertexColour;
}
";

		const string vertShader = @"#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 colour;

uniform mat4 projection;
uniform mat4 view;

out vec3 vertexColour;

void main() {
	gl_Position = vec4(position, 1) * view * projection;
	vertexColour = colour;
}
";
	}
}