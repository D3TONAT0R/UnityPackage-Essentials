using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials
{
	public interface IGraphicsBufferDataProvider<T> where T : struct
	{
		T GetGraphicsBufferData();
	}
	
	/// <summary>
	/// Helper class to build and upload GraphicsBuffers to the GPU.
	/// </summary>
	public class ShaderBufferBuilder<T> : IDisposable where T : struct
	{
		public readonly List<T> data = new List<T>();
		public readonly GraphicsBuffer buffer;
		public readonly bool useLockBufferForWrite;

		public int bufferParameterID;
		public int countParameterID;
		
		public int Count => data.Count;
		public int MaxCount { get; }

		public ShaderBufferBuilder(int maxCount, string bufferParameterName, string countParameterName, bool useLockBufferForWrite)
		{
			int stride = System.Runtime.InteropServices.Marshal.SizeOf<T>();
			if (stride % 16 != 0)
			{
				// Debug.LogWarning($"StructuredBuffer<{typeof(T).Name}> does not have a stride of multiple of 16 bytes (actual: {stride})");
			}
			// LockBufferForWrite causes shaders to receive delayed buffer data (and probably glitches too)
			//buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, GraphicsBuffer.UsageFlags.LockBufferForWrite, maxCount, stride);
			
			var usageFlags = useLockBufferForWrite ? GraphicsBuffer.UsageFlags.LockBufferForWrite : GraphicsBuffer.UsageFlags.None;
			buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, usageFlags, maxCount, stride);
			this.useLockBufferForWrite = useLockBufferForWrite;
			MaxCount = maxCount;
			bufferParameterID = Shader.PropertyToID(bufferParameterName);
			countParameterID = Shader.PropertyToID(countParameterName);
		}
		
		public void Clear() => data.Clear();
		
		public void Add(T element)
		{
			if (Count >= MaxCount)
			{
				Debug.LogWarning("Maximum number of elements reached for graphics buffer.");
				return;
			}
			data.Add(element);
		}

		public void Build<U>(IEnumerable<U> source) where U : IGraphicsBufferDataProvider<T>
		{
			Clear();
			foreach (var item in source)
			{
				Add(item.GetGraphicsBufferData());
			}
		}

		public void UploadToBuffer()
		{
			if (useLockBufferForWrite)
			{
				var array = buffer.LockBufferForWrite<T>(0, Count);
				try
				{
					for (int i = 0; i < Count; i++)
					{
						array[i] = data[i];
					}
				}
				finally
				{
					// Upload data to GPU
					// Debug.Log(Count);
					buffer.UnlockBufferAfterWrite<T>(Count);
				}
			}
			else
			{
				buffer.SetData(data);
			}
		}

		public void SetParameters(Material material)
		{
			// Set buffer and count for material
			material.SetBuffer(bufferParameterID, buffer);
			material.SetInt(countParameterID, Count);
		}
		
		public void SetGlobalParameters()
		{
			// Set global buffer and count for shader
			Shader.SetGlobalBuffer(bufferParameterID, buffer);
			Shader.SetGlobalInt(countParameterID, Count);
		}

		public void UploadParameters(Material material)
		{
			UploadToBuffer();
			SetParameters(material);
		}
		
		public void UploadGlobalParameters()
		{
			UploadToBuffer();
			SetGlobalParameters();
		}

		public void Dispose()
		{
			buffer?.Dispose();
		}
	}
}