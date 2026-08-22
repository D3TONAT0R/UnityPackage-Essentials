using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEssentials.Meshes
{
	public static class TopologyConverter
	{
		/// <summary>
		/// Converts the given triangular mesh into a line mesh.
		/// </summary>
		public static Mesh ConvertToLineMesh(Mesh triangleMesh, bool avoidDuplicates = true)
		{
			Mesh lineMesh;
			if(!triangleMesh.isReadable)
			{
#if UNITY_2021_2_OR_NEWER
				lineMesh = triangleMesh.GetReadableCopy();
#else
				throw new System.InvalidOperationException("Mesh must be readable: " + triangleMesh);
#endif
			}
			else lineMesh = Object.Instantiate(triangleMesh);

			List<int> srcIndices = new List<int>();
			List<int> indices = new List<int>();
HashSet<ulong> seenEdges = avoidDuplicates ? new HashSet<ulong>() : null;
for(int submesh = 0; submesh < lineMesh.subMeshCount; submesh++)
{
	srcIndices.Clear();
	indices.Clear();
	if(avoidDuplicates) seenEdges.Clear();
	var topology = lineMesh.GetTopology(submesh);
	lineMesh.GetIndices(srcIndices, submesh, true);
	if(topology == MeshTopology.Triangles)
	{
		int triangleCount = srcIndices.Count / 3;
		for(int i = 0; i < triangleCount; i++)
		{
			int index0 = srcIndices[i * 3];
			int index1 = srcIndices[i * 3 + 1];
			int index2 = srcIndices[i * 3 + 2];
			AddLine(indices, seenEdges, index0, index1, avoidDuplicates);
			AddLine(indices, seenEdges, index1, index2, avoidDuplicates);
			AddLine(indices, seenEdges, index2, index0, avoidDuplicates);
		}
	}
	else if(topology == MeshTopology.Quads)
	{
		int quadCount = srcIndices.Count / 4;
		for(int i = 0; i < quadCount; i++)
		{
			int index0 = srcIndices[i * 4];
			int index1 = srcIndices[i * 4 + 1];
			int index2 = srcIndices[i * 4 + 2];
			int index3 = srcIndices[i * 4 + 3];
			AddLine(indices, seenEdges, index0, index1, avoidDuplicates);
			AddLine(indices, seenEdges, index1, index2, avoidDuplicates);
			AddLine(indices, seenEdges, index2, index3, avoidDuplicates);
			AddLine(indices, seenEdges, index3, index0, avoidDuplicates);
		}
	}
	else
	{
		Debug.LogWarning($"Unable to convert submesh {submesh} into line topology; leaving the submesh unchanged.");
		continue;
				}
	lineMesh.SetIndices(indices, MeshTopology.Lines, submesh);
}
lineMesh.UploadMeshData(false);
return lineMesh;
		}

		/// <summary>
		/// Converts the given mesh into a point cloud mesh.
		/// </summary>
		public static Mesh ConvertToPointMesh(Mesh mesh)
		{
Mesh pointMesh;
if(!mesh.isReadable)
{
#if UNITY_2021_2_OR_NEWER
	pointMesh = mesh.GetReadableCopy();
#else
	throw new System.InvalidOperationException("Mesh must be readable: " + mesh);
#endif
}
else pointMesh = Object.Instantiate(mesh);

List<int> srcIndices = new List<int>();
List<int> indices = new List<int>();
for(int submesh = 0; submesh < pointMesh.subMeshCount; submesh++)
{
	srcIndices.Clear();
	pointMesh.GetIndices(srcIndices, submesh);
	indices.Clear();
	indices.AddRange(srcIndices.Distinct());
	pointMesh.SetIndices(indices, MeshTopology.Points, submesh);
}
pointMesh.UploadMeshData(false);
return pointMesh;
		}

		private static void AddLine(List<int> indices, HashSet<ulong> seenEdges, int i0, int i1, bool avoidDuplicates)
		{
if(avoidDuplicates)
{
	int minIndex = i0 < i1 ? i0 : i1;
	int maxIndex = i0 < i1 ? i1 : i0;
	ulong edgeKey = ((ulong)(uint)minIndex << 32) | (uint)maxIndex;
	if(!seenEdges.Add(edgeKey)) return;
			}
			indices.Add(i0);
			indices.Add(i1);
		}
	}
}
