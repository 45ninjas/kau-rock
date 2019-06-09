using KauRock;
using Loaders = KauRock.Loaders;

using OpenTK.Graphics.OpenGL4;
using OpenTK;
using Mathf = System.MathF;

namespace kauGame.Components {
	public class HelloTransform : Component {
		public HelloTransform (GameObject go) : base (go) {

		}

		float[] vertices = {
			// X			Y				Z		U			V
			-0.5f,	-0.5f,	0.0f,	0.0f,	0.0f,	// Bottom left.
			-0.5f,   0.5f,	0.0f, 0.0f, 1.0f,	// Top left.
			 0.5f,   0.5f,	0.0f, 1.0f, 1.0f,	// Top right.
			 0.5f,	-0.5f,	0.0f, 1.0f, 0.0f	// Bottom right.
		};

		uint[] triangles =  {
			0, 1, 2,
			0, 2, 3
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

				// set the tintColor for this shader.
				if(shader.TryGetUniformLocation("tintColor", out int tint)) {
					shader.SetVector4(tint, KauTheme.HighLight);
					tintColor = tint;
				}
			}

			using (var loader = new Loaders.Texture()) {
				loader.GenerateMipmap = true;
				loader.Filter = TextureMagFilter.Linear;
				texture = loader.Load("resources/textures/yotsuba-necb.jpg");
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
			GameObject.Transform.Rotate(0,0,MathHelper.DegreesToRadians(Time.Delta * 60));
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