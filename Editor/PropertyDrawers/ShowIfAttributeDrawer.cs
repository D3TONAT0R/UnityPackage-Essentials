using D3T;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ShowIfAttribute))]
	public class ShowIfAttributeDrawer : ModificationPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			bool shouldDraw = ShouldDraw(property);
			if(shouldDraw)
			{
				DrawProperty(position, property, label);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return ShouldDraw(property) ? EditorGUI.GetPropertyHeight(property, true) : 0;
		}

		private bool ShouldDraw(SerializedProperty property)
		{
			var attr = PropertyDrawerUtility.GetAttribute<ShowIfAttribute>(property, true);
			return attr.ShouldDraw(PropertyDrawerUtility.GetParent(property));
		}
	}

}