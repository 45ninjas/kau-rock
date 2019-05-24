using OpenTK;

namespace KauRock {
    public class Transform : Component {
        public Vector3 Position = Vector3.Zero;
        public Quaternion Rotation = Quaternion.Identity;
        public Vector3 Scale = Vector3.One;

        public Matrix4 Matrix = Matrix4.Identity;

        private bool matrixIsDirty = false;
        public Transform (GameObject gameObject) : base (gameObject) {
            UpdateMatrix();
        }
        public Transform (GameObject gameObject, Vector3 Position, Quaternion Rotation, Vector3 Scale) : base(gameObject) {
            this.Position = Position;
            this.Scale = Scale;
            this.Rotation = Rotation;
            UpdateMatrix();
        }


        void PostUpdate() {
            if(matrixIsDirty)
                UpdateMatrix();
        }

        void PreRender() {
            if(matrixIsDirty)
                UpdateMatrix();
        }

        void UpdateMatrix() {
            Matrix = Matrix4.Identity;

            Matrix *= Matrix4.CreateFromQuaternion(Rotation);
            Matrix *= Matrix4.CreateScale(Scale);
        }
    }
}