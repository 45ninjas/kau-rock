using OpenTK;
using System.Collections.Generic;
using System.Collections;
using KauRock;
using OpenTK.Graphics.OpenGL4;

namespace KauRock.Terrain {

  public class MeshMaker {

    public struct Triangle {
      public int vertA;
      public int vertB;
      public int vertC;

      // public Vector3 normal;
    }

    [System.Flags]
    public enum Faces {
      None = 0,
      NegX = 1,
      PosX = 2,
      NegY = 4,
      PosY = 8,
      NegZ = 16,
      PosZ = 32
    }

    Chunk activeChunk;

    int vertexCount = 0;
    Stack<VoxPos> vertices = new Stack<VoxPos>();
    Stack<int> triangles = new Stack<int>();

    const int VertexGridSize = Chunk.Size + 1;
    const int VertexGridCube = VertexGridSize * VertexGridSize * VertexGridSize;

    int vertexIndex (VoxPos position) {

      // Check that the position is within the range of the vertex grid.
      if (
        position.X >= VertexGridSize || position.X < 0 ||
        position.Y >= VertexGridSize || position.Y < 0 ||
        position.Z >= VertexGridSize || position.Z < 0
      ) {
        Log.Error( this, $"Vertex position is out of range {position}" );
      }

      return position.X + position.Y * VertexGridSize + position.Z * VertexGridSize * VertexGridSize;
    }

    public void UpdateChunk (Chunk chunk) {

      for ( int i = 0; i < VertexGridSize; i++ ) {
      }

      // Clear the vertices, triangles and vertex count.
      vertices.Clear();
      triangles.Clear();
      vertexCount = 0;

      activeChunk = chunk;
      VoxPos position = new VoxPos();
      for ( position.X = 0; position.X < Chunk.Size; position.X++ ) {
        for ( position.Z = 0; position.Z < Chunk.Size; position.Z++ ) {
          for ( position.Y = 0; position.Y < Chunk.Size; position.Y++ ) {
            March( position );
          }
        }
      }

      Vector3[] verts = new Vector3[vertices.Count];
      uint[] tris = new uint[triangles.Count];

      // Set the new mesh on the chunk.
      // chunk.SetMesh(vertices.ToArray(), triangles.ToArray());
    }
    void March (VoxPos pos) {
      // Left face.
      if ( !isSolid( pos + VoxPos.Left ) ) {
        triangle(pos, 1, 5, 4);
        triangle(pos, 1, 4, 0);
      }

      // Right face.
      if ( !isSolid( pos + VoxPos.Right ) ) {
        // triangle(pos, 3, 7, 6);
        // triangle(pos, 3, 6, 2);
      }

      // Top face.
      if ( !isSolid( pos + VoxPos.Up ) ) {
        // triangle(pos, 6, 7, 4);
        // triangle(pos, 6, 4, 5);
      }

      // Bottom face.
      if ( !isSolid( pos + VoxPos.Down ) ) {
        // triangle(pos, 3, 2, 1);
        // triangle(pos, 3, 1, 0);
      }

      // Front face.
      if ( !isSolid( pos + VoxPos.Forward ) ) {
        // triangle(pos, 4, 7, 3);
        // triangle(pos, 4, 3, 0);
      }

      // Back face.
      if ( !isSolid( pos + VoxPos.Backwards ) ) {
        // triangle(pos, 6, 5, 1);
        // triangle(pos, 6, 1, 2);
      }
    }
    public void doCube (VoxPos pos, Chunk chunk) {

      // Left face.
      if ( !isSolid( pos + VoxPos.Left ) ) {
        // triangle(pos, 1, 5, 4);
        // triangle(pos, 1, 4, 0);
      }

      // Right face.
      if ( !isSolid( pos + VoxPos.Right ) ) {
        // triangle(pos, 3, 7, 6);
        // triangle(pos, 3, 6, 2);
      }

      // Top face.
      if ( !isSolid( pos + VoxPos.Up ) ) {
        // triangle(pos, 6, 7, 4);
        // triangle(pos, 6, 4, 5);
      }

      // Bottom face.
      if ( !isSolid( pos + VoxPos.Down ) ) {
        // triangle(pos, 3, 2, 1);
        // triangle(pos, 3, 1, 0);
      }

      // Front face.
      if ( !isSolid( pos + VoxPos.Forward ) ) {
        // triangle(pos, 4, 7, 3);
        // triangle(pos, 4, 3, 0);
      }

      // Back face.
      if ( !isSolid( pos + VoxPos.Backwards ) ) {
        // triangle(pos, 6, 5, 1);
        // triangle(pos, 6, 1, 2);
      }
    }

    bool isSolid (VoxPos position) {

      // return false;

      // If we are not on the surface of a chunk just quickly get the value.
      if ( position.X >= 0 && position.X < Chunk.Size &&
      position.Y >= 0 && position.Y < Chunk.Size &&
      position.Z >= 0 && position.Z < Chunk.Size ) {
        return activeChunk.GetSolid( position );
      }

      return true;
    }

    VoxPos[] corner = new VoxPos[] {
				// Bottom Face.
				new VoxPos(0, 0, 1),
        new VoxPos(0, 0, 0),
        new VoxPos(1, 0, 0),
        new VoxPos(1, 0, 1),
				// Top Face.
				new VoxPos(0, 1, 1),
        new VoxPos(0, 1, 0),
        new VoxPos(1, 1, 0),
        new VoxPos(1, 1, 1),
      };

    // public void Cube(VoxPos pos, Faces visiableFaces) {
    // 	if(visiableFaces.HasFlag(Faces.NegX)) {
    // 		triangle(pos, 1, 5, 4);
    // 		triangle(pos, 1, 4, 0);
    // 	}
    // 	if(visiableFaces.HasFlag(Faces.PosX)) {
    // 		triangle(pos, 3, 7, 6);
    // 		triangle(pos, 3, 6, 2);
    // 	}
    // 	if(visiableFaces.HasFlag(Faces.NegY)){
    // 		triangle(pos, 3, 2, 1);
    // 		triangle(pos, 3, 1, 0);
    // 	}
    // 	if(visiableFaces.HasFlag(Faces.PosY)) {
    // 		triangle(pos, 6, 7, 4);
    // 		triangle(pos, 6, 4, 5);
    // 	}
    // 	if(visiableFaces.HasFlag(Faces.NegZ)) {
    // 		triangle(pos, 6, 5, 1);
    // 		triangle(pos, 6, 1, 2);
    // 	}
    // 	if(visiableFaces.HasFlag(Faces.PosZ)) {
    // 		triangle(pos, 4, 7, 3);
    // 		triangle(pos, 4, 3, 0);
    // 	}
    // }
    private void triangle(VoxPos position, int a, int b, int c) {
    	triangles.Push(getVertex(position + corner[a]));
    	triangles.Push(getVertex(position + corner[b]));
    	triangles.Push(getVertex(position + corner[c]));
    }
    private int getVertex(VoxPos position) {
    	int index = position.X + position.Y * LookupSize + position.Z * LookupSize * LookupSize;

    	// If the vert does not exist, add it.
    	 if(vertLookup[index] < 0) {
    		vertLookup[index] = verticesCount++;
    		vertices.Push(new Chunk.Vertex(position));
    	}

    	return vertLookup[index];
    }
  }
}