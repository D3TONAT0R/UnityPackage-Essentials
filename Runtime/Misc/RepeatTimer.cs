using UnityEssentials.PlayerLoop;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Helper class for setting up a repeating timer.
	/// </summary>
	[System.Serializable]
	public class RepeatTimer
	{
		public enum UpdateMode : byte
		{
			DeltaTime = 0,
			UnscaledDeltaTime = 1,
			FixedDeltaTime = 2
		}

		[Tooltip("Whether to use a random interval.")]
		public bool useRandomInterval = false;
		[ShowIf(nameof(useRandomInterval), false)]
		[Tooltip("The fixed interval on which this timer will trigger.")]
		public float interval = 1f;
		[ShowIf(nameof(useRandomInterval), true)]
		[Tooltip("The random interval range on which this timer will trigger.")]
		public FloatRange intervalRange = new FloatRange(0.5f, 1f);

		private float time = 0;
		private float lastUpdateTime = 0;
		private float nextTickRandom = -1;

		private UpdateMode autoUpdateMode;
		private MonoBehaviour autoUpdateOwner;

		/// <summary>
		/// An optional name for debugging purposes.
		/// </summary>
		public static string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Called when the timer triggers a tick.
		/// </summary>
		public event System.Action Tick;

		/// <summary>
		/// Whether the timer has triggered a tick this frame.
		/// </summary>
		public bool TriggeredThisFrame { get; private set; } = false;

		/// <summary>
		/// Whether the timer is currently set to auto update.
		/// </summary>
		public bool AutoUpdateActive { get; private set; }

		/// <summary>
		/// The number of ticks that have been triggered on this timer.
		/// </summary>
		public int TickNumber { get; private set; } = 0;

		/// <summary>
		/// The last delta time passed to the timer.
		/// </summary>
		public float DeltaTime { get; private set; } = 0;

		/// <summary>
		/// The current time interval this timer runs at. If random interval is used, this value is randomized every tick.
		/// </summary>
		public float Interval
		{
			get
			{
				if(!useRandomInterval) return lastUpdateTime + interval;
				else return lastUpdateTime + intervalRange.Lerp(nextTickRandom);
			}
		}

		/// <summary>
		/// The time elapsed since the last tick.
		/// </summary>
		public float LastUpdateDelta => time;

		/// <summary>
		/// The time until the next tick is triggered.
		/// </summary>
		public float NextUpdateDelta => Interval - time;

		private RepeatTimer(float interval)
		{
			this.interval = interval;
		}

		/// <summary>
		/// Creates a new repeating timer with the given interval.
		/// </summary>
		public static RepeatTimer Create(float interval)
		{
			return new RepeatTimer(interval);
		}

		/// <summary>
		/// Creates a new repeating timer with the given frame rate
		/// </summary>
		public static RepeatTimer CreateFrameRate(float frameRate)
		{
			return new RepeatTimer(1f / frameRate);
		}

		/// <summary>
		/// Creates a new repeating timer with a random interval.
		/// </summary>
		public static RepeatTimer CreateRandom(FloatRange intervalRange)
		{
			return new RepeatTimer(intervalRange.min) { useRandomInterval = true, intervalRange = intervalRange };
		}

		/// <summary>
		/// Enabled automatic updating of the timer.
		/// </summary>
		public void EnableAutoUpdate(MonoBehaviour owner, UpdateMode mode, bool early = true)
		{
			if(!owner) throw new System.NullReferenceException("Owner object must not be null.");
			autoUpdateMode = mode;
			if(mode == UpdateMode.DeltaTime || mode == UpdateMode.UnscaledDeltaTime)
			{
				if(early) UpdateLoop.PreUpdate += AutoUpdate;
				else UpdateLoop.Update += AutoUpdate;
			}
			else if(mode == UpdateMode.FixedDeltaTime)
			{
				UpdateLoop.FixedUpdate += AutoUpdate;
			}
			else throw new System.InvalidOperationException();
			AutoUpdateActive = true;
		}

		/// <summary>
		/// Disables automatic updating of the timer.
		/// </summary>
		public void DisableAutoUpdate()
		{
			autoUpdateOwner = null;
			UpdateLoop.PreUpdate -= AutoUpdate;
			UpdateLoop.Update -= AutoUpdate;
			UpdateLoop.FixedUpdate -= AutoUpdate;
			AutoUpdateActive = false;
		}

		/// <summary>
		/// Restarts the timer.
		/// </summary>
		public void Restart()
		{
			lastUpdateTime = 0;
			time = 0;
		}

		/// <summary>
		/// Updates the timer manually. Returns true if a tick has been triggered.
		/// </summary>
		public bool Update(float delta)
		{
			if(AutoUpdateActive)
			{
				throw new System.InvalidOperationException("RepeatTimer is set to auto update, no manual update is required.");
			}
			return UpdateInternal(delta);
		}

		/// <summary>
		/// Forces this timer to trigger immediately.
		/// </summary>
		public void ForceTick()
		{
			PerformTick();
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

			if(time >= Interval)
			{
				PerformTick();
				return true;
			}
			else
			{
				TriggeredThisFrame = false;
				return false;
			}
		}

		private void PerformTick()
		{
			TickNumber++;
			DeltaTime = time - lastUpdateTime;
			lastUpdateTime = time;
			TriggeredThisFrame = true;
			Tick?.Invoke();
			if(useRandomInterval) nextTickRandom = Random.value;
		}

		public override string ToString()
		{
			string s = "";
			if(Name != null) s += Name;
			if(useRandomInterval) s += $"({intervalRange.min}-{intervalRange.max})";
			else s += $"({interval})";
			return s;
		}
	}
}
