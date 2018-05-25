using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	/// <summary>
	/// 可接收绘图缓冲区的接口
	/// </summary>
	public interface IRenderTargetAdapter
	{
		void SetRenderBuffer(IRenderTarget renderBuffer);

	}
}
