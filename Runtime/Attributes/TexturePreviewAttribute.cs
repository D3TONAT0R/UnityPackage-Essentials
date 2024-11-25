using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Add this attribute to a texture field to show a preview of the texture in the inspector.
	/// </summary>
	public class TexturePreviewAttribute : PropertyAttribute
	{
		public enum Background : byte
		{
			None = 0,
			Black = 1,
			Gray = 2,
			White = 3,
		}

		public bool showObjectField;
		public int fixedHeight;
		public uint backgroundRGBA;

		public TexturePreviewAttribute(bool showObjectField = true, int fixedHeight = -1, uint backgroundRGBA = 0)
		{
			this.showObjectField = showObjectField;
			this.fixedHeight = fixedHeight;
			this.backgroundRGBA = backgroundRGBA;
		}
	}
}
