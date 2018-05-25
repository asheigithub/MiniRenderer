using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	/// <summary>
	/// RenderBuffer接口
	/// </summary>
	public interface IRenderTarget
	{
		int rt_width { get; }
		int rt_height { get; }


		int width { get; }
		int height { get; }

		//float4[][] getPixels();
		float4 getPixel(int x, int y);


		float4[][] getRealPixels();

	}
}
