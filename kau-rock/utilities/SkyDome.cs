// The sky dome is supposed to be a completley self contained sub-system.
// Sky dome uses it's own shaders stored as strings, using vertex colours
// to colour the sky. A uv-sphere with the bottom clipped of is create in
// the constructor.
//
// To render the sky behind everything is must be rendered first as it
// clears the depth buffer.

using KauRock.Utilities;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Mathf = System.MathF;

namespace KauRock {
  public class SkyDome : IGizmo, System.IDisposable {

    public Color Sky = new Color(64, 104, 173, 255);
    public Color Horizon = new Color(99, 144, 201, 255);
    public Color Ground = new Color(180, 180, 180, 0);

    int triangleCount = 0;
    ShaderProgram shader;
    int vertexBuffer;
    int elementBuffer;
    int vertexArray;

    public SkyDome() {
      GenerateDome(18, 6, 28, out Vertex[] verts, out int[] tris);
      triangleCount = tris.Length;

      // Load the frag and vert shaders from a string.
      using (var loader = new KauRock.Loaders.Shader()) {
        loader.AddRaw("sky.frag", fragShader, ShaderType.FragmentShader);
        loader.AddRaw("sky.vert", vertShader, ShaderType.VertexShader);

        shader = loader.Load("sky.frag", "sky.vert");
      }

      // Create our VertexArray.
      vertexArray = GL.GenVertexArray();
      vertexBuffer = GL.GenBuffer();

      GL.BindVertexArray(vertexArray);
      GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);

      // Use named storage (struct) for our vertex data.
      GL.NamedBufferStorage(vertexBuffer, Vertex.Size * verts.Length, verts, BufferStorageFlags.MapWriteBit);

      // Tell GL where to get 'aPosition' from in our shader data.
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

      // Tell GL where to get 'aColor' from in our shader data.
      int aColor = shader.GetAttribLocation("aColor");
      GL.VertexArrayAttribBinding(vertexArray, aColor, 0);
      GL.EnableVertexArrayAttrib(vertexArray, aColor);
      GL.VertexArrayAttribFormat(
        vertexArray,
        aColor,                             // The attribute that we are going to set.
        3,                                  // The size of the attribute (3 floats).
        VertexAttribType.Float,             // The type of attribute (floats).
        false,                              // Don't need to normalize as it's already normalized.
        3 * sizeof(float)                   // The offset of the item.
      );

      GL.VertexArrayVertexBuffer(vertexArray, 0, vertexBuffer, System.IntPtr.Zero, Vertex.Size);

