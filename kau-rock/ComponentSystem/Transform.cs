using OpenTK;

namespace KauRock {
	public class Transform : Component {
		public Vector3 Position = Vector3.Zero;
		public Quaternion Rotation = Quaternion.Identity;
		public Vector3 Scale = Vector3.One;

		public Matrix4 Matrix = Matrix4.Identity;

		private bool matrixIsDirty = false;
		public Transform (GameObject gameObject) : base (gameObject) {
			UpdateMatrix ();
		}
		public Transform (GameObject gameObject, Vector3 Position, Quaternion Rotation, Vector3 Scale) : base (gameObject) {
			this.Position = Position;
			this.Scale = Scale;
			this.Rotation = Rotation;
		}

		public override void OnStart () {
			UpdateMatrix ();
			base.OnStart ();

			Events.UpdateLast += MatrixDirtyUpdate;
			Events.RenderFirst += MatrixDirtyUpdate;
		}

		public override void OnDestroy() {
			Events.UpdateLast -= MatrixDirtyUpdate;
			Events.RenderFirst -= MatrixDirtyUpdate;
		}

		public void Rotate(float pitch, float yaw, float roll) {
			Rotation *= Quaternion.FromEulerAngles(pitch,yaw,roll);
			matrixIsDirty = true;
		}

		// Update the matrix if it's dirty.
		void MatrixDirtyUpdate () {
			if (matrixIsDirty)
				UpdateMatrix ();
		}

		void UpdateMatrix () {
			// Clear the matrix.
			Matrix = Matrix4.Identity;

			// Rotate, scale then translate the matrix.
			Matrix *= Matrix4.CreateFromQuaternion (Rotation);
			Matrix *= Matrix4.CreateScale (Scale);
			Matrix *= Matrix4.CreateTranslation(Position);
		}
	}
}