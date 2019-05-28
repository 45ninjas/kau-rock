using OpenTK.Graphics.OpenGL4;
using OpenTK;
using System.Collections.Generic;

namespace KauRock {
    public class ShaderProgram {

        private Dictionary<string, int> uniformLocations;

       public readonly int Program;

       public Transform Transform = null;
       private int? transformMatrixLocation = null;

        public ShaderProgram(int[] shaders) {
            Program = GL.CreateProgram();

            // Attach each shader.
            foreach(int shader in shaders)
                GL.AttachShader(Program, shader);

            // Link all the attached shaders to this program.
            GL.LinkProgram(Program);

            // Check for any linking errors.
            GL.GetProgram(Program, GetProgramParameterName.LinkStatus, out var status);
            if(status != (int)All.True) {
                string message = GL.GetProgramInfoLog(Program);
                throw new System.Exception($"Error while linking program. Details: {message}");
            }

            // Get all the uniforms for this program.
            GetAllUniforms();

            // Detach each shader.
            foreach(int shader in shaders) {
                GL.DetachShader(Program, shader);
            }
        }
        
        private void GetAllUniforms() {
            // Set the shader handle.

            uniformLocations = new Dictionary<string, int>();

            // Get the total amount of uniforms for this shader.
            GL.GetProgram(Program, GetProgramParameterName.ActiveUniforms, out int uniformCount);

            // Go over each uniform in this shader and save the location and name of each.            
            for (int i = 0; i < uniformCount; i++) {
                string name = GL.GetActiveUniform(Program, i, out _, out _);
                int location = GL.GetUniformLocation(Program, name);

                uniformLocations.Add(name, location);
            }

            // Set the transform matrix location if it exists.
            if(uniformLocations.TryGetValue("Matrix", out int matrixLocation))
                transformMatrixLocation = matrixLocation;
            else
                transformMatrixLocation = null;
        }

        // Use the shader program somewhere.
        public void UseProgram(bool useTransformMatrix = true) {

            // Use the shader
            GL.UseProgram(Program);

            // Set the transform matrix to the matrix on the transform if all is good.
            if(useTransformMatrix && transformMatrixLocation != null && Transform != null)
                GL.ProgramUniformMatrix4(Program, (int)transformMatrixLocation, true, ref Transform.Matrix);
        }
        // Destroy the shader program.
        public void Destroy() {
            GL.DeleteProgram(Program);
        }

        // Get the location of a uniform from the dictionary.
        public bool TryGetUniformLocation(string name, out int location)
            => uniformLocations.TryGetValue(name, out location);

        // Setting a matrix variable for the shader.
        public void SetMatrix(int location, Matrix4 matrix)
            => GL.ProgramUniformMatrix4(Program, location, true, ref matrix);
        public void SetMatrix(string name, Matrix4 matrix)
            => SetMatrix(uniformLocations[name], matrix);

        // Setting a float for the shader.
        public void SetFloat(int location, float data)
            => GL.ProgramUniform1(Program, location, data);
        public void SetFloat(string name, float data)
            => SetFloat(uniformLocations[name], data);

        // Setting a vector2 for the shader.
        public void SetVector2(int location, Vector2 data)
            => GL.ProgramUniform2(Program, location, data);
        public void SetVector2(string name, Vector2 data)
            => SetVector2(uniformLocations[name], data);

        // Setting a vector3 for the shader.
        public void SetVector3(int location, Vector3 data)
            => GL.ProgramUniform3(Program, location, data);
        public void SetVector3(string name, Vector3 data)
            => SetVector3(uniformLocations[name], data); 

        // Setting a vector4 for the shader.
        public void SetVector4(int location, Vector4 data)
            => GL.ProgramUniform4(Program, location, data);
        public void SetVector4(string name, Vector4 data)
            => SetVector4(uniformLocations[name], data);

        // Setting an int for the shader.
        public void SetInt(int location, int data)
            => GL.ProgramUniform1(Program, location, data);
        public void SetInt(string name, int data)
            => SetInt(uniformLocations[name], data);
    }
}