using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace KauRock {
  public class KauWindow : GameWindow {

    public Color ClearColor;
    public KauWindow (int width, int height, string title) : base( width, height, GraphicsMode.Default, title ) {

    }

    protected override void OnKeyDown (KeyboardKeyEventArgs e) {
      // Quit the game.
      if ( e.Key == Key.Escape ) {
        CursorVisible = true;
      }

      // Toggle Wireframe.
      if ( e.Key == Key.F10 ) {
        Wireframe.Enabled = !Wireframe.Enabled;
      }

      // Readline.
      if ( e.Key == Key.F1 ) {
        CommandManager.Execute( Console.ReadLine() );
      }

      // Shift time scale.
      if ( e.Modifiers == KeyModifiers.Alt ) {
        if ( e.Key == Key.Minus ) {
          Time.Scale -= 0.1f;
          Log.Debug( this, $"Time scale: {Time.Scale}" );
        }
        if ( e.Key == Key.Plus ) {
          Time.Scale += 0.1f;
          Log.Debug( this, $"Time scale: {Time.Scale}" );
        }
      }
    }

    protected override void OnMouseDown (MouseButtonEventArgs e) {
      CursorVisible = false;
    }

    protected override void OnMouseMove (MouseMoveEventArgs e) {
      if ( Focused && !CursorVisible ) {
        // Move the mouse to the middle of the window.
        Mouse.SetPosition( X + Width / 2f, Y + Height / 2f );
      }
    }

    protected override void OnClosed (EventArgs e) {
      SceneManager.DestroyAll();

      base.OnClosed( e );
    }

    protected override void OnLoad (EventArgs e) {
      base.OnLoad( e );

      // Enable Debugging open GL.
      GL.Enable( EnableCap.DebugOutput );
      GL.Enable( EnableCap.DebugOutputSynchronous );
      GL.DebugMessageCallback( GL_ErrorCallback, IntPtr.Zero );
      GL.DebugMessageControl( DebugSourceControl.DontCare, DebugTypeControl.DontCare, DebugSeverityControl.DontCare, 0, new int[0], true );

      CursorVisible = false;

      GL.ClearColor( ClearColor );
      GL.Enable( EnableCap.DepthTest );

      GL.Enable( EnableCap.CullFace );
      GL.FrontFace( FrontFaceDirection.Cw );
      GL.CullFace( CullFaceMode.Back );

      SceneManager.Start();
    }

    private void GL_ErrorCallback (DebugSource source, DebugType type, int id, DebugSeverity severity, int lenght, IntPtr message, IntPtr userParam) {

      Log.Level level;

      switch ( severity ) {
        case DebugSeverity.DebugSeverityHigh:
          level = Log.Level.Error;
          break;
        case DebugSeverity.DebugSeverityMedium:
          level = Log.Level.Error;
          break;
        case DebugSeverity.DebugSeverityLow:
          level = Log.Level.Warning;
          break;
        default:
          level = Log.Level.Info;
          break;
      }
      string strMessage = null;
      if ( message != IntPtr.Zero ) {
        strMessage = System.Runtime.InteropServices.Marshal.PtrToStringUTF8( message );
      }
      Log.Print( level, strMessage, $"GL {source}" );
    }

    protected override void OnResize (EventArgs e) {
      GL.Viewport( 0, 0, Width, Height );
      Events.InvokeWindowRezie( this );
      base.OnResize( e );
    }

    protected override void OnUpdateFrame (FrameEventArgs e) {
      // Firstly, set the time.
      Time.SetTime( ( float ) e.Time );

      Events.InvokeUpdateFirst();
      Events.InvokeUpdate();
      Events.InvokeUpdateLast();

      base.OnUpdateFrame( e );
    }

    protected override void OnRenderFrame (FrameEventArgs e) {
      // Clear the screen.
      GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
      // Invoke the Render events.
      Events.InvokeRenderFirst();
      Events.InvokeRender();
      Events.InvokeRenderLast();

      // Swap buffers to show what was just rendered.
      SwapBuffers();

      base.OnRenderFrame( e );
    }
  }
}