using UnityEssentials;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ShowIfAttribute), true)]
	public class ShowIfAttributeDrawer : ModificationPropertyDrawer
	{
		private CachedAttribute<SpaceAttribute> spaceAttribute;
		
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
			if(ShouldDraw(property))
			{
				return EditorGUI.GetPropertyHeight(property, label);
			}
			else
			{
				int extraHeight = 0;
                if(spaceAttribute.TryGet(fieldInfo, out SpaceAttribute space))
                {
                    extraHeight += (int)space.height;
				}
				return -(EditorGUIUtility.standardVerticalSpacing + extraHeight);
			}
		}

		private bool ShouldDraw(SerializedProperty property)
		{
			var attr = (ShowIfAttribute)attribute;
			return attr.ShouldDraw(PropertyDrawerUtility.GetParentObject(property));
		}
	}
}