using MiniRender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestScreen.programs.test4
{
	//matcap

	class VShader : VertexShader
	{
		public override v2f Execute()
		{
			v2f v2f = __init_v2f();

			v2f.SV_POSITION = mul(MATRIX_MVP, appdata.vertex);
			v2f.uv = appdata.uv;

			float3 worldNorm = ObjectToWorldDir(appdata.normal);
			worldNorm = mul((float3x3)MATRIX_V, worldNorm);
			v2f.worldNormal = worldNorm * 0.5 + 0.5;
			v2f.worldNormal.z = 0;
			
			return v2f;
		}
	}

	class FShader : FragementShader
	{
		public override bool HasDebug => false;

		protected override float4 Execute(v2f IN)
		{
			float4 tex= tex2D(0, IN.uv);

			float4 mc = tex2D(1, IN.worldNormal) * tex * 2;

			//float4 mc = (tex + (tex2D(1, IN.worldNormal) * 2.0) - 1.0);

			//return (tex + (mc * 2.0) - 1.0);

			return mc;
		}
	}



	class VShader_Bump : VertexShader
	{
		public override v2f Execute()
		{
			v2f v2f = __init_v2f();

			v2f.SV_POSITION = mul(MATRIX_MVP, appdata.vertex);
			v2f.uv = appdata.uv;


			// Declares 3x3 matrix 'rotation', filled with tangent space basis
			float3 worldNormal = ObjectToWorldNormal(normalize(appdata.normal));
			float3 worldTangent = ObjectToWorldDir(appdata.tangent.xyz);
			float3 worldBinormal = cross(v2f.worldNormal, worldTangent) * appdata.tangent.w;

			//创建切线空间
			v2f.tSpace0 = float3(worldTangent.x, worldBinormal.x, worldNormal.x);
			v2f.tSpace1 = float3(worldTangent.y, worldBinormal.y, worldNormal.y);
			v2f.tSpace2 = float3(worldTangent.z, worldBinormal.z, worldNormal.z);



			return v2f;
		}
	}

	class FShader_Bump : FragementShader
	{
		public override bool HasDebug => false;

		protected override float4 Execute(v2f i)
		{
			float4 tex = tex2D(0, i.uv);



			//从法线贴图中取出法线||此处*4，是因为机器人的法线需要平铺，其他地方不需要*4!!
			float3 normal = tex2D(2, i.uv * 4).xyz * 2 - 1;

			//将法线从切线空间旋转到世界空间 Rotate normals from tangent space to world space
			float3 worldNorm = float3(1, 1, 1);
			worldNorm.x = dot(i.tSpace0, normal);
			worldNorm.y = dot(i.tSpace1, normal);
			worldNorm.z = dot(i.tSpace2, normal);

			worldNorm = mul((float3x3)MATRIX_V, worldNorm);


			float4 mc = tex2D(1, worldNorm * 0.5+0.5) * tex * 2;

			//float4 mc = (tex + (tex2D(1, IN.worldNormal) * 2.0) - 1.0);

			//return (tex + (mc * 2.0) - 1.0);

			return mc;
		}
	}


}
