using UnityEssentials;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	[CustomPropertyDrawer(typeof(SeparatorAttribute))]
	public class SeparatorDrawer : DecoratorDrawer
	{
		public override void OnGUI(Rect position)
		{
			bool fullWidth = ((SeparatorAttribute)attribute).fullWidth;
			if(!fullWidth) position.xMin += EditorGUIUtility.labelWidth;
			EditorGUIExtras.SeparatorLine(position);
		}

		public override float GetHeight()
		{
			return EditorGUIUtility.singleLineHeight;
		}
	}
}
