using UnityEngine;

namespace UnityEssentials
{
	public static class TextureExtensions
	{
		/// <summary>
		/// Fills all pixels of this texture with a given color.
		/// </summary>
		public static void FillWithColor(this Texture2D tex, Color c, bool apply = true)
		{
			Color32[] cols = new Color32[tex.width * tex.height];
			for(int i = 0; i < cols.Length; i++) cols[i] = c;
			tex.SetPixels32(cols);
			if(apply) tex.Apply();
		}

		/// <summary>
		/// Returns a copy of this texture which is read/write enabled.
		/// </summary>
		public static Texture2D GetReadableCopy(this Texture2D tex, string name = null)
		{
			if(tex.isReadable)
			{
				Debug.LogWarning($"Texture '{tex.name}' is already readable.");
				return tex;
			}
			Texture2D copy = new Texture2D(tex.width, tex.height, tex.format, tex.mipmapCount, false)
			{
				name = name != null ? name : $"{tex.name} (Readable)",
				wrapMode = tex.wrapMode,
				filterMode = tex.filterMode
			};
			Graphics.CopyTexture(tex, copy);
			copy.Apply();
			return copy;
		}

		/// <summary>
		/// Returns the texture's width and height as a <see cref="Vector2Int"/>.
		/// </summary>
		public static Vector2Int GetResolution(this Texture tex)
		{
			return new Vector2Int(tex.width, tex.height);
		}

		/// <summary>
		/// Returns the texture's aspect ratio (width / height)
		/// </summary>
		public static float GetAspectRatio(this Texture tex)
		{
			return tex.width / (float)tex.height;
		}

		/// <summary>
		/// Returns the texture's inverse aspect ratio (height / width)
		/// </summary>
		public static float GetInverseAspectRatio(this Texture tex)
		{
			return tex.height / (float)tex.width;
		}
	}
}
