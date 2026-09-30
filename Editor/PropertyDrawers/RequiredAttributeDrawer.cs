using UnityEssentials;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(RequiredAttribute))]
	public class RequiredAttributeDrawer : PropertyDrawer
	{

		private string errorString = "";

		private void CheckTarget(SerializedProperty prop)
		{
			try
			{
				var attr = fieldInfo.GetCustomAttribute<RequiredAttribute>(true);
				errorString = "";
				
				if(attr.ignoreInPrefabs)
				{
					//TODO: this will ignore nested prefabs and being in prefab mode will ignore all [Required] attributes
					if(PrefabUtility.IsPartOfPrefabAsset(prop.serializedObject.targetObject)
					   || PrefabStageUtility.GetCurrentPrefabStage() != null)
					{
						return;
					}
				}
				
				// Unity objects and IObjectReference<Object>
				if (TryGetUnityObject(prop, out var unityObj))
				{
					if (unityObj != null)
					{
						var targetType = PropertyDrawerUtility.GetPropertyType(prop);
						if (targetType == typeof(GameObject) || typeof(Component).IsAssignableFrom(targetType))
						{
							GameObject go = unityObj as GameObject;
							if (!go)
							{
								if (unityObj is Transform t)
								{
									go = t.gameObject;
								}
								else if (unityObj is Component c)
								{
									go = (unityObj as Component).gameObject;
								}
							}

							errorString = "";
							List<Type> missingComps = new List<Type>();
							foreach (var comp in attr.components)
							{
								if (go.GetComponent(comp) == null) missingComps.Add(comp);
							}
							if (missingComps.Count > 0)
							{
								errorString = "Target object is missing required component(s):";
								foreach (var missing in missingComps)
								{
									errorString += " " + missing.Name;
								}
							}
						}
					}
					else if (attr.errorIfNull)
					{
						errorString = "A value is required";
					}
				}
				// Strings
				else if (prop.propertyType == SerializedPropertyType.String)
				{
					bool isEmpty = attr.tolerateWhitespaceStrings ? string.IsNullOrEmpty(prop.stringValue) : string.IsNullOrWhiteSpace(prop.stringValue);
					if (isEmpty && attr.errorIfNull)
					{
						errorString = "A value is required";
					}
				}
				// Floats
				else if (prop.propertyType == SerializedPropertyType.Float)
				{
					if(!attr.tolerateNegativeValues && prop.floatValue < 0f)
					{
						errorString = "Value must not be negative";
					}
					else if (Mathf.Approximately(prop.floatValue, 0f) && attr.errorIfNull)
					{
						errorString = "A value is required";
					}
				}
				// Integers
				else if (prop.propertyType == SerializedPropertyType.Integer)
				{
					if(!attr.tolerateNegativeValues && prop.intValue < 0)
					{
						errorString = "Value must not be negative";
					}
					if (prop.intValue == 0 && attr.errorIfNull)
					{
						errorString = "A value is required";
					}
				}
			}
			catch(Exception e)
			{
				Debug.LogException(new MessagedException("Failed to check for components", e));
			}
		}

		private bool TryGetUnityObject(SerializedProperty prop, out Object unityObj)
		{
			unityObj = null;
			if (prop.propertyType == SerializedPropertyType.ObjectReference)
			{
				unityObj = prop.objectReferenceValue;
				return true;
			}
			else if (prop.propertyType == SerializedPropertyType.Generic)
			{
				var type = PropertyDrawerUtility.GetPropertyType(prop);
				if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ObjectReference<>))
				{
					var objRef = prop.GetValue<IObjectReference>();
					unityObj = objRef.ValueObject;
					return true;
				}
			}
			return false;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if(!PropertyDrawerUtility.ValidatePropertyTypeForAttribute(position, property, label, 
				   SerializedPropertyType.ObjectReference,
				   SerializedPropertyType.String,
				   SerializedPropertyType.Float,
				   SerializedPropertyType.Integer,
				   SerializedPropertyType.Generic)) return;
			if(!string.IsNullOrEmpty(errorString))
			{
				var hr = position;
				hr.xMin += EditorGUIUtility.labelWidth;
				hr.height -= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				EditorGUI.HelpBox(hr, errorString, MessageType.Error);
			}
			position.yMin = position.yMax - EditorGUIUtility.singleLineHeight;
			PropertyDrawerUtility.DrawPropertyField(position, property, label);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float h = EditorGUIUtility.singleLineHeight;
			if(property.hasMultipleDifferentValues)
			{
				errorString = "";
				return h;
			}
			if(!(property.propertyType is SerializedPropertyType.ObjectReference 
			   or SerializedPropertyType.String
			   or SerializedPropertyType.Float
			   or SerializedPropertyType.Integer
			   or SerializedPropertyType.Generic)) return h;
			CheckTarget(property);
			if(errorString.Length > 0)
			{
				h += +EditorGUIUtility.standardVerticalSpacing + 30;
				//h += EditorStyles.helpBox.CalcHeight(new GUIContent(errorString), EditorGUIUtility.fieldWidth) + EditorGUIUtility.standardVerticalSpacing;
			}
			return h;
		}
	}
}