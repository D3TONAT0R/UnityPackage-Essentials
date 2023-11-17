using MIConvexHull;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEssentials.Utility
{
	/// <summary>
	/// Component that generates a convex mesh.
	/// </summary>
	[DisallowMultipleComponent]
	public class ConvexMeshBuilderComponent : MonoBehaviour
	{
		//TODO: do some refactoring here


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
			GeneratedMesh = ConvexMeshGenerator.CreateConvexMesh(vertices, flatNormals);
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
			this.EditorDelayCall(Validate);
		}
	}
}
