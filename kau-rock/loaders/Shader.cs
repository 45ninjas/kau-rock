using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using System.Collections.Generic;

namespace KauRock.Loaders {
	// Shader naming convention for this project is quite simple, [shader-name].[type].glsl.
	// Type can be found in the NameToType method.

    public class Shader : System.IDisposable{
		public Dictionary<string, int> shaderObjects = new Dictionary<string, int>();

		// Every time this Load function is called we create a new shader. This Loader class
		// is setup to load and create multiple different shaders. In the future it's possible
		// that some shaders might share the same shader files. To not have to re-compile these
		// shared shaders the shaderObjects dicotnary exist. These shader objects will stay in
		// memory until this object is destroyed.

		// Firstly, we clean up the shaderFile path and remove the last two extensions. We then
		// create a list of shader objects to set to the new ShaderProgram named newShaderObject.

		// Next, we iterate over each file that match [basename].*.glsl. If shaderObjects
		// contains the shader file we are looking at we have already loaded and compiled it.
		// We just add the shader object to the list of newShaderObjects. If not, we use
		// CreateShaderObject.

		// Finally we create and return our shiny new new ShaderProgram object using the list of
		// newShaderObjects in it's constructor. The ShaderProgram will deal with attaching, 
		// linking, and detaching.

        public ShaderProgram Load(string shaderFile) {

			Log.Info(this, $"Loading {shaderFile}");

			// Throw an exception it's nonexistant or the file extension is not glsl.
			if(File.Exists(shaderFile))
				throw new System.IO.FileNotFoundException();

			if(Path.GetExtension(shaderFile) != ".glsl")
				throw new System.IO.FileLoadException("The provided file does not have glsl as it's extension.");

			// Strip the extension from the filename, if the file still has an extension, remove it too.
			string filename = Path.GetFileNameWithoutExtension(shaderFile);

			if(Path.GetExtension(filename) != string.Empty)
				filename = Path.GetFileNameWithoutExtension(filename);

			Stack<int> newShaderObjects = new Stack<int>();

			// Get all files that match the shader-name.*.glsl pattern and iterate over them.
			string[] files = Directory.GetFiles(Path.GetDirectoryName(shaderFile), filename + ".*.glsl", SearchOption.TopDirectoryOnly);
			foreach(string file in files) {

				// Add the existing shader object if it exists.
				if(shaderObjects.TryGetValue(file, out int existingShaderObject)) {
					newShaderObjects.Push(existingShaderObject);
				}

				else {
					// The existing shader file does not exist, Create it.
					if(NameToType(file, out var shaderType)) {

						// Create our new shader object and add it to the list of shader objects
						// to send the new Shader
						newShaderObjects.Push(CreateShaderObject(file, shaderType));
					}
					else
						Log.Warning(this, $"{file} does not match any known shader formats. Skipping.");
				}
			}

			// Lastly, create our new shaderProgram with the newShaderObjects list and return.
			return new ShaderProgram(newShaderObjects.ToArray());
        }

		public int CreateShaderObject(string filename, ShaderType type) {

			// Read the file into a string with UTF8 encoding.
			string source = File.ReadAllText(filename, System.Text.UTF8Encoding.UTF8);

			// Create the new shader object, tell openGL where the string is and compile the shader.
			int shaderObject = GL.CreateShader(type);

			GL.ShaderSource(shaderObject, source);
			GL.CompileShader(shaderObject);

			Log.Info(this, $"Creating a {type} shader from {filename}.");

			// Get the compile status of the shader object. If there is one, get the InfoLog for the
			// shader object and throw an exception.
			
			GL.GetShader(shaderObject, ShaderParameter.CompileStatus, out var compileCode);
			if(compileCode != (int)All.True) {
				string info = GL.GetShaderInfoLog(shaderObject);
				throw new System.Exception($"Shader compile error:({filename})", new System.Exception(info));
			}

			// Wooh, the shader has successfully compiled, add it the the list of shaderObject in-case
			// we need to use it again.
			shaderObjects.Add(filename, shaderObject);

			return shaderObject;
		}

		// When this object is destroyed (finished loading if using a using block) tell
		// open gl to delete the shaderObjects we've accumulated.
		public void Dispose() {
			foreach(int shaderObject in shaderObjects.Values) {
				GL.DeleteShader(shaderObject);
			}
		}

		public static bool NameToType(string file, out ShaderType type) {

			// Elimate the .glsl extension of the file.
			string filename = Path.GetFileNameWithoutExtension(file);

			// Extract the next extension of the file.
			string extension = Path.GetExtension(filename);

			switch (extension)
			{
				case ".vt":
				case ".vrt":
				case ".vert":
					type = ShaderType.VertexShader;
					return true;
				case ".fg":
				case ".frg":
				case ".frag":
					type = ShaderType.FragmentShader;
					return true;
				case ".tc":
				case ".tsc":
				case ".tesc":
					type = ShaderType.TessControlShader;
					return true;
				case ".te":
				case ".tse":
				case ".tese":
					type = ShaderType.TessEvaluationShader;
					return true;
				case ".go":
				case ".geo":
				case ".geom":
					type = ShaderType.GeometryShader;
					return true;
				case ".cp":
				case ".cmp":
				case ".comp":
					type = ShaderType.ComputeShader;
					return true;
				default:
					type = ShaderType.FragmentShader;
					return false;
			}
		}
    }
}