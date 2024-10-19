using System.IO;
using UnityEngine;
using UnityEditor;
#if UNITY_2020_2_OR_NEWER
using ScriptedImporter = UnityEditor.AssetImporters.ScriptedImporter;
using AssetImportContext = UnityEditor.AssetImporters.AssetImportContext;
#else
using ScriptedImporter = UnityEditor.Experimental.AssetImporters.ScriptedImporter;
using AssetImportContext = UnityEditor.Experimental.AssetImporters.AssetImportContext;
#endif

namespace UnityEssentialsEditor
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
			ctx.SetMainObject(texture);
		}

		protected abstract Color GetPixelColor(int x, int y, Vector2 uv);

		public void Bake()
		{
			//Display dialog to confirm baking
			bool confirmed = EditorUtility.DisplayDialog("Bake Texture", "Are you sure you want to bake the generated texture and replace the original asset? This cannot be undone.", "OK", "Cancel");
			if(!confirmed) return;

			EditorApplication.delayCall += () =>
			{
				var pngPath = BakeTexture(assetPath);
				Selection.activeObject = AssetDatabase.LoadAssetAtPath<Texture2D>(pngPath);
			};
		}

		//TODO: bake operation breaks references in other assets (such as materials)
		public static string BakeTexture(string assetPath)
		{
			var scriptedImporter = (ScriptedTextureGenerator)GetAtPath(assetPath);
			var linear = scriptedImporter.linear;
			var generateMipMaps = scriptedImporter.generateMipMaps;
			var wrapMode = scriptedImporter.wrapMode;
			var filterMode = scriptedImporter.filterMode;

			var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
			var pngData = texture.EncodeToPNG();
			string pngPath = Path.ChangeExtension(assetPath, "png");
			File.WriteAllBytes(pngPath, pngData);
			AssetDatabase.ImportAsset(pngPath, ImportAssetOptions.ForceUpdate);

			var textureMetaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(pngPath);
			var assetMetaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPath);
			var assetMetaLines = File.ReadAllLines(assetMetaPath);
			string originalGUID = assetMetaLines[1].Split(':')[1].Trim();
			File.Delete(assetPath);
			File.Delete(assetMetaPath);
			AssetDatabase.Refresh();


			var textureMetaLines = File.ReadAllLines(textureMetaPath);
			textureMetaLines[1] = $"guid: {originalGUID}";
			File.WriteAllLines(textureMetaPath, textureMetaLines);
			//AssetDatabase.CreateAsset(Instantiate(texture), assetPath);

			var pngImporter = (TextureImporter)GetAtPath(pngPath);
			pngImporter.sRGBTexture = !linear;
			pngImporter.mipmapEnabled = generateMipMaps;
			pngImporter.wrapMode = wrapMode;
			pngImporter.filterMode = filterMode;
			pngImporter.alphaIsTransparency = true;
			pngImporter.SaveAndReimport();
			AssetDatabase.Refresh();
			return pngPath;
		}
	}
}