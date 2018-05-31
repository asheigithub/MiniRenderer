using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	/// <summary>
	/// 常量寄存器表
	/// </summary>
	class ProgramConstants
	{
		internal const int _ObjectToWorld_ROW0 = 0;
		internal const int _WorldToObject_ROW0 = 4;

		internal const int _MatrixV_ROW0 = 8;
		internal const int _matrix_projection_ROW0 = 12;

		internal const int _MatrixVP_ROW0 = 16;
		internal const int _MatrixInvV_ROW0 = 20;



		internal const int _WorldSpaceCameraPos_ROW = 50;
		internal const int _FogColor_ROW = 79;
		internal const int _FogParams_ROW = 80;

		public const int USERDEFINE_STARTIDX = 81;

		public float4x4 MATRIX_P
		{
			get
			{
				return loadMatrix(_matrix_projection_ROW0);
			}
		}

		public float4x4 MATRIX_V
		{
			get
			{
				return loadMatrix(_MatrixV_ROW0);
			}
		}

		public float4x4 MATRIX_I_V
		{
			get
			{
				return loadMatrix(_MatrixInvV_ROW0);
			}
		}

		public float4x4 MATRIX_VP
		{
			get
			{
				return loadMatrix(_MatrixVP_ROW0);
			}
		}

		public float4x4 MATRIX_M
		{
			get
			{
				return loadMatrix(_ObjectToWorld_ROW0);
			}
		}



		internal float4x4 loadMatrix(int idx)
		{
			return new float4x4(
				registers[idx],
				registers[idx+1],
				registers[idx+2],
				registers[idx+3]
				);
		}


		public float4[] registers = new float4[2048];

	}
}
