using UnityEssentials;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(NullableValue), true)]
	public class NullableValueDrawer : PropertyDrawer
	{
		private CachedSerializedProperty hasValue;
		private CachedSerializedProperty backingValue;
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var hv = hasValue.Find(property, "hasValue");
			var v = backingValue.Find(property, "backingValue");
			hv = property.FindPropertyRelative("hasValue");
			v = property.FindPropertyRelative("backingValue");
			// v.NextVisible(true);

			EditorGUI.BeginProperty(position, label, property);

			DrawToggle(position, label, hv);

			var guiEnabled = GUI.enabled;
			GUI.enabled &= hv.boolValue;
			EditorGUIUtility.labelWidth += 16;
			// EditorGUI.showMixedValue = v.hasMultipleDifferentValues;
			DrawValueField(position, v, " ");
			EditorGUIUtility.labelWidth -= 16;
			GUI.enabled = guiEnabled;

			EditorGUI.EndProperty();
		}

		private static void DrawToggle(Rect position, GUIContent label, SerializedProperty hv)
		{
			var togglePos = position;
			togglePos.width = EditorGUIUtility.labelWidth + 16;
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = hv.hasMultipleDifferentValues;
			bool b = EditorGUI.Toggle(togglePos, label, hv.boolValue);
			if(EditorGUI.EndChangeCheck())
			{
				hv.boolValue = b;
			}
			EditorGUI.showMixedValue = false;
		}

		protected virtual void DrawValueField(Rect position, SerializedProperty v, string displayName)
		{
			EditorGUI.PropertyField(position, v, new GUIContent(" "));
		}
	}

	[CustomPropertyDrawer(typeof(NullableBool))]
	public class NullableBoolDrawer : NullableValueDrawer
	{
		protected override void DrawValueField(Rect position, SerializedProperty v, string displayName)
		{
			position.xMin += 10;
			EditorGUI.PropertyField(position, v, new GUIContent(displayName));
		}
	}

	[CustomPropertyDrawer(typeof(NullableFloat))]
	public class NullableFloatDrawer : NullableValueDrawer
	{
		private CachedAttribute<NullableRangeAttribute> rangeAttribute;
		
		protected override void DrawValueField(Rect position, SerializedProperty v, string displayName)
		{
			EditorGUI.BeginChangeCheck();
			float value;
			if(rangeAttribute.TryGet(v, out var rangeAttr))
			{
				value = EditorGUI.Slider(position, displayName, v.floatValue, rangeAttr.min, rangeAttr.max);
			}
			else
			{
				value = EditorGUI.FloatField(position, displayName, v.floatValue);
			}
			if(EditorGUI.EndChangeCheck())
			{
				v.floatValue = value;
			}
		}
	}

	[CustomPropertyDrawer(typeof(NullableInt))]
	public class NullableIntDrawer : NullableValueDrawer
	{
		private CachedAttribute<NullableRangeAttribute> rangeAttribute;
		
		protected override void DrawValueField(Rect position, SerializedProperty v, string displayName)
		{
			EditorGUI.BeginChangeCheck();
			int value;
			if(rangeAttribute.TryGet(fieldInfo, out var rangeAttr))
			{
				value = EditorGUI.IntSlider(position, displayName, v.intValue, (int)rangeAttr.min, (int)rangeAttr.max);
			}
			else
			{
				value = EditorGUI.IntField(position, displayName, v.intValue);
			}
			if(EditorGUI.EndChangeCheck())
			{
				v.intValue = value;
			}
		}
	}

	public abstract class NullableVectorDrawer : NullableValueDrawer
	{
		protected abstract string[] SubLabels { get; }

		protected override void DrawValueField(Rect position, SerializedProperty v, string displayName)
		{
			var v1 = v.Copy();
			v1.NextVisible(true);
			EditorGUI.MultiPropertyField(position, SubLabels.Select(l => new GUIContent(l)).ToArray(), v1, new GUIContent(displayName));
		}
	}

	[CustomPropertyDrawer(typeof(NullableVector4))]
	public class NullableVector4Drawer : NullableVectorDrawer
	{
		protected override string[] SubLabels => new string[] { "X", "Y", "Z", "W" };
	}

	[CustomPropertyDrawer(typeof(NullableQuaternion))]
	public class NullableQuaternionDrawer : NullableVectorDrawer
	{
		protected override string[] SubLabels => new string[] { "X", "Y", "Z", "W" };
	}

	[CustomPropertyDrawer(typeof(NullableRect))]
	public class NullableRectDrawer : NullableVectorDrawer
	{
		protected override string[] SubLabels => new string[] { "X", "Y", "W", "H" };
	}

	[CustomPropertyDrawer(typeof(NullableColor))]
	public class NullableColorDrawer : NullableValueDrawer
	{
		private CachedAttribute<NullableColorUsageAttribute> colorUsageAttribute;
		
		protected override void DrawValueField(Rect position, SerializedProperty v, string displayName)
		{
			bool showAlpha = true;
			bool hdr = false;
			if(colorUsageAttribute.TryGet(fieldInfo, out var attr))
			{
				showAlpha = attr.showAlpha;
				hdr = attr.hdr;
			}
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = v.hasMultipleDifferentValues;
			var newColor = EditorGUI.ColorField(position, new GUIContent(displayName), v.colorValue, true, showAlpha, hdr);
			if(!showAlpha) newColor.a = 1;
			if(EditorGUI.EndChangeCheck())
			{
				v.colorValue = newColor;
			}
		}
	}
}