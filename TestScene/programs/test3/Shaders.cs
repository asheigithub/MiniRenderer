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
			
			return v2f;
		}
	}

	class FShader : FragementShader
	{
		public override bool HasDebug => true;

		private float INV_PI = 1 / 3.1415926f;


		float3 DisneyDiffuse(float3 In, float3 Out , float3 normal )
		{
			float3 H = normalize(In + Out);

			float CosThetaD = dot(H, In);
			float CosThetaL = dot(In, normal);
			float CosTheaaV = dot(Out, normal);

			float FD_90 = 0.5f + 2 * CosThetaD * CosThetaD * (1-Roughness);
			return _diffcolor * INV_PI * (1 + (FD_90 - 1) * pow(1 - CosThetaL, 5)) * (1 + (FD_90 - 1) * pow(1 - CosTheaaV, 5));
			

			//float oneMinusCosL = 1.0f - abs( dot(In,normal));
			//float oneMinusCosLSqr = oneMinusCosL * oneMinusCosL;
			//float oneMinusCosV = 1.0f - abs( dot(Out,normal));
			//float oneMinusCosVSqr = oneMinusCosV * oneMinusCosV;

			//// Roughness是粗糙度，IDotH的意思会在下一篇讲Microfacet模型时提到
			//float IDotH = dot(In, normalize(In + Out));
			//float F_D90 = 0.5f + 2.0f * IDotH * IDotH * Roughness;

			//return _diffcolor * INV_PI * (1.0f + (F_D90 - 1.0f) * oneMinusCosLSqr * oneMinusCosLSqr * oneMinusCosL) *
			//				(1.0f + (F_D90 - 1.0f) * oneMinusCosVSqr * oneMinusCosVSqr * oneMinusCosV);

		}

		float D_GGXaniso(float RoughnessX, float RoughnessY, float NoH, float3 H, float3 X, float3 Y)
		{
			float ax = RoughnessX * RoughnessX;
			float ay = RoughnessY * RoughnessY;
			float XoH = dot(X, H);
			float YoH = dot(Y, H);
			float d = XoH * XoH / (ax * ax) + YoH * YoH / (ay * ay) + NoH * NoH;
			return 1 / (PI * ax * ay * d * d);
		}

		float D_GGX(float Roughness, float NoH)
		{
			float a = Roughness * Roughness;
			float a2 = a * a;
			float d = (NoH * a2 - NoH) * NoH + 1;   // 2 mad
			return a2 / (PI * d * d);                   // 4 mul, 1 rcp
		}


		float GGXNormalDistribution(float roughness, float NdotH)
		{
			float roughnessSqr = roughness * roughness;
			float NdotHSqr = NdotH * NdotH;
			float TanNdotHSqr = (1 - NdotHSqr) / NdotHSqr;
			return INV_PI * sqrt(roughness / (NdotHSqr * (roughnessSqr + TanNdotHSqr)));
		}

		float TrowbridgeReitzNormalDistribution( float roughness, float NdotH)
		{
			float roughnessSqr = roughness * roughness;
			float Distribution = NdotH * NdotH * (roughnessSqr - 1.0f) + 1.0f;
			return roughnessSqr / (3.1415926535f * Distribution * Distribution);
		}

		float3 F_Schlick(float3 SpecularColor, float VoH)
		{
			float Fc = pow(1 - VoH,5);                   // 1 sub, 3 mul
														//return Fc + (1 - Fc) * SpecularColor;		// 1 add, 3 mad

			// Anything less than 2% is physically impossible and is instead considered to be shadowing
			return clamp(50.0 * SpecularColor.g , 0,1) * Fc + (1 - Fc) * SpecularColor;

		}

		float3 F_Fresnel(float3 SpecularColor, float VoH)
		{
			float3 SpecularColorSqrt = sqrt(clamp(float3(0, 0, 0), float3(0.99, 0.99, 0.99), SpecularColor));
			float3 n = (1 + SpecularColorSqrt) / (1 - SpecularColorSqrt);
			float3 g = sqrt(n * n + VoH * VoH - 1);
			return 0.5 * Square((g - VoH) / (g + VoH)) * (1 + Square(((g + VoH) * VoH - 1) / ((g - VoH) * VoH + 1)));
		}

		float3 Square(float3 v)
		{
			return float3(v.x * v.x, v.y * v.y, v.z * v.z);
		}

		float Square(float v)
		{
			return v * v;
		}

		float Vis_Schlick(float Roughness, float NoV, float NoL)
		{
			float k = Square(Roughness) * 0.5f;
			float Vis_SchlickV = NoV * (1 - k) + k;
			float Vis_SchlickL = NoL * (1 - k) + k;
			return 0.25f/ (Vis_SchlickV * Vis_SchlickL);
		}

		float Specular_G(float Roughness,float NoL,float NoV)
		{
			float k = Square(Roughness + 1) /8;

			float G1L = NoL / (NoL * (1 - k) + k);
			float G1V = NoV / (NoV * (1 - k) + k);

			float GLVH = G1L * G1V;

			GLVH = GLVH * 0.25f / (NoV* NoL);
			return GLVH;
		}

		float SchlickGGXGeometricShadowingFunction(float roughness,float NdotL, float NdotV)
		{
			float k = roughness / 2;


			float SmithL = (NdotL) / (NdotL * (1 - k) + k);
			float SmithV = (NdotV) / (NdotV * (1 - k) + k);


			float Gs = (SmithL * SmithV);
			return Gs;
		}


		//------------------------------------------------
		//schlick functions
		float SchlickFresnel(float i)
		{
			float x = clamp(1.0 - i, 0.0, 1.0);
			float x2 = x * x;
			return x2 * x2 * x;
		}
		float3 FresnelLerp(float3 x, float3 y, float d)
		{
			float t = SchlickFresnel(d);
			return mix(x, y, t);
		}


		private float3 _diffcolor = float3(0.6, 0.6,0.6);
		private float3 _SpecularColor = float3(230/255.0f, 125 / 255.0f, 40 / 255.0f) * 4;
		public float Roughness =0.7f;
		public float _Metallic = 0f;
		protected override float4 Execute(v2f i)
		{

			AddDebugInfo(float4(Roughness, 0, 0, 0), "粗糙度", MiniRender.debugger.DebugInfoType.Numeric, float3(0, 0, 0));
			AddDebugInfo(float4(_Metallic, 0, 0, 0), "金属性", MiniRender.debugger.DebugInfoType.Numeric, float3(0, 0, 0));


			float3 diffuseColor = _diffcolor * (1.0 - _Metallic);
			//float f0 = F0(NdotL, NdotV, LdotH, roughness);
			//diffuseColor *= f0;
			//diffuseColor += indirectDiffuse;

			//Specular calculations


			float3 specColor = mix(_SpecularColor, _diffcolor, 1-_Metallic);




			float3 viewDir = normalize( WorldSpaceViewDir(i.worldPos));
			float3 lightDir = normalize( float3(-6, 9, -9));
			float3 normal = normalize(i.worldNormal);

			float3 diff = //_diffcolor * INV_PI ; //
							DisneyDiffuse( lightDir, viewDir, i.worldNormal) * diffuseColor;

			

			float3 H = normalize(lightDir + viewDir);

			
			float VoH = clamp( dot(viewDir, normal),0,1);
			float NoL = clamp( dot(normal, lightDir),0,1);
			float NoV = clamp( dot(normal, viewDir),0,1);

			float D =
				//TrowbridgeReitzNormalDistribution(Roughness, dot(normal, H));
				D_GGX(Roughness, dot( normal, H));

			float3 F = F_Schlick(specColor, VoH);

			float G = //Specular_G(Roughness, NoL, NoV);

					Vis_Schlick(Roughness, NoV, NoL);
			//SchlickGGXGeometricShadowingFunction(Roughness, NoL, NoV);



			//PBR
			float3 specularity = D * F * G;
			float grazingTerm = clamp(Roughness + _Metallic,0,1);
			
			float3 unityIndirectSpecularity = 0 * FresnelLerp(specColor, float3( grazingTerm,grazingTerm,grazingTerm), NoV) * max(0.15, _Metallic) * (1 - Roughness * Roughness * Roughness);


			//float3 lightingModel = ((diffuseColor) + specularity + (unityIndirectSpecularity * 1));
			//lightingModel *= NoL;

			float3 lightingModel = ((diff  + NoL * diffuseColor) * (1-_Metallic)  + specularity + (unityIndirectSpecularity * 1));
			

			float4 finalDiffuse = float4(lightingModel * float3(1,1,1), 1);
			;
			return finalDiffuse;



			float4 c =
				diff +
				D * F * G;

			//c.rgb =  float3(G,G, G);

			return c;
		}
	}



}
