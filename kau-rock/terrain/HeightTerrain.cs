using OpenTK;

namespace KauRock.Terrain {
  public class HeightTerrain : IVoxelProvider {
    public readonly Noise.Value NoiseFunction;

    public readonly float NoiseScale = 0;

    readonly float persistance = 0.5f;
    readonly float lacunarity = 3f;
    readonly int octaves = 6;

    public readonly float HeightScale = 32f;
    public readonly float WorldScale = 0.04f;

    public HeightTerrain (int seed) {
      NoiseFunction = new Noise.Value( seed );
      NoiseScale = NoiseFunction.GetScale( persistance, lacunarity, octaves );
    }

    public float GetAt (VoxPos pos) {
      Vector2 position = new Vector2( pos.X, pos.Z ) * WorldScale;
      float height = NoiseFunction.GetAt( position, persistance, lacunarity, octaves ) * HeightScale;

      if ( height > pos.Z )
        return 1f;
      else
        return 0f;
    }

    public float[] GetChunk (VoxPos chunkPos) {
      var values = new float[Chunk.Size * Chunk.Size * Chunk.Size];

      var internalPos = new VoxPos();

      for ( internalPos.X = 0; internalPos.X < Chunk.Size; internalPos.X++ ) {
        for ( internalPos.Z = 0; internalPos.Z < Chunk.Size; internalPos.Z++ ) {

          Vector2 position = new Vector2( internalPos.X + (chunkPos.X * Chunk.Size), internalPos.Z + (chunkPos.Z * Chunk.Size) ) * WorldScale;
          float height = NoiseFunction.GetAt( position, persistance, lacunarity, octaves );

          for ( internalPos.Y = 0; internalPos.Y < height; internalPos.Y++ ) {
            VoxPos pos = internalPos + (chunkPos * Chunk.Size);
            values[pos.X + pos.Y * Chunk.Size + pos.Z * Chunk.Size * Chunk.Size] = 1f;
          }
        }
      }

      return values;
    }
  }
}