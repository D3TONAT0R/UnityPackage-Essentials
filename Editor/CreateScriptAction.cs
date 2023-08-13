using UnityEngine;
using UnityEditor.ProjectWindowCallback;
using UnityEditor;
using System.IO;
using System.Reflection;

namespace D3TEditor
{
	public class CreateScriptAction : EndNameEditAction
	{
		public override void Action(int instanceId, string pathName, string resourceFile)
		{
			var method = typeof(ProjectWindowUtil).GetMethod("CreateScriptAssetWithContent", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			string content = ModifyTemplateContent(File.ReadAllText(Path.GetFullPath(resourceFile)), pathName);
			Object o = (Object)method.Invoke(null, new object[] { pathName + ".cs", content });
			ProjectWindowUtil.ShowCreatedAsset(o);
		}

		private static string ModifyTemplateContent(string text, string pathName)
		{
			text = text.Replace("#NOTRIM#", "");
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
			text = text.Replace("#NAME#", fileNameWithoutExtension);
			string text2 = fileNameWithoutExtension.Replace(" ", "");
			text = text.Replace("#SCRIPTNAME#", text2);
			if(char.IsUpper(text2, 0))
			{
				text2 = char.ToLower(text2[0]) + text2.Substring(1);
				text = text.Replace("#SCRIPTNAME_LOWER#", text2);
			}
			else
			{
				text2 = "my" + char.ToUpper(text2[0]) + text2.Substring(1);
				text = text.Replace("#SCRIPTNAME_LOWER#", text2);
			}

			text = text.Replace("#NAMESPACE#", CreateScriptMenuUtility.DefaultNamespace);

			return text;
		}
	}
}
