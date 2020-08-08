using System;

using KauRock;
using Loaders = KauRock.Loaders;

using OpenTK.Graphics.OpenGL4;
using OpenTK;
using Mathf = System.MathF;

namespace kauGame.Components {
  public class Cube : Component {
    public Cube(GameObject go) : base(go) {

    }

    private struct Vertex {
      public const int Size = sizeof(float) * 5;

      public Vector3 Position;
      public Vector2 UV;

      public Vertex(float x, float y, float z, float u, float v) {
        Position.X = x;
        Position.Y = y;
        Position.Z = z;

        UV.X = u;
        UV.Y = v;
      }
    }

    Vertex[] vertices = {
			// X			Y				Z		U	 V
			new Vertex( -0.5f,  0.5f, -0.5f,  0, 2f / 3),	// 0	// Top face
			new Vertex( -0.5f,  0.5f,  0.5f,  0, 3f / 3),	// 1
			new Vertex(  0.5f,  0.5f,  0.5f,  1, 3f / 3),	// 2
			new Vertex(  0.5f,  0.5f, -0.5f,  1, 2f / 3), // 3

			new Vertex( -0.5f, -0.5f, -0.5f,  1, 1f / 3), // 4	// Bottom face.
			new Vertex( -0.5f, -0.5f,  0.5f,  1, 0f / 3), // 5
			new Vertex(  0.5f, -0.5f,  0.5f,  0, 0f / 3),	// 6
			new Vertex(  0.5f, -0.5f, -0.5f,  0, 1f / 3),	// 7

			new Vertex(  0.5f,  0.5f,  0.5f,  1, 2f / 3),	// 8	// Top loop
			new Vertex(  0.5f,  0.5f, -0.5f,  0, 2f / 3),	// 9
			new Vertex( -0.5f,  0.5f, -0.5f,  1, 2f / 3),	// 10
			new Vertex( -0.5f,  0.5f,  0.5f,  0, 2f / 3), // 11

			new Vertex(  0.5f, -0.5f,  0.5f,  1, 1f / 3), // 12	// Bottom Loop
			new Vertex(  0.5f, -0.5f, -0.5f,  0, 1f / 3), // 13
			new Vertex( -0.5f, -0.5f, -0.5f,  1, 1f / 3),	// 14
			new Vertex( -0.5f, -0.5f,  0.5f,  0, 1f / 3),	// 15
		};

    uint[] triangles =  {
      0, 2, 1,      0, 3, 2,		// Bottom face.
			4, 5, 6,      4, 6, 7,		// Top Face.

			15, 11, 8,    15, 8, 12,	// Front Face.
			10, 11, 15,   10, 15, 14,	// Left Face.
			10, 14, 13,   10, 13, 9,	// Back Face.
			13, 8, 9,     13, 12, 8,	// Right Face. 
		};

    int ebo, vertexBuffer, vertexArray;

    ShaderProgram shader;
    Texture2D texture;

    int? tintColor = null;

    public override void OnStart() {

      // Use the Shader loader to loait std the shaders.
      using (var loader = new Loaders.Shader()) {
        // Create a new shader from the loader.
        shader = loader.Load("resources/shaders/generic-unlit.glsl");
      }

      using (var loader = new Loaders.Texture()) {
        loader.Filter = TextureMagFilter.Nearest;
        texture = loader.Load("resources/textures/grass-block.png");
        shader.SetTexture("PrimaryTexture", texture);
      }

      // Create the buffers for our vertex array data and element.
      vertexArray = GL.GenVertexArray();
      vertexBuffer = GL.GenBuffer();
      ebo = GL.GenBuffer();

      // Tell open GL where we will be writing the vertex array to.
      GL.BindVertexArray(vertexArray);

      // Select the Array Buffer with the pointer of vertexBuffer and write the vertex data to it.
      GL.BindVertexArray(vertexBuffer);
      GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);

      GL.NamedBufferStorage(vertexBuffer, Vertex.Size * vertices.Length, vertices, BufferStorageFlags.MapWriteBit);

      int aPosition = shader.GetAttribLocation("aPosition");
      GL.VertexArrayAttribBinding(vertexArray, aPosition, 0);
      GL.EnableVertexArrayAttrib(vertexArray, aPosition);
      GL.VertexArrayAttribFormat(
        vertexArray,
        aPosition,                          // The attribute that we are going to set.
        3,                                  // The size of the attribute (3 floats).
        VertexAttribType.Float,             // The type of attribute (floats).
        false,                              // Don't need to normalize as it's already normalized.
        0                                   // The offset of the item.
      );

      int aTexCoord = shader.GetAttribLocation("aTexCoord");
      GL.VertexArrayAttribBinding(vertexArray, aTexCoord, 0);
      GL.EnableVertexArrayAttrib(vertexArray, aTexCoord);
      GL.VertexArrayAttribFormat(
        vertexArray,
        aTexCoord,                              // The attribute that we are going to set.
        2,                                  // The size of the attribute (3 floats).
        VertexAttribType.Float,             // The type of attribute (floats).
        false,                              // Don't need to normalize as it's already normalized.
        12                  // The offset of the item.
      );

      GL.VertexArrayVertexBuffer(vertexArray, 0, vertexBuffer, IntPtr.Zero, Vertex.Size);

      GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
      GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * triangles.Length, triangles, BufferUsageHint.StaticDraw);


      Events.Render += OnRender;
      Events.Update += OnUpdate;
    }

    void OnUpdate() {
      // GameObject.Transform.Rotate(0,0,MathHelper.DegreesToRadians(Time.Delta * 60));
    }

    public void OnRender() {

      // Use our shader.
      shader.UseProgram(GameObject.Transform.Matrix);

      // TODO: This is shit and inefficient. Get rid of it.
      shader.SetMatrix("view", Camera.ActiveCamera.GetViewMatrix());
      shader.SetMatrix("projection", Camera.ActiveCamera.GetProjectionMatrix());

      if (tintColor != null) {
        var start = new Vector4(KauTheme.Lightest.R, KauTheme.Lightest.G, KauTheme.Lightest.B, KauTheme.Lightest.A);
        var end = new Vector4(KauTheme.HighLight.R, KauTheme.HighLight.G, KauTheme.HighLight.B, KauTheme.HighLight.A);
        var color = Vector4.Lerp(start, end, 0.5f + Mathf.Sin(Time.GameTime) / 2) / byte.MaxValue;

        shader.SetVector4((int)tintColor, color);
      }

      // Bind the vertex array to use with the triangles.
      GL.BindVertexArray(vertexArray);

      // Draw the element buffer object. (triangles)
      GL.DrawElements(PrimitiveType.Triangles, triangles.Length, DrawElementsType.UnsignedInt, 0);

      // Un-bind the vertex array.
      GL.BindVertexArray(0);
    }
    public override void OnDestroy() {
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
      GL.UseProgram(0);

      // GL.DeleteBuffer (vertexBuffer);
      GL.DeleteVertexArray(vertexArray);
      GL.DeleteBuffer(ebo);

      Events.Render -= OnRender;
      Events.Update -= OnUpdate;
    }
  }
}