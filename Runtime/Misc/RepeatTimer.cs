using D3T;
using UnityEngine;

namespace D3T
{
	[System.Serializable]
	public class RepeatTimer
	{
		public bool useRandomInterval = false;
		[ShowIf(nameof(useRandomInterval), false)]
		public float interval = 1f;
		[ShowIf(nameof(useRandomInterval), true)]
		public FloatRange intervalRange = new FloatRange(0.5f, 1f);

		private float time = 0;
		private float lastUpdateTime = 0;
		private float nextTickRandom;

		public event System.Action Tick;

		public float NextUpdateTime => (lastUpdateTime + interval) - time;

		public RepeatTimer(float interval)
		{
			this.interval = interval;
			if(useRandomInterval) nextTickRandom = Random.value;
		}

		public bool Update(float delta)
		{
			time += delta;

			float nextTickTime;
			if(!useRandomInterval) nextTickTime = lastUpdateTime + interval;
			else nextTickTime = lastUpdateTime + intervalRange.Lerp(nextTickRandom);

			if(time >= nextTickTime)
			{
				lastUpdateTime = time;
				Tick?.Invoke();
				if(useRandomInterval) nextTickRandom = Random.value;
				return true;
			}
			return false;
		}
	}
}
