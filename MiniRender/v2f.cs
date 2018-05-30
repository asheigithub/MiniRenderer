using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	/// <summary>
	/// 顶点着色器的返回值
	/// </summary>
	public struct v2f
	{
		/// <summary>
		/// 输出的顶点
		/// </summary>
		public float4 SV_POSITION;
		
		public float4 color;

		public float3 uv;

		public float3 worldNormal;

		public float3 objNormal;

		public float3 tangent;

	}
}
