using KauRock;
using OpenTK;
namespace KauRock.Terrain {
	public interface IVoxelProvider {
		float GetAt(VoxPos pos);
		float[] GetChunk(VoxPos pos);
	}
}