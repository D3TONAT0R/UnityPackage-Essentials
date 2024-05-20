using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEssentials.Meshes
{
	public static class TopologyConverter
	{
		public static Mesh ConvertToLineMesh(Mesh triangleMesh, bool avoidDuplicates = true)
		{
			Mesh lineMesh = Object.Instantiate(triangleMesh);

			List<int> srcIndices = new List<int>();
			List<int> indices = new List<int>();
			for(int submesh = 0; submesh < triangleMesh.subMeshCount; submesh++)
			{
				srcIndices.Clear();
				indices.Clear();
				var topology = triangleMesh.GetTopology(submesh);
				triangleMesh.GetIndices(srcIndices, submesh, true);
				if(topology == MeshTopology.Triangles)
				{
					int triangleCount = srcIndices.Count / 3;
					for(int i = 0; i < triangleCount; i++)
					{
						int index0 = srcIndices[i * 3];
						int index1 = srcIndices[i * 3 + 1];
						int index2 = srcIndices[i * 3 + 2];
						//Line 1
						AddLine(indices, index0, index1, avoidDuplicates);
						//Line 2
						AddLine(indices, index1, index2, avoidDuplicates);
						//Line 3
						AddLine(indices, index2, index0, avoidDuplicates);
					}
				}
				else if(topology == MeshTopology.Quads)
				{
					int quadCount = srcIndices.Count / 4;
					for(int i = 0; i < quadCount; i++)
					{
						int index0 = srcIndices[i * 3];
						int index1 = srcIndices[i * 3 + 1];
						int index2 = srcIndices[i * 3 + 2];
						int index3 = srcIndices[i * 3 + 3];
						//Line 1
						AddLine(indices, index0, index1, avoidDuplicates);
						//Line 2
						AddLine(indices, index1, index2, avoidDuplicates);
						//Line 3
						AddLine(indices, index2, index3, avoidDuplicates);
						//Line 4
						AddLine(indices, index3, index0, avoidDuplicates);
					}
				}
				else
				{
					Debug.LogWarning($"Unable to convert submesh {submesh} into a line topology.");
					continue;
				}
				lineMesh.SetIndices(indices, MeshTopology.Lines, submesh);
			}
			lineMesh.UploadMeshData(false);
			return lineMesh;
		}

		public static Mesh ConvertToPointMesh(Mesh mesh)
		{
			var pointMesh = Object.Instantiate(mesh);
			List<int> srcIndices = new List<int>();
			List<int> indices = new List<int>();
			for(int submesh = 0; submesh < pointMesh.subMeshCount; submesh++)
			{
				srcIndices.Clear();
				mesh.GetIndices(srcIndices, submesh);
				indices.Clear();
				indices.AddRange(srcIndices.Distinct());
				pointMesh.SetIndices(indices, MeshTopology.Points, submesh);
			}
			pointMesh.UploadMeshData(false);
			return pointMesh;
		}

		private static void AddLine(List<int> indices, int i0, int i1, bool avoidDuplicates)
		{
			if(avoidDuplicates)
			{
				int lineCount = indices.Count / 2;
				for(int i = 0; i < lineCount; i++)
				{
					if((indices[i * 2] == i0 && indices[i * 2 + 1] == i1) || (indices[i * 2] == i1 && indices[i * 2 + 1] == i0))
					{
						//Line already exists, don't add it
						return;
					}
				}
			}
			indices.Add(i0);
			indices.Add(i1);
		}
	}
}
