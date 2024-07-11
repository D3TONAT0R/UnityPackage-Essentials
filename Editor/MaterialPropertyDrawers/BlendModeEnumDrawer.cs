using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.MaterialPropertyDrawers
{
	public class BlendModeEnumDrawer : MaterialPropertyDrawer
	{
		private enum BlendMode
		{
			AlphaBlend = 0,
			AlphaPremultiply = 1,
			Additive = 2,
			SoftAdditive = 3,
			Multiply = 4,
			Multiply2x = 5,
			Custom = 99
		}

		private BlendMode GetBlendMode(MaterialProperty prop) => (BlendMode)(int)prop.floatValue;

		public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
		{
			var mode = GetBlendMode(prop);
			if(mode != BlendMode.Custom || editor.targets.Length > 1)
			{
				return EditorGUIUtility.singleLineHeight;
			}
			else
			{
				return 3 * EditorGUIUtility.singleLineHeight + 2 * EditorGUIUtility.standardVerticalSpacing;
			}
		}

		public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
		{
			EditorGUIUtility.labelWidth -= 100;
			if(editor.targets.Length > 1)
			{
				label.text += " (Multi object editing not supported)";
				EditorGUI.LabelField(position, label);
				return;
			}
			var mode = GetBlendMode(prop);
			position.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.BeginChangeCheck();
			mode = (BlendMode)EditorGUI.EnumPopup(position, label, mode);
			prop.floatValue = (int)mode;
			Material material = (Material)editor.serializedObject.targetObject;
			if(EditorGUI.EndChangeCheck() && mode != BlendMode.Custom)
			{
				material.SetInt("_BlendSrc", (int)GetBlendSrcMode(mode));
				material.SetInt("_BlendDst", (int)GetBlendDstMode(mode));
				EditorUtility.SetDirty(material);
			}
			if(mode == BlendMode.Custom)
			{
				position.NextProperty();
				BlendModeField(position, material, "_BlendSrc", "Blend Source");
				position.NextProperty();
				BlendModeField(position, material, "_BlendDst", "Blend Destination");
			}
			EditorGUIUtility.labelWidth += 100;
		}

		private void BlendModeField(Rect position, Material material, string propertyName, string label)
		{
			var mode = (UnityEngine.Rendering.BlendMode)material.GetInt(propertyName);
			EditorGUI.BeginChangeCheck();
			mode = (UnityEngine.Rendering.BlendMode)EditorGUI.EnumPopup(position, label, mode);
			if(EditorGUI.EndChangeCheck())
			{
				material.SetInt(propertyName, (int)mode);
				EditorUtility.SetDirty(material);
			}
		}

		private UnityEngine.Rendering.BlendMode GetBlendSrcMode(BlendMode mode)
		{
			switch(mode)
			{
				case BlendMode.AlphaBlend:
					return UnityEngine.Rendering.BlendMode.SrcAlpha;
				case BlendMode.AlphaPremultiply:
					return UnityEngine.Rendering.BlendMode.One;
				case BlendMode.Additive:
					return UnityEngine.Rendering.BlendMode.One;
				case BlendMode.SoftAdditive:
					return UnityEngine.Rendering.BlendMode.OneMinusDstColor;
				case BlendMode.Multiply:
					return UnityEngine.Rendering.BlendMode.DstColor;
				case BlendMode.Multiply2x:
					return UnityEngine.Rendering.BlendMode.DstColor;
				default:
					throw new InvalidOperationException();
			}
		}

		private UnityEngine.Rendering.BlendMode GetBlendDstMode(BlendMode mode)
		{
			switch(mode)
			{
				case BlendMode.AlphaBlend:
					return UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
				case BlendMode.AlphaPremultiply:
					return UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
				case BlendMode.Additive:
					return UnityEngine.Rendering.BlendMode.One;
				case BlendMode.SoftAdditive:
					return UnityEngine.Rendering.BlendMode.One;
				case BlendMode.Multiply:
					return UnityEngine.Rendering.BlendMode.Zero;
				case BlendMode.Multiply2x:
					return UnityEngine.Rendering.BlendMode.SrcColor;
				default:
					throw new InvalidOperationException();
			}
		}
	}
}
