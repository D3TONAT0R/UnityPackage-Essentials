using UnityEngine;

namespace UnityEssentialsEditor
{
	public static class GUIContentExtensions
	{
		/// <summary>
		/// Sets the text of this GUIContent.
		/// </summary>
		public static GUIContent SetText(this GUIContent content, string text)
		{
			content.text = text;
			return content;
		}

		/// <summary>
		/// Sets the tooltip of this GUIContent.
		/// </summary>
		public static GUIContent SetTooltip(this GUIContent content, string tooltip)
		{
			content.tooltip = tooltip;
			return content;
		}
		
		/// <summary>
		/// Sets the image of this GUIContent.
		/// </summary>
		public static GUIContent SetImage(this GUIContent content, Texture image)
		{
			content.image = image;
			return content;
		}
		
		/// <summary>
		/// Clears all content from this GUIContent.
		/// </summary>
		public static GUIContent Clear(this GUIContent content)
		{
			content.text = string.Empty;
			content.tooltip = string.Empty;
			content.image = null;
			return content;
		}
	}
}