using kauGame.Components;
using KauRock;
using KauRock.Terrain;

namespace KauRock {
  public static class VoxelScenes {
    public static GameObject TestScene (int seed) {

      // Create the terrain, chunk manager and Chunks gameobject.
      var terrain = new HeightTerrain(seed);
      var manager = new ChunkManager();
      var Chunks = new GameObject("Chunks", terrain, manager);

      // Fill up the world with a grid of 4x4x4 chunks.
      VoxPos chunkPos = new VoxPos();
      for ( chunkPos.X = 0; chunkPos.X < 4; chunkPos.X++ ) {
        for ( chunkPos.Z = 0; chunkPos.Z < 4; chunkPos.Z++ ) {
          for ( chunkPos.Y = 0; chunkPos.Y < 4; chunkPos.Y++ ) {
            var chunk = new Chunk(chunkPos, terrain, manager);
            new GameObject( Chunks, $"Chunk {chunkPos}", chunk);
          }
        }
      }
      return Chunks;
    }
  }
}