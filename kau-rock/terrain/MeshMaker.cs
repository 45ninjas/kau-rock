using OpenTK;
using System.Collections.Generic;
using System.Collections;
using KauRock;
using OpenTK.Graphics.OpenGL4;

namespace KauRock.Terrain {
	public class MeshMaker {

		[System.Flags]
		public enum Faces {
			None = 0,
			NegX = 1,
			PosX = 2,
			NegY = 4,
			PosY = 8,
			NegZ = 16,
			PosZ = 32
		}
		int[] vertLookup;

		Stack<uint> triangles;
		Stack<Chunk.Vertex> vertices;
		int verticesCount;
		

		public MeshMaker() {
			// Create our vertex lookup.
			vertLookup = new int[Chunk.Size * Chunk.Size * Chunk.Size];

		}

		public void UpdateChunk(Chunk chunk) {
			// Start a new mesh.
			startNew();

			// Create a cube with the following faces.
			Cube(new VoxPos(0,0,0), Faces.NegX |	Faces.PosX |	Faces.NegY |	Faces.PosY |	Faces.NegZ |	Faces.PosZ);
			
			// Set the new mesh on the chunk.
			chunk.SetMesh(vertices.ToArray(), triangles.ToArray());
		}

		void startNew() {
			// Set all values in the vertex lookup to 0.
			for(int i = vertLookup.Length - 1; i >= 0; i --)
				vertLookup[i] = -1;
				
			// Create our list of vertices and triangles.
			vertices = new Stack<Chunk.Vertex>();
			triangles = new Stack<uint>();

			verticesCount = 0;
		}

		public delegate void MeshUpdate(Chunk.Vertex[] vertices, uint[] triangles);
		public MeshUpdate OnUpdatedMesh;

		VoxPos[] corner = new VoxPos[] {
				// Bottom Face.
				new VoxPos(0, 0, 1),
				new VoxPos(0, 0, 0),
				new VoxPos(1, 0, 0),
				new VoxPos(1, 0, 1),
				// Top Face.
				new VoxPos(0, 1, 1),
				new VoxPos(0, 1, 0),
				new VoxPos(1, 1, 0),
				new VoxPos(1, 1, 1),
			};
		public void Cube(VoxPos pos, Faces visiableFaces) {
			if(visiableFaces.HasFlag(Faces.NegX)) {
				triangle(pos, 1, 5, 4);
				triangle(pos, 1, 4, 0);
			}
			if(visiableFaces.HasFlag(Faces.PosX)) {
				triangle(pos, 3, 7, 6);
				triangle(pos, 3, 6, 2);
			}
			if(visiableFaces.HasFlag(Faces.NegY)){
				triangle(pos, 3, 2, 1);
				triangle(pos, 3, 1, 0);
			}
			if(visiableFaces.HasFlag(Faces.PosY)) {
				triangle(pos, 6, 7, 4);
				triangle(pos, 6, 4, 5);
			}
			if(visiableFaces.HasFlag(Faces.NegZ)) {
				triangle(pos, 6, 5, 1);
				triangle(pos, 6, 1, 2);
			}
			if(visiableFaces.HasFlag(Faces.PosZ)) {
				triangle(pos, 4, 7, 3);
				triangle(pos, 4, 3, 0);
			}
		}
		private void triangle(VoxPos position, int a, int b, int c) {
			triangles.Push((uint)getVertex(position + corner[a]));
			triangles.Push((uint)getVertex(position + corner[b]));
			triangles.Push((uint)getVertex(position + corner[c]));
		}
		private int getVertex(VoxPos position) {
			int index = position.X + position.Y * Chunk.Size + position.Z * Chunk.Size * Chunk.Size;

			// If the vert does not exist, add it.
			if(vertLookup[index] < 0) {
				vertLookup[index] = verticesCount++;
				vertices.Push(new Chunk.Vertex(position));
			}

			return vertLookup[index];
		}
	}
}