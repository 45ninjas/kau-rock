using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;

namespace KauRock {
    public class KauWindow : GameWindow {

        public Color ClearColor;
        public KauWindow (int width, int height, string title) : base ( width, height, GraphicsMode.Default, title) {

        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {

            // Toggle Wireframe.
            if(e.Key == Key.F10) {
                Wireframe.Enabled = !Wireframe.Enabled;
            }

            // Readline.
            if(e.Key == Key.F1) {
                CommandManager.Execute(Console.ReadLine());
            }
        }

        protected override void OnClosed(EventArgs e) {
            SceneManager.DestroyAll();

            base.OnClosed(e);
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            GL.ClearColor(ClearColor);

            SceneManager.Start();
        }

        protected override void OnResize(EventArgs e) {
            GL.Viewport(0,0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            // Firstly, set the time.
            Time.SetTime((float)e.Time);

            Events.InvokeUpdateFirst(); 
            Events.InvokeUpdate(); 
            Events.InvokeUpdateLast(); 

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            // Clear the screen.
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Invoke the Render events.
            Events.InvokeRenderFirst();
            Events.InvokeRender(); 
            Events.InvokeRenderLast(); 

            // Swap buffers to show what was just rendered.
            SwapBuffers();

            base.OnRenderFrame(e);
        }
    }
}