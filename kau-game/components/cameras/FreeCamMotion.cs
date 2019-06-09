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

		public FreeCamMotion(GameObject gameObject) : base(gameObject) {
		}

		public override void OnStart() {
			Events.Update += Update;
			transform = GameObject.Transform;
		}
		public override void OnDestroy() {
			Events.Update -= Update;
		}

		void Update() {
			// Set the time delta to unscaled or scaled depending on what the UseUnscaledTime is at.
			float timeDelta = ((UseUnscaledTime)? Time.UnscaledDelta : Time.Delta);


			// Get the keyboard state and convert WASD into a vector.
			var input = Keyboard.GetState();
			Vector3 inputVector = Vector3.Zero;
			if(input.IsKeyDown(Key.W))
				inputVector.Z += 1;
			if(input.IsKeyDown(Key.S))
				inputVector.Z -= 1;
			if(input.IsKeyDown(Key.A))
				inputVector.X += 1;
			if(input.IsKeyDown(Key.D))
				inputVector.X -= 1;

			if(input.IsKeyDown(Key.Down)) {
				pitch += 30 * timeDelta;
			}
			if(input.IsKeyDown(Key.Up)) {
				pitch -= 30 * timeDelta;
			}
			if(input.IsKeyDown(Key.Left)) {
				yaw += 30 * timeDelta;
			}
			if(input.IsKeyDown(Key.Right)) {
				yaw -= 30 * timeDelta;
			}

			// Normalize the input if it's bigger than 1 squared. (z and x motion).
			if(inputVector.LengthSquared > 1.4f)
				inputVector.NormalizeFast();

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
			yaw -= deltaX * Sensitivity;
			pitch = MathHelper.Clamp(pitch + deltaY * Sensitivity, MinPitch, MaxPitch);
			
			Quaternion yawQat = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(yaw));
			Quaternion pitchQat = Quaternion.FromAxisAngle(transform.Right, MathHelper.DegreesToRadians(pitch));
			transform.Rotation = Quaternion.Multiply(pitchQat, yawQat).Normalized();
			// transform.Rotation = pitchQat;
			// transform.Rotation = yawQat;

			transform.UpdateMatrix();

			// Rotate the input vector by the camera's rotation then translate the camera.
			Vector3 moveVector = Vector3.TransformNormal(inputVector, transform.Matrix);
			//transform.Position += moveVector * MoveSpeed * timeDelta;
		}
	}
}