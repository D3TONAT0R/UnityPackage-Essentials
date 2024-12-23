using UnityEditor;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(TexturePreviewAttribute))]
	public class TexturePreviewAttributeDrawer : PropertyDrawer
	{
		const int BOX_PADDING = 4;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			var fieldPos = position;
			var texturePos = position;
			texturePos.xMin += EditorGUIUtility.labelWidth + 2;

			fieldPos.height = EditorGUIUtility.singleLineHeight;
			var attr = GetAttribute();
			if(attr.showObjectField)
			{
				EditorGUI.PropertyField(fieldPos, property, label);
				texturePos.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			}
			else
			{
				EditorGUI.LabelField(fieldPos, label);
			}

			var texture = GetTextureFromProperty(property);
			if(texture && !property.hasMultipleDifferentValues)
			{
				GUI.Box(texturePos, new GUIContent("", GetTextureInfo(texture)), EditorStyles.helpBox);
				texturePos = texturePos.Inset(BOX_PADDING);
				float imageAspect = (float)texture.width / texture.height;
				if(texturePos.GetAspectRatio() > imageAspect)
				{
					float newWidth = texturePos.height * imageAspect;
					float diff = texturePos.width - newWidth;
					texturePos.width = newWidth;
					texturePos.x += diff / 2;
				}
				else
				{
					float newHeight = texturePos.width / imageAspect;
					float diff = texturePos.height - newHeight;
					texturePos.height = newHeight;
					texturePos.y += diff / 2;
				}
				byte r = (byte)((attr.backgroundRGBA >> 24) & 0xFF);
				byte g = (byte)((attr.backgroundRGBA >> 16) & 0xFF);
				byte b = (byte)((attr.backgroundRGBA >> 8) & 0xFF);
				byte a = (byte)(attr.backgroundRGBA & 0xFF);
				EditorGUI.DrawRect(texturePos, new Color32(r, g, b, a));
				GUI.DrawTexture(texturePos, texture);
			}
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var attr = GetAttribute();
			var texture = GetTextureFromProperty(property);
			float h = 0;
			if(texture && !property.hasMultipleDifferentValues)
			{
				if(attr.showObjectField)
				{
					h += EditorGUIUtility.singleLineHeight;
					h += EditorGUIUtility.standardVerticalSpacing;
				}
				h += attr.fixedHeight >= 0 ? attr.fixedHeight : Mathf.Min(texture.height, 256);
				h += 2 * BOX_PADDING;
			}
			else
			{
				h = base.GetPropertyHeight(property, label);
			}
			return h;
		}

		private TexturePreviewAttribute GetAttribute()
		{
			return attribute as TexturePreviewAttribute;
		}

		private Texture2D GetTextureFromProperty(SerializedProperty property)
		{
			return property.objectReferenceValue as Texture2D;
		}

		private string GetTextureInfo(Texture2D texture)
		{
			if(texture == null)
			{
				return null;
			}
			return $"\"{texture.name}\"\n{texture.width}x{texture.height}\n{texture.format}\n{texture.filterMode}";
		}
	}
}