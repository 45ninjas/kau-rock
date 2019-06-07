using kauGame.Components;
using KauRock;

namespace kauGame {
	// https://coolors.co/002626-0e4749-ffba08-e3e7af-0f1a20
	class Program {
		static void Main (string[] args) {
			KauWindow window = new KauWindow (1240, 720, "Kau Game");

			GameObject test = new GameObject ("Test game object");
			var quad = new HelloTransform (test);
			
			window.ClearColor = KauTheme.Darkest;
			window.Run (60);
		}
	}
}