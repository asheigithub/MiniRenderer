using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	public class Mathf
	{
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

		public static float sin(float v)
		{
			return (float)Math.Sin(v);
		}
		public static float cos(float v)
		{
			return (float)Math.Cos(v);
		}

		public static float sqrt(float v)
		{
			return (float)Math.Sqrt(v);
		}

	}
}
