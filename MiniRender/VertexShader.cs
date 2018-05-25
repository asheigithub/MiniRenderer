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
		
	}
}
