using UnityEditor;
using UnityEngine;
using UnityEssentials.Collections;

namespace UnityEssentialsEditor
{
	public class StackElementDrawer
	{

		public virtual void OnHeaderGUI(Rect position, int elementIndex, SerializedProperty elem, StackElement obj, SerializedProperty stack, SerializedProperty array)
		{
			PolymorphicStackDrawer.DrawItemHeader(position, elementIndex, elem, obj, array);
		}

		public virtual void OnGUI(ref Rect position, SerializedProperty elem, StackElement obj, SerializedProperty stack)
		{
			bool enter = true;
			var last = elem.GetEndProperty();
			while(elem.NextVisible(enter) && !SerializedProperty.EqualContents(elem, last))
			{
				position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
				enter = false;
				var child = elem;
				position.height = EditorGUI.GetPropertyHeight(child, true);
				EditorGUI.PropertyField(position, child, true);
			}
			position.yMax += EditorGUIUtility.standardVerticalSpacing;
		}
	}
}