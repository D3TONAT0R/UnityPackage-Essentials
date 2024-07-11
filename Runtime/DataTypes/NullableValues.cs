using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEssentials
{
	/// <summary>
	/// A nullable value type that can be serialized.
	/// </summary>
	public abstract class NullableValue
	{
		public abstract bool HasValue { get; set; }

		public abstract object ValueObject { get; }

		public abstract object GetSystemNullableObject();

		public static NullableValue CreateInstance(bool hasValue, object value)
		{
			var type = value.GetType();
			if(type == typeof(bool)) return new NullableBool(hasValue, (bool)value);
			else if(type == typeof(byte)) return new NullableByte(hasValue, (byte)value);
			else if(type == typeof(short)) return new NullableShort(hasValue, (short)value);
			else if(type == typeof(int)) return new NullableInt(hasValue, (int)value);
			else if(type == typeof(long)) return new NullableLong(hasValue, (long)value);
			else if(type == typeof(float)) return new NullableFloat(hasValue, (float)value);
			else if(type == typeof(double)) return new NullableDouble(hasValue, (double)value);
			else if(type == typeof(Vector2)) return new NullableVector2(hasValue, (Vector2)value);
			else if(type == typeof(Vector3)) return new NullableVector3(hasValue, (Vector3)value);
			else if(type == typeof(Vector4)) return new NullableVector4(hasValue, (Vector4)value);
			else if(type == typeof(Quaternion)) return new NullableQuaternion(hasValue, (Quaternion)value);
			else if(type == typeof(Rect)) return new NullableRect(hasValue, (Rect)value);
			else if(type == typeof(Vector2Int)) return new NullableVector2Int(hasValue, (Vector2Int)value);
			else if(type == typeof(Vector3Int)) return new NullableVector3Int(hasValue, (Vector3Int)value);
			else if(type == typeof(Color)) return new NullableColor(hasValue, (Color)value);
			else if(type == typeof(Color32)) return new NullableColor32(hasValue, (Color32)value);
			else throw new NotImplementedException($"Unable to create NullableValue for base type {type}");
		}

		public static NullableValue CreateInstance(Type t, object nullableObject)
		{
			bool hasValue = nullableObject != null;
			if(nullableObject == null) nullableObject = Activator.CreateInstance(t);
			return CreateInstance(hasValue, nullableObject);
		}
	}

	/// <summary>
	/// A nullable value type that can be serialized.
	/// </summary>
	public abstract class NullableValue<T> : NullableValue where T : struct
	{
		[FormerlySerializedAs("value")]
		public T backingValue;
		[SerializeField]
		protected bool hasValue;

		public T Value
		{
			get => GetValue();
			set
			{
				backingValue = value;
				hasValue = true;
			}
		}

		public override object ValueObject => GetValue();

		public override bool HasValue
		{
			get => hasValue;
			set => hasValue = value;
		}

		public T? Nullable
		{
			get
			{
				return hasValue ? Value : (T?)null;
			}
			set
			{
				if(value.HasValue)
				{
					backingValue = value.Value;
					hasValue = true;
				}
				else
				{
					hasValue = false;
				}
			}
		}

		public NullableValue(T? startValue, T fallback = default)
		{
			hasValue = startValue.HasValue;
			backingValue = startValue ?? fallback;
		}

		public NullableValue(bool hasStartValue, T startValue)
		{
			hasValue = hasStartValue;
			backingValue = startValue;
		}

		public T GetOrFallback(T fallback)
		{
			if(HasValue)
			{
				return Value;
			}
			else
			{
				return fallback;
			}
		}

		public T GetOrDefault() => GetOrFallback(default);

		public T GetValue(bool ignoreNoValue = false)
		{
			if(hasValue || ignoreNoValue)
			{
				return backingValue;
			}
			else
			{
				throw new NullReferenceException("Nullable does not have a value.");
			}
		}

		public override object GetSystemNullableObject() => Nullable;

		public override bool Equals(object obj)
		{
			if(obj is NullableValue<T> other)
			{
				return hasValue == other.hasValue && backingValue.Equals(other.backingValue);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return unchecked(hasValue.GetHashCode() + Value.GetHashCode());
		}

		public override string ToString()
		{
			return $"({hasValue}): {backingValue}";
		}

		public static implicit operator T?(NullableValue<T> v) => v.Nullable;

		public static implicit operator bool(NullableValue<T> v) => v.hasValue;
	}

	/// <summary>
	/// A nullable boolean value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableBool : NullableValue<bool>
	{
		public NullableBool(bool? startValue, bool fallback = default) : base(startValue, fallback) { }

		public NullableBool(bool hasValue, bool startValue) : base(hasValue, startValue) { }
	}

	/// <summary>
	/// A nullable byte value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableByte : NullableValue<byte>
	{
		public NullableByte(byte? startValue, byte fallback = default) : base(startValue, fallback) { }

		public NullableByte(bool hasValue, byte startValue) : base(hasValue, startValue) { }
	}

	/// <summary>
	/// A nullable short value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableShort : NullableValue<short>
	{
		public NullableShort(short? startValue, short fallback = default) : base(startValue, fallback) { }

		public NullableShort(bool hasValue, short startValue) : base(hasValue, startValue) { }
	}

	/// <summary>
	/// A nullable integer value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableInt : NullableValue<int>
	{
		public NullableInt(int? startValue, int fallback = default) : base(startValue, fallback) { }

		public NullableInt(bool hasValue, int startValue) : base(hasValue, startValue) { }
	}

	/// <summary>
	/// A nullable long value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableLong : NullableValue<long>
	{
		public NullableLong(long? startValue, long fallback = default) : base(startValue, fallback) { }

		public NullableLong(bool hasValue, long startValue) : base(hasValue, startValue) { }
	}

	/// <summary>
	/// A nullable float value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableFloat : NullableValue<float>
	{
		public NullableFloat(float? startValue, float fallback = default) : base(startValue, fallback) { }

		public NullableFloat(bool hasValue, float startValue) : base(hasValue, startValue) { }
	}

	/// <summary>
	/// A nullable double value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableDouble : NullableValue<double>
	{
		public NullableDouble(double? startValue, double fallback = default) : base(startValue, fallback) { }

		public NullableDouble(bool hasValue, double startValue) : base(hasValue, startValue) { }
	}

	/// <summary>
	/// A nullable Vector2 value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableVector2 : NullableValue<Vector2>
	{
		public NullableVector2(Vector2? startValue, Vector2 fallback = default) : base(startValue, fallback) { }

		public NullableVector2(bool hasValue, Vector2 startValue) : base(hasValue, startValue) { }
	}

	/// <summary>
	/// A nullable Vector3 value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableVector3 : NullableValue<Vector3>
	{
		public NullableVector3(Vector3? startValue, Vector3 fallback = default) : base(startValue, fallback) { }

		public NullableVector3(bool hasValue, Vector3 startValue) : base(hasValue, startValue) { }
	}

	/// <summary>
	/// A nullable Vector4 value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableVector4 : NullableValue<Vector4>
	{
		public NullableVector4(Vector4? startValue, Vector4 fallback = default) : base(startValue, fallback) { }

		public NullableVector4(bool hasValue, Vector4 startValue) : base(hasValue, startValue) { }
	}

	/// <summary>
	/// A nullable Quaternion value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableQuaternion : NullableValue<Quaternion>
	{
		public NullableQuaternion(Quaternion? startValue, Quaternion fallback = default) : base(startValue, fallback) { }

		public NullableQuaternion(bool hasValue, Quaternion startValue) : base(hasValue, startValue) { }
	}

	/// <summary>
	/// A nullable Rect value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableRect : NullableValue<Rect>
	{
		public NullableRect(Rect? startValue, Rect fallback = default) : base(startValue, fallback) { }

		public NullableRect(bool hasValue, Rect startValue) : base(hasValue, startValue) { }
	}

	/// <summary>
	/// A nullable Vector2Int value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableVector2Int : NullableValue<Vector2Int>
	{
		public NullableVector2Int(Vector2Int? startValue, Vector2Int fallback = default) : base(startValue, fallback) { }

		public NullableVector2Int(bool hasValue, Vector2Int startValue) : base(hasValue, startValue) { }
	}

	/// <summary>
	/// A nullable Vector3Int value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableVector3Int : NullableValue<Vector3Int>
	{
		public NullableVector3Int(Vector3Int? startValue, Vector3Int fallback = default) : base(startValue, fallback) { }

		public NullableVector3Int(bool hasValue, Vector3Int startValue) : base(hasValue, startValue) { }
	}

	/// <summary>
	/// A nullable Color value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableColor : NullableValue<Color>
	{
		public NullableColor(Color? startValue, Color fallback = default) : base(startValue, fallback) { }

		public NullableColor(bool hasValue, Color startValue) : base(hasValue, startValue) { }
	}

	/// <summary>
	/// A nullable Color32 value that can be serialized.
	/// </summary>
	[Serializable]
	public class NullableColor32 : NullableValue<Color32>
	{
		public NullableColor32(Color32? startValue, Color32 fallback = default) : base(startValue, fallback) { }

		public NullableColor32(bool hasValue, Color32 startValue) : base(hasValue, startValue) { }
	}
}
