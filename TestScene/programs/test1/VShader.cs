using MiniRender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestScreen.programs.test1
{
	class VShader : VertexShader
	{
		public override v2f Execute()
		{
			v2f v2f = __init_v2f();

			v2f.SV_POSITION = mul(MATRIX_VP, mul(MATRIX_M, appdata.vertex));
			v2f.color = appdata.color;
			v2f.uv = appdata.uv;
			
			return v2f;
		}
	}
}
