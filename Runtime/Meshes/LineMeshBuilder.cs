using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Meshes
{
	public class LineMeshBuilder : MeshBuilderBase
	{
		public List<int> indices = new List<int>();

		public override void Clear()
		{
			verts.Clear();
			vertexColors.Clear();
			indices.Clear();
		}

		public void AddLine(Vector3 a, Vector3 b)
		{
			int startIndex = verts.Count;
			AddTransformedVertex(a);
			AddTransformedVertex(b);
			ConnectVertices(startIndex, startIndex + 1);
		}

		public void AddLineStrip(params Vector3[] points)
		{
			int startIndex = verts.Count;
			int pointCount = 0;
			foreach(var point in points)
			{
				AddTransformedVertex(point);
				pointCount++;
			}
			for(int i = 0; i < pointCount - 1; i++)
			{
				ConnectVertices(startIndex + i, startIndex + i + 1);
			}
		}

		public void AddCube(Vector3 center, Vector3 size, Color32? color = null)
		{
			var extents = size * 0.5f;
			int startIndex = verts.Count;

			AddTransformedVertex(center + new Vector3(-extents.x, -extents.y, -extents.z));
			AddTransformedVertex(center + new Vector3(extents.x, -extents.y, -extents.z));
			AddTransformedVertex(center + new Vector3(-extents.x, -extents.y, extents.z));
			AddTransformedVertex(center + new Vector3(extents.x, -extents.y, extents.z));
			AddTransformedVertex(center + new Vector3(-extents.x, extents.y, -extents.z));
			AddTransformedVertex(center + new Vector3(extents.x, extents.y, -extents.z));
			AddTransformedVertex(center + new Vector3(-extents.x, extents.y, extents.z));
			AddTransformedVertex(center + new Vector3(extents.x, extents.y, extents.z));

			ConnectVertices(startIndex, startIndex + 1);
			ConnectVertices(startIndex, startIndex + 2);
			ConnectVertices(startIndex + 1, startIndex + 3);
			ConnectVertices(startIndex + 2, startIndex + 3);

			ConnectVertices(startIndex, startIndex + 4);
			ConnectVertices(startIndex + 1, startIndex + 5);
			ConnectVertices(startIndex + 2, startIndex + 6);
			ConnectVertices(startIndex + 3, startIndex + 7);

			ConnectVertices(startIndex + 4, startIndex + 5);
			ConnectVertices(startIndex + 4, startIndex + 6);
			ConnectVertices(startIndex + 5, startIndex + 7);
			ConnectVertices(startIndex + 6, startIndex + 7);
		}

		public void AddCubeFromTo(Vector3 a, Vector3 b, Color32? color = null)
		{
			var center = (a + b) / 2f;
			var size = (b - a).Abs();
			AddCube(center, size, color);
		}

		public void AddRectangle(Vector3 center, Quaternion rotation, Vector2 size, Color32? color = null)
		{
			var extents = size / 2f;
			var matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
			int startIndex = verts.Count;

			AddTransformedVertex(matrix.MultiplyPoint(new Vector3(-extents.x, 0, -extents.y)));
			AddTransformedVertex(matrix.MultiplyPoint(new Vector3(extents.x, 0, -extents.y)));
			AddTransformedVertex(matrix.MultiplyPoint(new Vector3(-extents.x, 0, extents.y)));
			AddTransformedVertex(matrix.MultiplyPoint(new Vector3(extents.x, 0, extents.y)));

			ConnectVertices(startIndex, startIndex + 1);
			ConnectVertices(startIndex, startIndex + 2);
			ConnectVertices(startIndex + 1, startIndex + 3);
			ConnectVertices(startIndex + 2, startIndex + 3);
		}

		public void AddCircle(Vector3 center, Quaternion rotation, float radius, int detail = 32)
		{
			GetCirclePoints(tempVertexCache, detail, radius);
			var matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
			int startIndex = verts.Count;

			foreach(var pt in tempVertexCache) AddTransformedVertex(matrix.MultiplyPoint(pt));

			for(int i = 0; i < detail - 1; i++) ConnectVertices(startIndex + i, startIndex + i + 1);
			ConnectVertices(startIndex + detail - 1, startIndex);
		}

		public void AddCylinder(Vector3 center, Quaternion rotation, float radius1, float radius2, float height, int detail = 32, Color32? color = null)
		{
			GetCirclePoints(tempVertexCache, detail, 1f);
			var matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
			int startIndex = verts.Count;

			//Lower base
			foreach(var pt in tempVertexCache) AddTransformedVertex(matrix.MultiplyPoint((pt * radius1).WithY(-height / 2f)));
			for(int i = 0; i < detail - 1; i++) ConnectVertices(startIndex + i, startIndex + i + 1);
			ConnectVertices(startIndex + detail - 1, startIndex);

			//Upper base
			foreach(var pt in tempVertexCache) AddTransformedVertex(matrix.MultiplyPoint((pt * radius2).WithY(height / 2f)));
			for(int i = 0; i < detail - 1; i++) ConnectVertices(startIndex + detail + i, startIndex + detail + i + 1);
			ConnectVertices(startIndex + 2 * detail - 1, startIndex + detail);

			//Connect bases
			for(int i = 0; i < detail; i++) ConnectVertices(startIndex + i, startIndex + detail + i);
		}

		public void AddCylinder(Vector3 center, Quaternion rotation, float radius, float height, int detail = 32, Color32? color = null)
		{
			AddCylinder(center, rotation, radius, radius, height, detail, color);
		}

		public void AddCone(Vector3 center, Quaternion rotation, float radius, float height, int detail = 32, Color32? color = null)
		{
			GetCirclePoints(tempVertexCache, detail, radius);
			var matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
			int startIndex = verts.Count;

			//Cone tip
			AddTransformedVertex(matrix.MultiplyPoint(Vector3.up * height));

			//Base
			foreach(var pt in tempVertexCache) AddTransformedVertex(matrix.MultiplyPoint(pt.WithY(-height / 2f)));
			for(int i = 0; i < detail - 1; i++) ConnectVertices(startIndex + i + 1, startIndex + i + 2);
			ConnectVertices(startIndex + detail, startIndex + 1);

			//Connections to cone tip
			for(int i = 0; i < detail; i++) ConnectVertices(startIndex + i + 1, startIndex);
		}

		public void AddSphere(Vector3 pos, float radius, int latDetail = 32, int lonDetail = 32, Color32? color = null)
		{
			int offset = verts.Count;
			lonDetail /= 2;
			for(int v = 0; v <= lonDetail; v++)
			{
				var vAngle = v / (float)lonDetail * Mathf.PI;
				for(int i = 0; i <= latDetail; i++)
				{
					var hAngle = i / (float)latDetail * Mathf.PI * -2f;
					float x = Mathf.Sin(hAngle);
					float z = Mathf.Cos(hAngle);
					float y = -Mathf.Cos(vAngle);
					float m = Mathf.Sin(vAngle);
					var unitVector = new Vector3(x * m, y, z * m);
					AddTransformedVertex(pos + unitVector * radius);
				}
			}

			for(int i = 0; i < lonDetail; i++)
			{
				int lower = offset + (latDetail + 1) * i;
				int upper = offset + (latDetail + 1) * (i + 1);
				for(int l = 0; l < latDetail; l++)
				{
					ConnectVertices(lower + l, upper + l);
					ConnectVertices(lower + l, lower + l + 1);
				}
			}
		}

		public override void BuildMesh(Mesh mesh)
		{
			mesh.SetVertices(verts);
			if(vertexColors != null && vertexColors.Count > 0) mesh.SetColors(vertexColors);
			mesh.SetIndices(indices, MeshTopology.Lines, 0);
			mesh.UploadMeshData(false);
		}

		private void ConnectVertices(int i0, int i1)
		{
			indices.Add(i0);
			indices.Add(i1);
		}
	}
}