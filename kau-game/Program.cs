using kauGame.Components;
using KauRock;
using kauGame.Components.Cameras;
using OpenTK;

namespace kauGame {
	// https://coolors.co/002626-0e4749-ffba08-e3e7af-0f1a20
	class Program {
		static void Main (string[] args) {
			KauWindow window = new KauWindow (1240, 720, "Kau Game");

			GameObject companionCube = new GameObject("Companion Cube Fiend");
			var cube = new Cube(companionCube);
			companionCube.Transform.Position = new Vector3(0, -2, 0);

			GameObject camera = new GameObject("Game Camera");
			var cam = new Camera(camera, (float)window.Width / (float)window.Height);
			var camMotion = new FreeCamMotion(camera);
			camera.Transform.Position = new Vector3(0, 0, 5);
			cam.Sky = new SkyDome();
			
			window.ClearColor = KauTheme.Darkest;
			window.Run ();
		}
	}
}