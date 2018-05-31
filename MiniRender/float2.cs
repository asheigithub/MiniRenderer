using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	public struct float2
	{
		public float2(float x, float y)
		{
			v1 = x; v2 = y;
		}

		public override string ToString()
		{
			return string.Format("{0},{1}", v1.ToString("F4"), v2.ToString("F4"));
		}


		private float v1;
		private float v2;


		public float x
		{
			get { return v1; }
			set { v1 = value; }
		}

		public float y
		{
			get { return v2; }
			set { v2 = value; }
		}



		public static float2 operator +(float2 lhs, float2 rhs)
		{
			return new float2(lhs.x + rhs.x, lhs.y + rhs.y);
		}

		public static float2 operator -(float2 lhs, float2 rhs)
		{
			return new float2(lhs.x - rhs.x, lhs.y - rhs.y);
		}

		public static float2 operator *(float2 lhs, float2 rhs)
		{
			return new float2(lhs.x * rhs.x, lhs.y * rhs.y);
		}

		public static float2 operator /(float2 lhs, float2 rhs)
		{
			return new float2(lhs.x / rhs.x, lhs.y / rhs.y);
		}

		public static float2 operator *(float2 lhs, float v)
		{
			return new float2(lhs.x * v, lhs.y * v);
		}
		public static float2 operator /(float2 lhs, float v)
		{
			float m = 1 / v;
			return new float2(lhs.x * m, lhs.y * m);
		}
	}
}
