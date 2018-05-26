using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender.geom
{
	/// <summary>
	/// 3D矩阵类 4x4,列向量存储
	///     | 0  4  8  12 |
	///	    |             |
	///	    | 1  5  9  13 |
	///		|             |
	///	    | 2  6  10 14 |
	///	    |             |
	///	    | 3  7  11 15 |
	/// </summary>
	public struct Matrix3D
	{
		public float M00, M01, M02, M03,
			M10, M11, M12, M13,
			M20, M21, M22, M23,
			M30, M31, M32, M33
			;

		public Matrix3D(float m00, float m01, float m02, float m03,
				float m10, float m11, float m12, float m13,
				float m20, float m21, float m22, float m23,
				float m30, float m31, float m32, float m33)
		{
			M00=(m00); M01=(m01); M02=(m02); M03=(m03);
			M10=(m10); M11=(m11); M12=(m12); M13=(m13);
			M20=(m20); M21=(m21); M22=(m22); M23=(m23);
			M30=(m30); M31=(m31); M32=(m32); M33=(m33);
		}



		public Matrix3D identity()
		{
			M00 = 1; M01 = 0; M02 = 0; M03 = 0;
			M10 = 0; M11 = 1; M12 = 0; M13 = 0;
			M20 = 0; M21 = 0; M22 = 1; M23 = 0;
			M30 = 0; M31 = 0; M32 = 0; M33 = 1;

			return this;
		}

		/// <summary>
		/// 通过将另一个 Matrix3D 对象与当前 Matrix3D 对象相乘来后置一个矩阵。
		/// </summary>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public Matrix3D append(Matrix3D rhs)
		{
			Matrix3D pSrc1 = this;
			Matrix3D pSrc2 = rhs;

			M00 = pSrc1.M00 * pSrc2.M00 + pSrc1.M01 * pSrc2.M10 + pSrc1.M02* pSrc2.M20 + pSrc1.M03 * pSrc2.M30;
			M01 = pSrc1.M00 * pSrc2.M01 + pSrc1.M01 * pSrc2.M11 + pSrc1.M02* pSrc2.M21 + pSrc1.M03 * pSrc2.M31;
			M02 = pSrc1.M00 * pSrc2.M02 + pSrc1.M01 * pSrc2.M12 + pSrc1.M02* pSrc2.M22 + pSrc1.M03 * pSrc2.M32;
			M03 = pSrc1.M00 * pSrc2.M03 + pSrc1.M01 * pSrc2.M13 + pSrc1.M02* pSrc2.M23 + pSrc1.M03 * pSrc2.M33;

			M10 = pSrc1.M10 * pSrc2.M00 + pSrc1.M11 * pSrc2.M10 + pSrc1.M12 * pSrc2.M20 + pSrc1.M13 * pSrc2.M30;
			M11 = pSrc1.M10 * pSrc2.M01 + pSrc1.M11 * pSrc2.M11 + pSrc1.M12 * pSrc2.M21 + pSrc1.M13 * pSrc2.M31;
			M12 = pSrc1.M10 * pSrc2.M02 + pSrc1.M11 * pSrc2.M12 + pSrc1.M12 * pSrc2.M22 + pSrc1.M13 * pSrc2.M32;
			M13 = pSrc1.M10 * pSrc2.M03 + pSrc1.M11 * pSrc2.M13 + pSrc1.M12 * pSrc2.M23 + pSrc1.M13 * pSrc2.M33;

			M20 = pSrc1.M20 * pSrc2.M00 + pSrc1.M21 * pSrc2.M10 + pSrc1.M22 * pSrc2.M20 + pSrc1.M23 * pSrc2.M30;
			M21 = pSrc1.M20 * pSrc2.M01 + pSrc1.M21 * pSrc2.M11 + pSrc1.M22 * pSrc2.M21 + pSrc1.M23 * pSrc2.M31;
			M22 = pSrc1.M20 * pSrc2.M02 + pSrc1.M21 * pSrc2.M12 + pSrc1.M22 * pSrc2.M22 + pSrc1.M23 * pSrc2.M32;
			M23 = pSrc1.M20 * pSrc2.M03 + pSrc1.M21 * pSrc2.M13 + pSrc1.M22 * pSrc2.M23 + pSrc1.M23 * pSrc2.M33;

			M30 = pSrc1.M30 * pSrc2.M00 + pSrc1.M31 * pSrc2.M10 + pSrc1.M32 * pSrc2.M20 + pSrc1.M33 * pSrc2.M30;
			M31 = pSrc1.M30 * pSrc2.M01 + pSrc1.M31 * pSrc2.M11 + pSrc1.M32 * pSrc2.M21 + pSrc1.M33 * pSrc2.M31;
			M32 = pSrc1.M30 * pSrc2.M02 + pSrc1.M31 * pSrc2.M12 + pSrc1.M32 * pSrc2.M22 + pSrc1.M33 * pSrc2.M32;
			M33 = pSrc1.M30 * pSrc2.M03 + pSrc1.M31 * pSrc2.M13 + pSrc1.M32 * pSrc2.M23 + pSrc1.M33 * pSrc2.M33;




			return this;
		}

		private void swap(ref float e1,ref float e2)
		{
			float temp = e2;
			e2 = e1;
			e1 = temp;
		}

		/// <summary>
		/// 转置当前矩阵。
		/// </summary>
		/// <returns></returns>
		public Matrix3D transpose()
		{
			this = getTranspose();
			return this;
		}

		public Matrix3D getTranspose()
		{
			Matrix3D temp = new Matrix3D(
				M00, M10, M20, M30,
				M01, M11, M21, M31,
				M02, M12, M22, M32,
				M03, M13, M23, M33

				);
			return temp;
		}

		public static Matrix3D operator *(Matrix3D lhs, Matrix3D rhs)
		{
			return lhs.append(rhs);
		}


		public static Matrix3D lookAtLH(float eyex, float eyey, float eyez,
				float lookatx, float lookaty, float lookatz,
				float upx, float upy, float upz)
		{
			// 相机UVN
			//N= lookAt- eye
			//U=UP * N
			//V = N *U

			Vector3 lookat =new Vector3(lookatx, lookaty, lookatz);
			Vector3 eye =new Vector3(eyex, eyey, eyez);
			Vector3 up =new Vector3(upx, upy, upz);


			var N = lookat - eye;

			var U = up.cross(N); //Vector3.Cross(up, N);

			var V = N.cross(U); //Vector3.Cross(N, U);

			N.normalize();
			U.normalize();
			V.normalize();


			Matrix3D m1 = new Matrix3D(U.x, U.y, U.z, 0.0f,
				V.x, V.y, V.z, 0.0f,
				N.x, N.y, N.z, 0.0f,
				0.0f, 0.0f, 0.0f, 1.0f);
			Matrix3D m2 = new  Matrix3D(

				1.0f, 0.0f, 0.0f, -eyex,
				0.0f, 1.0f, 0.0f, -eyey,
				0.0f, 0.0f, 1.0f, -eyez,
				0.0f, 0.0f, 0.0f, 1.0f

				);

			m1.append(m2);
			m1.transpose();
			return m1;
		}



		public static Matrix3D perspectiveOffCenterLH(float left,
				float right,
				float bottom,
				float top,
				float zNear,
				float zFar)
		{

			float a = zFar / (zFar - zNear);
			float b = -zNear * zFar / (zFar - zNear);


			Matrix3D m=new Matrix3D(
					2.0f * zNear / (right - left), 0.0f, (left + right) / (left - right), 0.0f,
					0.0f, 2.0f * zNear / (top - bottom), (bottom + top) / (bottom - top), 0.0f,
					0.0f, 0.0f, a, b,
					0.0f, 0.0f, 1.0f, 0.0f
					);
			m.transpose();
			return m;
		}


	}
}
