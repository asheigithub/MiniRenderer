using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	public struct float4x4
	{
		public float4 row0;
		public float4 row1;
		public float4 row2;
		public float4 row3;

		public float4x4(float4 r0,float4 r1,float4 r2,float4 r3)
		{
			row0 = r0;row1 = r1;row2 = r2;row3 = r3;
		}

		public override string ToString()
		{
			return row0.ToString() + " " + row1.ToString() + " " + row2.ToString() + " " + row3.ToString() ;
		}

		public float4 this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return row0;
					case 1:
						return row1;
					case 2:
						return row2;
					case 3:
						return row3;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		public static explicit operator float3x3(float4x4 m4)
		{
			return new float3x3(m4[0].xyz, m4[1].xyz, m4[2].xyz);
		}
	}
}
