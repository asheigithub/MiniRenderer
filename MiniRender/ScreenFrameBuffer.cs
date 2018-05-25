using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	/// <summary>
	/// 屏幕帧缓冲
	/// </summary>
	public class ScreenFrameBuffer : IRenderTarget
	{
		public int rt_width { get;private set; }
		public int rt_height { get; private set; }

		public int width { get; private set; }
		public int height { get; private set; }

		public float4[][] pixels;

		private int xm;
		private int ym;

		public ScreenFrameBuffer(int width,int height,int xMultiple,int yMultiple)
		{
			
			this.width = width;
			this.height = height;

			this.rt_width = width * xMultiple;
			this.rt_height = height * yMultiple;

			xm = xMultiple;
			ym = yMultiple;

			pixels = new float4[rt_width][];
			for (int i = 0; i < rt_width; i++)
			{
				pixels[i] = new float4[rt_height];
			}

		}

		

		public float4 getPixel(int x, int y)
		{
			float4 final = new float4();
			for (int i = 0; i < xm; i++)
			{
				for (int j = 0; j < ym; j++)
				{
					final += pixels[x * xm+i][ y * ym+j];
				}
			}

			return final / xm / ym;
		}

		public float4[][] getRealPixels()
		{
			return pixels;
		}

		
	}
}
