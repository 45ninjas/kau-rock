using KauRock;
using OpenTK;
using OpenTK.Input;
using MathF = System.MathF;

namespace kauGame.Components.Cameras {
	public class FreeCamMotion : Component {
		public float MoveSpeed = 5;
		public float MinPitch = -89f;
		public float MaxPitch = 89f;

		public float Sensitivity = 0.1f;

		public bool UseUnscaledTime = true;

		private Transform transform;

		private Vector2 mousePos;
		private bool firstFrame = true;

		private float pitch;
		private float yaw;

		private float Pitch {
			get => MathHelper.RadiansToDegrees(pitch);
			set => pitch = MathHelper.DegreesToRadians(MathHelper.Clamp(value, MinPitch, MaxPitch));
		}
		private float Yaw {
			get => MathHelper.RadiansToDegrees(yaw);
			set => yaw = MathHelper.DegreesToRadians(value);
		}

		public FreeCamMotion(GameObject gameObject) : base(gameObject) {
		}

		public override void OnStart() {
			Events.Update += Update;
			transform = GameObject.Transform;
		}
		public override void OnDestroy() {
			Events.Update -= Update;
		}

		void UpdateInputs(float timeDelta, out Vector3 moveVector) {
			// Get the keyboard state and convert WASD into a vector.
			var input = Keyboard.GetState();

			moveVector = Vector3.Zero;
			if(input.IsKeyDown(Key.W))
				moveVector += transform.Forward;
			if(input.IsKeyDown(Key.S))
				moveVector -= transform.Forward;
			if(input.IsKeyDown(Key.A))
				moveVector -= transform.Right;
			if(input.IsKeyDown(Key.D))
				moveVector += transform.Right;

			if(input.IsKeyDown(Key.Down)) {
				pitch += Sensitivity * timeDelta * 10;
			}
			if(input.IsKeyDown(Key.Up)) {
				pitch -= Sensitivity * timeDelta * 10;
			}
			if(input.IsKeyDown(Key.Left)) {
				yaw -= Sensitivity * timeDelta * 10;
			}
			if(input.IsKeyDown(Key.Right)) {
				yaw += Sensitivity * timeDelta * 10;
			}

			// Normalize the input if it's bigger than 1 squared. (z and x motion).
			if(moveVector.LengthSquared > 1.4f)
				moveVector.NormalizeFast();

			// Do mouse inputs.
			var mouseInput = Mouse.GetState();

			// If this is the first frame, set the mouse input to the current input.
			if(firstFrame) {
				mousePos = new Vector2(mouseInput.X, mouseInput.Y);
				firstFrame = false;
			}

			// Get the mouse delta.
			float deltaX = mouseInput.X - mousePos.X;
			float deltaY = mouseInput.Y - mousePos.Y;

			// Update the mouse position for next frame.
			mousePos.X = mouseInput.X;
			mousePos.Y = mouseInput.Y;

			// Set the yaw and pitch while clamping pitch.
			Yaw += deltaX * Sensitivity;
			Pitch += deltaY * Sensitivity;
		}

		void Update() {
			// Set the time delta to unscaled or scaled depending on what the UseUnscaledTime is at.
			float timeDelta = ((UseUnscaledTime)? Time.UnscaledDelta : Time.Delta);

			// Get the inputs from the keyboard and mouse.
			UpdateInputs(timeDelta, out Vector3 moveVector);

			// Rotate the camera based on the pitch and yaw.
			var rotation = Quaternion.FromEulerAngles(pitch, yaw, 0);

			// Set the rotation of the transform.
			transform.Rotation = rotation;

			// Move the camera.
			transform.Position += moveVector * MoveSpeed * timeDelta;
		}
	}
}