using System.Collections.Generic;
using UnityEngine;

namespace D3T.Meshes
{
	public class VoxelMeshBuilder : VoxelMeshBuilder<bool>
	{
		public VoxelMeshBuilder(Vector3Int size, float voxelSize = 1) : base(size, voxelSize)
		{

		}

		protected override bool IsVisible(bool value) => value;

		protected override bool IsSolid(bool value) => value;
	}

	public abstract class VoxelMeshBuilder<T> : MeshBuilderBase where T : struct
	{
		private readonly T[,,] voxelData;
		private readonly List<Vector3> normals = new List<Vector3>();
		private readonly List<Vector2> uvs = new List<Vector2>();
		private readonly List<int> tris = new List<int>();

		public Vector3Int Dimensions { get; private set; }
		public float VoxelSize { get; set; } = 1f;

		public virtual T OutOfBoundsValue => default;

		public bool GenerateBoundary { get; set; } = true;

		public VoxelMeshBuilder(Vector3Int size, float voxelSize = 1f)
		{
			Dimensions = size;
			voxelData = new T[size.x, size.y, size.z];
			VoxelSize = voxelSize;
		}

		public void EnsureVertexCapacity(int capacity)
		{
			EnsureCapacity(verts, capacity);
			EnsureCapacity(vertexColors, capacity);
			EnsureCapacity(normals, capacity);
			EnsureCapacity(uvs, capacity);
			EnsureCapacity(tris, capacity);
		}

		private void EnsureCapacity<U>(List<U> list, int capacity)
		{
			if(list.Capacity < capacity)
			{
				list.Capacity = capacity;
			}
		}

		public void SetVoxel(int x, int y, int z, T value)
		{
			voxelData[x, y, z] = value;
		}

		public void SetVoxel(Vector3Int pos, T value)
		{
			SetVoxel(pos.x, pos.y, pos.z, value);
		}

		public T GetVoxel(int x, int y, int z)
		{
			if(!IsWithinBounds(x, y, z)) return OutOfBoundsValue;
			return voxelData[x, y, z];
		}

		public T GetVoxel(Vector3Int pos)
		{
			return GetVoxel(pos.x, pos.y, pos.z);
		}

		protected abstract bool IsVisible(T value);

		protected abstract bool IsSolid(T value);

		protected virtual bool CheckFace(Vector3Int block, AxisDirection face)
		{
			OffsetPosition(ref block, face);
			if(!IsWithinBounds(block)) return !GenerateBoundary;
			else return IsSolid(voxelData[block.x, block.y, block.z]);
		}

		protected virtual Color32 GetFaceColor(T value, AxisDirection face)
		{
			return new Color32(255, 255, 255, 255);
		}

		protected virtual Vector4 GetFaceUVs(Vector3Int pos, T voxel, AxisDirection face)
		{
			return new Vector4(0, 0, 1, 1);
		}

		protected static void OffsetPosition(ref Vector3Int pos, AxisDirection direction)
		{
			switch(direction)
			{
				case AxisDirection.XNeg:
					pos.x--;
					break;
				case AxisDirection.XPos:
					pos.x++;
					break;
				case AxisDirection.YNeg:
					pos.y--;
					break;
				case AxisDirection.YPos:
					pos.y++;
					break;
				case AxisDirection.ZNeg:
					pos.z--;
					break;
				case AxisDirection.ZPos:
					pos.z++;
					break;
			}
		}

		public bool IsWithinBounds(Vector3Int pos)
		{
			return IsWithinBounds(pos.x, pos.y, pos.z);
		}

		public bool IsWithinBounds(int x, int y, int z)
		{
			return x >= 0 && x < Dimensions.x && y >= 0 && y < Dimensions.y && z >= 0 && z < Dimensions.z;
		}

		public override void BuildMesh(Mesh mesh)
		{
			verts.Clear();
			vertexColors.Clear();
			normals.Clear();
			uvs.Clear();
			tris.Clear();
			for(int z = 0; z < Dimensions.z; z++)
			{
				for(int y = 0; y < Dimensions.y; y++)
				{
					for(int x = 0; x < Dimensions.x; x++)
					{
						var voxel = voxelData[x, y, z];
						if(IsVisible(voxel)) AddVoxelToMesh(x, y, z, voxel);
					}
				}
			}
			mesh.Clear();
			mesh.SetVertices(verts);
			mesh.SetColors(vertexColors);
			mesh.SetNormals(normals);
			mesh.SetUVs(0, uvs);
			mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
			mesh.SetTriangles(tris, 0);
			mesh.RecalculateBounds();
		}

