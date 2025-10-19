using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEssentials.Meshes
{
	/// <summary>
	/// Component that generates a convex mesh.
	/// </summary>
	[DisallowMultipleComponent, AddComponentMenu("Mesh/Convex Mesh Builder")]
	public class ConvexMeshBuilderComponent : MeshBuilderComponent
	{
		private const float CUBE_EXTENTS = 0.5f;

		public List<Vector3> vertices = new List<Vector3>();
		public bool flatNormals = true;

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

			AddVertex(new Vector3(-CUBE_EXTENTS, -CUBE_EXTENTS, -CUBE_EXTENTS));
			AddVertex(new Vector3(-CUBE_EXTENTS, -CUBE_EXTENTS, CUBE_EXTENTS));
			AddVertex(new Vector3(CUBE_EXTENTS, -CUBE_EXTENTS, -CUBE_EXTENTS));
			AddVertex(new Vector3(CUBE_EXTENTS, -CUBE_EXTENTS, CUBE_EXTENTS));

			AddVertex(new Vector3(-CUBE_EXTENTS, CUBE_EXTENTS, -CUBE_EXTENTS));
			AddVertex(new Vector3(-CUBE_EXTENTS, CUBE_EXTENTS, CUBE_EXTENTS));
			AddVertex(new Vector3(CUBE_EXTENTS, CUBE_EXTENTS, -CUBE_EXTENTS));
			AddVertex(new Vector3(CUBE_EXTENTS, CUBE_EXTENTS, CUBE_EXTENTS));

			OnValidate();
		}

		protected override void Generate(ref Mesh mesh)
		{
			if(vertices == null)
			{
				vertices = new List<Vector3>();
				InitCube();
			}
			mesh = ConvexMeshGenerator.CreateConvexMesh(vertices, flatNormals);
		}

		protected override void Reset()
		{
			Mesh mesh = TryGetComponent<MeshCollider>(out var collider) ? collider.sharedMesh : null;
			if(mesh == null) mesh = TryGetComponent<MeshFilter>(out var filter) ? filter.sharedMesh : null;
			if(mesh != null && mesh.vertexCount < 256)
			{
				vertices = mesh.vertices.ToList();
			}
			else
			{
				InitCube();
			}
		}
	}
}
