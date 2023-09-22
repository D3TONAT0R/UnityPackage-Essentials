using MIConvexHull;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace D3T.Utility
{
	/// <summary>
	/// Component that generates a convex mesh.
	/// </summary>
	[DisallowMultipleComponent]
	public class ConvexMeshBuilder : MonoBehaviour
	{
		//TODO: do some refactoring here

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

		public enum TargetComponents
		{
			None,
			MeshCollider,
			MeshFilter,
			Both
		}

		const float cubeExtents = 0.5f;

		public TargetComponents applyTo = TargetComponents.MeshCollider;
		public List<Vector3> vertices;
		public bool flatNormals = true;

		public Mesh GeneratedMesh { get; private set; }

		// Start is called before the first frame update
		void Awake()
		{
			RebuildMesh();
			AssignToComponents();
		}

		public void AddVertex(Vector3? pos = null)
		{
			vertices.Add(pos ?? Vector3.zero);
		}

		public void AddVertex(float x, float y, float z)
		{
			vertices.Add(new Vector3(x, y, z));
		}

		public void InitCube()
		{
			vertices.Clear();

			AddVertex(new Vector3(-cubeExtents, -cubeExtents, -cubeExtents));
			AddVertex(new Vector3(-cubeExtents, -cubeExtents, cubeExtents));
			AddVertex(new Vector3(cubeExtents, -cubeExtents, -cubeExtents));
			AddVertex(new Vector3(cubeExtents, -cubeExtents, cubeExtents));

			AddVertex(new Vector3(-cubeExtents, cubeExtents, -cubeExtents));
			AddVertex(new Vector3(-cubeExtents, cubeExtents, cubeExtents));
			AddVertex(new Vector3(cubeExtents, cubeExtents, -cubeExtents));
			AddVertex(new Vector3(cubeExtents, cubeExtents, cubeExtents));

			OnValidate();
		}

		public Mesh RebuildMesh()
		{
			if(vertices == null)
			{
				vertices = new List<Vector3>();
				InitCube();
			}
			GeneratedMesh = CreateConvexMesh(vertices, flatNormals);
			return GeneratedMesh;
		}

		public void AssignToComponents()
		{
			if((applyTo == TargetComponents.MeshCollider || applyTo == TargetComponents.Both) && TryGetComponent<MeshCollider>(out var meshCollider))
			{
				meshCollider.sharedMesh = GeneratedMesh;
			}
			if((applyTo == TargetComponents.MeshFilter || applyTo == TargetComponents.Both) && TryGetComponent<MeshFilter>(out var meshFilter))
			{
				meshFilter.sharedMesh = GeneratedMesh;
			}
		}

		public void Validate()
		{
			if(this == null) return;
			RebuildMesh();
			AssignToComponents();
		}

		private void OnValidate()
		{
			this.InvokeValidation(Validate);
		}

		public static Mesh CreateConvexMesh(Mesh nonConvexMesh, bool hardEdges = true)
		{
			return CreateConvexMesh(nonConvexMesh.vertices, hardEdges);
		}

		public static Mesh CreateConvexMesh(IEnumerable<Vector3> inputVertices, bool hardEdges = true)
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
				if(hardEdges)
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
	}
}
