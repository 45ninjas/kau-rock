using kauGame.Components;
using KauRock;
using kauGame.Components.Cameras;
using OpenTK;

namespace kauGame {
	// https://coolors.co/002626-0e4749-ffba08-e3e7af-0f1a20
	class Program {
		static void Main (string[] args) {
			KauWindow window = new KauWindow (1240, 720, "Kau Game");

			int dim = 5;
			for (int x = 0; x <= dim; x++) {
				for (int z = 0; z <= dim; z++) {
					for (int y = 0; y <= dim; y++) {
						if(x == 0 || y == 0 || z == 0 || x == dim || y == dim || z == dim) {
							GameObject go = new GameObject ("Test game object");
							var q = new HelloTransform (go);
							go.Transform.Position = (new Vector3(x,y,z) - new Vector3(dim * 0.5f, dim * 0.5f, dim * 0.5f)) * 2;
						}
					}
				}
			}

			GameObject camera = new GameObject("Game Camera");
			var cam = new Camera(camera, (float)window.Width / (float)window.Height);
			var camMotion = new FreeCamMotion(camera);
			// camera.Transform.Position = new Vector3(0,1.5f,-4);
			
			window.ClearColor = KauTheme.Darkest;
			window.Run ();
		}
	}
}