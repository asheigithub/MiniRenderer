using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniRender;

namespace TestScreen.programs.test1
{
	class FShader : FragementShader
	{
		protected override float4 Execute(v2f i)
		{
			return i.color;
		}
	}
}
