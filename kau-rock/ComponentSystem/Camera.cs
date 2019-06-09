using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
namespace KauRock {
	public class Camera : Component {
		public static Camera ActiveCamera;

		private Transform transform;

		// If any of the following have been changed the view matrix needs to be updated.
		private float zClipFar = 1000;
		// How far away to stop rendering.
		public float FarClip {
			get => zClipFar;
			set {
				zClipFar = value;
				UpdateProjectionMatrix();
			}
		}

		private float zClipNear = 0.1f;
		// How close to the camera do we start rendering.
		public float NearClip {
			get => zClipNear;
			set {
				zClipNear = value;
				UpdateProjectionMatrix();
			}
		}

		private float fovRads = MathHelper.DegreesToRadians(60);
		// The vertical Field Of View of the camera.
		public float FOV {
			get => MathHelper.RadiansToDegrees(fovRads);
			set {
				fovRads = MathHelper.DegreesToRadians(value);
				fovRads = MathHelper.Clamp(fovRads, 1f, MathHelper.Pi);
				UpdateProjectionMatrix();
			}
		}
		private float aspect;
		// The aspect ratio of the output of the camera.
		public float AspectRatio {
			get => aspect;
			set {
				aspect = value;
				UpdateProjectionMatrix();
			}
		}

		Matrix4 projectionMatrix = Matrix4.Identity;
		public Camera(GameObject gameObject, float AspectRatio) : base(gameObject) {
			aspect = AspectRatio;
		}

		public Matrix4 GetViewMatrix() {
			Vector3 forward = transform.Forward;
			return Matrix4.LookAt(transform.Position, transform.Position + forward, Vector3.UnitY);
		}
		public Matrix4 GetProjectionMatrix() => projectionMatrix;

		void UpdateProjectionMatrix() {
			projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fovRads, aspect, zClipNear, zClipFar);
		}

		public override void OnStart() {
			// Update the view matrix when this camera component has started.
			UpdateProjectionMatrix();

			// Warn that a camera will replace the current active camera.
			if(ActiveCamera != null)
				Log.Warning(this, $"Replacing {ActiveCamera.GameObject.Name}'s camera with {GameObject.Name}'s camera as the ActiveCamera.");

			// Set the active camera to this one.
			ActiveCamera = this;

			// subscribe to some events.
			Events.RenderFirst += Render;
			Events.WindowResize += OnResize;

			transform = GameObject.Transform;
		}

		public override void OnDestroy() {
			// Unset the active camera if it's this one.
			if(ActiveCamera == this)
				ActiveCamera = null;

			// Un-subscribe from the events.
			Events.RenderFirst -= Render;
			Events.WindowResize -= OnResize;
		}

		// When the window is resized, update the aspect ratio and matrix.
		void OnResize(KauWindow window) {
			AspectRatio = (float)window.Width / (float)window.Height;
		}

		void Render() {
			if(ActiveCamera == this) {
				
			}
		}
	}
}