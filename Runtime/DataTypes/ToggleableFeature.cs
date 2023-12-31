using UnityEngine;

namespace D3T
{

	/// <summary>
	/// Base class for subclasses with an 'enabled' checkbox.
	/// </summary>
	public abstract class ToggleableFeature
	{
		//The 'enabled' toggle will be drawn by the property drawer itself
		[HideInInspector, SerializeField]
		protected bool enabled = true;

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
