using MiniRender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestScreen.programs.test1
{
	class VShader : MiniRender.VertexShader
	{
		public override v2f Execute()
		{
			v2f v2f;

			v2f.SV_POSITION =  mul(M, appdata.vertex);
			v2f.color = appdata.color;


			return v2f;
		}
	}
}
