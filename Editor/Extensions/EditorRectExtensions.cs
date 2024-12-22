using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public static class EditorRectExtensions
	{
		/// <summary>
		/// Moves the rect to the next property position.
		/// </summary>
		public static void NextProperty(this ref Rect r, float propertyHeight = -1, bool verticalSpace = true)
		{
			if(propertyHeight <= 0) propertyHeight = EditorGUIUtility.singleLineHeight;
			if(verticalSpace)
			{
				r.y += r.height + EditorGUIUtility.standardVerticalSpacing;
			}
			else
			{
				r.y += r.height;
			}
			r.height = propertyHeight;
		}
	}
}
