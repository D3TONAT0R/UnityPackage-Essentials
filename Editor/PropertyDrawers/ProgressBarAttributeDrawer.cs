using UnityEngine;
using UnityEditor;
using UnityEssentials;
using UnityEngine.UIElements;

namespace UnityEssentialsEditor
{
	[CustomPropertyDrawer(typeof(ProgressBarAttribute))]
	public class ProgressBarAttributeDrawer : PropertyDrawer
	{
		private static GUIStyle progressBarBack = "ProgressBarBack";
		private static GUIStyle progressBarFill = "ProgressBarBar";
		private static GUIStyle progressBarText = "ProgressBarText";

		private static GUIStyle manualEditButton = "PaneOptions";
		private static GUIStyle manualEditCloseButton = "WinBtnClose";

		private bool manualEdit = false;

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			manualEditButton = new GUIStyle("PaneOptions");
			return base.CreatePropertyGUI(property);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if(!PropertyDrawerUtility.ValidatePropertyTypeForAttribute(position, property, label, SerializedPropertyType.Float, SerializedPropertyType.Integer)) return;
			var attr = PropertyDrawerUtility.GetAttribute<ProgressBarAttribute>(property, true);

			position.SplitHorizontalRight(position.height, out position, out var btnPosition, 2);

			if(!manualEdit) DrawBarGUI(position, property, label, attr);
			else DrawManualEditGUI(position, property, label, attr);

			btnPosition.height = 16;
			btnPosition.y++;
			manualEdit = GUI.Toggle(btnPosition, manualEdit, GUIContent.none, manualEdit ? manualEditCloseButton : manualEditButton);
		}

		private void DrawBarGUI(Rect position, SerializedProperty property, GUIContent label, ProgressBarAttribute attr)
		{
			float value;
			if(property.propertyType == SerializedPropertyType.Float) value = property.floatValue;
			else value = property.intValue;

			EditorGUI.LabelField(position, label);
			position.xMin += EditorGUIUtility.labelWidth + 3;

			EditorGUIUtility.AddCursorRect(position, MouseCursor.SlideArrow);
			EditorGUI.BeginChangeCheck();
			value = GUI.HorizontalSlider(position, value, attr.min, attr.max, GUIStyle.none, GUIStyle.none);
			if(EditorGUI.EndChangeCheck())
			{
				var mod = Event.current.modifiers;
				int roundingSteps = 100;
				if(mod.HasFlag(EventModifiers.Control) || mod.HasFlag(EventModifiers.Command)) roundingSteps = 20;
				else if(mod.HasFlag(EventModifiers.Shift)) roundingSteps = 0;
				if(roundingSteps > 0)
				{
					float step = Mathf.Abs(attr.max - attr.min) / roundingSteps;
					value = attr.min + (value - attr.min).RoundTo(step);
				}
				value = Mathf.Clamp(value, attr.min, attr.max);

				if(property.propertyType == SerializedPropertyType.Float) property.floatValue = value;
				else property.intValue = (int)value;
			}

			DrawProgressBar(position, attr, value);
		}

		private void DrawManualEditGUI(Rect position, SerializedProperty property, GUIContent label, ProgressBarAttribute attr)
		{
			position.SplitHorizontalRelative(0.85f, out position, out var barRect);
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, property, label);
			if(EditorGUI.EndChangeCheck())
			{
				if(property.propertyType == SerializedPropertyType.Float) property.floatValue = Mathf.Clamp(property.floatValue, attr.min, attr.max);
				else property.intValue = (int)Mathf.Clamp(property.intValue, attr.min, attr.max);
			}
			DrawProgressBar(barRect, attr, property.propertyType == SerializedPropertyType.Float ? property.floatValue : property.intValue);
		}

		private void DrawProgressBar(Rect position, ProgressBarAttribute attr, float value)
		{
			var fillRect = position;
			fillRect.width *= Mathf.Clamp01(Mathf.InverseLerp(attr.min, attr.max, value));

			progressBarBack.DrawOnRepaint(position);
			if(fillRect.width > 0) progressBarFill.DrawOnRepaint(fillRect);

			string valueString = (value * attr.valueScale).ToString((attr.showAsPercentage ? "P" : "F") + attr.decimals);
			GUI.Label(position, valueString, progressBarText);
		}
	}
}
