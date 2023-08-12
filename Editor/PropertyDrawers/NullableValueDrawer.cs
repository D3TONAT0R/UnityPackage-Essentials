using D3T;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(NullableValue), true)]
	public class NullableValueDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var hv = property.FindPropertyRelative("hasValue");
			var v = property.FindPropertyRelative("backingValue");

			var togglePos = position;
			togglePos.width = EditorGUIUtility.labelWidth + 16;
			EditorGUI.PropertyField(togglePos, hv, label);
			var guiEnabled = GUI.enabled;
			GUI.enabled &= hv.boolValue;
			EditorGUIUtility.labelWidth += 16;
			DrawValueField(position, v, " ");
			EditorGUIUtility.labelWidth -= 16;
			GUI.enabled = guiEnabled;
		}

		protected virtual void DrawValueField(Rect position, SerializedProperty v, string displayName)
		{
			EditorGUI.PropertyField(position, v, new GUIContent(" "));
		}

		protected bool TryGetAttribute<T>(SerializedProperty v, out T attr) where T : System.Attribute
		{
			return PropertyDrawerUtility.TryGetAttribute<T>(PropertyDrawerUtility.GetParentProperty(v), true, out attr);
		}
	}

	[CustomPropertyDrawer(typeof(NullableFloat))]
	public class NullableFloatDrawer : NullableValueDrawer
	{
		protected override void DrawValueField(Rect position, SerializedProperty v, string displayName)
		{
			if(TryGetAttribute<NullableRangeAttribute>(v, out var rangeAttr))
			{
				v.floatValue = EditorGUI.Slider(position, displayName, v.floatValue, rangeAttr.min, rangeAttr.max);
			}
			else
			{
				v.floatValue = EditorGUI.FloatField(position, displayName, v.floatValue);
			}
		}
	}

	[CustomPropertyDrawer(typeof(NullableInt))]
	public class NullableIntDrawer : NullableValueDrawer
	{
		protected override void DrawValueField(Rect position, SerializedProperty v, string displayName)
		{
			if(TryGetAttribute<NullableRangeAttribute>(v, out var rangeAttr))
			{
				v.intValue = EditorGUI.IntSlider(position, displayName, v.intValue, (int)rangeAttr.min, (int)rangeAttr.max);
			}
			else
			{
				v.intValue = EditorGUI.IntField(position, displayName, v.intValue);
			}
		}
	}

	[CustomPropertyDrawer(typeof(NullableColor))]
	public class NullableColorDrawer : NullableValueDrawer
	{
		protected override void DrawValueField(Rect position, SerializedProperty v, string displayName)
		{
			bool showAlpha = true;
			bool hdr = false;
			if(TryGetAttribute<NullableColorUsageAttribute>(v, out var attr))
			{
				showAlpha = attr.showAlpha;
				hdr = attr.hdr;
			}
			v.colorValue = EditorGUI.ColorField(position, new GUIContent(displayName), v.colorValue, true, showAlpha, hdr);
		}
	}
}