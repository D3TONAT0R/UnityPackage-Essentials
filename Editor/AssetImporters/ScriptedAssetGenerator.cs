using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace D3TEditor
{
	public abstract class ScriptedTextureGenerator : ScriptedImporter
	{
		public Vector2Int resolution = new Vector2Int(128, 128);
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
					Vector2 uv = new Vector2((x + 0.5f) / resolution.x, (y + 0.5f) / resolution.y);
					texture.SetPixel(x, y, GetPixelColor(x, y, uv));
				}
			}
			texture.Apply();
			ctx.AddObjectToAsset("texture", texture);
		}

		protected abstract Color GetPixelColor(int x, int y, Vector2 uv);
	}
}