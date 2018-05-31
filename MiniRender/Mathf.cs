using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	public class Mathf
	{
		public static float4 clamp01(float4 v)
		{
			return new float4(
				clamp01(v.x),
				clamp01(v.y),
				clamp01(v.z),
				clamp01(v.w)
				);
		}

		public static float clamp01(float v)
		{
			if (v < 0)
				return 0;
			if (v > 1)
				return 1;

			return v;
		}

		public static float min(float v1, float v2)
		{
			if (v1 < v2)
				return v1;
			else
				return v2;
		}

		public static float max(float v1, float v2)
		{
			if (v1 > v2)
				return v1;
			else
				return v2;
		}

		public static float floor(float v)
		{
			return (float)Math.Floor(v);
		}



		public static float sin(float v)
		{
			return (float)Math.Sin(v);
		}
		public static float cos(float v)
		{
			return (float)Math.Cos(v);
		}


		public static float pow(float x,float y)
		{
			return (float)Math.Pow(x,y);
		}


		public static float sqrt(float v)
		{
			return (float)Math.Sqrt(v);
		}


		public static bool unequalf(float v1, float v2)
		{
			const float EPSILON = 1.192092896e-07F;
			return ((v1 < v2 - EPSILON) || (v1 > (v2 + EPSILON)));

		}
		public static bool equalf(float v1, float v2)
		{
			return !unequalf(v1, v2);
		}
	}
}
