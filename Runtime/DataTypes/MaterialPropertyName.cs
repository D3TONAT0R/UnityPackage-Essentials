using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Definition of a material property name.
	/// </summary>
	[System.Serializable]
	public struct MaterialPropertyName
	{
		[SerializeField]
		private string propertyName;

		private int propertyID;

#if UNITY_EDITOR
		private string lastPropertyName;
#endif

		public int PropertyID
		{
			get
			{
#if UNITY_EDITOR
				//Regenerate property ID in case of a property name change
				if(propertyID > 0 && propertyName != lastPropertyName)
				{
					propertyID = Shader.PropertyToID(propertyName);
					lastPropertyName = propertyName;
				}
#endif
				if(propertyID == 0)
				{
					//Initialize property ID
					propertyID = Shader.PropertyToID(propertyName);
				}
				return propertyID;
			}
		}

		public MaterialPropertyName(string name)
		{
			propertyName = name;
			propertyID = 0;
#if UNITY_EDITOR
			lastPropertyName = null;
#endif
		}

		public static implicit operator MaterialPropertyName(string name) => new MaterialPropertyName(name);

		public static implicit operator int(MaterialPropertyName prop) => prop.PropertyID;

		public override string ToString()
		{
			return $"('{propertyName}' [ID:{PropertyID}])";
		}
	}
}
