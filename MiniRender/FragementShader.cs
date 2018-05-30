using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	/// <summary>
	/// 片段着色器
	/// </summary>
	public abstract class FragementShader : Shader
	{
		public enum DerivativeOperations
		{
			UV,
		}

		private FragementUnit unit;

		internal void Run(FragementUnit unit)
		{
			this.unit = unit;
			unit.output = Execute(unit.input);
			this.unit = null;
		}

		/// <summary>
		/// 求DDX
		/// 由于软件光栅化限制，只能读取输入的值的ddx。
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		protected float4 ddx( DerivativeOperations op)
		{
			switch (op)
			{
				case DerivativeOperations.UV:
					{
						var d = unit.dpdx_v1.input.uv - unit.dpdx_v2.input.uv;
						return new float4(d.x, d.y, 0, 0);
					}
				default:
					return new float4();
			}
		}
		/// <summary>
		/// 求DDY
		/// 由于软件光栅化限制，只能读取输入的值的ddy。
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		protected float4 ddy( DerivativeOperations op)
		{
			switch (op)
			{
				case DerivativeOperations.UV:
					var d = unit.dpdy_v1.input.uv - unit.dpdy_v2.input.uv;
					return new float4(d.x, d.y, 0, 0);
				default:
					return new float4();
			}
		}

		protected abstract float4 Execute(v2f IN);
	}
}
