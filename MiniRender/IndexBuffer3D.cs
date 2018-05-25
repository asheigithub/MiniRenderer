using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	/// <summary>
	/// IndexBuffer3D 用于表示顶点索引列表，由图形子系统保留的图形元素构成。
	/// </summary>
	public class IndexBuffer3D
	{
		private int numIndices;
		internal IndexBuffer3D(int numIndices)
		{
			this.numIndices = numIndices;
		}

		internal List<uint> data;

		public void uploadFromVector(IEnumerable<uint> data)
		{
			this.data = new List<uint>();
			this.data.AddRange(data);

			if (this.data.Count != numIndices)
			{
				throw new ArgumentException("Index数量不匹配");
			}
		}

	}
}
