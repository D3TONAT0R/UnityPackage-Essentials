using System;
using UnityEditor;
using UnityEngine;

namespace D3TEditor
{
	public class CustomIndentLevelScope : IDisposable
	{
		private readonly int lastIndentLevel;

		public CustomIndentLevelScope(int newIndentLevel)
		{
			lastIndentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = newIndentLevel;
		}

		public void Dispose()
		{
			EditorGUI.indentLevel = lastIndentLevel;
		}
	}

	public class ZeroIndentLevelScope : CustomIndentLevelScope
	{
		public ZeroIndentLevelScope() : base(0)
		{

		}
	}

	public class LabelWidthScope : IDisposable
	{
		private readonly float lastLabelWidth;
		private readonly int? lastIndentLevel;

		public LabelWidthScope(float newLabelWidth, bool removeIndentLevel)
		{
			if(removeIndentLevel)
			{
				lastIndentLevel = EditorGUI.indentLevel;
				EditorGUI.indentLevel = 0;
			}
			else
			{
				lastIndentLevel = null;
			}
			lastLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = newLabelWidth;
		}

		public void Dispose()
		{
			if(lastIndentLevel != null) EditorGUI.indentLevel = lastIndentLevel.Value;
			EditorGUIUtility.labelWidth = lastLabelWidth;
		}
	}

	public class EnabledScope : IDisposable
	{
		private readonly bool lastState;

		public EnabledScope(bool enabled)
		{
			lastState = GUI.enabled;
			GUI.enabled = enabled;
		}

		public void Dispose()
		{
			GUI.enabled = lastState;
		}
	}

	public class ColorScope : IDisposable
	{
		private readonly Color lastColor;

		public ColorScope(Color color, bool multiply = false)
		{
			lastColor = GUI.color;
			if(multiply)
			{
				GUI.color *= color;
			}
			else
			{
				GUI.color = color;
			}
		}

		public void Dispose()
		{
			GUI.color = lastColor;
		}
	}
}
