using MIConvexHull;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace D3T.Meshes
{
	/// <summary>
	/// Utility class for generating convex meshes. 
	/// </summary>
	public static class ConvexMeshGenerator
	{
		private class Vertex : IVertex
		{
			public double[] Position { get; set; }
			public Vertex(double x, double y, double z)
			{
				Position = new double[3] { x, y, z };
			}
			public Vertex(Vector3 ver)
			{
				Position = new double[3] { ver.x, ver.y, ver.z };
			}
			public Vector3 ToVec()
			{
				return new Vector3((float)Position[0], (float)Position[1], (float)Position[2]);
			}
		}

		/// <summary>
		/// Creates a convex mesh enclosing all given input vertices.
		/// </summary>
		public static Mesh CreateConvexMesh(IEnumerable<Vector3> inputVertices, bool flatShaded = true)
		{
			Mesh m = new Mesh();
			m.name = "MIConvexMesh";
			List<int> triangles = new List<int>();

			var vertices = inputVertices.Select(x => new Vertex(x)).ToList();

			var result = ConvexHull.Create(vertices).Result;
			if(result != null)
			{
				m.vertices = result.Points.Select(x => x.ToVec()).ToArray();
				var xxx = result.Points.ToList();

				foreach(var face in result.Faces)
				{
					triangles.Add(xxx.IndexOf(face.Vertices[0]));
					triangles.Add(xxx.IndexOf(face.Vertices[1]));
					triangles.Add(xxx.IndexOf(face.Vertices[2]));
				}

				m.triangles = triangles.ToArray();
				if(flatShaded)
				{
					MeshBuilder.FlatShadeMesh(m);
				}
				else
				{
					m.RecalculateNormals();
					m.RecalculateTangents();
				}
				return m;
			}
			else
			{
				Debug.LogError("Creating convex mesh failed. At least 4 vertices are required to build a convex mesh.");
				return null;
			}
		}

		/// <summary>
		/// Generates a convex mesh enclosing the given mesh.
		/// </summary>
		public static Mesh CreateConvexMesh(Mesh nonConvexMesh, bool flatShaded = true)
		{
			return CreateConvexMesh(nonConvexMesh.vertices, flatShaded);
		}
	}
}
