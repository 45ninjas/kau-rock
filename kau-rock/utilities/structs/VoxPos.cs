using OpenTK;

namespace KauRock {
	public struct VoxPos {
		public int X;
		public int Y;
		public int Z;

		public VoxPos(int X, int Y, int Z) {
			this.X = X;
			this.Y = Y;
			this.Z = Z;
		}

		public static readonly VoxPos[] AllDirections = {
			new VoxPos(-1, 0, 0),		// Left.
			new VoxPos(1, 0, 0),		// Right.
			new VoxPos(0, -1, 0),		// Down.
			new VoxPos(0, 1, 0),		// Up.
			new VoxPos(0, 0, -1),		// Backwards.
			new VoxPos(0, 0, 1),		// Forward.
		};

		public static VoxPos Left => AllDirections[0];
		public static VoxPos Right => AllDirections[1];
		public static VoxPos Down => AllDirections[2];
		public static VoxPos Up => AllDirections[3];
		public static VoxPos Backwards => AllDirections[4];
		public static VoxPos Forward => AllDirections[5];

		override public string ToString() => $"({X}, {Y}, {Z})";

		override public bool Equals(object obj) {
			if(obj == null)
				return false;
			
			if(this.GetType() != obj.GetType())
				return false;
			
			VoxPos p = (VoxPos)obj;
			return this.X == p.X && this.Y == p.Y && this.Z == p.Z;
		}
		
		public static implicit operator Vector3(VoxPos pos) {
			return new Vector3(pos.X,pos.Y,pos.Z);
		}
		public static bool operator == (VoxPos a, VoxPos b) {
			return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
		}
		public static bool operator != (VoxPos a, VoxPos b) {
			return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
		}
		public static VoxPos operator + (VoxPos a, VoxPos b) {
			return new VoxPos(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}
		public static VoxPos operator - (VoxPos a, VoxPos b) {
			return new VoxPos(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}
		public static VoxPos operator * (VoxPos a, VoxPos b) {
			return new VoxPos(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		}
		public static VoxPos operator * (VoxPos a, int b) {
			return new VoxPos(a.X * b, a.Y * b, a.Z * b);
		}
		public static VoxPos operator / (VoxPos a, VoxPos b) {
			return new VoxPos(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
		}
		public static VoxPos operator / (VoxPos a, int b) {
			return new VoxPos(a.X / b, a.Y / b, a.Z / b);
		}
	}
}