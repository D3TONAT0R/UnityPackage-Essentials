using D3T;
using System;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(TexturePreviewAttribute))]
	public class TexturePreviewAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			var fieldPos = position;
			var texturePos = position;
			texturePos.xMin += EditorGUIUtility.labelWidth;

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
			if(texture)
			{
				float imageAspect = (float)texture.width / texture.height;
				if(texturePos.GetAspectRatio() > imageAspect)
				{
					texturePos.width = texturePos.height * imageAspect;
				}
				else
				{
					texturePos.height = texturePos.width / imageAspect;
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
			if(texture)
			{
				if(attr.showObjectField)
				{
					h += EditorGUIUtility.singleLineHeight;
					h += EditorGUIUtility.standardVerticalSpacing;
				}
				h += attr.fixedHeight >= 0 ? attr.fixedHeight : Mathf.Min(texture.height, 256);
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
	}
}