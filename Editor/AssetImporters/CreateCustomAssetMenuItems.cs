using UnityEditor;

namespace UnityEssentialsEditor
{
	internal static class CreateCustomAssetMenuItems
	{
		[MenuItem("Assets/Create/Texture2D/Gradient Texture")]
		public static void CreateGradientTextureAsset() => CreateAsset("New Gradient Texture.gradient");

		[MenuItem("Assets/Create/Texture2D/Checker Texture")]
		public static void CreateCheckerTextureAsset() => CreateAsset("New Checker Texture.checker");

		public static void CreateAsset(string newFileName)
		{
			ProjectWindowUtil.CreateAssetWithContent(newFileName, "");
		}
	}
}
