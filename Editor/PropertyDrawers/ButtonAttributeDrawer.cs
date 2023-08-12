using D3T;
using D3T.Utility;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ButtonAttribute))]
	public class ButtonAttributeDrawer : ModificationPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var attr = PropertyDrawerUtility.GetAttribute<ButtonAttribute>(property, true);
			if(!attr.below)
			{
				position.SplitVertical(EditorGUIUtility.singleLineHeight, out var top, out var bottom, EditorGUIUtility.standardVerticalSpacing);
				if(GUI.Button(top, attr.buttonText))
				{
					Invoke(property, attr);
				}
				DrawProperty(bottom, property, label);
			}
			else
			{
				position.SplitVerticalBottom(EditorGUIUtility.singleLineHeight, out var top, out var bottom, EditorGUIUtility.standardVerticalSpacing);
				DrawProperty(top, property, label);
				if(GUI.Button(bottom, attr.buttonText))
				{
					Invoke(property, attr);
				}
			}
		}

		private static void Invoke(SerializedProperty property, ButtonAttribute attr)
		{
			object target;
			var parentProp = PropertyDrawerUtility.GetParentProperty(property);
			if(parentProp != null) target = PropertyDrawerUtility.GetTargetObjectOfProperty(parentProp);
			else target = property.serializedObject.targetObject;

			var method = target.GetType().GetMethod(attr.methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			method.Invoke(target, new object[0]);

			var onValidate = ReflectionUtility.FindMemberInType(target.GetType(), "OnValidate") as MethodInfo;
			onValidate?.Invoke(target, new object[0]);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + GetBaseHeight(property, label);
		}
	}
}