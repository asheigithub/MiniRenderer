using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	public struct float3
	{
		public float3(float2 xy, float z)
		{
			v1 = xy.x; v2 = xy.y; v3 = z;
		}
		public float3(float x, float y, float z)
		{
			v1 = x; v2 = y; v3 = z;
		}

		public override string ToString()
		{
			return string.Format("{0},{1},{2}", v1.ToString("F4"), v2.ToString("F4"), v3.ToString("F4"));
		}

		private float v1;
		private float v2;
		private float v3;


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

		public float z
		{
			get { return v3; }
			set { v3 = value; }
		}



		public float r
		{
			get { return v1; }
			set { v1 = value; }
		}

		public float g
		{
			get { return v2; }
			set { v2 = value; }
		}

		public float b
		{
			get { return v3; }
			set { v3 = value; }
		}



		public float2 xy
		{
			get { return new float2(v1, v2); }
		}

		public float2 xz
		{
			get { return new float2(v1, v3); }
		}

		public float2 yz
		{
			get { return new float2(v2, v3); }
		}

		public static float3 operator +(float3 lhs, double v)
		{
			return new float3(lhs.x + (float)v, lhs.y + (float)v, lhs.z + (float)v);
		}
		public static float3 operator +( double v,float3 rhs)
		{
			return new float3(rhs.x + (float)v, rhs.y + (float)v, rhs.z + (float)v);
		}
		public static float3 operator -(float3 lhs, double v)
		{
			return new float3(lhs.x - (float)v, lhs.y -(float)v, lhs.z - (float)v);
		}
		public static float3 operator -( double v,float3 rhs)
		{
			return new float3(rhs.x - (float)v, rhs.y - (float)v, rhs.z - (float)v);
		}

		public static float3 operator +(float3 lhs, float3 rhs)
		{
			return new float3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
		}

		public static float3 operator -(float3 lhs, float3 rhs)
		{
			return new float3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
		}

		public static float3 operator -(float3 lhs)
		{
			return new float3(-lhs.x, -lhs.y, -lhs.z);
		}
		public static float3 operator +(float3 lhs)
		{
			return lhs;
		}

		public static float3 operator *(float3 lhs, float3 rhs)
		{
			return new float3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
		}

		public static float3 operator /(float3 lhs, float3 rhs)
		{
			return new float3(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
		}

		public static float3 operator *(float3 lhs, double v)
		{
			return new float3(lhs.x * (float)v, lhs.y * (float)v, lhs.z * (float)v);
		}

		public static float3 operator *(double v,float3 rhs)
		{
			return new float3(rhs.x * (float)v, rhs.y * (float)v, rhs.z * (float)v);
		}

		public static float3 operator *(float3 lhs, float v)
		{
			return new float3(lhs.x * v, lhs.y * v, lhs.z * v);
		}

		public static float3 operator /(float3 lhs, double v)
		{
			return new float3(lhs.x / (float)v, lhs.y / (float)v, lhs.z / (float)v);
		}
		public static float3 operator /( double v,float3 rhs)
		{
			return new float3(rhs.x / (float)v, rhs.y / (float)v, rhs.z / (float)v);
		}
		public static float3 operator /(float3 lhs, float v)
		{
			float m = 1 / v;
			return new float3(lhs.x * m, lhs.y * m, lhs.z * m);
		}
	}
}
