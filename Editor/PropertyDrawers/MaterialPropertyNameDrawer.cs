using D3T;
using D3T.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(MaterialPropertyName))]
	public class MaterialPropertyNameDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var attr = GetAttribute(property);
			if(attr != null) position.width -= 20;
			var nameField = property.FindPropertyRelative("propertyName");
			EditorGUI.PropertyField(position, nameField, label);
			if(attr != null)
			{
				position.xMin += position.width;
				position.width = 20;
				if(GUI.Button(position, GUIContent.none, EditorStyles.popup))
				{
					ShowSuggestionsMenu(property.serializedObject, nameField.propertyPath, attr);
				}
			}
		}

		private void ShowSuggestionsMenu(SerializedObject serializedObject, string namePropertyPath, MaterialPropertyHintAttribute attr)
		{
			var menu = new GenericMenu();
			List<Material> targetMaterials = new List<Material>();
			if(!string.IsNullOrEmpty(attr.targetMemberName))
			{
				var refObj = ReflectionUtility.GetMemberValueByName(serializedObject.targetObject, attr.targetMemberName);
				if(refObj != null)
				{
					//Simple fields
					if(refObj is Material material && material != null)
					{
						targetMaterials.Add(material);
					}
					else if(refObj is Renderer renderer && renderer != null)
					{
						targetMaterials.AddRange(renderer.sharedMaterials);
					}

					//Arrays
					else if(refObj is Material[] materials && materials != null)
					{
						targetMaterials.AddRange(materials);
					}
					else if(refObj is Renderer[] renderers && renderers != null)
					{
						targetMaterials.AddRange(renderers.SelectMany(r => r.sharedMaterials));
					}

					//Lists
					else if(refObj is List<Material> materialsList && materialsList != null)
					{
						targetMaterials.AddRange(materialsList);
					}
					else if(refObj is Renderer[] renderersList && renderersList != null)
					{
						targetMaterials.AddRange(renderersList.SelectMany(r => r.sharedMaterials));
					}
				}
			}
			else
			{
				if(serializedObject.targetObject is Component comp && comp.TryGetComponent<Renderer>(out var r))
				{
					targetMaterials.AddRange(r.sharedMaterials);
				}
			}
			if(targetMaterials.Count > 0)
			{
				var field = serializedObject.FindProperty(namePropertyPath);
				List<string> names = new List<string>();
				foreach(var mat in targetMaterials)
				{
					if(mat) names.AddRange(GetPropertyNames(mat, attr.propertyType, attr));
				}
				if(names.Count > 0)
				{
					foreach(var propName in names.Distinct()) 
					{
						if(propName.StartsWith("unity_"))
						{
							continue;
						}
						menu.AddItem(new GUIContent(propName), field.stringValue == propName, () =>
						{
							serializedObject.Update();
							serializedObject.FindProperty(namePropertyPath).stringValue = propName;
							serializedObject.ApplyModifiedProperties();
						});
					}
				}
				else
				{
					menu.AddDisabledItem(new GUIContent("No matches"));
				}
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("No matches"));
			}
			menu.ShowAsContext();
		}

		private IEnumerable<string> GetPropertyNames(Material mat, MaterialPropertyType type, MaterialPropertyHintAttribute attribute)
		{
#if UNITY_2022_2_OR_NEWER
			foreach(var name in mat.GetPropertyNames(attribute.propertyType)) yield return name;
#endif
			if(mat.shader)
			{
				var shader = mat.shader;
				//Ints as shader properties are not supported in older versions of unity
				if(type == MaterialPropertyType.Int) type = MaterialPropertyType.Float;

				for(int i = 0; i < shader.GetPropertyCount(); i++)
				{
					var propType = GetPropertyType(shader.GetPropertyType(i));
					if(propType == type) yield return shader.GetPropertyName(i);
				}
			}
		}

		private MaterialPropertyHintAttribute GetAttribute(SerializedProperty property)
		{
			return PropertyDrawerUtility.GetAttribute<MaterialPropertyHintAttribute>(property, true);
		}

		private MaterialPropertyType GetPropertyType(UnityEngine.Rendering.ShaderPropertyType t)
		{
			switch(t)
			{
				case UnityEngine.Rendering.ShaderPropertyType.Color: 
				case UnityEngine.Rendering.ShaderPropertyType.Vector:
					return MaterialPropertyType.Vector;
				case UnityEngine.Rendering.ShaderPropertyType.Float:
				case UnityEngine.Rendering.ShaderPropertyType.Range:
					return MaterialPropertyType.Float;
				case UnityEngine.Rendering.ShaderPropertyType.Texture:
					return MaterialPropertyType.Texture;
				default: throw new NotImplementedException();
			}
		}
	}
}