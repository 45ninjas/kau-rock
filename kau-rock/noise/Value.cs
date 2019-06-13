using System.IO;
using System.Drawing;

using KauRock;
using OpenTK;
using Mathf = System.MathF;

namespace KauRock.Noise {
	public class Value {

		private readonly int xSample;
		private readonly int ySample;
		private readonly int zSample;
		private readonly int scaler;

		public Value(int fakeSeed) {
			unchecked {
				fakeSeed = fakeSeed.GetHashCode();
				xSample = fakeSeed * 345;
				ySample = fakeSeed * 8793;
				zSample = fakeSeed * 682;
				scaler = fakeSeed * 19;
			}
			
			// CreateImage(256, 256, "test-noise.png");
		}

		void CreateImage(int width, int height, string filename) {
			System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height);
			VoxPos pos = new VoxPos();

			float h = float.MinValue, l = float.MaxValue;

			int octaves = 6;
			float persistance = 0.5f;
			float lacunarity = 3f;

			float scale = GetScale(persistance, lacunarity, octaves);

			for (pos.X = 0; pos.X < width; pos.X++) {
					for (pos.Y = 0; pos.Y < width; pos.Y++) {
						Vector2 noisePos = new Vector2(pos.X, pos.Y) * 0.04f;
						
						float noise = GetAt(noisePos, persistance, lacunarity, octaves) * scale;

						if(noise > h)
							h = noise;
						else if (noise < l)
							l = noise;

						byte value = (byte)(noise * byte.MaxValue);
						bmp.SetPixel(pos.X, pos.Y, System.Drawing.Color.FromArgb(value, value, value));
					}
			}

			Log.Debug(this, $"min {l} max {h}");

			bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
		}

		float sample(Vector2 position) {
			// Make a crazy big number and do some stuff to it. Then get only
			// the decimal part of this big number.
			unchecked {
				float t = position.X * xSample + position.Y * ySample;
				t = (1 + Mathf.Sin(t) * 0.5f) * scaler;
				return t - Mathf.Truncate(t);
			}
		}

		float lerp (float a, float b, float mix) {
			return (1f - mix) * a + b * mix;
		}

		float smoothLerp(float a, float b, float mix) {
			float t = mix * mix * (3 - 2 * mix);
			return lerp(a,b, t);
		}

		// Calculate the amplitude scale.
		public float GetScale(float persistance, float lacunarity, int octaves) {
			float amplitude = 1;
			float accumulatedAmplitude = 0;
			for (int o = 0; o < octaves; o++) {
				accumulatedAmplitude += amplitude;
				amplitude *= persistance;
			}
			return 1f / accumulatedAmplitude;
		}

		// Get the noise at a location using octaves.
		public float GetAt(Vector2 position, float persistance, float lacunarity, int octaves) {
			float value = 0;
			float frequency = 1;
			float amplitude = 1;

			for (int o = 0; o < octaves; o++) {
					value += GetAt(position * frequency) * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
			}
			return value;
		}

		// Get the noise at a location.
		public float GetAt(Vector2 position) {

			// This is heavily based on the video by 'The Art of Code'
			// https://www.youtube.com/watch?v=zXsWftRdsvU

			Vector2 intiger = new Vector2() {
				X = Mathf.Truncate(position.X),
				Y = Mathf.Truncate(position.Y)
			};
			Vector2 fraction = position - intiger;

			// Smoothly interperlate the fraction of each 'cell'
			fraction = fraction * fraction * new Vector2(3 - 2 * fraction.X, 3 - 2 * fraction.Y);

			// I am not blending diagonally as I want the noise to have a bias to stick to the voxel grid.
			float btmL = sample(intiger);
			float btmR = sample(intiger + Vector2.UnitX);

			float topL = sample(intiger + Vector2.UnitY);
			float topR = sample(intiger + Vector2.One);

			float btm = lerp(btmL, btmR, fraction.X);
			float top = lerp(topL, topR, fraction.X);

			return lerp(btm, top, fraction.Y);
		}
	}
}