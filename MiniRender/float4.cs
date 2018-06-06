using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	public struct float4
	{
		
		public float4(float x,float y,float z,float w)
		{
			v1 = x;v2 = y;v3 = z;v4 = w;
		}

		public float4(float3 xyz, double w)
		{
			v1 = xyz.x; v2 = xyz.y; v3 = xyz.z; v4 = (float)w;
		}

		public float4(float3 xyz, float w)
		{
			v1 = xyz.x; v2 = xyz.y; v3 = xyz.z; v4 = w;
		}

		public static implicit operator float4(float3 v)
		{
			return new float4(v, 1);
		}


		public override string ToString()
		{
			return string.Format("{0},{1},{2},{3}",v1.ToString("F4"),v2.ToString("F4"), v3.ToString("F4"), v4.ToString("F4"));
		}

		private float v1;
		private float v2;
		private float v3;
		private float v4;

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

		public float w
		{
			get { return v4; }
			set { v4 = value; }
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

		public float a
		{
			get { return v4; }
			set { v4 = value; }
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

		public float3 xyz
		{
			set
			{
				v1 = value.x;
				v2 = value.y;
				v3 = value.z;
			}
			get
			{
				return new float3(v1, v2,v3);
			}
		}

		public float3 rgb
		{
			set
			{
				v1 = value.x;
				v2 = value.y;
				v3 = value.z;
			}
			get
			{
				return new float3(v1, v2, v3);
			}
		}

		public static float4 operator +(float4 lhs, double v)
		{
			return new float4(lhs.x + (float)v, lhs.y + (float)v, lhs.z + (float)v, lhs.w + (float)v);
		}

		public static float4 operator +(float4 lhs, float4 rhs)
		{
			return new float4(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w);
		}

		public static float4 operator -(float4 lhs, double v)
		{
			return new float4(lhs.x - (float)v, lhs.y -(float)v, lhs.z -(float)v, lhs.w - (float)v);
		}

		public static float4 operator -(float4 lhs, float4 rhs)
		{
			return new float4(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w);
		}

		public static float4 operator *(float4 lhs, float4 rhs)
		{
			return new float4(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z, lhs.w * rhs.w);
		}

		public static float4 operator /(float4 lhs, float4 rhs)
		{
			return new float4(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z, lhs.w / rhs.w);
		}

		
		public static float4 operator *(float4 lhs, float v)
		{
			return new float4(lhs.x * v, lhs.y * v, lhs.z * v, lhs.w * v);
		}
		public static float4 operator *(float4 lhs, double v)
		{
			return new float4(lhs.x * (float)v, lhs.y * (float)v, lhs.z * (float)v, lhs.w * (float)v);
		}

		public static float4 operator *( double v,float4 rhs)
		{
			return new float4(rhs.x * (float)v, rhs.y * (float)v, rhs.z * (float)v, rhs.w * (float)v);
		}

		public static float4 operator /(float4 lhs, float v)
		{
			float m = 1 / v;
			return new float4(lhs.x * m, lhs.y * m, lhs.z * m, lhs.w * m);
		}
		public static float4 operator /(float4 lhs, double v)
		{
			float m = 1 / (float)v;
			return new float4(lhs.x * m, lhs.y * m, lhs.z * m, lhs.w * m);
		}
	}
}
