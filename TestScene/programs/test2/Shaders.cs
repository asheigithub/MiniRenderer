using MiniRender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestScreen.programs.test2
{
	class VShader : VertexShader
	{
		public override v2f Execute()
		{
			v2f v2f=__init_v2f();

			v2f.SV_POSITION = mul(MATRIX_MVP, appdata.vertex);
			v2f.color = appdata.color;
			v2f.uv = appdata.uv;
			
			v2f.worldNormal = 
				normalize(_WorldToObject[0].xyz * appdata.normal.x + _WorldToObject[1].xyz * appdata.normal.y + _WorldToObject[2].xyz * appdata.normal.z);

			v2f.tangent = mul((float3x3)_ObjectToWorld, appdata.tangent);

			float d = dot(v2f.worldNormal,v2f.tangent);


			return v2f;
		}
	}

	class FShader : FragementShader
	{
		

		protected override float4 Execute(v2f i)
		{
			float3 lightDir = normalize( float3(0,8,-4));
			
			float diff = max(0, dot(i.worldNormal, lightDir));
			//fixed nh = max(0, dot(s.Normal, halfDir));
			//fixed spec = pow(nh, s.Specular * 128) * s.Gloss;

			float4 c = float4(0,0,0,1) ;
			c.rgb = float3(diff, diff, diff) ; //(s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
			
			return c;

			//return i.color;

			//float3 ni = normalize(i.normal);
			//float3 bi = normalize( cross(i.normal, i.tangent));
			//float3 ti = normalize(cross(bi, i.normal));


			//float3x3 tbn = float3x3(ti, bi, ni);


			//float3 n2 = mul(tbn, i.tangent);

			//return n2 * 0.5 + 0.5;

			//return i.normal * 0.5+0.5;
		}
	}

}
