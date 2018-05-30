using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	public struct float3x3
	{
		public float3 row0;
		public float3 row1;
		public float3 row2;

		public float3x3(float3 r0, float3 r1, float3 r2)
		{
			row0 = r0; row1 = r1; row2 = r2;
		}

		public override string ToString()
		{
			return row0.ToString() + " " + row1.ToString() + " " + row2.ToString();
		}

		public float3 this[int index]
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
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

	}
}
