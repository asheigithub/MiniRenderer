using MiniRender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestScreen.programs.test3
{


	class VShader : VertexShader
	{
		public override v2f Execute()
		{
			v2f v2f = __init_v2f();

			v2f.SV_POSITION = mul(MATRIX_MVP, appdata.vertex);
			v2f.worldPos = mul(_ObjectToWorld, appdata.vertex).xyz;
			v2f.objPos = appdata.vertex.xyz;
			v2f.color = appdata.color;
			v2f.uv = appdata.uv;
			v2f.worldNormal = ObjectToWorldNormal(normalize(appdata.normal));
				//normalize(_WorldToObject[0].xyz * appdata.normal.x + _WorldToObject[1].xyz * appdata.normal.y + _WorldToObject[2].xyz * appdata.normal.z);
			v2f.worldTangent = mul((float3x3)_ObjectToWorld, appdata.tangent);


			//float3 lightDir = normalize(float3(0, 4, -4));
			//float3 normalDir = v2f.worldNormal;

			////计算高光
			////计算反射光夹角
			////fixed3 reflectDir = normalize(reflect(-lightDir,normalDir));

			//float3 viewDir = normalize(WorldSpaceViewDir(mul(_ObjectToWorld, appdata.vertex).xyz));
			////平分线
			//float3 halfDir = normalize(viewDir + lightDir);

			//float _Gloss = 50;
			//float3 _Specular = float3(1, 1, 1);
			//float4 _LightColor0 = float4(1, 1, 1, 1);

			////控制高光颜色
			//float3 specular = _Specular * _LightColor0.rgb * pow(max(dot(normalDir, halfDir), 0), _Gloss);

			//float diff = max(0, dot(normalDir, lightDir));
			////fixed nh = max(0, dot(s.Normal, halfDir));
			////fixed spec = pow(nh, s.Specular * 128) * s.Gloss;

			//float4 c = float4(0, 0, 0, 1);
			//c.rgb = _LightColor0.rgb * diff + specular;

			//v2f.color = c;


			return v2f;
		}
	}

	class FShader : FragementShader
	{
		public override bool HasDebug => true;

		protected override float4 Execute(v2f i)
		{
			
			//Blinn-Phong

			float3 lightDir = normalize(float3(0, 9, -4));
			float3 normalDir = normalize(i.worldNormal);


			//计算高光
			//计算反射光夹角
			float3 reflectDir = normalize(reflect(-lightDir, normalDir));

			float3 refractDir = refract(-lightDir, normalDir, 0.6f);


			float3 viewDir = normalize(WorldSpaceViewDir(mul(_ObjectToWorld, i.objPos).xyz));
			//平分线
			float3 halfDir = normalize(viewDir + lightDir);


			AddDebugInfo(lightDir, "lightDir", MiniRender.debugger.DebugInfoType.Vector, float3(1, 1, 0));
			//AddDebugInfo(viewDir, "viewDir", MiniRender.debugger.DebugInfoType.Vector, float3(0, 1, 0));
			//AddDebugInfo(halfDir, "halfDir", MiniRender.debugger.DebugInfoType.Vector, float3(1, 1, 0));
			AddDebugInfo(normalDir, "normalDir", MiniRender.debugger.DebugInfoType.Vector, float3(0, 0, 1));
			//AddDebugInfo(i.worldTangent, "worldTangent", MiniRender.debugger.DebugInfoType.Vector, float3(0, 1, 1));
			AddDebugInfo(reflectDir, "reflectDir", MiniRender.debugger.DebugInfoType.Vector, float3(0, 1, 0));
			AddDebugInfo(refractDir, "reflectDir", MiniRender.debugger.DebugInfoType.Vector, float3(1, 0, 0));




			float _Gloss = 10;
			float3 _Specular = float3(1, 1, 1);
			float4 _LightColor0 = float4(1, 1, 1, 1);

			//控制高光颜色
			float3 specular = _Specular * _LightColor0.rgb * pow(max(dot(normalDir, halfDir), 0), _Gloss);



			float diff = max(0, dot(normalDir, lightDir));
			//fixed nh = max(0, dot(s.Normal, halfDir));
			//fixed spec = pow(nh, s.Specular * 128) * s.Gloss;


			float4 c = float4(0.7, 0.7, 0.7, 0.9);
			c.rgb = c.rgb * _LightColor0.rgb * diff + specular;

			return c;



			//float3 lightDir = normalize(float3(0, 4, -4));
			//float3 normalDir = normalize( i.worldNormal);

			//float3 viewDir = normalize(WorldSpaceViewDir(mul(_ObjectToWorld, i.objPos).xyz));
			//float3 halfDir = normalize(viewDir + lightDir);

			//float _Gloss = 20;
			//float _Specular = 3.0f;//float3(1, 1, 1);
			//float4 _LightColor0 = float4(1, 1, 1, 1);


			//float diff = max(0, dot(normalDir, lightDir));
			//float nh = max(0, dot(normalDir, halfDir));



			//float spec = pow(nh, _Specular * 128) * _Gloss;



			//float4 c=float4(1,1,1,1);
			//c.rgb = ( _LightColor0.rgb * diff + _LightColor0.rgb * spec) * 1;

			//return c;




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
