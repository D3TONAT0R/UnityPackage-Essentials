using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.MaterialPropertyDrawers
{
	public class InfoBoxDecorator : MaterialPropertyDrawer
	{
		private string text;

		private float lastWidth = 200;

		public InfoBoxDecorator(string text)
		{
			this.text = text;
		}

		public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
		{
			return EditorStyles.helpBox.CalcHeight(new GUIContent(text), lastWidth);
		}

		public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
		{
			EditorGUI.HelpBox(position, text, MessageType.None);
			lastWidth = position.width;
		}
	}
}
