using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	public abstract class Shader
	{
		internal ProgramConstants constants;

		protected m4 M
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


	}
}
