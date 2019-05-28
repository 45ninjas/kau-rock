using KauRock;
using Loaders = KauRock.Loaders;

using OpenTK.Graphics.OpenGL4;

namespace kauGame.Components {
    public class HelloDesperate : Component {
        public HelloDesperate(GameObject go) : base(go) {
            
        }

        private readonly float[] _vertices =
        {
            -0.5f, -0.5f, 0.0f, // Bottom-left vertex
             0.5f, -0.5f, 0.0f, // Bottom-right vertex
             0.0f,  0.5f, 0.0f  // Top vertex
        };

        private int _vertexBufferObject;
        private int _vertexArrayObject;

        ShaderProgram shader;

        public override void OnStart() {

            using(var loader = new Loaders.Shader()) {
                shader = loader.Load("resources/shaders/generic.glsl");
            }
            // Tell the loader to load the shader.
            // Vertex buffer object.
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            shader.UseProgram();

            // Vertex array object.
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            Events.Render += OnRender;
        }

        public void OnRender() {
            shader.UseProgram();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            GL.BindVertexArray(_vertexArrayObject);
        }
        public override void OnDestroy() {
            Events.Render -= OnRender;
        }
    }
}