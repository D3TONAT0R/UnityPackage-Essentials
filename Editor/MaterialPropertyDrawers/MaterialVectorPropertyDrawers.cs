using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using D3T;

namespace D3TEditor.MaterialPropertyDrawers
{
    public class ShowAsVector2Drawer : MaterialPropertyDrawer
    {
		public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
		{
			EditorGUI.showMixedValue = prop.hasMixedValue;
			Vector2 vec2 = prop.vectorValue;
			EditorGUIUtility.labelWidth = position.width * 0.4f;
			EditorGUI.BeginChangeCheck();
			vec2 = EditorGUI.Vector2Field(position, label, vec2);
			EditorGUI.showMixedValue = false;
			if(EditorGUI.EndChangeCheck())
			{
				prop.vectorValue = vec2;
			}
		}
	}

    public class ShowAsVector3Drawer : MaterialPropertyDrawer
    {
		public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
		{
			EditorGUI.showMixedValue = prop.hasMixedValue;
			Vector3 vec3 = prop.vectorValue;
			EditorGUIUtility.labelWidth = position.width * 0.4f;
			EditorGUI.BeginChangeCheck();
			vec3 = EditorGUI.Vector3Field(position, label, vec3);
			EditorGUI.showMixedValue = false;
			if(EditorGUI.EndChangeCheck())
			{
				prop.vectorValue = vec3;
			}
		}
	}

	public class NamedVectorDrawer : MaterialPropertyDrawer
	{
		string[] names;

		public NamedVectorDrawer(string names)
		{
			this.names = names.Split('_');
		}

		public NamedVectorDrawer(string x, string y)
		{
			names = new string[] { x, y };
		}

		public NamedVectorDrawer(string x, string y, string z)
		{
			names = new string[] { x, y, z };
		}

		public NamedVectorDrawer(string x, string y, string z, string w)
		{
			names = new string[] { x, y, z, w };
		}

		public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
		{
			EditorGUI.showMixedValue = prop.hasMixedValue;
			var vec = prop.vectorValue;
			position.SplitHorizontalRelative(0.4f, out var labelRect, out position);
			EditorGUI.LabelField(labelRect, label);
			EditorGUI.BeginChangeCheck();
			if(names != null && names.Length > 0)
			{
				Rect[] comps;
				if(names.Length > 2)
				{
					position.SplitVertical(EditorGUIUtility.singleLineHeight, out var top, out var bottom, EditorGUIUtility.standardVerticalSpacing);
					top.SplitHorizontalRelative(0.5f, out var ul, out var ur, 4);
					bottom.SplitHorizontalRelative(0.5f, out var ll, out var lr, 4);
					comps = new Rect[] { ul, ur, ll, lr };
				}
				else
				{
					position.SplitHorizontalRelative(0.5f, out var l, out var r, 4);
					comps = new Rect[] { l, r };
				}
				EditorGUIUtility.labelWidth = comps[0].width * 0.3f;
				for(int i = 0; i < names.Length; i++)
				{
					vec[i] = EditorGUI.FloatField(comps[i], names[i], vec[i]);
				}
			}
			else
			{
				vec = EditorGUI.Vector4Field(position, label, vec);
			}
			EditorGUI.showMixedValue = false;
			if(EditorGUI.EndChangeCheck())
			{
				prop.vectorValue = vec;
			}
		}

		public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
		{
			if(names == null || names.Length <= 2)
			{
				return base.GetPropertyHeight(prop, label, editor);
			}
			else
			{
				return 2 * base.GetPropertyHeight(prop, label, editor) + EditorGUIUtility.standardVerticalSpacing;
			}
		}
	}
}
