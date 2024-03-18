using System.Collections.Generic;
using UnityEngine;

namespace D3T
{
	public class LineMeshBuilder
	{
		public List<Vector3> verts = new List<Vector3>();
		public List<Color32> colors = new List<Color32>();
		public List<int> indices = new List<int>();

		public void AddLine(Vector3 a, Vector3 b, Color32? color = null)
		{
			int startIndex = verts.Count;
			AddVertex(a, color);
			AddVertex(b, color);
			ConnectVertices(startIndex, startIndex + 1);
		}

		public void AddLineStrip(Color32? color, params Vector3[] points)
		{
			int startIndex = verts.Count;
			int pointCount = 0;
			foreach(var point in points)
			{
				AddVertex(point, color);
				pointCount++;
			}
			for(int i = 0; i < pointCount - 1; i++)
			{
				ConnectVertices(startIndex + i, startIndex + i + 1);
			}
		}

		public void AddLineStrip(params Vector3[] points)
		{
			AddLineStrip(null, points);
		}

		public void AddCube(Vector3 center, Vector3 size, Color32? color = null)
		{
			var extents = size * 0.5f;
			int startIndex = verts.Count;

			AddVertex(center + new Vector3(-extents.x, -extents.y, -extents.z), color);
			AddVertex(center + new Vector3(extents.x, -extents.y, -extents.z), color);
			AddVertex(center + new Vector3(-extents.x, -extents.y, extents.z), color);
			AddVertex(center + new Vector3(extents.x, -extents.y, extents.z), color);
			AddVertex(center + new Vector3(-extents.x, extents.y, -extents.z), color);
			AddVertex(center + new Vector3(extents.x, extents.y, -extents.z), color);
			AddVertex(center + new Vector3(-extents.x, extents.y, extents.z), color);
			AddVertex(center + new Vector3(extents.x, extents.y, extents.z), color);

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

			AddVertex(matrix.MultiplyPoint(new Vector3(-extents.x, 0, -extents.y)), color);
			AddVertex(matrix.MultiplyPoint(new Vector3(extents.x, 0, -extents.y)), color);
			AddVertex(matrix.MultiplyPoint(new Vector3(-extents.x, 0, extents.y)), color);
			AddVertex(matrix.MultiplyPoint(new Vector3(extents.x, 0, extents.y)), color);

			ConnectVertices(startIndex, startIndex + 1);
			ConnectVertices(startIndex, startIndex + 2);
			ConnectVertices(startIndex + 1, startIndex + 3);
			ConnectVertices(startIndex + 2, startIndex + 3);
		}

		public void AddDisc(Vector3 center, Quaternion rotation, float radius, int detail = 32, Color32? color = null)
		{
			Vector2[] pts = MeshBuilder.GetCirclePoints(detail, radius);
			var matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
			int startIndex = verts.Count;

			//TODO
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
					AddVertex(pos + unitVector * radius, color);
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

		public Mesh CreateMesh(string meshName = null)
		{
			Mesh mesh = new Mesh();
			if(meshName != null) mesh.name = meshName;
			mesh.SetVertices(verts);
			mesh.SetColors(colors);
			mesh.SetIndices(indices, MeshTopology.Lines, 0);
			mesh.UploadMeshData(false);
			return mesh;
		}

		private void AddVertex(Vector3 position, Color32? color)
		{
			verts.Add(position);
			colors.Add(color ?? Color.white);
		}

		private void ConnectVertices(int i0, int i1)
		{
			indices.Add(i0);
			indices.Add(i1);
		}
	}
}