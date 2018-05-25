using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	/// <summary>
	/// Program3D 类表示上载到渲染上下文的一对渲染程序（也称为“着色器”）。 
	///	由 Program3D 对象管理的程序控制 drawTriangles 调用期间的整个三角形渲染。
	///这些程序始终由两个相互关联的部分组成：顶点程序和片段程序。 
	///顶点程序会操作 VertexBuffer3D 中定义的数据，负责将顶点投影到剪辑空间，并将任何所需的顶点数据（例如颜色）传递到片段着色器。
	///片段着色器会操作顶点程序传递给它的属性，并为三角形的每个栅格化片段生成颜色，最终形成像素颜色。请注意，片段程序在 3D 编程文献中具有多个名称，包括片段着色器和像素着色器。
	///通过将相应 Program3D 实例传递到 Context3D setProgram() 方法，指定后续渲染操作要使用的程序对。
	///您无法直接创建 Program3D 对象；请改用 Context3D createProgram() 方法。
	/// </summary>
	public class Program3D
	{
		internal Program3D() { }


		internal VertexShader vertexShader;

		public void upload(VertexShader vertexShader)
		{
			this.vertexShader = vertexShader;
		}

		internal bool Validate()
		{
			if (vertexShader == null)
				return false;

			return true;

		}


	}
}
