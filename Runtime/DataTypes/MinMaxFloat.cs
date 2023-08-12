﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace D3T
{
	[System.Serializable]
	public struct MinMaxFloat
	{
		public float min;
		public float max;

		public float Center => (min + max) * 0.5f;
		public float Range => max - min;

		public MinMaxFloat(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		public MinMaxFloat(Vector2 v) : this(v.x, v.y)
		{

		}

		public bool IsBetween(float value, bool exclusive = false)
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

		public float InverseLerp(float v) => Mathf.InverseLerp(min, max, v);

		public float Map(float v) => InverseLerp(v);

		public Vector2 ToVector2() => new Vector2(min, max);
	} 
}
