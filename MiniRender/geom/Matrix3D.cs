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
		public static Matrix3D Identity
		{
			get
			{
				return new Matrix3D(
					1, 0, 0, 0,
					0, 1, 0, 0,
					0, 0, 1, 0,
					0, 0, 0, 1
					);
			}
		}


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
			M00 = (m00); M01 = (m01); M02 = (m02); M03 = (m03);
			M10 = (m10); M11 = (m11); M12 = (m12); M13 = (m13);
			M20 = (m20); M21 = (m21); M22 = (m22); M23 = (m23);
			M30 = (m30); M31 = (m31); M32 = (m32); M33 = (m33);
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

			M00 = pSrc1.M00 * pSrc2.M00 + pSrc1.M01 * pSrc2.M10 + pSrc1.M02 * pSrc2.M20 + pSrc1.M03 * pSrc2.M30;
			M01 = pSrc1.M00 * pSrc2.M01 + pSrc1.M01 * pSrc2.M11 + pSrc1.M02 * pSrc2.M21 + pSrc1.M03 * pSrc2.M31;
			M02 = pSrc1.M00 * pSrc2.M02 + pSrc1.M01 * pSrc2.M12 + pSrc1.M02 * pSrc2.M22 + pSrc1.M03 * pSrc2.M32;
			M03 = pSrc1.M00 * pSrc2.M03 + pSrc1.M01 * pSrc2.M13 + pSrc1.M02 * pSrc2.M23 + pSrc1.M03 * pSrc2.M33;

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

		private void swap(ref float e1, ref float e2)
		{
			float temp = e2;
			e2 = e1;
			e1 = temp;
		}



		public Matrix3D appendRotation(float angle, Vector3 axis)
		{
			Quaternion q
				= Quaternion.identity.fromAxisAngle(axis, angle);

			Matrix3D rot = q.toMatrix3D();
			//q.setMatrix3D(&rot);

			//this->append(rot);
			//return *this;

			append(rot);

			return this;
		}


		public Matrix3D appendTranslation(float x, float y, float z)
		{
			append(new Matrix3D(
				1.0f, 0.0f, 0.0f, 0.0f,
				0.0f, 1.0f, 0.0f, 0.0f,
				0.0f, 0.0f, 1.0f, 0.0f,
				x, y, z, 1.0f));

			return this;
		}


		public Matrix3D appendScale(float xScale, float yScale, float zScale)
		{
			this.append(new Matrix3D(
				xScale, 0.0f, 0.0f, 0.0f,
				0.0f, yScale, 0.0f, 0.0f,
				0.0f, 0.0f, zScale, 0.0f,
				0.0f, 0.0f, 0.0f, 1.0f));


			return this;

		}

		/// <summary>
		/// 计算2阶方阵的行列式
		/// </summary>
		/// <param name="m00"></param>
		/// <param name="m01"></param>
		/// <param name="m10"></param>
		/// <param name="m11"></param>
		/// <returns></returns>
		float DET2X2(float m00, float m01,
			float m10, float m11)
		{
			return m00 * m11 - m01 * m10;
		}

		/// <summary>
		/// 计算3阶方阵的行列式
		/// </summary>
		/// <param name="m00"></param>
		/// <param name="m01"></param>
		/// <param name="m02"></param>
		/// <param name="m10"></param>
		/// <param name="m11"></param>
		/// <param name="m12"></param>
		/// <param name="m20"></param>
		/// <param name="m21"></param>
		/// <param name="m22"></param>
		/// <returns></returns>
		float DET3X3(
			float m00, float m01, float m02,
			float m10, float m11, float m12,
			float m20, float m21, float m22)
		{
			return m00 * DET2X2(m11, m12, m21, m22) -
				m10 * DET2X2(m01, m02, m21, m22) +
				m20 * DET2X2(m01, m02, m11, m12);
		}

		/// <summary>
		/// 计算4阶方阵的行列式
		/// </summary>
		/// <param name="m00"></param>
		/// <param name="m01"></param>
		/// <param name="m02"></param>
		/// <param name="m03"></param>
		/// <param name="m10"></param>
		/// <param name="m11"></param>
		/// <param name="m12"></param>
		/// <param name="m13"></param>
		/// <param name="m20"></param>
		/// <param name="m21"></param>
		/// <param name="m22"></param>
		/// <param name="m23"></param>
		/// <param name="m30"></param>
		/// <param name="m31"></param>
		/// <param name="m32"></param>
		/// <param name="m33"></param>
		/// <returns></returns>
		float DET4X4(float m00, float m01, float m02, float m03,
			float m10, float m11, float m12, float m13,
			float m20, float m21, float m22, float m23,
			float m30, float m31, float m32, float m33)
		{
			return m00 * DET3X3(m11, m12, m13, m21, m22, m23, m31, m32, m33) -
				m10 * DET3X3(m01, m02, m03, m21, m22, m23, m31, m32, m33) +
				m20 * DET3X3(m01, m02, m03, m11, m12, m13, m31, m32, m33) -
				m30 * DET3X3(m01, m02, m03, m11, m12, m13, m21, m22, m23);
		}



		/// <summary>
		/// 获取当前矩阵的行列式。如果值为0，则不可逆。Matrix3D必须是可逆的矩阵
		/// </summary>
		/// <returns></returns>
		float determinant()
		{
			float fDeterminant = DET4X4(M00, M01, M02, M03,
				M10, M11, M12, M13,
				M20, M21, M22, M23,
				M30, M31, M32, M33);

			return fDeterminant;

		}


		/// <summary>
		/// 将当前矩阵转化为当前矩阵的逆矩阵
		/// </summary>
		/// <returns></returns>
		public Matrix3D invert()
		{
			float fDeterminant = determinant();

			if (Mathf.equalf(fDeterminant, 0))
			{
				this.identity();
				return this;
			}

			float fScale = 1.0f / fDeterminant;

			Matrix3D s = this;

			M00 = +DET3X3(s.M11, s.M12, s.M13, s.M21, s.M22, s.M23, s.M31, s.M32, s.M33) * fScale;
			M10 = -DET3X3(s.M10, s.M12, s.M13, s.M20, s.M22, s.M23, s.M30, s.M32, s.M33) * fScale;
			M20 = +DET3X3(s.M10, s.M11, s.M13, s.M20, s.M21, s.M23, s.M30, s.M31, s.M33) * fScale;
			M30 = -DET3X3(s.M10, s.M11, s.M12, s.M20, s.M21, s.M22, s.M30, s.M31, s.M32) * fScale;

			M01 = -DET3X3(s.M01, s.M02, s.M03, s.M21, s.M22, s.M23, s.M31, s.M32, s.M33) * fScale;
			M11 = +DET3X3(s.M00, s.M02, s.M03, s.M20, s.M22, s.M23, s.M30, s.M32, s.M33) * fScale;
			M21 = -DET3X3(s.M00, s.M01, s.M03, s.M20, s.M21, s.M23, s.M30, s.M31, s.M33) * fScale;
			M31 = +DET3X3(s.M00, s.M01, s.M02, s.M20, s.M21, s.M22, s.M30, s.M31, s.M32) * fScale;

			M02 = +DET3X3(s.M01, s.M02, s.M03, s.M11, s.M12, s.M13, s.M31, s.M32, s.M33) * fScale;
			M12 = -DET3X3(s.M00, s.M02, s.M03, s.M10, s.M12, s.M13, s.M30, s.M32, s.M33) * fScale;
			M22 = +DET3X3(s.M00, s.M01, s.M03, s.M10, s.M11, s.M13, s.M30, s.M31, s.M33) * fScale;
			M32 = -DET3X3(s.M00, s.M01, s.M02, s.M10, s.M11, s.M12, s.M30, s.M31, s.M32) * fScale;

			M03 = -DET3X3(s.M01, s.M02, s.M03, s.M11, s.M12, s.M13, s.M21, s.M22, s.M23) * fScale;
			M13 = +DET3X3(s.M00, s.M02, s.M03, s.M10, s.M12, s.M13, s.M20, s.M22, s.M23) * fScale;
			M23 = -DET3X3(s.M00, s.M01, s.M03, s.M10, s.M11, s.M13, s.M20, s.M21, s.M23) * fScale;
			M33 = +DET3X3(s.M00, s.M01, s.M02, s.M10, s.M11, s.M12, s.M20, s.M21, s.M22) * fScale;

			return this;

		}

		/// <summary>
		/// 获取当前矩阵的逆矩阵
		/// </summary>
		/// <returns></returns>
		public Matrix3D getInvert()
		{
			Matrix3D ret = this;
			return ret.invert();
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

			Vector3 lookat = new Vector3(lookatx, lookaty, lookatz);
			Vector3 eye = new Vector3(eyex, eyey, eyez);
			Vector3 up = new Vector3(upx, upy, upz);


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
			Matrix3D m2 = new Matrix3D(

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


			Matrix3D m = new Matrix3D(
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
