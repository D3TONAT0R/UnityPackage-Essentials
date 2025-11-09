using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	[System.Serializable]
	public abstract class ProceduralTexureFormat : ScriptableObject
	{
		public Vector2Int resolution = new Vector2Int(128, 128);

		public virtual void Read(string json)
		{
			JsonUtility.FromJsonOverwrite(json, this);
		}

		public virtual string Write()
		{
			return JsonUtility.ToJson(this, true);
		}

		public abstract Color GetPixelColor(int x, int y, Vector2 uv);
	}
}