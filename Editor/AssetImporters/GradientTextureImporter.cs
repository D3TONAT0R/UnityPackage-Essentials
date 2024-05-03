using D3T;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif
using UnityEngine;

namespace D3TEditor
{
	[ScriptedImporter(0, "gradient")]
	internal class GradientTextureImporter : ScriptedImporter
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

		public Vector2Int resolution = new Vector2Int(64, 64);
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

		[Space(10)]
		public bool linear = false;
		public bool generateMipMaps = true;
		public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
		public FilterMode filterMode = FilterMode.Bilinear;

		public override void OnImportAsset(AssetImportContext ctx)
		{
			resolution.x = Mathf.Clamp(resolution.x, 1, 4096);
			resolution.y = Mathf.Clamp(resolution.y, 1, 4096);
			var texture = new Texture2D(resolution.x, resolution.y, TextureFormat.RGBA32, generateMipMaps, linear);
			texture.alphaIsTransparency = true;
			texture.wrapMode = wrapMode;
			texture.filterMode = filterMode;

			Random.InitState(0);
			for(int x = 0; x < resolution.x; x++)
			{
				for(int y = 0; y < resolution.y; y++)
				{
					Vector2 uv = new Vector2((x + 0.5f) / resolution.x, (y + 0.5f) / resolution.y);
					float pos = GetSamplePos(uv, mode);
					if(reversed) pos = 1f - pos;
					var color = SampleAt(pos);
					texture.SetPixel(x, y, color);
				}
			}
			texture.Apply();
			ctx.AddObjectToAsset("gradient", texture);
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
}
