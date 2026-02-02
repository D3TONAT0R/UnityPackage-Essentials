using UnityEditor;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor
{
	[CustomPropertyDrawer(typeof(AffixAttribute), true)]
	public class AffixAttributeDrawer : PropertyDrawer
	{
		private CachedAttribute<PrefixAttribute> prefix;
		private CachedAttribute<SuffixAttribute> suffix;
		private CachedAttribute<MonospaceAttribute> monospaceAttribute;
		
		private float fieldWidth;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GetAffix(ref prefix, out var prefixAttr, out var prefixWidth);
			GetAffix(ref suffix, out var suffixAttr, out var suffixWidth);
			if(Event.current.type == EventType.Repaint)
			{
				fieldWidth = position.width - EditorGUIUtility.labelWidth;
			}
			if((fieldWidth - prefixWidth - suffixWidth) < 30)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			if(suffixAttr != null)
			{
				//Place suffix label at the end
				position.SplitHorizontalRight(suffixWidth, out position, out var suffixPos, 2);
				GUI.Label(suffixPos, suffixAttr);
			}
			var lw = EditorGUIUtility.labelWidth;
			if(prefixAttr != null)
			{
				//Place prefix label at the start, and extend the label width to accomodate it
				EditorGUIUtility.labelWidth += prefixWidth;
				var prefixPos = position;
				prefixPos.width = prefixWidth;
				prefixPos.x += lw;
				GUI.Label(prefixPos, prefixAttr);
			}
			bool monospace = monospaceAttribute.TryGet(fieldInfo, out _);
			EditorGUI.BeginProperty(position, label, property);
			DrawProperty(position, property, label, monospace);
			EditorGUI.EndProperty();
			EditorGUIUtility.labelWidth = lw;
		}

		private void DrawProperty(Rect position, SerializedProperty property, GUIContent label, bool monospace)
		{
			EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
			//TODO: Handle monospace font
			PropertyDrawerUtility.DrawPropertyWithAttributeExcept(position, property, label, typeof(AffixAttribute), 0, true);
			EditorGUI.showMixedValue = false;
		}

		private void GetAffix<T>(ref CachedAttribute<T> cache, out GUIContent content, out float width) where T : AffixAttribute
		{
			var attr = cache.Get(fieldInfo);
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
