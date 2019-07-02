using kauGame.Components;
using KauRock;
using kauGame.Components.Cameras;
using OpenTK;

namespace kauGame {
	// https://coolors.co/002626-0e4749-ffba08-e3e7af-0f1a20
	class Program {
		static void Main (string[] args) {
			KauWindow window = new KauWindow (1240, 720, "Kau Game");

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
			camera.Transform.Position = new Vector3(0, 3, 5);

			GameObject testCube = new GameObject("A Test Cube");
			new Cube(testCube);
			testCube.Transform.Position = new Vector3(0, -2, 0);

			GameObject testChunk = new GameObject("Test Chunk");
			new KauRock.Terrain.Chunk(new VoxPos(0,0,0), null, testChunk);
			
			window.ClearColor = KauTheme.Darkest;
			window.Run ();
		}
	}
}