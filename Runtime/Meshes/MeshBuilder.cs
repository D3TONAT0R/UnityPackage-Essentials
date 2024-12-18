﻿using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Meshes
{

	/// <summary>
	/// Factory class for generating procedural meshes.
	/// </summary>
	public class MeshBuilder : MeshBuilderBase
	{
		[System.Flags]
		public enum CubeFaces
		{
			None = 0b000000,
			XNeg = 0b000001,
			XPos = 0b000010,
			YNeg = 0b000100,
			YPos = 0b001000,
			ZNeg = 0b010000,
			ZPos = 0b100000,
			All = 0b111111
		}

		public const int DEFAULT_CIRCLE_DETAIL = 32;

		public List<int> tris = new List<int>();
		public List<Vector3> normals = new List<Vector3>();

		public List<Vector2> uv0 = new List<Vector2>();

		/// <summary>
		/// If <see langword="true"/>, all future triangles and normals will be reversed.
		/// </summary>
		public bool Reversed { get; set; } = false;


		public MeshBuilder()
		{
			verts = new List<Vector3>();
			tris = new List<int>();
			normals = new List<Vector3>();
			uv0 = new List<Vector2>();
			vertexColors = new List<Color32>();
		}

		public MeshBuilder(int vertexCapacity)
		{
			verts = new List<Vector3>(vertexCapacity);
			tris = new List<int>((int)(vertexCapacity * 0.6f));
			normals = new List<Vector3>(vertexCapacity);
			uv0 = new List<Vector2>(vertexCapacity);
			vertexColors = new List<Color32>(vertexCapacity);
		}

		public override void TransformVector(ref Vector3 vector)
		{
			base.TransformVector(ref vector);
			if(Reversed) vector = -vector;
		}

		public override Vector3 TransformVector(Vector3 vector)
		{
			vector = base.TransformVector(vector);
			if(Reversed) vector = -vector;
			return vector;
		}

		public override void Clear()
		{
			verts.Clear();
			tris.Clear();
			normals.Clear();
			uv0.Clear();
			vertexColors.Clear();
			CurrentVertexColor = null;
			Reversed = false;
			ResetMatrix();
		}

		/// <summary>
		/// Builds the current mesh data into the given mesh object.
		/// </summary>
		public void BuildMesh(Mesh mesh, bool recalculateTangents)
		{
			mesh.Clear();
			if(verts.Count > 3)
			{
				mesh.SetVertices(verts);
				mesh.SetTriangles(tris, 0);
				mesh.SetNormals(normals);
				mesh.SetUVs(0, uv0);
				if(vertexColors.Count > 0)
				{
					if(vertexColors.Count == verts.Count)
					{
						mesh.SetColors(vertexColors);
					}
					else
					{
						Debug.LogError("vertex color count does not match the number of vertices.");
					}
				}
				if(recalculateTangents) mesh.RecalculateTangents();
				mesh.UploadMeshData(false);
			}
		}

		/// <summary>
		/// Builds the current mesh data into the given mesh object.
		/// </summary>
		public override void BuildMesh(Mesh mesh)
		{
			BuildMesh(mesh, true);
		}

		/// <summary>
		/// Adds a (flat shaded) triangle to the mesh. Vertices should be arranged clockwise for correct facing.
		/// </summary>
		public void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 normal, Vector2 uv_a, Vector2 uv_b, Vector2 uv_c)
		{
			TransformPoint(ref a);
			TransformPoint(ref b);
			TransformPoint(ref c);
			TransformVector(ref normal);

			AddVertex(a);
			AddVertex(b);
			AddVertex(c);
			normals.Add(normal);
			normals.Add(normal);
			normals.Add(normal);
			uv0.Add(uv_a);
			uv0.Add(uv_b);
			uv0.Add(uv_c);
			var i = verts.Count;
			MakeTriangle(i - 3, i - 2, i - 1);
		}

		/// <summary>
		/// Adds a rectangular face (made of two triangles) to the mesh, using default uv mappings.
		/// </summary>
		public void AddQuad(Vector3 ll, Vector3 lr, Vector3 ul, Vector3 ur, Vector3 normal)
		{
			AddQuad(ll, lr, ul, ur, normal, Vector2.zero, Vector2.right, Vector2.up, Vector2.one);
		}

		/// <summary>
		/// Adds a rectangular face (made of two triangles) to the mesh.
		/// </summary>
		public void AddQuad(Vector3 ll, Vector3 lr, Vector3 ul, Vector3 ur, Vector3 normal, Vector2 uv_ll, Vector2 uv_lr, Vector2 uv_ul, Vector2 uv_ur)
		{
			TransformPoint(ref ll);
			TransformPoint(ref lr);
			TransformPoint(ref ul);
			TransformPoint(ref ur);
			TransformVector(ref normal);

			AddVertex(ul);
			AddVertex(ur);
			AddVertex(ll);
			AddVertex(lr);
			normals.Add(normal);
			normals.Add(normal);
			normals.Add(normal);
			normals.Add(normal);
			uv0.Add(uv_ul);
			uv0.Add(uv_ur);
			uv0.Add(uv_ll);
			uv0.Add(uv_lr);

			var i = verts.Count;
			MakeTriangle(i - 4, i - 3, i - 2);
			MakeTriangle(i - 3, i - 1, i - 2);
		}

		/// <summary>
		/// Adds a cube with the given center position and size to the mesh.
		/// </summary>
		public void AddCube(Vector3 pos, Vector3 size, CubeFaces faceFlags = CubeFaces.All)
		{
			size /= 2f;
			AddCubeFromTo(pos - size, pos + size, faceFlags);
		}

		/// <summary>
		/// Adds a cube with the given center position and size to the mesh.
		/// </summary>
		public void AddCube(Vector3 pos, Vector3 size, Vector2 lowerCornerUV, Vector2 UpperCornerUV, CubeFaces faceFlags = CubeFaces.All)
		{
			size /= 2f;
			AddCubeFromTo(pos - size, pos + size, lowerCornerUV, UpperCornerUV, faceFlags);
		}

		/// <summary>
		/// Adds a cube with the given lower and upper bound points to the mesh.
		/// </summary>
		public void AddCubeFromTo(Vector3 a, Vector3 b, CubeFaces faceFlags = CubeFaces.All)
		{
			AddCubeFromTo(a, b, Vector2.zero, Vector2.one, faceFlags);
		}

		/// <summary>
		/// Adds a cube with the given lower and upper bound points to the mesh.
		/// </summary>
		public void AddCubeFromTo(Vector3 a, Vector3 b, Vector2 lowerCornerUV, Vector2 upperCornerUV, CubeFaces faceFlags = CubeFaces.All)
		{
			Vector3 lower = new Vector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
			Vector3 upper = new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
			/*
					   6-----7 <- upper
					  /|    /|		  y
					 4-+---5 |		  ^  z
					 | 2---+-3		  | ^
					 |/    |/         |/
			lower -> 0-----1		  o----> x
			*/

			Vector3 v0 = TransformPoint(lower);
			Vector3 v1 = TransformPoint(new Vector3(upper.x, lower.y, lower.z));
			Vector3 v2 = TransformPoint(new Vector3(lower.x, lower.y, upper.z));
			Vector3 v3 = TransformPoint(new Vector3(upper.x, lower.y, upper.z));
			Vector3 v4 = TransformPoint(new Vector3(lower.x, upper.y, lower.z));
			Vector3 v5 = TransformPoint(new Vector3(upper.x, upper.y, lower.z));
			Vector3 v6 = TransformPoint(new Vector3(lower.x, upper.y, upper.z));
			Vector3 v7 = TransformPoint(upper);

			Vector2 uv_ll = lowerCornerUV;
			Vector2 uv_lr = new Vector2(upperCornerUV.x, lowerCornerUV.y);
			Vector2 uv_ul = new Vector2(lowerCornerUV.x, upperCornerUV.y);
			Vector2 uv_ur = upperCornerUV;

			//Back
			if(faceFlags.HasFlag(CubeFaces.ZNeg))
				AddQuad(v0, v1, v4, v5, Vector3.back, uv_ll, uv_lr, uv_ul, uv_ur);
			//Right
			if(faceFlags.HasFlag(CubeFaces.XPos))
				AddQuad(v1, v3, v5, v7, Vector3.right, uv_ll, uv_lr, uv_ul, uv_ur);
			//Front
			if(faceFlags.HasFlag(CubeFaces.ZPos))
				AddQuad(v3, v2, v7, v6, Vector3.forward, uv_ll, uv_lr, uv_ul, uv_ur);
			//Left
			if(faceFlags.HasFlag(CubeFaces.XNeg))
				AddQuad(v2, v0, v6, v4, Vector3.left, uv_ll, uv_lr, uv_ul, uv_ur);
			//Top
			if(faceFlags.HasFlag(CubeFaces.YPos))
				AddQuad(v4, v5, v6, v7, Vector3.up, uv_ll, uv_lr, uv_ul, uv_ur);
			//Bottom
			if(faceFlags.HasFlag(CubeFaces.YNeg))
				AddQuad(v2, v3, v0, v1, Vector3.down, uv_ll, uv_lr, uv_ul, uv_ur);
		}

		/// <summary>
		/// Adds a sphere to the mesh.
		/// </summary>
		public void AddSphere(Vector3 pos, float radius, int latDetail = DEFAULT_CIRCLE_DETAIL, int lonDetail = DEFAULT_CIRCLE_DETAIL)
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
					AddVertex(TransformPoint(pos + unitVector * radius));
					Vector3 normal = unitVector;
					/*
					if(v == 0) normal = Vector3.down;
					else if(v == lonDetail) normal = Vector3.up;
					else normal = unitVector;
					*/
					normals.Add(TransformVector(normal));
					uv0.Add(new Vector2(i / (float)latDetail, v / (float)lonDetail));
				}
			}

			for(int i = 0; i < lonDetail; i++)
			{
				int lower = offset + (latDetail + 1) * i;
				int upper = offset + (latDetail + 1) * (i + 1);
				for(int l = 0; l < latDetail; l++)
				{
					int r = (l + 1);

					if(i < lonDetail - 1)
					{
						MakeTriangle(lower + l, upper + l, upper + r);
					}

					if(i > 0)
					{
						MakeTriangle(lower + l, upper + r, lower + r);
					}
				}
			}
		}

		/// <summary>
		/// Adds a hemisphere to the mesh.
		/// </summary>
		public void AddHemisphere(Vector3 pos, float radius, float height, int latDetail = DEFAULT_CIRCLE_DETAIL, int lonDetail = DEFAULT_CIRCLE_DETAIL)
		{
			const float HALF_PI = Mathf.PI / 2;
			int offset = verts.Count;
			lonDetail /= 4;
			float heightRatio = height * 2f / radius;
			for(int v = 0; v <= lonDetail; v++)
			{
				var vAngle = HALF_PI + (v / (float)lonDetail * HALF_PI);
				for(int i = 0; i <= latDetail; i++)
				{
					var hAngle = i / (float)latDetail * Mathf.PI * -2f;
					float x = Mathf.Sin(hAngle);
					float z = Mathf.Cos(hAngle);
					float y = -Mathf.Cos(vAngle);
					float m = Mathf.Sin(vAngle);
					var unitVector = new Vector3(x * m, y * heightRatio, z * m);
					AddVertex(TransformPoint(pos + unitVector * radius));
					normals.Add(TransformVector(unitVector));
					uv0.Add(new Vector2(i / (float)latDetail, v / (float)lonDetail));
				}
			}

			for(int i = 0; i < lonDetail; i++)
			{
				int lower = offset + (latDetail + 1) * i;
				int upper = offset + (latDetail + 1) * (i + 1);
				for(int l = 0; l < latDetail; l++)
				{
					int r = (l + 1);

					if(i < lonDetail - 1)
					{
						MakeTriangle(lower + l, upper + l, upper + r);
					}

					if(i >= 0)
					{
						MakeTriangle(lower + l, upper + r, lower + r);
					}
				}
			}
		}

		/// <summary>
		/// Adds a vertical capsule to the mesh.
		/// </summary>
		public void AddCapsule(Vector3 pos, float radius, float height, int latDetail = 32, int lonDetail = 32)
		{

			void AddVertRing(float vAngle, float voffset, bool upper)
			{
				float y = -Mathf.Cos(vAngle);
				float m = Mathf.Sin(vAngle);
				float p = Mathf.Abs(y);
				float sec = radius / Mathf.Max(2 * radius, height);
				//float uvY = v / (float)lonDetail / 2f;
				float uvY;
				if(!upper)
				{
					uvY = (1 - p) * sec;
				}
				else
				{
					uvY = 1 - ((1 - p) * sec);
				}

				for(int i = 0; i <= latDetail; i++)
				{
					var hAngle = i / (float)latDetail * Mathf.PI * -2f;
					float x = Mathf.Sin(hAngle);
					float z = Mathf.Cos(hAngle);
					var unitVector = new Vector3(x * m, y, z * m);
					var vert = TransformPoint(pos + unitVector * radius + Vector3.up * voffset);
					AddVertex(vert);
					normals.Add(TransformVector(unitVector));
					uv0.Add(new Vector2(i / (float)latDetail, uvY));
				}
			}

			lonDetail += lonDetail % 2;
			int indexOffset = verts.Count;
			lonDetail /= 2;
			float offset = -Mathf.Max(0, height / 2f - radius);
			for(int v = 0; v <= lonDetail; v++)
			{
				bool middle = v == lonDetail / 2;
				var vAngle = v / (float)lonDetail * Mathf.PI;
				AddVertRing(vAngle, offset, v > lonDetail / 2);
				if(middle)
				{
					offset = -offset;
					AddVertRing(vAngle, offset, true);
				}
			}

			for(int i = 0; i <= lonDetail; i++)
			{
				int lower = indexOffset + (latDetail + 1) * i;
				int upper = indexOffset + (latDetail + 1) * (i + 1);
				for(int l = 0; l < latDetail; l++)
				{
					int r = (l + 1);

					if(i < lonDetail)
					{
						MakeTriangle(lower + l, upper + l, upper + r);
					}

					if(i > 0)
					{
						MakeTriangle(lower + l, upper + r, lower + r);
					}
				}
			}
		}

		/// <summary>
		/// Adds a capsule to the mesh, aligned to the given axis.
		/// </summary>
		public void AddCapsule(Vector3 pos, Axis axis, float radius, float height, int latDetail = 32, int lonDetail = 32)
		{
			using(PushMatrixScope())
			{
				ApplyMatrix(Matrix4x4.TRS(pos, GetAxisRotation(axis), Vector3.one));
				AddCapsule(Vector3.zero, radius, height, latDetail, lonDetail);
			}
		}

		/// <summary>
		/// Adds a flat circle to the mesh, transformed by the given matrix.
		/// </summary>
		public void AddCircle(Matrix4x4 matrix, float radius, int detail = 32)
		{
			PushMatrix();
			ApplyMatrix(matrix);
			var nrm = TransformVector(Vector3.down);
			GetCirclePoints(tempVertexCache, detail, 1f);
			AddTransformedVertex(Vector3.zero);
			int b = verts.Count;
			normals.Add(nrm);
			uv0.Add(Vector2.one * 0.5f);

			for(int i = 0; i < tempVertexCache.Count; i++)
			{
				AddTransformedVertex(tempVertexCache[i].XZY() * radius);
				normals.Add(nrm);
				Vector2 uv = (tempVertexCache[i].XY() + Vector2.one) * 0.5f;
				uv.x = 1 - uv.x;
				uv0.Add(uv);
			}

			for (int i = 0; i < tempVertexCache.Count - 1; i++)
			{
				MakeTriangle(b - 1, b + i, b + i + 1);
			}
			MakeTriangle(b - 1, b + tempVertexCache.Count - 1, b);
			PopMatrix();
		}

		/// <summary>
		/// Adds a flat disc to the mesh.
		/// </summary>
		public void AddCircle(Vector3 pos, Vector3 upNormal, float radius, int detail = 32)
		{
			upNormal = Vector3.Normalize(upNormal);
			Quaternion rotation = Quaternion.LookRotation(upNormal) * Quaternion.Euler(90, 180, 0);
			if(upNormal != Vector3.up && upNormal != Vector3.down) rotation *= Quaternion.Euler(0, 180, 0);
			AddCircle(Matrix4x4.TRS(pos, rotation, Vector3.one), radius, detail);
		}

		/// <summary>
		/// Adds a vertical cylinder to the mesh.
		/// </summary>
		public void AddCylinder(Vector3 pos, float radius1, float radius2, float height, int detail = DEFAULT_CIRCLE_DETAIL, bool caps = true)
		{
			if(detail < 3 || detail > 256)
			{
				Debug.LogError("Detail level was out of range. Must be between 3 and 256.");
				detail = Mathf.Clamp(detail, 3, 256);
			}

			float h2 = height * 0.5f;

			var nrmL = Vector3.down;
			var nrmU = Vector3.up;
			GetCirclePoints(tempVertexCache, detail, 1f);

			if(caps)
			{
				AddVertex(TransformPoint(pos + Vector3.down * h2));
				int bL = verts.Count;
				normals.Add(TransformVector(nrmL));
				uv0.Add(new Vector2(0.5f, 0.5f));
				for(int i = 0; i < tempVertexCache.Count - 1; i++)
				{
					AddVertex(TransformPoint(pos + (tempVertexCache[i] * radius1).XZY().WithY(-h2)));
					normals.Add(TransformVector(nrmL));
					uv0.Add(tempVertexCache[i].XY() * 0.5f + new Vector2(0.5f, 0.5f));
					MakeTriangle(bL - 1, bL + i, bL + i + 1);
				}
				AddVertex(TransformPoint(pos + (tempVertexCache[tempVertexCache.Count - 1] * radius1).XZY().WithY(-h2)));
				normals.Add(TransformVector(nrmL));
				uv0.Add(tempVertexCache[tempVertexCache.Count - 1].XY() * 0.5f + new Vector2(0.5f, 0.5f));
				MakeTriangle(bL - 1, bL + tempVertexCache.Count - 1, bL);

				AddVertex(TransformPoint(pos + Vector3.up * h2));
				int bU = verts.Count;
				normals.Add(TransformVector(nrmU));
				uv0.Add(new Vector2(0.5f, 0.5f));
				for(int i = 0; i < tempVertexCache.Count - 1; i++)
				{
					AddVertex(TransformPoint(pos + (tempVertexCache[i] * radius2).XZY().WithY(h2)));
					normals.Add(TransformVector(nrmU));
					uv0.Add(tempVertexCache[i].XY() * 0.5f + new Vector2(0.5f, 0.5f));
					MakeTriangle(bU - 1, bU + i + 1, bU + i);
				}
				AddVertex(TransformPoint(pos + (tempVertexCache[tempVertexCache.Count - 1] * radius2).XZY().WithY(h2)));
				normals.Add(TransformVector(nrmU));
				uv0.Add(tempVertexCache[tempVertexCache.Count - 1].XY() * 0.5f + new Vector2(0.5f, 0.5f));
				MakeTriangle(bU - 1, bU, bU + tempVertexCache.Count - 1);
			}

			float sideNormalY = (radius2 - radius1) / height;
			for(int i = 0; i < tempVertexCache.Count; i++)
			{
				int bM = verts.Count;
				AddVertex(TransformPoint(pos + (tempVertexCache[i] * radius1).XZY().WithY(-h2)));
				AddVertex(TransformPoint(pos + (tempVertexCache[i] * radius2).XZY().WithY(h2)));
				Vector3 normal = tempVertexCache[i].normalized.XZY().WithY(sideNormalY).normalized;
				normals.Add(TransformVector(normal));
				normals.Add(TransformVector(normal));
				uv0.Add(new Vector2(i / (float)tempVertexCache.Count, 0));
				uv0.Add(new Vector2(i / (float)tempVertexCache.Count, 1));
				MakeTriangle(bM, bM + 1, bM + 2);
				MakeTriangle(bM + 3, bM + 2, bM + 1);
			}

			AddVertex(TransformPoint(pos + (tempVertexCache[0] * radius1).XZY().WithY(-h2)));
			AddVertex(TransformPoint(pos + (tempVertexCache[0] * radius2).XZY().WithY(h2)));
			uv0.Add(new Vector2(1, 0));
			uv0.Add(new Vector2(1, 1));
			Vector3 normal1 = tempVertexCache[0].normalized.XZY().WithY(sideNormalY).normalized;
			normals.Add(TransformVector(normal1));
			normals.Add(TransformVector(normal1));
		}

		/// <summary>
		/// Adds a vertical cylinder to the mesh.
		/// </summary>
		public void AddCylinder(Vector3 pos, float radius, float height, int detail = DEFAULT_CIRCLE_DETAIL, bool caps = true)
		{
			AddCylinder(pos, radius, radius, height, detail, caps);
		}

		/// <summary>
		/// Adds a cylinder to the mesh, aligned to the given axis.
		/// </summary>
		public void AddCylinder(Vector3 pos, Axis axis, float radius, float height, int detail = 32, bool caps = true)
		{
			using(PushMatrixScope())
			{
				ApplyMatrix(Matrix4x4.TRS(pos, GetAxisRotation(axis), Vector3.one));
				AddCylinder(Vector3.zero, radius, height, detail, caps);
			}
		}

		/// <summary>
		/// Adds a cylinder to the mesh, starting from the given position and extruded with the given height.
		/// </summary>
		public void AddCylinderFrom(Vector3 pos, AxisDirection direction, float radius, float height, int detail = DEFAULT_CIRCLE_DETAIL, bool caps = true)
		{
			using(PushMatrixScope())
			{
				ApplyMatrix(Matrix4x4.TRS(pos, GetAxisRotation(direction), Vector3.one));
				AddCylinder(Vector3.zero + Vector3.up * height * 0.5f, radius, height, detail, caps);
			}
		}

		/// <summary>
		/// Adds a cone to the mesh, starting from the given position and extruded with the given height.
		/// </summary>
		public void AddCone(Vector3 pos, AxisDirection direction, float radius, float height, int detail = DEFAULT_CIRCLE_DETAIL, bool cap = true)
		{
			using(PushMatrixScope())
			{
				ApplyMatrix(Matrix4x4.TRS(pos, GetAxisRotation(direction), Vector3.one));

				GetCirclePoints(tempVertexCache, detail, radius);
				if(cap)
				{
					int start = verts.Count;
					AddTransformedVertex(Vector3.zero);
					normals.Add(TransformVector(Vector3.down));
					uv0.Add(new Vector2(0.5f, 0.5f));
					for(int i = 0; i < detail; i++)
					{
						AddTransformedVertex(tempVertexCache[i].XZY());
						normals.Add(TransformVector(Vector3.down));
						uv0.Add(tempVertexCache[i].XY() * new Vector2(1, -1) / radius * 0.5f + new Vector2(0.5f, 0.5f));
					}

					for(int i = 0; i < detail - 1; i++)
					{
						MakeTriangle(start + i + 1, start + i + 2, start);
					}
					MakeTriangle(start + detail, start + 1, start);
				}

				int topVertex = verts.Count;
				AddTransformedVertex(Vector3.up * height);
				normals.Add(TransformVector(Vector3.up));
				uv0.Add(new Vector2(0.5f, 0.5f));

				float sideNormalY = radius / height;
				for(int i = 0; i < detail; i++)
				{
					AddTransformedVertex(tempVertexCache[i].XZY());
					Vector3 normal = tempVertexCache[i].normalized.XZY().WithY(sideNormalY).normalized;
					normals.Add(TransformVector(normal));
					uv0.Add(tempVertexCache[i].XY() / radius * 0.5f + new Vector2(0.5f, 0.5f));
				}

				for(int i = 0; i < detail - 1; i++)
				{
					MakeTriangle(topVertex + i + 2, topVertex + i + 1, topVertex);
				}
				MakeTriangle(topVertex + detail, topVertex, topVertex + 1);
			}
		}

		/// <summary>
		/// Adds another mesh to this mesh.
		/// </summary>
		public void AddMesh(Mesh otherMesh)
		{
			int offset = verts.Count;
			foreach(var vert in otherMesh.vertices)
			{
				AddVertex(TransformPoint(vert));
			}
			foreach(var nrm in otherMesh.normals)
			{
				normals.Add(TransformVector(nrm));
			}
			foreach(var uv in otherMesh.uv)
			{
				uv0.Add(uv);
			}
			for (int i = 0; i < otherMesh.triangles.Length; i += 3)
			{
				MakeTriangle(offset + otherMesh.triangles[i], offset + otherMesh.triangles[i + 1], offset + otherMesh.triangles[i + 2]);
			}
		}

		/// <summary>
		/// Adds another mesh to this mesh.
		/// </summary>
		public void AddMesh(Mesh otherMesh, Matrix4x4 matrix)
		{
			using(PushMatrixScope())
			{
				ApplyMatrix(matrix);
				AddMesh(otherMesh);
			}
		}

		private void MakeTriangle(int i0, int i1, int i2)
		{
			if (Reversed)
			{
				tris.Add(i0);
				tris.Add(i2);
				tris.Add(i1);
			}
			else
			{
				tris.Add(i0);
				tris.Add(i1);
				tris.Add(i2);
			}
		}

		/// <summary>
		/// Checks if the current state of the mesh data is valid for mesh creation.
		/// </summary>
		public bool Validate(bool logErrors = false)
		{
			var vertexCount = verts.Count;
			if(uv0.Count > 0 && uv0.Count != vertexCount)
			{
				if(logErrors) Debug.LogError($"Mesh validation failed: UV data length is {uv0.Count} but should be {vertexCount}.");
				return false;
			}
			if(normals.Count > 0 && normals.Count != vertexCount)
			{
				if(logErrors) Debug.LogError($"Mesh validation failed: Normal data length is {normals.Count} but should be {vertexCount}.");
				return false;
			}
			if(vertexColors.Count > 0 && vertexColors.Count != vertexCount)
			{
				if(logErrors) Debug.LogError($"Mesh validation failed: Vertex color data length is {vertexColors.Count} but should be {vertexCount}.");
				return false;
			}
			return true;
		}

		/// <summary>
		/// Applies flat shading to this mesh.
		/// </summary>
		public static void FlatShadeMesh(Mesh m)
		{
			var oldVerts = m.vertices;
			var oldTris = m.triangles;
			var oldUVs = m.uv;
			MeshBuilder mb = new MeshBuilder();
			for(int i = 0; i < oldTris.Length; i += 3)
			{
				int i0 = oldTris[i];
				int i1 = oldTris[i + 1];
				int i2 = oldTris[i + 2];
				Vector2 uv0;
				Vector2 uv1;
				Vector2 uv2;
				if(m.uv.Length > 0)
				{
					uv0 = oldUVs[i0];
					uv1 = oldUVs[i1];
					uv2 = oldUVs[i2];
				}
				else
				{
					uv0 = Vector2.zero;
					uv1 = Vector2.zero;
					uv2 = Vector2.zero;
				}
				mb.AddTriangle(oldVerts[i0], oldVerts[i1], oldVerts[i2], Vector3.zero, uv0, uv1, uv2);
			}
			mb.BuildMesh(m, false);
			m.RecalculateNormals();
			m.RecalculateTangents();
		}
	}
}
