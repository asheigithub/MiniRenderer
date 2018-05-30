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




		public static float3 normalize(float3 v)
		{
			float len = Mathf.sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
			return v / len;
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

		public static float max(float v1, float v2)
		{
			return Mathf.max(v1, v2);
		}



		public static float floor(float v)
		{
			return Mathf.floor(v);
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
