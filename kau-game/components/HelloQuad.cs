using KauRock;
using Loaders = KauRock.Loaders;

using OpenTK.Graphics.OpenGL4;

namespace kauGame.Components {
	public class HelloQuad : Component {
		public HelloQuad (GameObject go) : base (go) {

		}

		float[] vertices = {
			-0.5f,	-0.5f,	0.0f,      // Bottom left.
			-0.5f,   0.5f,	0.0f,      // Top left.
			 0.5f,   0.5f,	0.0f,      // Top right.
			 0.5f,	-0.5f,	0.0f       // Bottom right.
		};

		uint[] triangles =  {
			0, 1, 2,
			0, 2, 3
		};

		int ebo, vbo, vao;

		ShaderProgram shader;

		public override void OnStart () {

			// Use the Shader loader to loait std the shaders.
			using (var loader = new Loaders.Shader ()) {
				// Create a new shader from the loader.
				shader = loader.Load ("resources/shaders/generic.glsl");

				// Set the shader's Transform.
				shader.Transform = GameObject.Transform;
			}

			// Create the buffers for our vertex array data and element.
			vao = GL.GenVertexArray();
			vbo = GL.GenBuffer();
			ebo = GL.GenBuffer();

			// Tell open GL where we will be writing the vertex array to.
			GL.BindVertexArray(vao);

			// Select the Array Buffer with the pointer of vbo and write the vertex data to it.
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(int) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

			// Select the Element Array Buffer with the pointer of ebo and write the triangles to it.
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
			GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * triangles.Length, triangles, BufferUsageHint.StaticDraw);

			// Tell GL the structure of our vertex data and enable what we defined.
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
			GL.EnableVertexAttribArray(0);

			// We are done here, let's clean up and un-bind everything we bound.
			GL.BindVertexArray(0);

			Events.Render += OnRender;
		}

		public void OnRender () {

			// Use our shader.
			shader.UseProgram ();

			// Bind the vertex array to use with the triangles.
			GL.BindVertexArray(vao);

			// Draw the element buffer object. (triangles)
			GL.DrawElements(PrimitiveType.Triangles, triangles.Length, DrawElementsType.UnsignedInt, 0);

			// Un-bind the vertex array.
			GL.BindVertexArray (0);
		}
		public override void OnDestroy () {
			GL.BindBuffer (BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray (0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			GL.UseProgram (0);

			// GL.DeleteBuffer (vbo);
			GL.DeleteVertexArray (vao);
			GL.DeleteBuffer (ebo);

			Events.Render -= OnRender;
		}
	}
}