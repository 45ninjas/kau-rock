using OpenTK;

namespace KauRock {
	public struct VoxPos {
		public int X;
		public int Y;
		public int Z;

		static public implicit operator Vector3(VoxPos pos) {
			return new Vector3(pos.X,pos.Y,pos.Z);
		}
	}
}