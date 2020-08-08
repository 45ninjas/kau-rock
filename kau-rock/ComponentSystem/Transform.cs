using OpenTK;

namespace KauRock {
  public class Transform : Component {
    private Vector3 position = Vector3.Zero;
    public Vector3 Position {
      get => position;
      set {
        position = value;
        matrixIsDirty = true;
      }
    }

    private Quaternion rotation = Quaternion.Identity;
    public Quaternion Rotation {
      get => rotation;
      set {
        rotation = value;
        matrixIsDirty = true;
      }
    }
    private Vector3 scale = Vector3.One;
    public Vector3 Scale {
      get => scale;
      set {
        scale = value;
        matrixIsDirty = true;
      }
    }

    public Matrix4 Matrix = Matrix4.Identity;

    private bool matrixIsDirty = false;
    public Transform() {
      UpdateMatrix();
    }
    public Transform(Vector3 Position, Quaternion Rotation, Vector3 Scale){
      this.Position = Position;
      this.Scale = Scale;
      this.Rotation = Rotation;
    }

    public override void OnStart() {
      UpdateMatrix();
      base.OnStart();

      Events.UpdateLast += MatrixDirtyUpdate;
      Events.RenderFirst += MatrixDirtyUpdate;
    }

    public override void OnDestroy() {
      Events.UpdateLast -= MatrixDirtyUpdate;
      Events.RenderFirst -= MatrixDirtyUpdate;
    }

    public void Rotate(float pitch, float yaw, float roll) {
      Rotation *= Quaternion.FromEulerAngles(pitch, yaw, roll);
      matrixIsDirty = true;
    }

    // Directions.
    // public Vector3 Forward => Vector3.Normalize(Rotation * Vector3.UnitZ);
    public Vector3 Forward {
      get {
        Vector4 rt = -Matrix.Column2;
        return new Vector3(rt.X, rt.Y, rt.Z);
        //return Vector3.TransformVector(Vector3.UnitZ, Matrix);
      }
    }
    //public Vector3 Up => Vector3.Normalize(Rotation * Vector3.UnitY);
    public Vector3 Up {
      get {
        //return Vector3.TransformVector(Vector3.UnitY, Matrix);
        Vector4 rt = Matrix.Column1;
        return new Vector3(rt.X, rt.Y, rt.Z);
      }
    }
    //public Vector3 Right => Vector3.Normalize(Rotation * Vector3.UnitX);
    public Vector3 Right {
      get {
        // return Vector3.TransformVector(Vector3.UnitX, Matrix);
        Vector4 rt = Matrix.Column0;
        return new Vector3(rt.X, rt.Y, rt.Z);
      }
    }

    // Update the matrix if it's dirty.
    void MatrixDirtyUpdate() {
      if (matrixIsDirty)
        UpdateMatrix();
    }

    // TODO: Add https://answers.unity.com/questions/467614/what-is-the-source-code-of-quaternionlookrotation.html

    public void UpdateMatrix() {
      // Clear the matrix.
      Matrix = Matrix4.Identity;

      // Rotate, scale then translate the matrix.
      Matrix *= Matrix4.CreateFromQuaternion(Rotation);
      Matrix *= Matrix4.CreateScale(Scale);
      Matrix *= Matrix4.CreateTranslation(Position);

      matrixIsDirty = false;
    }

    public void SetMatrix(Matrix4 matrix) {
      matrixIsDirty = false;

      scale = matrix.ExtractScale();
      rotation = matrix.ExtractRotation();
      position = matrix.ExtractTranslation();

      Matrix = matrix;
    }
  }
}