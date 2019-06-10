using KauRock;
using Loaders = KauRock.Loaders;

using OpenTK.Graphics.OpenGL4;
using OpenTK;
using Mathf = System.MathF;

namespace kauGame.Components {
	public class Cube : Component {
		public Cube (GameObject go) : base (go) {

		}

		float[] vertices = {
			// X			Y				Z		U	 V
			-0.5f,  0.5f, -0.5f,	0, 2f / 3,	// 0	// Top face
			-0.5f,  0.5f,  0.5f,	0, 3f / 3,	// 1
			 0.5f,  0.5f,  0.5f,	1, 3f / 3,	// 2
			 0.5f,  0.5f, -0.5f,	1, 2f / 3, // 3

			-0.5f, -0.5f, -0.5f,	1, 1f / 3, // 4	// Bottom face.
			-0.5f, -0.5f,  0.5f,	1, 0f / 3, // 5
			 0.5f, -0.5f,  0.5f,	0, 0f / 3,	// 6
			 0.5f, -0.5f, -0.5f,	0, 1f / 3,	// 7

			 0.5f,	0.5f,  0.5f,	1, 2f / 3,	// 8	// Top loop
			 0.5f,	0.5f, -0.5f,	0, 2f / 3,	// 9
			-0.5f,	0.5f, -0.5f,	1, 2f / 3,	// 10
			-0.5f,	0.5f,  0.5f,	0, 2f / 3, // 11

			 0.5f, -0.5f,  0.5f,	1, 1f / 3, // 12	// Bottom Loop
			 0.5f, -0.5f, -0.5f,	0, 1f / 3, // 13
			-0.5f, -0.5f, -0.5f,	1, 1f / 3,	// 14
			-0.5f, -0.5f,  0.5f,	0, 1f / 3,	// 15

			
		};

		uint[] triangles =  {
			0, 2, 1,			0, 3, 2,		// Bottom face.
			4, 5, 6,			4, 6, 7,		// Top Face.

			15, 11, 8,		15, 8, 12,	// Front Face.
			10, 11, 15,	  10, 15, 14,	// Left Face.
			10, 14, 13,		10, 13, 9,	// Back Face.
			13, 8, 9,			13, 12, 8,	// Right Face. 
		};

		int ebo, vbo, vao;

		ShaderProgram shader;
		Texture2D texture;
		
		int? tintColor = null;

		public override void OnStart () {

			// Use the Shader loader to loait std the shaders.
			using (var loader = new Loaders.Shader ()) {
				// Create a new shader from the loader.
				shader = loader.Load ("resources/shaders/generic.glsl");

				// Set the shader's Transform.
				shader.Transform = GameObject.Transform;
			}

			using (var loader = new Loaders.Texture ()) {
				loader.Filter = TextureMagFilter.Nearest;
				texture = loader.Load("resources/textures/grass-block.png");
				shader.SetTexture("PrimaryTexture", texture);
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
			int stride = (3 + 2) * sizeof(float);
			
			int vertPos = shader.GetAttribLocation("aPosition");
			GL.VertexAttribPointer(vertPos, 3, VertexAttribPointerType.Float, false, stride, 0);
			GL.EnableVertexAttribArray(vertPos);

			// Texture coordinates.
			int texPos = shader.GetAttribLocation("aTexCoord");
			GL.VertexAttribPointer(texPos, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
			GL.EnableVertexAttribArray(texPos);

			// We are done here, let's clean up and un-bind everything we bound.
			GL.BindVertexArray(0);

			Events.Render += OnRender;
			Events.Update += OnUpdate;
		}

		void OnUpdate() {
			// GameObject.Transform.Rotate(0,0,MathHelper.DegreesToRadians(Time.Delta * 60));
		}

		public void OnRender () {

			// Use our shader.
			shader.UseProgram();

			// TODO: This is shit and inefficient. Get rid of it.
			shader.SetMatrix("view", Camera.ActiveCamera.GetViewMatrix());
			shader.SetMatrix("projection", Camera.ActiveCamera.GetProjectionMatrix());

			if(tintColor != null) {
				var start = new Vector4(KauTheme.Lightest.R, KauTheme.Lightest.G, KauTheme.Lightest.B, KauTheme.Lightest.A);
				var end = new Vector4(KauTheme.HighLight.R, KauTheme.HighLight.G, KauTheme.HighLight.B, KauTheme.HighLight.A);
				var color = Vector4.Lerp(start,end, 0.5f + Mathf.Sin(Time.GameTime) / 2) / byte.MaxValue;

				shader.SetVector4((int)tintColor, color);
			}

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
			Events.Update -= OnUpdate;
		}
	}
}