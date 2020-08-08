using OpenTK;

using KauGame.Components;
using KauRock;
using KauRock.Terrain;


namespace KauGame {
  // https://coolors.co/002626-0e4749-ffba08-e3e7af-0f1a20
  class Program {
    static void Main (string[] args) {
      KauWindow window = new KauWindow( 1240, 720, "Kau Game" );

      // Setup the camera object with a Camera and a FreeCam Component.
      var camera = new GameObject( "Camera",
        new Camera( ( float ) window.Width / window.Height ) {
          Sky = new SkyDome(),
          Gizmos = {
            new KauRock.Gizmos.AxisPreview( window, 100, 50)
        }
        },
        new FreeCam() {
          Pitch = -30
        }
      );
      camera.Transform.Position = new Vector3( 2, 4.1f, 2 ) * Chunk.Size;

      VoxelScenes.TestScene( "Hello World".GetHashCode() );

      window.ClearColor = KauTheme.Darkest;
      window.Run();
    }
  }
}