﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace D3T
{
	public static class EnumExtensions
	{
		/// <summary>
		/// Compares the given values using this operator.
		/// </summary>
		public static bool Operate(this ComparisonOperator op, int l, int r)
		{
			switch(op)
			{
				case ComparisonOperator.Less: return l < r;
				case ComparisonOperator.LessOrEqual: return l <= r;
				case ComparisonOperator.Equal: return l == r;
				case ComparisonOperator.MoreOrEqual: return l >= r;
				case ComparisonOperator.More: return l > r;
				default: throw new System.InvalidOperationException();
			}
		}

		/// <summary>
		/// Compares the given values using this operator.
		/// </summary>
		public static bool Operate(this ComparisonOperator op, float l, float r)
		{
			switch(op)
			{
				case ComparisonOperator.Less: return l < r;
				case ComparisonOperator.LessOrEqual: return l <= r;
				case ComparisonOperator.Equal: return l == r;
				case ComparisonOperator.MoreOrEqual: return l >= r;
				case ComparisonOperator.More: return l > r;
				default: throw new System.InvalidOperationException();
			}
		}

		/// <summary>
		/// Applies a boolean operation with the given values.
		/// </summary>
		public static bool Operate(this BooleanOperator op, bool l, bool r)
		{
			switch(op)
			{
				case BooleanOperator.AND: return l && r;
				case BooleanOperator.OR: return l || r;
				case BooleanOperator.XOR: return l != r;
				default: throw new System.InvalidOperationException();
			}
		}

		/// <summary>
		/// Returns a direction vector that represents this axis.
		/// </summary>
		public static Vector3 GetDirectionVector(this Axis a)
		{
			switch(a)
			{
				case Axis.X: return Vector3.right;
				case Axis.Y: return Vector3.up;
				case Axis.Z: return Vector3.forward;
				default: throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Returns a direction vector that represents this axis and direction.
		/// </summary>
		public static Vector3 GetDirectionVector(this AxisDirection d)
		{
			switch(d)
			{
				case AxisDirection.XNeg: return Vector3.left;
				case AxisDirection.XPos: return Vector3.right;
				case AxisDirection.YNeg: return Vector3.down;
				case AxisDirection.YPos: return Vector3.up;
				case AxisDirection.ZNeg: return Vector3.back;
				case AxisDirection.ZPos: return Vector3.forward;
				default: throw new InvalidOperationException();
			}
		}
	} 
}
