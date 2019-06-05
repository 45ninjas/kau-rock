using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace KauRock {
	public class Texture2D {
		public readonly int TextureObject;

		public Texture2D(int glTexture) {
			TextureObject = glTexture;
		}

		public void Use(TextureUnit unit = TextureUnit.Texture0) {
			GL.ActiveTexture(unit);
			GL.BindTexture(TextureTarget.Texture2D, TextureObject);
		}

		public void Destroy() {
			GL.DeleteTexture(TextureObject);
		}
	}
}