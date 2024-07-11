using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using Object = UnityEngine.Object;

namespace UnityEssentialsEditor
{
	internal class CreateScriptAction : EndNameEditAction
	{
		public override void Action(int instanceId, string pathName, string resourceFile)
		{
			var method = typeof(ProjectWindowUtil).GetMethod("CreateScriptAssetWithContent", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			string content = ModifyTemplateContent(File.ReadAllText(Path.GetFullPath(resourceFile)), pathName);
			string finalPath = AssetDatabase.GenerateUniqueAssetPath(pathName + ".cs");
			Object o = (Object)method.Invoke(null, new object[] { finalPath, content });
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

			var additionalUsings = EssentialsProjectSettings.Instance.additionalDefaultUsings;
			string usingsString = "";
			if(additionalUsings != null && additionalUsings.Length > 0)
			{
				for(int i = 0; i < additionalUsings.Length; i++)
				{
					//if(i > 0) usingsString += Environment.NewLine;
					usingsString += $"using {additionalUsings[i]};{Environment.NewLine}";
				}
			}
			text = Regex.Replace(text, @"#USINGS#.*\n", usingsString);

			return text;
		}
	}
}
