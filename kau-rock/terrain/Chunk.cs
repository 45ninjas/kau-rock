using KauRock;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace KauRock.Terrain {
	public class Chunk : Component
	{
		public const int Size = 16;
		public IVoxelProvider Provider;

		public VoxPos Position;

		int triCount;

		int elementBuffer, vertexBuffer, vertexArray;
		public static ShaderProgram shader;

		public static MeshMaker MeshMaker;

		public Chunk(VoxPos Position, IVoxelProvider Provider, GameObject gameObject) : base(gameObject)
		{
			// Move the transform to the chunk position.
			var transform = GameObject.Transform;
			transform.Position = (Vector3)Position * Size;

			if(MeshMaker == null)
				MeshMaker = new MeshMaker();
		}

		private static  void LoadShader() {
			if(shader != null)
				return;

			using(var loader = new Loaders.Shader()) {
				shader = loader.Load("resources/shaders/chunk.glsl");
			}
		}

		public void SetMesh(Chunk.Vertex[] vertices, uint[] triangles) {
			triCount = triangles.Length;

			Log.Debug(this, $"{vertices.Length} vertices, {triangles.Length / 3} triangles");

			// Generate our vertex arrays, buffer and element buffer.
			vertexArray = GL.GenVertexArray();
			vertexBuffer = GL.GenBuffer();
			elementBuffer = GL.GenBuffer();

			GL.BindVertexArray(vertexArray);
			GL.BindVertexArray(vertexBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);

			GL.NamedBufferStorage(vertexBuffer, vertices.Length * Vertex.Size, vertices, BufferStorageFlags.MapWriteBit);

			int aPosition = shader.GetAttribLocation("aPosition");
			GL.VertexArrayAttribBinding(vertexArray, aPosition, 0);
			GL.EnableVertexArrayAttrib(vertexArray, aPosition);
			GL.VertexArrayAttribFormat(
				vertexArray,
				aPosition,													// The attribute that we are going to set.
				3,																	// The size of the attribute (3 floats).
				VertexAttribType.Float,							// The type of attribute (floats).
				false,															// Don't need to normalize as it's already normalized.
				0																		// The offset of the item.
			);

			int aColour = shader.GetAttribLocation("aColour");
			GL.VertexArrayAttribBinding(vertexArray, aColour, 0);
			GL.EnableVertexArrayAttrib(vertexArray, aColour);
			GL.VertexArrayAttribFormat(
				vertexArray,
				aColour,													// The attribute that we are going to set.
				3,																	// The size of the attribute (3 floats).
				VertexAttribType.Float,							// The type of attribute (floats).
				false,															// Don't need to normalize as it's already normalized.
				12																	// The offset of the item.
			);

			GL.VertexArrayVertexBuffer(vertexArray, 0, vertexBuffer, System.IntPtr.Zero, Vertex.Size);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBuffer);
			GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * triangles.Length, triangles, BufferUsageHint.StaticDraw);
		}

		public override void OnStart() {
			LoadShader();

			MeshMaker.UpdateChunk(this);

			Events.Render += Render;
		}
		public override void OnDestroy() {
			Events.Render -= Render;
		}

		void Render() {
			shader.UseProgram(GameObject.Transform.Matrix);

			GL.BindVertexArray(vertexArray);
			GL.DrawElements(PrimitiveType.Triangles, triCount, DrawElementsType.UnsignedInt, 0);
		}

		public struct Vertex {
			public Vector3 Position;
			public Vector3 Colour;

			public const int Size = sizeof(float) * 6;

			public Vertex(Vector3 Position) {
				this.Position = Position;
				Colour = new Vector3(1,1,0);
			}
		}
	}
}