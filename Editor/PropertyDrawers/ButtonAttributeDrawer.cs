using UnityEssentials;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ButtonAttribute), true)]
	public class ButtonAttributeDrawer : ModificationPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var attr = PropertyDrawerUtility.GetAttribute<ButtonAttribute>(property, true);
			if(!attr.Below)
			{
				position.SplitVertical(EditorGUIUtility.singleLineHeight, out var top, out var bottom, EditorGUIUtility.standardVerticalSpacing);
				DrawButtons(property, attr, top);
				DrawProperty(bottom, property, label);
			}
			else
			{
				position.SplitVerticalBottom(EditorGUIUtility.singleLineHeight, out var top, out var bottom, EditorGUIUtility.standardVerticalSpacing);
				DrawProperty(top, property, label);
				DrawButtons(property, attr, bottom);
			}
		}

		private static void DrawButtons(SerializedProperty property, ButtonAttribute attr, Rect position)
		{
			bool enabled = true;
			switch(attr.EnabledIn)
			{
				case ButtonAttribute.Usage.Never: enabled = false; break;
				case ButtonAttribute.Usage.EditMode: enabled = !Application.isPlaying; break;
				case ButtonAttribute.Usage.PlayMode: enabled = Application.isPlaying; break;
				case ButtonAttribute.Usage.Both: enabled = true; break;
			}

			using(new EnabledScope(enabled))
			{
				var rects = position.DivideHorizontal(attr.buttonMethodNames.Length, 4);
				for(int i = 0; i < rects.Length; i++)
				{
					string name = attr.buttonNames[i] ?? ObjectNames.NicifyVariableName(attr.buttonMethodNames[i]);
					if(GUI.Button(rects[i], name))
					{
						Invoke(property, attr.buttonMethodNames[i]);
					}
				}
			}
		}

		private static void Invoke(SerializedProperty property, string methodName)
		{
			object target;
			var parentProp = PropertyDrawerUtility.GetParentProperty(property);
			if(parentProp != null) target = PropertyDrawerUtility.GetTargetObjectOfProperty(parentProp);
			else target = property.serializedObject.targetObject;

			var method = target.GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
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