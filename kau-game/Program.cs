using kauGame.Components;
using KauRock;
using kauGame.Components.Cameras;
using OpenTK;
using KauRock.Terrain;

namespace kauGame {
  // https://coolors.co/002626-0e4749-ffba08-e3e7af-0f1a20
  class Program {
    static void Main(string[] args) {
      KauWindow window = new KauWindow(1240, 720, "Kau Game");

      // Setup the camera object with a Camera and FreeCameMotion Component.
      GameObject camera = new GameObject("Game Camera");
      new Camera(camera, (float)window.Width / (float)window.Height) {
        Sky = new SkyDome(),
        Gizmos = {
          new KauRock.Gizmos.AxisPreview(window, 100, 50)
        }
      };
      new FreeCamMotion(camera) {
        Pitch = -30
      };

      // Place the camera just above top middle chunk.
      camera.Transform.Position = new Vector3(2, 4.1f, 2) * Chunk.Size;

      var terrain = new HeightTerrain("Hello World".GetHashCode());

      // Spawn a 8 x 8 x 4 block of chunks.

      var chunkManager = new ChunkManager();

      VoxPos chunkPos = new VoxPos();
      for (chunkPos.X = 0; chunkPos.X < 4; chunkPos.X++) {
        for (chunkPos.Z = 0; chunkPos.Z < 4; chunkPos.Z++) {
          for (chunkPos.Y = 0; chunkPos.Y < 4; chunkPos.Y++) {

            GameObject chunkGo = new GameObject("Chunk " + chunkPos);
            var chunk = new Chunk(chunkPos, terrain, chunkManager, chunkGo);

          }
        }
      }

      window.ClearColor = KauTheme.Darkest;
      window.Run();
    }
  }
}