		public override void Clear()
		{
			for(int z = 0; z < Dimensions.z; z++)
			{
				for(int y = 0; y < Dimensions.y; y++)
				{
					for(int x = 0; x < Dimensions.x; x++)
					{
						voxelData[x, y, z] = default;
					}
				}
			}
			currentMatrix = Matrix4x4.identity;
			matrixStack.Clear();
			verts.Clear();
			vertexColors.Clear();
			normals.Clear();
			uvs.Clear();
			tris.Clear();
		}

		private void AddVoxelToMesh(int x, int y, int z, T voxel)
		{
			Vector3Int block = new Vector3Int(x, y, z);
			float x1 = x * VoxelSize;
			float y1 = y * VoxelSize;
			float z1 = z * VoxelSize;
			float x2 = x1 + VoxelSize;
			float y2 = y1 + VoxelSize;
			float z2 = z1 + VoxelSize;
			//Top face
			if(!CheckFace(block, AxisDirection.YPos))
			{
				AddQuad(block, voxel, new Vector3(x1, y2, z1), new Vector3(VoxelSize, 0, 0), new Vector3(0, 0, VoxelSize), AxisDirection.YPos);
			}
			//Bottom face
			if(!CheckFace(block, AxisDirection.YNeg))
			{
				AddQuad(block, voxel, new Vector3(x1, y1, z2), new Vector3(VoxelSize, 0, 0), new Vector3(0, 0, -VoxelSize), AxisDirection.YNeg);
			}
			//Front face
			if(!CheckFace(block, AxisDirection.ZNeg))
			{
				AddQuad(block, voxel, new Vector3(x1, y1, z1), new Vector3(VoxelSize, 0, 0), new Vector3(0, VoxelSize, 0), AxisDirection.ZNeg);
			}
			//Back face
			if(!CheckFace(block, AxisDirection.ZPos))
			{
				AddQuad(block, voxel, new Vector3(x2, y1, z2), new Vector3(-VoxelSize, 0, 0), new Vector3(0, VoxelSize, 0), AxisDirection.ZPos);
			}
			//Left face
			if(!CheckFace(block, AxisDirection.XNeg))
			{
				AddQuad(block, voxel, new Vector3(x1, y1, z2), new Vector3(0, 0, -VoxelSize), new Vector3(0, VoxelSize, 0), AxisDirection.XNeg);
			}
			//Right face
			if(!CheckFace(block, AxisDirection.XPos))
			{
				AddQuad(block, voxel, new Vector3(x2, y1, z1), new Vector3(0, 0, VoxelSize), new Vector3(0, VoxelSize, 0), AxisDirection.XPos);
			}
		}

		private void AddQuad(Vector3Int block, T voxel, Vector3 pos, Vector3 right, Vector3 up, AxisDirection face)
		{
			int firstIndex = verts.Count;
			AddTransformedVertex(pos);
			AddTransformedVertex(pos + right);
			AddTransformedVertex(pos + up);
			AddTransformedVertex(pos + right + up);
			var normal = face.GetDirectionVector();
			normal = TransformVector(normal);
			normals.Add(normal);
			normals.Add(normal);
			normals.Add(normal);
			normals.Add(normal);
			Vector4 uv = GetFaceUVs(block, voxel, face);
			uvs.Add(new Vector2(uv.x, uv.y));
			uvs.Add(new Vector2(uv.z, uv.y));
			uvs.Add(new Vector2(uv.x, uv.w));
			uvs.Add(new Vector2(uv.z, uv.w));
			Color32 color = GetFaceColor(voxel, face);
			vertexColors.Add(color);
			vertexColors.Add(color);
			vertexColors.Add(color);
			vertexColors.Add(color);

			tris.Add(firstIndex);
			tris.Add(firstIndex + 2);
			tris.Add(firstIndex + 1);

			tris.Add(firstIndex + 2);
			tris.Add(firstIndex + 3);
			tris.Add(firstIndex + 1);
		}
	}
}
