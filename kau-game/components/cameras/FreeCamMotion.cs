using KauRock;
using OpenTK;
using OpenTK.Input;
using MathF = System.MathF;

namespace kauGame.Components.Cameras {
  public class FreeCamMotion : Component {

    // How sensitive the mouse is.
    public float Sensitivity = 0.1f;

    // How fast the camera speeds up and slows down.
    public float Drag = 12f;
    public float Acceleration = 1;

    // The limits for how low and high the player can look.
    public float MinPitch = -89f;
    public float MaxPitch = 89f;

    // The pitch and yaw of the camera.
    private float pitchRads = 0;
    public float Pitch {
      get => MathHelper.RadiansToDegrees(pitchRads);
      set => pitchRads = MathHelper.DegreesToRadians(MathHelper.Clamp(value, MinPitch, MaxPitch));
    }

    private float yawRads = 0;
    public float Yaw {
      get => MathHelper.RadiansToDegrees(yawRads);
      set => yawRads = MathHelper.DegreesToRadians(value);
    }

    private Transform transform;

    private Vector2 lastMousePos;
    private bool firstFrame = true;

    private Vector3 velocity;

    public FreeCamMotion(GameObject gameObject) : base(gameObject) {
    }

    public override void OnStart() {
      Events.Update += Update;
      transform = GameObject.Transform;
    }
    public override void OnDestroy() {
      Events.Update -= Update;
    }

    Vector3 UpdateInputs() {
      // Move the camera in the local X and Z directions then normalize
      // it if it's sqr root is bigger than 1.4 to stop faster motion
      // while moving in two axises.
      Vector3 moveVector = Vector3.Zero;
      var input = Keyboard.GetState();
      if (input.IsKeyDown(Key.W))
        moveVector += transform.Forward;        // Move forward.
      if (input.IsKeyDown(Key.S))
        moveVector -= transform.Forward;        // Move backward.
      if (input.IsKeyDown(Key.A))
        moveVector -= transform.Right;          // Move left.
      if (input.IsKeyDown(Key.D))
        moveVector += transform.Right;          // Move right.

      if (moveVector.LengthSquared > 1.4f)
        moveVector.Normalize();

      // Get the mouse input and the alternative keyboard arrow keys.
      var mouseInput = Mouse.GetState();

      if (input.IsKeyDown(Key.Down))
        Pitch += Sensitivity * Time.UnscaledDelta * 250;  // Look down.
      if (input.IsKeyDown(Key.Up))
        Pitch -= Sensitivity * Time.UnscaledDelta * 250;  // Look up.
      if (input.IsKeyDown(Key.Left))
        Yaw -= Sensitivity * Time.UnscaledDelta * 250;    // Look left.
      if (input.IsKeyDown(Key.Right))
        Yaw += Sensitivity * Time.UnscaledDelta * 250;    // Look right.


      // On the first frame set the lastMousePos to the current mouse position.
      // This is done to stop the camera jumping when the game loads as the delta
      // will be extremely high.
      if (firstFrame) {
        lastMousePos = new Vector2(mouseInput.X, mouseInput.Y);
        firstFrame = false;
      }

      // Add yaw and pitch to the camera based on the delta of the mouse and update
      // the lastMousePos so we can get the delta next frame.
      Yaw += (mouseInput.X - lastMousePos.X) * Sensitivity;
      Pitch -= (mouseInput.Y - lastMousePos.Y) * Sensitivity;

      lastMousePos.X = mouseInput.X;
      lastMousePos.Y = mouseInput.Y;

      return moveVector;
    }

    void Update() {
      // Update the inputs and rotate the camera.
      var moveVector = UpdateInputs();
      transform.Rotation = Quaternion.FromEulerAngles(-pitchRads, yawRads, 0);

      // Add our moveVector to our velocity then apply drag. Finally apply the
      // velocity to our position.
      velocity += moveVector * Acceleration * Time.UnscaledDelta;
      velocity -= velocity * Drag * Time.UnscaledDelta;
      transform.Position += velocity;
    }
  }
}