using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace KauRock {
	public class KauWindow : GameWindow {

		public Color ClearColor;
		public KauWindow (int width, int height, string title) : base (width, height, GraphicsMode.Default, title) {

		}

		protected override void OnKeyDown (KeyboardKeyEventArgs e) {
			// Quit the game.
			if (e.Key == Key.Escape) {
				CursorVisible = true;
			}

			// Toggle Wireframe.
			if (e.Key == Key.F10) {
				Wireframe.Enabled = !Wireframe.Enabled;
			}

			// Readline.
			if (e.Key == Key.F1) {
				CommandManager.Execute (Console.ReadLine ());
			}
		}

		protected override void OnMouseDown(MouseButtonEventArgs e) {
			CursorVisible = false;
		}

		protected override void OnMouseMove(MouseMoveEventArgs e) {
			if(Focused && !CursorVisible) {
				// Move the mouse to the middle of the window.
				Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
			}
		}

		protected override void OnClosed (EventArgs e) {
			SceneManager.DestroyAll ();

			base.OnClosed (e);
		}

		protected override void OnLoad (EventArgs e) {
			base.OnLoad (e);

			CursorVisible = false;

			GL.ClearColor (ClearColor);
			GL.Enable(EnableCap.DepthTest);

			SceneManager.Start ();
		}

		protected override void OnResize (EventArgs e) {
			GL.Viewport (0, 0, Width, Height);
			Events.InvokeWindowRezie(this);
			base.OnResize (e);
		}

		protected override void OnUpdateFrame (FrameEventArgs e) {
			// Firstly, set the time.
			Time.SetTime ((float) e.Time);

			Events.InvokeUpdateFirst ();
			Events.InvokeUpdate ();
			Events.InvokeUpdateLast ();

			base.OnUpdateFrame (e);
		}

		protected override void OnRenderFrame (FrameEventArgs e) {
			// Clear the screen.
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			// Invoke the Render events.
			Events.InvokeRenderFirst ();
			Events.InvokeRender ();
			Events.InvokeRenderLast ();

			// Swap buffers to show what was just rendered.
			SwapBuffers ();

			base.OnRenderFrame (e);
		}
	}
}