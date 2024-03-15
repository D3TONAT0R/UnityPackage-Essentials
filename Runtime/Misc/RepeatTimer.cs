using D3T;
using UnityEngine;

namespace D3T
{
	[System.Serializable]
	public class RepeatTimer
	{
		public enum UpdateMode
		{
			DeltaTime,
			UnscaledDeltaTime,
			FixedDeltaTime
		}

		public bool useRandomInterval = false;
		[ShowIf(nameof(useRandomInterval), false)]
		public float interval = 1f;
		[ShowIf(nameof(useRandomInterval), true)]
		public FloatRange intervalRange = new FloatRange(0.5f, 1f);

		private float time = 0;
		private float lastUpdateTime = 0;
		private float nextTickRandom = -1;

		private UpdateMode autoUpdateMode;
		private MonoBehaviour autoUpdateOwner;

		public event System.Action Tick;

		public float NextUpdateTime => (lastUpdateTime + interval) - time;

		public bool TriggeredThisFrame { get; private set; } = false;

		public bool AutoUpdateActive { get; private set; }

		public static RepeatTimer Create(float interval)
		{
			return new RepeatTimer() { useRandomInterval = false, interval = interval };
		}

		public static RepeatTimer CreateRandom(FloatRange intervalRange)
		{
			return new RepeatTimer() { useRandomInterval = true, intervalRange = intervalRange };
		}

		public void EnableAutoUpdate(MonoBehaviour owner, UpdateMode mode)
		{
			if(!owner) throw new System.NullReferenceException("Owner object must not be null.");
			autoUpdateMode = mode;
			if(mode == UpdateMode.DeltaTime || mode == UpdateMode.UnscaledDeltaTime)
			{
				UpdateLoop.PreUpdate += AutoUpdate;
			}
			else if(mode == UpdateMode.FixedDeltaTime)
			{
				UpdateLoop.FixedUpdate += AutoUpdate;
			}
			else throw new System.InvalidOperationException();
			AutoUpdateActive = true;
		}

		public void DisableAutoUpdate()
		{
			autoUpdateOwner = null;
			UpdateLoop.PreUpdate -= AutoUpdate;
			UpdateLoop.FixedUpdate -= AutoUpdate;
			AutoUpdateActive = false;
		}

		private void AutoUpdate()
		{
			if(autoUpdateOwner == null)
			{
				DisableAutoUpdate();
				return;
			}
			switch(autoUpdateMode)
			{
				case UpdateMode.DeltaTime: Update(Time.deltaTime); break;
				case UpdateMode.UnscaledDeltaTime: Update(Time.unscaledDeltaTime); break;
				case UpdateMode.FixedDeltaTime: Update(Time.fixedDeltaTime); break;
				default: throw new System.InvalidOperationException();
			}
		}

		private bool UpdateInternal(float delta)
		{
			time += delta;

			if(useRandomInterval && nextTickRandom < 0) nextTickRandom = Random.value;

			float nextTickTime;
			if(!useRandomInterval) nextTickTime = lastUpdateTime + interval;
			else nextTickTime = lastUpdateTime + intervalRange.Lerp(nextTickRandom);

			if(time >= nextTickTime)
			{
				lastUpdateTime = time;
				TriggeredThisFrame = true;
				Tick?.Invoke();
				if(useRandomInterval) nextTickRandom = Random.value;
				return true;
			}
			else
			{
				TriggeredThisFrame = false;
				return false;
			}
		}

		public bool Update(float delta)
		{
			if(AutoUpdateActive)
			{
				throw new System.InvalidOperationException("RepeatTimer is set to auto update, no manual update is required.");
			}
			return UpdateInternal(delta);
		}
	}
}
