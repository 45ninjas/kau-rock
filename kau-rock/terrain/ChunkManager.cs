using KauRock;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace KauRock.Terrain {

  public class ChunkManager {
    private Dictionary<VoxPos, Chunk> loadedChunks = new Dictionary<VoxPos, Chunk>();

    public void AddChunk (Chunk chunk) {
      if ( !loadedChunks.ContainsKey( chunk.Position ) )
        loadedChunks.Add( chunk.Position, chunk );

      else
        Log.Warning( this, $"Chunk {chunk.Position} already exists." );
    }

    public void RemoveChunk (Chunk chunk) {
      if ( loadedChunks.ContainsKey( chunk.Position ) )
        loadedChunks.Remove( chunk.Position );

      else
        Log.Warning( this, $"Chunk {chunk.Position} does not exist." );
    }

    public Chunk GetChunk (VoxPos position) => loadedChunks.GetValueOrDefault( position );
    public bool TryGetChunk (VoxPos position, out Chunk chunk) => loadedChunks.TryGetValue( position, out chunk );

    public VoxPos[] GetNeighbors (Chunk chunk) {
      Stack<VoxPos> neighbors = new Stack<VoxPos>();

      for ( int i = 0; i < 6; i++ ) {
        VoxPos pos = chunk.Position + VoxPos.AllDirections[i];
        if ( loadedChunks.ContainsKey( pos ) )
          neighbors.Push( pos );
      }

      return neighbors.ToArray();
    }
  }
}