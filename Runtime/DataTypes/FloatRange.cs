using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Defines a range using a min and max value.
	/// </summary>
	[System.Serializable]
	public struct FloatRange
	{
		public float min;
		public float max;

		public float Center => (min + max) * 0.5f;
		public float Range => max - min;

		public FloatRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		public FloatRange(Vector2 v) : this(v.x, v.y)
		{

		}

		public bool Contains(float value, bool exclusive = false)
		{
			if(exclusive)
			{
				return value > min && value < max;
			}
			else
			{
				return value >= min && value <= max;
			}
		}

		public float Random()
		{
			return UnityEngine.Random.Range(min, max);
		}

		public float Lerp(float t) => Mathf.Lerp(min, max, t);

		public float LerpUnclamped(float t) => Mathf.LerpUnclamped(min, max, t);

		public float InverseLerp(float v) => Mathf.InverseLerp(min, max, v);

		public float InverseLerpUnclamped(float v) => (v - min) / (max - min);

		public float ClampValue(float v) => Mathf.Clamp(v, min, max);

		public Vector2 ToVector2() => new Vector2(min, max);
	}
}
