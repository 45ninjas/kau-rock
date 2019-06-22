using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace KauRock {
	public class ShaderProgram {

		private Dictionary<string, int> uniformLocations;

		public readonly int Program;

		private int viewMatrixLocation;
		private int projectionMatrixLocation;
		private float lastTime = 0;

		private int transformMatrixLocation;

		private const bool TRANSPOSE = true;

		public ShaderProgram (int[] shaders) {
			Program = GL.CreateProgram ();

			// Attach each shader.
			foreach (int shader in shaders)
				GL.AttachShader (Program, shader);

			// Link all the attached shaders to this program.
			GL.LinkProgram (Program);

			// Check for any linking errors.
			GL.GetProgram (Program, GetProgramParameterName.LinkStatus, out var status);
			if (status != (int) All.True) {
				string message = GL.GetProgramInfoLog (Program);
				throw new System.Exception ($"Error while linking program. Details: {message}");
			}

			// Get all the uniforms for this program.
			GetAllUniforms ();

			// Detach each shader.
			foreach (int shader in shaders) {
				GL.DetachShader (Program, shader);
			}
		}

		private void GetAllUniforms () {

			// Get all the locations for the uniforms and put them into the dictionary for later.
			uniformLocations = new Dictionary<string, int> ();
			GL.GetProgram (Program, GetProgramParameterName.ActiveUniforms, out int uniformCount);
     
			for (int i = 0; i < uniformCount; i++) {
				string name = GL.GetActiveUniform (Program, i, out _, out _);
				int location = GL.GetUniformLocation (Program, name);

				uniformLocations.Add (name, location);
				Log.Debug(this, $"Shader Uniform {name} at {location}");
			}


			// Set the matrix locations and If they don't exist use -1.
			if (!uniformLocations.TryGetValue ("transform", out transformMatrixLocation))
				transformMatrixLocation = -1;

			if (!uniformLocations.TryGetValue ("view", out viewMatrixLocation))
				viewMatrixLocation = -1;

			if (!uniformLocations.TryGetValue ("projection", out projectionMatrixLocation))
				projectionMatrixLocation = -1;
		}

		public void ListAttribs() {
			GL.GetProgram(Program, GetProgramParameterName.ActiveAttributes, out int attribCount);

			for (int i = 0; i < attribCount; i++) {
				string name = GL.GetActiveAttrib(Program, i, out int size, out ActiveAttribType type);
				int location = GL.GetAttribLocation(Program, name);
				Log.Debug(this, $"{name}[{type}] is at {location}");
			}
		}


		public void UseProgram(Matrix4 transform, Matrix4 view, Matrix4 projection) {
			
			if(viewMatrixLocation >= 0)
				GL.ProgramUniformMatrix4(Program, viewMatrixLocation, TRANSPOSE, ref view);
			else
				Log.Warning(this, "this shader does not have a view matrix uniform but one was set.");

			if(viewMatrixLocation >= 0)
				GL.ProgramUniformMatrix4(Program, projectionMatrixLocation, TRANSPOSE, ref projection);
			else
				Log.Warning(this, "this shader does not have a projection matrix uniform but one was set.");


			GL.UseProgram (Program);
		}


		public void UseProgram (Matrix4 transform) {
			// Set the transform matrix.
			if (transformMatrixLocation >= 0)
				GL.ProgramUniformMatrix4 (Program, (int) transformMatrixLocation, TRANSPOSE, ref transform);

			// We only want to assign the camera matrices once, only do it if the time is different.
			if(lastTime != Time.UnscaledGameTime) {
			
				// Set the view matrix if it's set.
				if(viewMatrixLocation >= 0) {
					var view = Camera.ActiveCamera.GetViewMatrix();
					GL.ProgramUniformMatrix4(Program, viewMatrixLocation, TRANSPOSE, ref view);
				}
			
				// Set the projection matrix if it's set.
				if(projectionMatrixLocation >= 0) {
					var projection = Camera.ActiveCamera.GetProjectionMatrix();
					GL.ProgramUniformMatrix4(Program, projectionMatrixLocation, TRANSPOSE, ref projection);
				}
			}

			GL.UseProgram (Program);
		}
		// Destroy the shader program.
		public void Destroy () {
			GL.DeleteProgram (Program);
		}

		// Get the location of a uniform from the dictionary.
		public bool TryGetUniformLocation (string name, out int location) => uniformLocations.TryGetValue (name, out location);
		public int GetUniformLocation(string name) => uniformLocations[name];

		// Get an attribute location.
		public int GetAttribLocation(string attribName) => GL.GetAttribLocation(Program, attribName);

		// Setting a matrix variable for the shader.
		public void SetMatrix (int location, Matrix4 matrix) => GL.ProgramUniformMatrix4 (Program, location, TRANSPOSE, ref matrix);
		public void SetMatrix (string name, Matrix4 matrix) => SetMatrix (uniformLocations[name], matrix);

		// Setting a float for the shader.
		public void SetFloat (int location, float data) => GL.ProgramUniform1 (Program, location, data);
		public void SetFloat (string name, float data) => SetFloat (uniformLocations[name], data);

		// Setting a vector2 for the shader.
		public void SetVector2 (int location, Vector2 data) => GL.ProgramUniform2 (Program, location, data);
		public void SetVector2 (string name, Vector2 data) => SetVector2 (uniformLocations[name], data);

		// Setting a vector3 for the shader.
		public void SetVector3 (int location, Vector3 data) => GL.ProgramUniform3 (Program, location, data);
		public void SetVector3 (string name, Vector3 data) => SetVector3 (uniformLocations[name], data);

		// Setting a vector4 for the shader.
		public void SetVector4 (int location, Vector4 data) => GL.ProgramUniform4 (Program, location, data);
		public void SetVector4 (string name, Vector4 data) => SetVector4 (uniformLocations[name], data);
		
		// support colours too.
		public void SetVector4 (int location, Color data) => GL.ProgramUniform4 (Program, location, data);
		public void SetVector4 (string name, Color data) => SetVector4 (uniformLocations[name], data);

		// Setting an int for the shader.
		public void SetInt (int location, int data) => GL.ProgramUniform1 (Program, location, data);
		public void SetInt (string name, int data) => SetInt (uniformLocations[name], data);

		// Set a texture2D.
		public void SetTexture(int location, Texture2D texture) => SetInt(location,texture.TextureObject);
		public void SetTexture(string name, Texture2D texture) => SetInt(name,texture.TextureObject);
	}
}