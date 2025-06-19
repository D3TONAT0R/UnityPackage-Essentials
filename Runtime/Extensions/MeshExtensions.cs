using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEssentials
{
	public static class MeshExtensions
	{
		/// <summary>
		/// Returns a copy of this mesh which is read/write enabled.
		/// </summary>
		public static Mesh GetReadableCopy(this Mesh mesh, string name = null)
		{
			if(mesh.isReadable)
			{
				Debug.LogWarning($"Mesh '{mesh.name}' is already readable.");
				return mesh;
			}
			var copy = new Mesh { name = $"{mesh.name} (Readable)" };
			copy.indexFormat = mesh.indexFormat;
			// Handle vertices
			GraphicsBuffer verticesBuffer = mesh.GetVertexBuffer(0);
			int totalSize = verticesBuffer.stride * verticesBuffer.count;
			byte[] data = new byte[totalSize];
			verticesBuffer.GetData(data);
			copy.SetVertexBufferParams(mesh.vertexCount, mesh.GetVertexAttributes());
			copy.SetVertexBufferData(data, 0, 0, totalSize);
			verticesBuffer.Release();
			// Handle triangles
			copy.subMeshCount = mesh.subMeshCount;
			GraphicsBuffer indexesBuffer = mesh.GetIndexBuffer();
			int tot = indexesBuffer.stride * indexesBuffer.count;
			byte[] indexesData = new byte[tot];
			indexesBuffer.GetData(indexesData);
			copy.SetIndexBufferParams(indexesBuffer.count, mesh.indexFormat);
			copy.SetIndexBufferData(indexesData, 0, 0, tot);
			indexesBuffer.Release();
			// Restore submesh structure
			uint currentIndexOffset = 0;
			for(int i = 0; i < copy.subMeshCount; i++)
			{
				uint subMeshIndexCount = mesh.GetIndexCount(i);
				copy.SetSubMesh(i, new SubMeshDescriptor((int)currentIndexOffset, (int)subMeshIndexCount));
				currentIndexOffset += subMeshIndexCount;
			}
			//TODO: copy bone information
			// Recalculate normals and bounds
			//copy.RecalculateNormals();
			//copy.RecalculateBounds();
			copy.bounds = mesh.bounds;
			return copy;
		}
	}
}
