using System;

namespace KauRock {
  public static class Events {
    public delegate void Frame ();
    public delegate void WindowChange (KauWindow window);

    public static event Frame UpdateFirst = null;
    public static event Frame Update = null;
    public static event Frame UpdateLast = null;

    internal static void InvokeUpdateFirst () => UpdateFirst?.Invoke();
    internal static void InvokeUpdate () => Update?.Invoke();
    internal static void InvokeUpdateLast () => UpdateLast?.Invoke();

    public static event Frame RenderFirst = null;
    public static event Frame Render = null;
    public static event Frame RenderLast = null;

    internal static void InvokeRenderFirst () => RenderFirst?.Invoke();
    internal static void InvokeRender () => Render?.Invoke();
    internal static void InvokeRenderLast () => RenderLast?.Invoke();

    public static event WindowChange WindowResize = null;
    internal static void InvokeWindowRezie (KauWindow window) => WindowResize?.Invoke( window );
  }
}