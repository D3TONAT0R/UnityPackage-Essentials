using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor
{
	[CustomPropertyDrawer(typeof(AffixAttribute), true)]
	public class AffixAttributeDrawer : PropertyDrawer
	{
		private float fieldWidth;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GetAffix<PrefixAttribute>(property, out var prefix, out var prefixWidth);
			GetAffix<SuffixAttribute>(property, out var suffix, out var suffixWidth);
			if(Event.current.type == EventType.Repaint)
			{
				fieldWidth = position.width - EditorGUIUtility.labelWidth;
			}
			if((fieldWidth - prefixWidth - suffixWidth) < 30)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			if(suffix != null)
			{
				//Place suffix label at the end
				position.SplitHorizontalRight(suffixWidth, out position, out var suffixPos, 2);
				GUI.Label(suffixPos, suffix);
			}
			var lw = EditorGUIUtility.labelWidth;
			if(prefix != null)
			{
				//Place prefix label at the start, and extend the label width to accomodate it
				EditorGUIUtility.labelWidth += prefixWidth;
				var prefixPos = position;
				prefixPos.width = prefixWidth;
				prefixPos.x += lw;
				GUI.Label(prefixPos, prefix);
			}
			bool monospace = property.TryGetAttribute<MonospaceAttribute>(out _);
			EditorGUI.BeginProperty(position, label, property);
			PropertyDrawerUtility.DrawPropertyDirect(position, label, property, monospace);
			EditorGUI.EndProperty();
			EditorGUIUtility.labelWidth = lw;
		}

		private void GetAffix<T>(SerializedProperty property, out GUIContent content, out float width) where T : AffixAttribute
		{
			var attr = PropertyDrawerUtility.GetAttribute<T>(property, true);
			if(attr == null)
			{
				content = null;
				width = 0;
				return;
			}
			content = attr.content;
			width = attr.fixedWidth;
			if(width <= 0)
			{
				var requiredWidth = Mathf.Ceil((EditorStyles.label.CalcSize(attr.content).x + 2) / 10f) * 10f; //Round to 10 pixel increments
				width = Mathf.Max(requiredWidth, 20);
			}
		}
	}
}
