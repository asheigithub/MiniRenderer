using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	/// <summary>
	/// Z-Buffer 16位深度
	/// </summary>
	class ZBuffer
	{

		private ushort[][] buffer;


		public int width { get;private set; }
		public int height { get; private set; }




		public ZBuffer(int width,int height)
		{
			this.width = width;
			this.height = height;

			buffer = new ushort[width][];
			for (int i = 0; i < width; i++)
			{
				buffer[i] = new ushort[height];
				Array.Clear(buffer[i], 0, height);
			}
		}

		public ushort Convert(float depth)
		{
			depth = Mathf.clamp01(depth);
			return (ushort)(depth * ushort.MaxValue);
		}

		public void WriteDepth(int i, int j, ushort short_depth)
		{
			buffer[i][j] = short_depth;
		}

		public ushort this[int i, int j]
		{
			get
			{
				return buffer[i][j] ;
			}
		}

		public void clear(float depth)
		{
			
			for (int i = 0; i < width ; i++)
			{
				for (int j = 0; j < height; j++)
				{
					buffer[i][j] = Convert(depth);
				}
			}
		}

	}
}
