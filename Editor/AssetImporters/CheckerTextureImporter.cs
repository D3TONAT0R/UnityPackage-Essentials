#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif
using UnityEngine;

namespace D3TEditor
{
	[ScriptedImporter(0, "checker")]
	public class CheckerTextureImporter : ScriptedImporter
	{

		public Vector2Int resolution = new Vector2Int(128, 128);

		public int pixelsPerSquare = 64;
		public int border = 0;
		public Color color1 = Color.black;
		public Color color2 = Color.white;
		public Color borderColor = Color.gray;

		[Space(10)]
		public bool linear = false;
		public bool generateMipMaps = true;
		public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
		public FilterMode filterMode = FilterMode.Bilinear;

		public override void OnImportAsset(AssetImportContext ctx)
		{
			resolution.x = Mathf.Clamp(resolution.x, 1, 4096);
			resolution.y = Mathf.Clamp(resolution.y, 1, 4096);
			var texture = new Texture2D(resolution.x, resolution.y, TextureFormat.RGBA32, generateMipMaps, linear);
			texture.alphaIsTransparency = true;
			texture.wrapMode = wrapMode;
			texture.filterMode = filterMode;

			for(int x = 0; x < resolution.x; x++)
			{
				for(int y = 0; y < resolution.y; y++)
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
					texture.SetPixel(x, y, color);
				}
			}
			texture.Apply();
			ctx.AddObjectToAsset("texture", texture);
		}
	} 
}
