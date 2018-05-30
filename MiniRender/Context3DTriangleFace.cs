using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	/// <summary>
	/// 用于指定三角形相对于视图点的方向的枚举。 
	/// </summary>
	public enum Context3DTriangleFace
	{
		BACK,
		FRONT,
		FRONT_AND_BACK,
		NONE
	}
}
