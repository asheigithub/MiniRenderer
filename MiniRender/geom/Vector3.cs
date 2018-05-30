using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender.geom
{
	public struct Vector3
	{
		public static Vector3 X_AXIS
		{
			get
			{
				return new Vector3(1, 0, 0);
			}
		}
		public static Vector3 Y_AXIS
		{
			get
			{
				return new Vector3(0, 1, 0);
			}
		}
		public static Vector3 Z_AXIS
		{
			get
			{
				return new Vector3(0, 0, 1);
			}
		}

		public float x;
		public float y;
		public float z;

		public Vector3(float x,float y,float z)
		{
			this.x = x;this.y = y;this.z = z;
		}

		public override string ToString()
		{
			return string.Format("{0},{1},{2}",x,y,z);
		}

		public float getLength() 
		{
			return Mathf.sqrt(x* x +
				y* y +
				z* z);
		}

		public Vector3 getNormalize()
		{
			float fLength = getLength();
			if (fLength != 0.0f)
				fLength = 1.0f / fLength;

			return new Vector3(x, y, z) * fLength;
		}

		public Vector3 normalize()
		{
			float fLength = getLength();
			if (fLength != 0.0f)
				fLength = 1.0f / fLength;

			this *= fLength;
			return this;
		}


		public static Vector3 operator *(Vector3 vector, float v)
		{
			return new Vector3(vector.x * v, vector.y * v, vector.z * v);
		}


		public static Vector3 operator +(Vector3 vVector0, Vector3 vVector1)
		{
			return new Vector3(vVector0.x + vVector1.x,
				vVector0.y + vVector1.y,
				vVector0.z + vVector1.z
				);
		}
		public static Vector3 operator -(Vector3 vVector0, Vector3 vVector1)
		{
			return new Vector3(vVector0.x - vVector1.x,
				vVector0.y - vVector1.y,
				vVector0.z - vVector1.z
				);
		}


		public Vector3 cross(Vector3 vVector1)
		{
			return cross(this, vVector1);
		}

		public static Vector3 cross(Vector3 vVector0, Vector3 vVector1)
		{
			Vector3 vResult;
			vResult.x = vVector0.y * vVector1.z - vVector0.z * vVector1.y;
			vResult.y = vVector0.z * vVector1.x - vVector0.x * vVector1.z;
			vResult.z = vVector0.x * vVector1.y - vVector0.y * vVector1.x;
			return vResult;
		}

	}
}
