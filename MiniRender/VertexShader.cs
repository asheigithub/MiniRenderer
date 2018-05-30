using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	/// <summary>
	/// 顶点着色器
	/// </summary>
	public abstract class VertexShader :Shader
	{
		public Vertex appdata;
		
		public abstract v2f Execute();

		/// <summary>
		/// 构造一个v2f结构
		/// </summary>
		/// <returns></returns>
		protected v2f __init_v2f()
		{
			return new v2f();
		}

	}
}
