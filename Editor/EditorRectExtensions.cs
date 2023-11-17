using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public static class EditorRectExtensions
	{
		public static void NextProperty(this ref Rect r, float propertyHeight = -1, bool verticalSpace = true)
		{
			if (propertyHeight <= 0) propertyHeight = EditorGUIUtility.singleLineHeight;
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
