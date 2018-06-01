using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	public abstract class Shader
	{
		internal ProgramConstants constants;

		protected float PI = (float)Math.PI;


		protected float4x4 _ObjectToWorld
		{
			get
			{
				return constants.MATRIX_M;
			}
		}

		protected float4x4 _WorldToObject
		{
			get
			{
				return constants.loadMatrix(ProgramConstants._WorldToObject_ROW0);
			}
		}

		/// <summary>
		/// 当前投影矩阵
		/// </summary>
		protected float4x4 MATRIX_P
		{
			get
			{
				return constants.MATRIX_P;
			}
		}

		protected float4x4 MATRIX_V
		{
			get
			{
				return constants.MATRIX_V;
			}
		}

		/// <summary>
		/// MATRIX_V 的逆矩阵
		/// </summary>
		protected float4x4 MATRIX_I_V
		{
			get
			{
				return constants.MATRIX_I_V;
			}
		}


		protected float4x4 MATRIX_VP
		{
			get
			{
				return constants.MATRIX_VP;
			}
		}


		protected float4x4 MATRIX_M
		{
			get
			{
				return constants.MATRIX_M;
			}
		}

		protected float4x4 MATRIX_MVP
		{
			get
			{
				return mul( constants.MATRIX_VP, _ObjectToWorld);
			}
		}


		/// <summary>
		/// World space position of the camera.
		/// </summary>
		protected float3 _WorldSpaceCameraPos
		{
			get
			{
				return constants.registers[ProgramConstants._WorldSpaceCameraPos_ROW].xyz;
			}
		}




		/// <summary>
		/// Returns world space direction (not normalized) from given object space vertex position towards the camera.
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		protected float3 WorldSpaceViewDir(float3 worldPos)
		{
			return _WorldSpaceCameraPos - worldPos;
		}

		/// <summary>
		/// Returns object space direction (not normalized) from given object space vertex position towards the camera.
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		protected float3 ObjSpaceViewDir(float4 v)
		{
			float3 objSpaceCameraPos = mul(_WorldToObject, float4(_WorldSpaceCameraPos, 1)).xyz;
			return objSpaceCameraPos - v.xyz;
		}

 
		/// <summary>
		/// Transforms direction from object to world space
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		protected float3 ObjectToWorldDir(float3 dir)
		{
			return normalize(mul((float3x3)_ObjectToWorld, dir));
		}

		/// <summary>
		/// Transforms normal from object to world space
		/// </summary>
		/// <param name="norm">规格化的normal</param>
		/// <returns></returns>
		protected float3 ObjectToWorldNormal(float3 norm)
		{
			// mul(IT_M, norm) => mul(norm, I_M) => {dot(norm, I_M.col0), dot(norm, I_M.col1), dot(norm, I_M.col2)}
			return normalize(mul(norm, (float3x3)_WorldToObject));
		}

		/// <summary>
		/// Transforms direction from world to object space
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		protected float3 WorldToObjectDir(float3 dir)
		{
			return normalize(mul((float3x3)_WorldToObject, dir));
		}


		public static float3 normalize(float3 v)
		{
			float len = Mathf.sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
			return v / len;
		}


		public static float abs(double v)
		{
			return Math.Abs((float)v);
		}






		public static float3 cross(float3 vVector0, float3 vVector1)
		{
			
			float x = vVector0.y * vVector1.z - vVector0.z * vVector1.y;
			float y = vVector0.z * vVector1.x - vVector0.x * vVector1.z;
			float z = vVector0.x * vVector1.y - vVector0.y * vVector1.x;
			return float3(x,y,z);
		}

		public static float dot(float3 v1, float3 v2)
		{
			float3 v = v1 * v2;
			return v.x + v.y + v.z;
		}

		public static float dot(float4 v1, float4 v2)
		{
			float4 v = v1 * v2;
			return v.x + v.y + v.z + v.w;
		}

		public static float pow(double x,double y)
		{
			return Mathf.pow((float)x, (float)y);
		}

		public static float3x3 transpose(float3x3 m)
		{
			return new float3x3(
				float3(m.row0.x, m.row1.x, m.row2.x),
				float3(m.row0.y, m.row1.y, m.row2.y),
				float3(m.row0.z, m.row1.z, m.row2.z)
				);
		}

		public static float4x4 transpose(float4x4 m)
		{
			return new float4x4(
				float4(m.row0.x, m.row1.x, m.row2.x, m.row3.x),
				float4(m.row0.y, m.row1.y, m.row2.y, m.row3.y),
				float4(m.row0.z, m.row1.z, m.row2.z, m.row3.z),
				float4(m.row0.w, m.row1.w, m.row2.w, m.row3.w)
				);
		}


		public static float4x4 mul(float4x4 m1,float4x4 m2)
		{
			float4x4 m2t = transpose(m2);
			return new float4x4(
				float4(dot(m1.row0, m2t.row0), dot(m1.row0, m2t.row1), dot(m1.row0, m2t.row2), dot(m1.row0, m2t.row3)),
				float4(dot(m1.row1, m2t.row0), dot(m1.row1, m2t.row1), dot(m1.row1, m2t.row2), dot(m1.row1, m2t.row3)),
				float4(dot(m1.row2, m2t.row0), dot(m1.row2, m2t.row1), dot(m1.row2, m2t.row2), dot(m1.row2, m2t.row3)),
				float4(dot(m1.row3, m2t.row0), dot(m1.row3, m2t.row1), dot(m1.row3, m2t.row2), dot(m1.row3, m2t.row3))

				);
		}


		public static float4 mul(float4x4 matrix, float4 vector)
		{
			return
				new float4(
				dot(matrix[0], vector),
				dot(matrix[1], vector),
				dot(matrix[2], vector),
				dot(matrix[3], vector)
				);
		}


		public static float3 mul(float3x3 matrix, float3 vector)
		{
			return
				new float3(
				dot(matrix[0], vector),
				dot(matrix[1], vector),
				dot(matrix[2], vector)
				);
		}

		protected float3 mul(float3 vector, float3x3 matrix)
		{
			return _WorldToObject[0].xyz * vector.x + _WorldToObject[1].xyz * vector.y + _WorldToObject[2].xyz * vector.z;
		}

		public static float max(double v1, double v2)
		{
			return Mathf.max((float)v1, (float)v2);
		}
		public static float min(double v1, double v2)
		{
			return Mathf.min((float)v1, (float)v2);
		}
		public static float3 min(float3 v1, double v2)
		{
			return float3(min(v1.x, v2), min(v1.y, v2), min(v1.z, v2));
		}
		public static float3 min(float3 v1, float3 v2)
		{
			return float3(min(v1.x, v2.x), min(v1.y, v2.y), min(v1.z, v2.z));
		}

		public static float3 max(float3 v1, double v2)
		{
			return float3(max(v1.x, v2), max(v1.y, v2), max(v1.z, v2));
		}
		public static float3 max(float3 v1, float3 v2)
		{
			return float3(max(v1.x, v2.x), max(v1.y, v2.y), max(v1.z, v2.z));
		}

		public static float log(double x)
		{
			return (float)Math.Log(x);
		}

		public static float log2(double x)
		{
			return (float)Math.Log(x,2);
		}

		/// <summary>
		/// return the natural exponentiation of the parameter
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static float exp(double x)
		{
			return (float)Math.Exp(x);
		}

		/// <summary>
		/// return 2 raised to the power of the parameter
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static float exp2(double x)
		{
			return (float)Math.Pow(2,x);
		}

		public static float3 mix(float3 x, float3 y, float a)
		{
			return x * (1 - a) + y * a;
		}


		/// <summary>
		/// extract the sign of the parameter.
		/// sign returns -1.0 if x is less than 0.0, 0.0 if x is equal to 0.0, and +1.0 if x is greater than 0.0. 
		/// </summary>
		/// <returns></returns>
		public static float sign(double x)
		{
			return Math.Sign(x);
		}


		public static float sqrt(double v)
		{
			return Mathf.sqrt((float)v);
		}
		public static float3 sqrt(float3 v)
		{
			return float3(sqrt(v.x), sqrt(v.y), sqrt(v.z));
		}

		public static float floor(double v)
		{
			return Mathf.floor((float)v);
		}
		public static float floor(float v)
		{
			return Mathf.floor(v);
		}
		public static float2 floor(float2 v)
		{
			return float2( Mathf.floor(v.x), Mathf.floor(v.y));
		}
		public static float ceil(double v)
		{
			return (float)Math.Ceiling(v);
		}


		public static float3 clamp(float3 x,float minVal,float maxVal)
		{
			return min(max(x, minVal), maxVal);
		}
		public static float3 clamp(float3 x, float3 minVal, float3 maxVal)
		{
			return min(max(x, minVal), maxVal);
		}
		public static float clamp(double x, double minVal, double maxVal)
		{
			return min(max(x, minVal), maxVal);
		}






		public static float frac(double v)
		{
			return (float)v - floor((float)v);
		}

		/// <summary>
		/// calculate the reflection direction for an incident vector
		/// </summary>
		/// <param name="I"></param>
		/// <param name="N"></param>
		/// <returns></returns>
		public static float3 reflect(float3 I,float3 N)
		{
			return I - N * (2 * dot(N, I)) ;
		}

		/// <summary>
		/// calculate the refraction direction for an incident vector
		/// </summary>
		/// <param name="I"></param>
		/// <param name="N"></param>
		/// <param name="eta"></param>
		/// <returns></returns>
		public static float3 refract(float3 I,float3 N,float eta)
		{
			//Description
			//For a given incident vector I, surface normal N and ratio of indices of refraction, eta, refract returns the refraction vector, R.
			//R is calculated as: 

			//	k = 1.0 - eta * eta * (1.0 - dot(N, I) * dot(N, I));
			//			if (k < 0.0)
			//				R = genType(0.0);       // or genDType(0.0)
			//			else
			//				R = eta * I - (eta * dot(N, I) + sqrt(k)) * N;

			//			The input parameters I and N should be normalized in order to achieve the desired result. 

			float k = 1 - eta * eta * (1 - dot(N, I) * dot(N, I));
			if (k < 0)
				return float3(0, 0, 0);
			else
				return I * eta - N*(dot(N, I) * eta + sqrt(k)) ;

		}



		#region 各种 float 构造函数

		public static float4 float4(float3 xyz, double w)
		{
			return new float4(xyz, (float)w);
		}

		public static float4 float4(double x, double y, double z, double w)
		{
			return new float4((float)x, (float)y, (float)z, (float)w);
		}

		public static float3 float3(float2 xy, double z)
		{
			return new float3(xy, (float)z);
		}

		public static float3 float3(double x, double y, double z)
		{
			return new float3((float)x, (float)y, (float)z);
		}

		public static float2 float2(double x, double y)
		{
			return new float2((float)x,(float)y);
		}

		public static float3x3 float3x3(float3 row0,
										float3 row1,
										float3 row2

			)
		{
			return new float3x3(
				row0,
				row1,
				row2
				);
		}

		public static float3x3 float3x3(double m00,double m01,double m02,
										double m10, double m11, double m12,
										double m20, double m21, double m22

			)
		{
			return new float3x3(
				float3(m00,m01,m02),
				float3(m10, m11, m12),
				float3(m20, m21, m22)
				);
		}


		#endregion

	}
}
