using UnityEditor;

namespace D3TEditor
{
	internal static class CreateCustomAssetMenuItems
	{
		[MenuItem("Assets/Create/Texture/Gradient Texture")]
		public static void CreateGradientTextureAsset() => CreateAsset("New Gradient Texture.gradient");

		[MenuItem("Assets/Create/Texture/Checker Texture")]
		public static void CreateCheckerTextureAsset() => CreateAsset("New Checker Texture.checker");

		public static void CreateAsset(string newFileName)
		{
			ProjectWindowUtil.CreateAssetWithContent(newFileName, "");
		}
	}
}
