using System.Collections.Generic;
using UnityEngine;

namespace D3T
{
	public class VoxelMeshBuilder : MeshBuilderBase
	{
		public struct Voxel
		{
			public bool value;
			public Color32 color;

			public Voxel(bool value, Color32? color)
			{
				this.value = value;
				this.color = color ?? new Color32(255, 255, 255, 255);
			}
		}

		private Vector3Int size;
		private Voxel[,,] voxelData;

		public float VoxelSize { get; set; } = 1f;

		public bool GenerateBoundary { get; set; } = true;

		private List<Vector3> normals = new List<Vector3>();
		private List<Vector2> uvs = new List<Vector2>();
		private List<int> tris = new List<int>();

		public VoxelMeshBuilder(Vector3Int size, float voxelSize = 1f)
		{
			this.size = size;
			voxelData = new Voxel[size.x, size.y, size.z];
			this.VoxelSize = voxelSize;
		}

		public VoxelMeshBuilder(int sizeX, int sizeY, int sizeZ, float voxelSize = 1f)
		{
			voxelData = new Voxel[sizeX, sizeY, sizeZ];
			size = new Vector3Int(sizeX, sizeY, sizeZ);
			this.VoxelSize = voxelSize;
		}

		public void SetVoxel(int x, int y, int z, bool value, Color32? color = null)
		{
			voxelData[x, y, z].value = value;
			voxelData[x, y, z].color = color ?? new Color32(255, 255, 255, 255);
		}

		public void SetVoxel(Vector3Int pos, bool value, Color32? color = null)
		{
			SetVoxel(pos.x, pos.y, pos.z, value, color);
		}

		public bool GetVoxel(int x, int y, int z)
		{
			if(!IsWithinBounds(x, y, z)) return false;
			return voxelData[x, y, z].value;
		}

		public bool GetVoxel(Vector3Int pos)
		{
			return GetVoxel(pos.x, pos.y, pos.z);
		}

		private bool CheckFace(int x, int y, int z)
		{
			if(!IsWithinBounds(x, y, z)) return !GenerateBoundary;
			else return voxelData[x, y, z].value;
		}

		public bool IsWithinBounds(Vector3Int pos)
		{
			return IsWithinBounds(pos.x, pos.y, pos.z);
		}

		public bool IsWithinBounds(int x, int y, int z)
		{
			return x >= 0 && x < size.x && y >= 0 && y < size.y && z >= 0 && z < size.z;
		}

		public override void BuildMesh(Mesh mesh)
		{
			for(int z = 0; z < size.z; z++)
			{
				for(int y = 0; y < size.y; y++)
				{
					for(int x = 0; x < size.x; x++)
					{
						var voxel = voxelData[x, y, z];
						if(voxel.value) AddVoxelToMesh(x, y, z, voxel.color);
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
			currentMatrix = Matrix4x4.identity;
			matrixStack.Clear();
			verts.Clear();
			vertexColors.Clear();
			voxelData = new Voxel[size.x, size.y, size.z];
			normals.Clear();
			uvs.Clear();
			tris.Clear();
		}

		private void AddVoxelToMesh(int x, int y, int z, Color32 color)
		{
			float x1 = x * VoxelSize;
			float y1 = y * VoxelSize;
			float z1 = z * VoxelSize;
			float x2 = x1 + VoxelSize;
			float y2 = y1 + VoxelSize;
			float z2 = z1 + VoxelSize;
			//Top face
			if(!CheckFace(x, y + 1, z))
			{
				AddQuad(new Vector3(x1, y2, z1), new Vector3(VoxelSize, 0, 0), new Vector3(0, 0, VoxelSize), Vector3.up, color);
			}
			//Bottom face
			if(!CheckFace(x, y - 1, z))
			{
				AddQuad(new Vector3(x1, y1, z2), new Vector3(VoxelSize, 0, 0), new Vector3(0, 0, -VoxelSize), Vector3.down, color);
			}
			//Front face
			if(!CheckFace(x, y, z - 1))
			{
				AddQuad(new Vector3(x1, y1, z1), new Vector3(VoxelSize, 0, 0), new Vector3(0, VoxelSize, 0), Vector3.back, color);
			}
			//Back face
			if(!CheckFace(x, y, z + 1))
			{
				AddQuad(new Vector3(x2, y1, z2), new Vector3(-VoxelSize, 0, 0), new Vector3(0, VoxelSize, 0), Vector3.forward, color);
			}
			//Left face
			if(!CheckFace(x - 1, y, z))
			{
				AddQuad(new Vector3(x1, y1, z2), new Vector3(0, 0, -VoxelSize), new Vector3(0, VoxelSize, 0), Vector3.left, color);
			}
			//Right face
			if(!CheckFace(x + 1, y, z))
			{
				AddQuad(new Vector3(x2, y1, z1), new Vector3(0, 0, VoxelSize), new Vector3(0, VoxelSize, 0), Vector3.right, color);
			}
		}

		private void AddQuad(Vector3 pos, Vector3 right, Vector3 up, Vector3 normal, Color32 color)
		{
			int firstIndex = verts.Count;
			AddTransformedVertex(pos);
			AddTransformedVertex(pos + right);
			AddTransformedVertex(pos + up);
			AddTransformedVertex(pos + right + up);
			normal = TransformVector(normal);
			normals.Add(normal);
			normals.Add(normal);
			normals.Add(normal);
			normals.Add(normal);
			uvs.Add(Vector2.zero);
			uvs.Add(Vector2.right);
			uvs.Add(Vector2.up);
			uvs.Add(Vector2.one);
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
