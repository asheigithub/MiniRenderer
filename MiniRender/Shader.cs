using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	public abstract class Shader
	{
		internal ProgramConstants constants;

		protected m4 MVP
		{
			get
			{
				return new m4(constants.registers[0], constants.registers[1], constants.registers[2], constants.registers[3]);
			}
		}

		public float dot(float4 v1, float4 v2)
		{
			float4 v = v1 * v2;
			return v.x + v.y + v.z + v.w;
		}

		public float4 mul(m4 matrix, float4 vector)
		{
			return
				new float4(
				dot(matrix[0], vector),
				dot(matrix[1], vector),
				dot(matrix[2], vector),
				dot(matrix[3], vector)
				);
			


		}



		#region 各种 float 构造函数

		public float4 float4(float3 xyz, double w)
		{
			return new float4(xyz, (float)w);
		}

		public float4 float4(double x, double y, double z, double w)
		{
			return new float4((float)x, (float)y, (float)z, (float)w);
		}

		public float3 float3(float2 xy, double z)
		{
			return new float3(xy, (float)z);
		}

		public float3 float3(double x, double y, double z)
		{
			return new float3((float)x, (float)y, (float)z);
		}

		public float2 float2(double x, double y)
		{
			return new float2((float)x,(float)y);
		}

		#endregion

	}
}
