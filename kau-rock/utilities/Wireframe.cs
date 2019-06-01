using OpenTK.Graphics.OpenGL4;
namespace KauRock {
	public static class Wireframe {
		private static bool enabled = false;
		public static bool Enabled {
			get {
				return enabled;
			}
			set {
				enabled = value;
				SetEnabled (value);
			}
		}

		static Wireframe () {
			CommandManager.Add ("wireframe", wireframeCommand);
		}

		[CommandInfo ("Toggles wireframe mode on or off")]
		private static string wireframeCommand (params string[] args) {

			bool status = Enabled;

			// Only set wireframe if there is at least one arg, the arg can be parsed into a bool
			// and the input is different to the existing value.
			if (args.Length > 0 && bool.TryParse (args[0], out status) && status != Enabled)
				SetEnabled (status);

			// Or if nothing was sent, just toggle it.
			else if (args.Length == 0)
				SetEnabled (!status);

			return Enabled ? "Wireframe On" : "Wireframe Off";
		}

		public static void SetEnabled (bool value) {
			Log.Debug (typeof (Wireframe).Name, value ? "Enabled" : "Disabled");
			if (value)
				GL.PolygonMode (MaterialFace.FrontAndBack, PolygonMode.Line);
			else
				GL.PolygonMode (MaterialFace.FrontAndBack, PolygonMode.Fill);
		}
	}
}