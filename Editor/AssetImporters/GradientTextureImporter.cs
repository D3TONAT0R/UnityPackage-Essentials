using UnityEssentials;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif
using UnityEngine;

namespace UnityEssentialsEditor
{
	internal class GradientTextureFormat : ProceduralTexureFormat
	{
		public enum InputType
		{
			Gradient,
			Curve
		}

		public enum GradientMode
		{
			Horizontal,
			HorizontalReflected,
			Vertical,
			VerticalReflected,
			Radial,
			Square,
		}

		public InputType inputType = InputType.Gradient;

		[ShowIf(nameof(inputType), InputType.Gradient)]
		public Gradient gradient = new Gradient() { colorKeys = new GradientColorKey[] { new GradientColorKey(Color.black, 0), new GradientColorKey(Color.white, 1) } };

		[ShowIf(nameof(inputType), InputType.Curve)]
		public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
		[ShowIf(nameof(inputType), InputType.Curve)]
		public Color lowColor = Color.black;
		[ShowIf(nameof(inputType), InputType.Curve)]
		public Color highColor = Color.white;

		[Space(10)]
		public GradientMode mode;
		[ShowIf("mode", GradientMode.Radial)]
		[Range(0, 1)]
		public float expandToCorners = 0f;
		public bool reversed = false;
		public bool dithering = false;

		public override Color GetPixelColor(int x, int y, Vector2 uv)
		{
			float pos = GetSamplePos(uv, mode);
			if(reversed) pos = 1f - pos;
			return SampleAt(pos);
		}

		Color32 SampleAt(float pos)
		{
			Color sample;
			if(inputType == InputType.Gradient)
			{
				sample = gradient.Evaluate(pos);
			}
			else
			{
				sample = Color.Lerp(lowColor, highColor, Mathf.Clamp01(curve.Evaluate(pos)));
			}
			//if(HDR) sample *= 8f;

			if(dithering)
			{
				Color32 dithered = Color.clear;
				float dither = Random.value;
				for(int i = 0; i < 4; i++)
				{
					float f = sample[i] * 255f;
					byte v = (byte)f;
					if((f - v) > dither) v++;
					dithered[i] = v;
				}
				return dithered;
			}
			else
			{
				return sample;
			}
		}

		float GetSamplePos(Vector2 uv, GradientMode m)
		{
			switch(m)
			{
				case GradientMode.Horizontal: return uv.x;
				case GradientMode.HorizontalReflected:
					uv *= 2f;
					return uv.x > 1f ? 2f - uv.x : uv.x;
				case GradientMode.Vertical: return uv.y;
				case GradientMode.VerticalReflected:
					uv *= 2f;
					return uv.y > 1f ? 2f - uv.y : uv.y;
				case GradientMode.Radial:
					uv = uv * 2f - Vector2.one;
					return Mathf.Clamp01(uv.magnitude / Mathf.Lerp(1f, 1.414f, expandToCorners));
				case GradientMode.Square:
					uv = uv * 2f - Vector2.one;
					return Mathf.Max(Mathf.Abs(uv.x), Mathf.Abs(uv.y));
				default: return 0;
			}
		}
	}

	[ScriptedImporter(0, "gradient")]
	internal class GradientTextureImporter : ProceduralTextureImporter<GradientTextureFormat>
	{

	}
}
