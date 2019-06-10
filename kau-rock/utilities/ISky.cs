using OpenTK;
namespace KauRock.Utilities {
	public interface ISky {
		void Render(Matrix4 view, Matrix4 projection);
	}
}