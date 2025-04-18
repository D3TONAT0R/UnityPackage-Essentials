using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Defines an integer range using a min and max value.
	/// </summary>
	[System.Serializable]
	public struct IntRange
	{
		public int min;
		public int max;

		public float Center => (min + max) * 0.5f;
		public int Range => max - min;

		public IntRange(int min, int max)
		{
			this.min = min;
			this.max = max;
		}

		public IntRange(Vector2Int v) : this(v.x, v.y)
		{

		}

		public bool Contains(int value, bool exclusive = false)
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
			return UnityEngine.Random.Range(min, max + 1);
		}

		public float ClampValue(int v) => Mathf.Clamp(v, min, max);

		public Vector2 ToVector2() => new Vector2(min, max);

		public FloatRange ToFloatRange() => new FloatRange(min, max);
	}
}
