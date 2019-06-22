using OpenTK;
namespace KauRock.Utilities {
	public interface IGizmo {
		void Render(Matrix4 view, Matrix4 projection);
	}
}