using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender.geom
{

	public struct Quaternion
	{
		public float qx;
		public float qy;
		public float qz;
		public float qw;

		public static Quaternion identity
		{
			get
			{
				return new Quaternion(0, 0, 0, 1);
			}
		}


		public Quaternion(float qx,float qy,float qz,float qw)
		{
			this.qx = qx;
			this.qy = qy;
			this.qz = qz;
			this.qw = qw;
		}


		public Quaternion fromAxisAngle( Vector3 axis,float angle )
		{
			float sin_a = Mathf.sin(angle / 2);
			float cos_a = Mathf.cos(angle / 2);

			float len = 1 / axis.getLength();

			 qx = axis.x* len * sin_a;
			 qy = axis.y* len * sin_a;
			 qz = axis.z* len * sin_a;
			 qw = cos_a;

			return this;
		}



		private static float[] _tq = new float[4];
		public Quaternion fromMatrix3D(Matrix3D matrix)
		{
			float s;
			float[] tq =_tq;
			int i, j;

			// Use tq to store the largest trace
			tq[0] = 1 + matrix.M00 + matrix.M11 + matrix.M22;
			tq[1] = 1 + matrix.M00 - matrix.M11 - matrix.M22;
			tq[2] = 1 - matrix.M00 + matrix.M11 - matrix.M22;
			tq[3] = 1 - matrix.M00 - matrix.M11 + matrix.M22;

			// Find the maximum (could also use stacked if's later)
			j = 0;
			for (i = 1; i<4; i++) j = (tq[i]>tq[j]) ? i : j;

			// check the diagonal
			if (j == 0)
			{

				qw = tq[0];
				qx = matrix.M12 - matrix.M21;
				qy = matrix.M20 - matrix.M02;
				qz = matrix.M01 - matrix.M10;
			}
			else if (j == 1)
			{
				qw = matrix.M12 - matrix.M21;
				qx = tq[1];
				qy = matrix.M01 + matrix.M10;
				qz = matrix.M20 + matrix.M02;
			}
			else if (j == 2)
			{
				qw = matrix.M20 - matrix.M02;
				qx = matrix.M01 + matrix.M10;
				qy = tq[2];
				qz = matrix.M12 + matrix.M21;
			}
			else
			{
				qw = matrix.M01 - matrix.M10;
				qx = matrix.M20 + matrix.M02;
				qy = matrix.M12 + matrix.M21;
				qz = tq[3];
			}

			s = Mathf.sqrt(0.25f / tq[j]);
			qw *= s;
			qx *= s;
			qy *= s;
			qz *= s;

			return  this;
		}


		public Matrix3D toMatrix3D()
		{
			Matrix3D output;

			float xx = qx * qx;
			float xy = qx * qy;
			float xz = qx * qz;
			float xw = qx * qw;
			float yy = qy * qy;
			float yz = qy * qz;
			float yw = qy * qw;
			float zz = qz * qz;
			float zw = qz * qw;

			//float* mat = input->rawData;
			output.M00 = 1 - 2 * (yy + zz);
			output.M10 = 2 * (xy - zw);
			output.M20 = 2 * (xz + yw);
			output.M01 = 2 * (xy + zw);
			output.M11 = 1 - 2 * (xx + zz);
			output.M21 = 2 * (yz - xw);
			output.M02 = 2 * (xz - yw);
			output.M12 = 2 * (yz + xw);
			output.M22 = 1 - 2 * (xx + yy);
			output.M30 = output.M31 = output.M32 = output.M03 = output.M13 = output.M23 = 0;
			output.M33 = 1;

			return output;

		}



	}
}
