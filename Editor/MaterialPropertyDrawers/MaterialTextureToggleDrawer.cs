using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.MaterialPropertyDrawers
{
	[CanEditMultipleObjects]
	public class TextureToggleDrawer : MaterialPropertyDrawer
	{
		protected readonly string keyword;

		public TextureToggleDrawer()
		{
		}

		public TextureToggleDrawer(string keyword)
		{
			this.keyword = keyword;
		}

		private static bool IsPropertyTypeSuitable(MaterialProperty prop)
		{
			return prop.type == MaterialProperty.PropType.Texture;
		}

		protected virtual void SetKeyword(MaterialProperty prop)
		{
			UpdateKeyword(prop, "_ON");
		}

		protected void UpdateKeyword(MaterialProperty prop, string defaultKeywordSuffix)
		{
			string text = (!string.IsNullOrEmpty(keyword)) ? keyword : (prop.name.ToUpperInvariant() + defaultKeywordSuffix);
			UnityEngine.Object[] targets = prop.targets;
			for(int i = 0; i < targets.Length; i++)
			{
				Material material = (Material)targets[i];
				var texture = material.GetTexture(prop.name);
				bool on = texture;
				if(on)
				{
					material.EnableKeyword(text);
				}
				else
				{
					material.DisableKeyword(text);
				}
			}
		}

		public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
		{
			if(!IsPropertyTypeSuitable(prop))
			{
				GUIContent label2 = new GUIContent("Toggle used on a non-texture property: " + prop.name);
				EditorGUI.LabelField(position, label2, EditorStyles.helpBox);
			}
			else
			{
				EditorGUI.BeginChangeCheck();
				editor.TextureProperty(position, prop, label.text);
				if(EditorGUI.EndChangeCheck())
				{
					SetKeyword(prop);
				}
			}
		}

		public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
		{
			return 70;
		}

		public override void Apply(MaterialProperty prop)
		{
			base.Apply(prop);
			if(IsPropertyTypeSuitable(prop))
			{
				SetKeyword(prop);
			}
		}
	}
}
