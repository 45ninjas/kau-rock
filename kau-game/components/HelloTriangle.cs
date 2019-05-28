using KauRock;
using Loaders = KauRock.Loaders;

using OpenTK.Graphics.OpenGL4;

namespace kauGame.Components
{
    public class HelloTriangle : Component
    {
        public HelloTriangle(GameObject go) : base(go)
        {

        }

        float[] vertices = {
            // first triangle
             0.5f,  0.5f, 0.0f,  // top right
             0.5f, -0.5f, 0.0f,  // bottom right
            -0.5f,  0.5f, 0.0f,  // top left 
            // second triangle
             0.5f, -0.5f, 0.0f,  // bottom right
            -0.5f, -0.5f, 0.0f,  // bottom left
            -0.5f,  0.5f, 0.0f   // top left
        };

        int vbo;
        int vao;

        ShaderProgram shader;

        public override void OnStart()
        {

            // Use the Shader loader to loait std the shaders.
            using (var loader = new Loaders.Shader())
            {
                // Create a new shader from the loader.
                shader = loader.Load("resources/shaders/generic.glsl");

                // Set the shader's Transform.
                shader.Transform = GameObject.Transform;
            }

            // Create a buffer to put our vertices in GL land.
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            Events.Render += OnRender;
        }

        public void OnRender()
        {

            // Use our shader.
            shader.UseProgram();

            // Tell gl how to draw our vertex array object.
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            // Draw the vertex array object.
            GL.BindVertexArray(vao);
        }
        public override void OnDestroy()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(vbo);
            GL.DeleteVertexArray(vao);

            Events.Render -= OnRender;
        }
    }
}