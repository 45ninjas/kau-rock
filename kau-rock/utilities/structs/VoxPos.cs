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

		override public string ToString() => $"({X}, {Y}, {Z})";
		
		public static implicit operator Vector3(VoxPos pos) {
			return new Vector3(pos.X,pos.Y,pos.Z);
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