      // Give GL our triangle structure.
      elementBuffer = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBuffer);
      GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * tris.Length, tris, BufferUsageHint.StaticDraw);

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
    }

    private void GenerateDome(int horizontalLines, int verticalClip, int verticalLines, out Vertex[] verts, out int[] tris) {
      // This function was a pain to create. It produces a UV sphere.
      // The top and bottom of the sphere is made using a triangle fan.
      // The top triangle fan is created followed by the the rest of the
      // sphere until the verticalClip is reached. Lastly the bottom cap
      // is added.

      // Define our vertice and triangle arrays along with our radius,
      // triangle index (tc) and vertex index (i).
      verts = new Vertex[((horizontalLines - verticalClip - 1) * verticalLines) + 2];
      tris = new int[((((horizontalLines - verticalClip - 1) * verticalLines) * 2)) * 3];
      float radius = 1f;
      int tc = 0;
      int i = 0;

      // Create our top triangle fan.
      for (int m = 0; m < verticalLines; m++) {
        tris[tc++] = 0;                                     // Middle of fan.
        tris[tc++] = m + 1;                                 // The next vertex.
        if (m != 0)
          tris[tc++] = m;                                   // The current vertex.
        else
          tris[tc++] = verticalLines;                       // The final vertex.
      }
      verts[i++] = new Vertex(Vector3.UnitY * radius, Sky);

      // We would like to know what the lowest vertex is so we can place
      // our final bottom vertex at the same y position.
      float lowestY = float.PositiveInfinity;

      for (int m = 1; m < horizontalLines - verticalClip; m++) {
        for (int n = 0; n < verticalLines; n++) {

          // Create a X, Y and Z position from our m and n values.
          float x = Mathf.Sin(Mathf.PI * m / horizontalLines) * Mathf.Cos(2 * Mathf.PI * n / verticalLines);
          float z = Mathf.Sin(Mathf.PI * m / horizontalLines) * Mathf.Sin(2 * Mathf.PI * n / verticalLines);
          float y = Mathf.Cos(Mathf.PI * m / horizontalLines);

          // Update the lowest known Y for later.
          if (y < lowestY)
            lowestY = y;

          // Don't place triangles down if we are on an edge.
          if (m < horizontalLines - (verticalClip + 1) && m > 0) {

            // Standard triangles.
            if (n < verticalLines - 1) {
              tris[tc++] = i;
              tris[tc++] = i + 1;
              tris[tc++] = i + 1 + verticalLines;

              tris[tc++] = i;
              tris[tc++] = i + 1 + verticalLines;
              tris[tc++] = i + 0 + verticalLines;

            }
            else {
              // Triangles on the edges of each loop.
              tris[tc++] = i;
              tris[tc++] = i + 1 - verticalLines;
              tris[tc++] = i + 1;

              tris[tc++] = i;
              tris[tc++] = i + 1;
              tris[tc++] = i + 0 + verticalLines;
            }
          }

          Vector3 color = new Vector3(255, 255, 0);

          // Make the ground.
          if (m == horizontalLines - (verticalClip + 1))
            color = new Vector3(Ground.R, Ground.G, Ground.B);
          // Make the sky by interperlating between the sky and horizon.
          else {
            float t = Mathf.Pow(1 - y, 3);
            if (t > 1)
              t = 1;
            if (t < 0)
              t = 0;
            color = Vector3.Lerp(new Vector3(Sky.R, Sky.G, Sky.B), new Vector3(Horizon.R, Horizon.G, Horizon.B), t);
          }

          // Lastly, create our shiny new vertex and do it all over again.
          verts[i++] = new Vertex(new Vector3(x, y, z) * radius, color / 255);
        }
      }

      // Finally, create our bottom triangle fan (floor).
      for (int m = 0; m < verticalLines; m++) {
        tris[tc] = i;
        tc++;
        tris[tc++] = (i - verticalLines) + m;

        if (m == verticalLines - 1)
          tris[tc++] = i - verticalLines;
        else
          tris[tc++] = (i - verticalLines) + m + 1;
      }
      verts[i++] = new Vertex(Vector3.UnitY * radius * lowestY, Ground);
    }

    public void Dispose() {
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
      GL.DeleteBuffer(vertexBuffer);

      GL.DeleteVertexArray(vertexArray);
      GL.DeleteBuffer(elementBuffer);
    }

    public void Render(Matrix4 view, Matrix4 projection) {
      // Use our pretty shader and our vertex array.
      // shader.SetMatrix("view", view.ClearTranslation().ClearScale());
      // shader.SetMatrix("projection", projection);
      shader.UseProgram(Matrix4.Identity, view.ClearTranslation(), projection);
      GL.BindVertexArray(vertexArray);

      // Draw the element buffer object.
      GL.DrawElements(PrimitiveType.Triangles, triangleCount, DrawElementsType.UnsignedInt, 0);

      // Clear the buffer mask so the sky is at the back.
      GL.Clear(ClearBufferMask.DepthBufferBit);
    }

    // The Vertex data for our shaders.
    public struct Vertex {
      public const int Size = sizeof(float) * 6;

      public Vector3 Position;
      public Vector3 Color;

      public Vertex(Vector3 position, Color color) {
        Position = position;
        Color.X = (float)color.R / byte.MaxValue;
        Color.Y = (float)color.G / byte.MaxValue;
        Color.Z = (float)color.B / byte.MaxValue;
      }
      public Vertex(Vector3 position, Vector3 color) {
        Position = position;
        Color.X = color.X;
        Color.Y = color.Y;
        Color.Z = color.Z;
      }
    }

    // The shaders used in our sky.
    const string fragShader = @"#version 330 core
out vec3 fragmentColor;
in vec3 vertexColor;

void main() {
	fragmentColor = vertexColor;
}
";
    const string vertShader = @"#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aColor;

uniform mat4 projection;
uniform mat4 view;

out vec3 vertexColor;

void main() {
	gl_Position = vec4(aPosition, 1) * view * projection;
	vertexColor = aColor;
}
";
  }
}