#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif
using UnityEngine;

namespace D3TEditor
{
	[ScriptedImporter(0, "checker")]
	internal class CheckerTextureImporter : ScriptedTextureGenerator
	{
		[Space(20)]
		public int pixelsPerSquare = 64;
		public int border = 0;
		public Color color1 = Color.black;
		public Color color2 = Color.white;
		public Color borderColor = Color.gray;

		protected override Color GetPixelColor(int x, int y, Vector2 uv)
		{
			Color color;
			int xMod = x % pixelsPerSquare;
			int yMod = y % pixelsPerSquare;
			if(xMod < border || yMod < border || xMod >= pixelsPerSquare - border || yMod >= pixelsPerSquare - border)
			{
				color = borderColor;
			}
			else
			{
				bool c2 = x / pixelsPerSquare % 2 == y / pixelsPerSquare % 2;
				color = c2 ? color2 : color1;
			}
			return color;
		}
	}
}
