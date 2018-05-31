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

		/// <summary>
		/// 指示此片段着色器是否有调试信息显示
		/// </summary>
		public virtual bool HasDebug
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// 为了正确可视化向量，必须在顶点着色器中正确计算worldPos,特别注意!
		/// </summary>
		/// <param name="data"></param>
		/// <param name="label"></param>
		/// <param name="debugInfoType"></param>
		/// <param name="visualizationColor"></param>
		protected void AddDebugInfo(float4 data,string label,debugger.DebugInfoType debugInfoType ,float3 visualizationColor)
		{
			debugger.DebugInfo debugInfo = new debugger.DebugInfo();
			debugInfo.label = label;
			debugInfo.data = data;
			debugInfo.type = debugInfoType;
			debugInfo.visualizationColor = visualizationColor;

			debugData.debugInfos.Add(debugInfo);

		}


		private debugger.FrameDebugData debugData;
		private FragementUnit unit;

		internal void Run(FragementUnit unit,debugger.FrameDebugData debugData)
		{
			this.unit = unit;this.debugData = debugData;
			unit.output = Execute(unit.input);
			this.unit = null;this.debugData = null;
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
