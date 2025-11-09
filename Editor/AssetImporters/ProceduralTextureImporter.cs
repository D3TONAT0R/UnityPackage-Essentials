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
	internal interface IProceduralTextureGenerator
	{
		ProceduralTexureFormat LoadFormat();

		void Bake();
	}

	public abstract class ProceduralTextureImporter<T> : ScriptedImporter, IProceduralTextureGenerator where T : ProceduralTexureFormat, new()
	{
		[Space(10)]
		public bool linear = false;
		public bool generateMipMaps = true;
		public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
		public FilterMode filterMode = FilterMode.Bilinear;

		public ProceduralTexureFormat LoadFormat()
		{
			var text = File.ReadAllText(assetPath);
			var format = ScriptableObject.CreateInstance<T>();
			format.Read(text);
			return format;
		}

		public override void OnImportAsset(AssetImportContext ctx)
		{
			var format = ScriptableObject.CreateInstance<T>();
			var texture = CreateBlankTexture(format);

			for(int x = 0; x < texture.width; x++)
			{
				for(int y = 0; y < texture.height; y++)
				{
					Vector2 uv = new Vector2((x + 0.5f) / texture.width, (y + 0.5f) / texture.height);
					texture.SetPixel(x, y, format.GetPixelColor(x, y, uv));
				}
			}
			texture.Apply();
			ctx.AddObjectToAsset("texture", texture);
			ctx.SetMainObject(texture);
		}

		private Texture2D CreateBlankTexture(ProceduralTexureFormat format)
		{
			format.Read(File.ReadAllText(assetPath));
			var resolution = format.resolution;
			resolution.x = Mathf.Clamp(resolution.x, 1, 4096);
			resolution.y = Mathf.Clamp(resolution.y, 1, 4096);
			var texture = new Texture2D(resolution.x, resolution.y, TextureFormat.RGBA32, generateMipMaps, linear);
			texture.alphaIsTransparency = true;
			texture.wrapMode = wrapMode;
			texture.filterMode = filterMode;
			return texture;
		}

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
			pngImporter.sRGBTexture = texture.isDataSRGB;
			pngImporter.mipmapEnabled = texture.mipmapCount > 1;
			pngImporter.wrapMode = texture.wrapMode;
			pngImporter.filterMode = texture.filterMode;
			pngImporter.alphaIsTransparency = texture.alphaIsTransparency;
			pngImporter.SaveAndReimport();
			AssetDatabase.Refresh();
			return pngPath;
		}
	}
}