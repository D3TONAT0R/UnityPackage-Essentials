using UnityEngine;

namespace UnityEssentials {

	/// <summary>
	/// Base class for subclasses with an 'enabled' checkbox.
	/// </summary>
	public abstract class ToggleableFeature
	{
		//The 'enabled' toggle will be drawn by the property drawer itself
		[HideInInspector, SerializeField]
		protected bool enabled = true;

		/// <summary>
		/// Whether this feature is enabled, represented by a checkbox in the inspector.
		/// </summary>
		public bool Enabled
		{
			get { return enabled; }
			set
			{
				if(value != enabled)
				{
					enabled = value;
					if(Application.isPlaying)
					{
						if(enabled)
						{
							OnEnable();
						}
						else
						{
							OnDisable();
						}
					}
				}
			}
		}

		/// <summary>
		/// An optional custom name for this feature.
		/// </summary>
		public virtual string CustomName => null;

		public ToggleableFeature(bool enabled)
		{
			this.enabled = enabled;
		}

		public ToggleableFeature() : this(true)
		{

		}

		protected virtual void OnEnable()
		{

		}

		protected virtual void OnDisable()
		{

		}
	}
}
