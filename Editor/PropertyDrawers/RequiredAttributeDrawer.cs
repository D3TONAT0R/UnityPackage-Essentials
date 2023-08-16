using D3T;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(RequiredAttribute))]
	public class RequiredAttributeDrawer : PropertyDrawer
	{

		private string errorString = "";

		private void CheckTarget(SerializedProperty prop)
		{
			try
			{
				//var attr = PropertyDrawerUtility.GetAttribute<CheckForComponentsAttribute>(prop, true);
				var attr = fieldInfo.GetCustomAttribute<RequiredAttribute>(true);
				if(attr == null) throw new ArgumentException("Failed to find required attribute (CheckForComponentAttribute)");

				errorString = "";
				var obj = prop.objectReferenceValue;
				if(obj != null)
				{
					GameObject go = obj as GameObject;
					if(!go)
					{
						if(obj is Transform t)
						{
							go = t.gameObject;
						}
						else
						{
							go = (obj as Component).gameObject;
						}
					}

					errorString = "";
					List<Type> missingComps = new List<Type>();
					foreach(var comp in attr.components)
					{
						if(go.GetComponent(comp) == null) missingComps.Add(comp);
					}
					if(missingComps.Count > 0)
					{
						errorString = "Target object is missing required component(s):";
						foreach(var missing in missingComps)
						{
							errorString += " " + missing.Name;
						}
					}
				}
				else if(attr.errorIfNull)
				{
					errorString = "A value is required";
				}
			}
			catch(Exception e)
			{
				Debug.LogException(new MessagedException("Failed to check for components", e));
			}
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (errorString.Length > 0)
			{
				var hr = position;
				hr.xMin += EditorGUIUtility.labelWidth;
				hr.height -= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				EditorGUI.HelpBox(hr, errorString, MessageType.Error);
			}
			position.yMin = position.yMax - EditorGUIUtility.singleLineHeight;
			PropertyDrawerUtility.DrawPropertyField(position, property, label);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float h = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			CheckTarget(property);
			if (errorString.Length > 0)
			{
				h += 30;
				//h += EditorStyles.helpBox.CalcHeight(new GUIContent(errorString), EditorGUIUtility.fieldWidth) + EditorGUIUtility.standardVerticalSpacing;
			}
			return h;
		}
	}
}