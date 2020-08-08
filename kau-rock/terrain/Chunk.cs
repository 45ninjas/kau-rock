using KauRock;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace KauRock.Terrain {
  public class Chunk : Component {
    // Chunk stuff.
    public const int Size = 16;
    public VoxPos Position;
    public bool IsDirty = false;

    internal bool[] Solid = new bool[Chunk.Size * Chunk.Size * Chunk.Size];

    private readonly ChunkManager manager;

    // Rendering stuff.
    int triCount;
    int elementBuffer, vertexBuffer, vertexArray;
    public static ShaderProgram shader;

    public static MeshMaker MeshMaker;

    public Chunk (VoxPos position, IVoxelProvider provider, ChunkManager manager) {
      Position = position;

      if ( MeshMaker == null )
        MeshMaker = new MeshMaker();

      var values = provider.GetChunk( Position );
      for ( int i = 0; i < Solid.Length; i++ ) {
        Solid[i] = values[i] > 0.5f;
      }

      this.manager = manager;
    }

    private static void LoadShader () {
      if ( shader != null )
        return;

      using ( var loader = new Loaders.Shader() ) {
        shader = loader.Load( "resources/shaders/chunk.glsl" );
      }
    }

    public bool GetSolid (VoxPos position) {
      return Solid[position.X + position.Y * Chunk.Size + position.Z * Chunk.Size * Chunk.Size];
    }

    public void SetSolid (VoxPos position, bool value, bool autoRebuild) {

      if ( !autoRebuild )
        Solid[position.X + position.Y * Chunk.Size + position.Z * Chunk.Size * Chunk.Size] = value;
      else {
        bool oldVal = GetSolid( position );
        Solid[position.X + position.Y * Chunk.Size + position.Z * Chunk.Size * Chunk.Size] = value;

        if ( oldVal != value )
          IsDirty = true;
      }
    }

    public void SetMesh (Chunk.Vertex[] vertices, uint[] triangles) {
      triCount = triangles.Length;

      Log.Debug( this, $"{vertices.Length} vertices, {triangles.Length / 3} triangles" );

      // Generate our vertex arrays, buffer and element buffer.
      vertexArray = GL.GenVertexArray();
      vertexBuffer = GL.GenBuffer();
      elementBuffer = GL.GenBuffer();

      GL.BindVertexArray( vertexArray );
      GL.BindVertexArray( vertexBuffer );
      GL.BindBuffer( BufferTarget.ArrayBuffer, vertexBuffer );

      GL.NamedBufferStorage( vertexBuffer, vertices.Length * Vertex.Size, vertices, BufferStorageFlags.MapWriteBit );

      int aPosition = shader.GetAttribLocation( "aPosition" );
      GL.VertexArrayAttribBinding( vertexArray, aPosition, 0 );
      GL.EnableVertexArrayAttrib( vertexArray, aPosition );
      GL.VertexArrayAttribFormat(
        vertexArray,
        aPosition,                          // The attribute that we are going to set.
        3,                                  // The size of the attribute (3 floats).
        VertexAttribType.Float,             // The type of attribute (floats).
        false,                              // Don't need to normalize as it's already normalized.
        0                                   // The offset of the item.
      );

      int aColour = shader.GetAttribLocation( "aColour" );
      GL.VertexArrayAttribBinding( vertexArray, aColour, 0 );
      GL.EnableVertexArrayAttrib( vertexArray, aColour );
      GL.VertexArrayAttribFormat(
        vertexArray,
        aColour,                          // The attribute that we are going to set.
        3,                                  // The size of the attribute (3 floats).
        VertexAttribType.Float,             // The type of attribute (floats).
        false,                              // Don't need to normalize as it's already normalized.
        12                                  // The offset of the item.
      );

      GL.VertexArrayVertexBuffer( vertexArray, 0, vertexBuffer, System.IntPtr.Zero, Vertex.Size );

      GL.BindBuffer( BufferTarget.ElementArrayBuffer, elementBuffer );
      GL.BufferData( BufferTarget.ElementArrayBuffer, sizeof( uint ) * triangles.Length, triangles, BufferUsageHint.StaticDraw );
    }

    public override void OnStart () {
      GameObject.Transform.Position = ( Vector3 ) Position * Size;

      LoadShader();

      Events.Render += Render;
      Events.UpdateLast += UpdateLast;

      manager.AddChunk( this );
      MeshMaker.UpdateChunk( this );
    }
    public override void OnDestroy () {
      manager.RemoveChunk( this );

      Events.Render -= Render;
      Events.UpdateLast -= UpdateLast;
    }

    void Render () {
      shader.UseProgram( GameObject.Transform.Matrix );

      GL.BindVertexArray( vertexArray );
      GL.DrawElements( PrimitiveType.Triangles, triCount, DrawElementsType.UnsignedInt, 0 );
    }

    void UpdateLast () {
      if ( IsDirty ) {
        IsDirty = false;
        Log.Debug( this, "Chunk is dirty, update the mesh." );
      }
    }

    public struct Vertex {
      public Vector3 Position;
      public Vector3 Colour;

      public const int Size = sizeof( float ) * 6;

      public Vertex (Vector3 Position) {
        this.Position = Position;
        Colour = new Vector3( 1, 1, 0 );
      }
    }
  }
}