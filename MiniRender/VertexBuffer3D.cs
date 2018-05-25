using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	/// <summary>
	/// VertexBuffer3D 类表示上载到渲染上下文的一组顶点数据。 
	/// </summary>
	public class VertexBuffer3D
	{
		private int numVertices;
		internal VertexBuffer3D(int numVertices)
		{
			this.numVertices = numVertices;
		}

		internal List<Vertex> vertices;

		public void uploadFromVector(IEnumerable<Vertex> vertices)
		{
			this.vertices = new List<Vertex>();
			this.vertices.AddRange(vertices);

			if (this.vertices.Count != numVertices)
			{
				throw new ArgumentException("Vertices数量不匹配");
			}

		}

	}
}
