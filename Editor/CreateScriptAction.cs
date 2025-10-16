using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using Object = UnityEngine.Object;

namespace UnityEssentialsEditor
{
	internal class CreateScriptAction : EndNameEditAction
	{
		public override void Action(int instanceId, string fullPath, string resourceFile)
		{
			fullPath = CheckFileName(fullPath);
			var method = typeof(ProjectWindowUtil).GetMethod("CreateScriptAssetWithContent", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			string content = ModifyTemplateContent(File.ReadAllText(Path.GetFullPath(resourceFile)), fullPath);
			string finalPath = AssetDatabase.GenerateUniqueAssetPath(fullPath + ".cs");
			Object o = (Object)method.Invoke(null, new object[] { finalPath, content });
			ProjectWindowUtil.ShowCreatedAsset(o);
		}

		private string CheckFileName(string fullPath)
		{
			var fileName = Path.GetFileName(fullPath);
			var directory = Path.GetDirectoryName(fullPath);
			if(fileName.Contains(" "))
			{
				Debug.LogWarning("Script name contains spaces, spaces have been removed.");
				fileName = fileName.Replace(" ", "");
			}
			if(!char.IsLetter(fileName[0]) && fileName[0] != '_')
			{
				Debug.LogWarning("Script name starts with illegal character, underscore has been added.");
				fileName = "_" + fileName;
			}
			if(char.IsLower(fileName[0]))
			{
				Debug.LogWarning("Script name starts with lowercase character, changing to uppercase.");
				fileName = char.ToUpper(fileName[0]) + fileName.Substring(1);
			}
			return Path.Combine(directory, fileName);
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

			if(EssentialsProjectSettings.Instance.useDefaultNamespace)
			{
				text = text.Replace("#NAMESPACE#", "namespace " + EssentialsProjectSettings.Instance.GetScriptRootNamespace());
				text = text.Replace("#{#", "{");
				text = text.Replace("#}#", "}");
			}
            else
            {
				//Remove namespace from template
				var lines = text.Split('\n').ToList();
				bool inNamespace = false;
				for(int i = 0; i < lines.Count; i++)
				{
					if(lines[i].StartsWith("#NAMESPACE#"))
					{
						//Namespace definition
						lines.RemoveAt(i);
						i--;
					}
					else if(lines[i].StartsWith("#{#"))
					{
						//Namespace scope start
						inNamespace = true;
						lines.RemoveAt(i);
						i--;
					}
					else if(lines[i].StartsWith("#}#"))
					{
						//Namespace scope end
						inNamespace = false;
						lines.RemoveAt(i);
						i--;
					}
					if(inNamespace)
					{
						//Remove indentation
						if(lines[i].StartsWith("\t"))
						{
							lines[i] = lines[i].Substring(1);
						}
						else if(lines[i].StartsWith("    ")) {
							lines[i] = lines[i].Substring(4);
						}
					}
				}
				text = string.Join("\n", lines);
			}

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
