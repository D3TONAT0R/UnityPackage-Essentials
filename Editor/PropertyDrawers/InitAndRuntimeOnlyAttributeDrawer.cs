using UnityEssentials;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(InitOnlyAttribute), true)]
	[CustomPropertyDrawer(typeof(RuntimeOnlyAttribute), true)]
	public class InitAndRuntimeOnlyAttributeDrawer : ModificationPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var lastState = GUI.enabled;
			GUI.enabled &= IsEnabled(property);
			DrawProperty(position, property, label);
			GUI.enabled = lastState;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return GetBaseHeight(property, label);
		}

		private bool IsEnabled(SerializedProperty property)
		{
			var attr = (PropertyModifierAttribute)attribute;
			var monoBehaviour = property.serializedObject.targetObject as MonoBehaviour;
			if (monoBehaviour)
			{
				bool initialized = Application.isPlaying && monoBehaviour.gameObject.scene.IsValid();
				if (attr is InitOnlyAttribute) return !initialized;
				else if(attr is RuntimeOnlyAttribute) return initialized;
				else return false;
			}
			else
			{
				// Default to always enabled for non-MonoBehaviour objects (e.g., ScriptableObjects)
				return true;
			}
		}
	}
}
