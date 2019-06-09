using System.Collections.Generic;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Drawing.Imaging;

namespace KauRock.Loaders {
	// Shader naming convention for this project is quite simple, [shader-name].[type].glsl.
	// Type can be found in the NameToType method.

	public class Texture : ILoader<Texture2D> {

		public bool GenerateMipmap = true;

		public TextureWrapMode WrapMode = TextureWrapMode.Repeat;
		public TextureMagFilter Filter = TextureMagFilter.Linear;

		public Color BorderColor = Color.Magenta;

		public Texture2D Load(string path) {

			// Error out if the file does not exist.
			if(!File.Exists(path))
				throw new System.IO.FileNotFoundException ($"Unable to find the specified file. {path}");

			// Get the file as a bitmap then flip it vertically.
			System.Drawing.Bitmap image = new System.Drawing.Bitmap(path);
			image.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);

			// Get some data from the image.
			BitmapData data = image.LockBits(
				new System.Drawing.Rectangle(0,0,image.Width, image.Height),
				System.Drawing.Imaging.ImageLockMode.ReadOnly,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			// Create our texture.
			int textureID = GL.GenTexture();
			GL.TexImage2D(
				TextureTarget.Texture2D,											// The Type of texture we are creating.
				0,																						// The mip level of the texture.
				PixelInternalFormat.Rgba,											// The internal pixel format.
				data.Width, data.Height,											// The width and height of the texture.
				0,																						// The border of the texture.
				OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,			// The format of our pixles from the data.
				PixelType.UnsignedByte,												// The type used in our data.
				data.Scan0																		// The actual data to use in our texture.
			);

			// Set the X and Y wrapping modes of the texture.
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)WrapMode);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)WrapMode);

			// Set the filters.
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)Filter);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)Filter);

			// Set the border colour if it exists and is needed.
			if(WrapMode == TextureWrapMode.ClampToBorder) {
				float[] border = {
					BorderColor.R,
					BorderColor.G,
					BorderColor.B,
					BorderColor.A
				};

				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, border);
			}

			// Generate the mipmaps.
			if(GenerateMipmap)
				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

			image.UnlockBits(data);

			// Return a new texture object.
			return new Texture2D(textureID);
		}

		public void Dispose() {
			// The texture has finished loading.
		}
	}
}