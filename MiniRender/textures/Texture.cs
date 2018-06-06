using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender.textures
{
	/// <summary>
	/// The Texture class represents a 2-dimensional texture uploaded to a rendering context. 
	/// Defines a 2D texture for use during rendering.
	/// Texture cannot be instantiated directly.Create instances by using Context3D createTexture() method.
	/// </summary>
	public class Texture :TextureBase
	{
		private static Texture _white;

		public static Texture white
		{
			get
			{
				if (_white == null)
				{
					_white = new Texture(2, 2);

					_white.uploadFromByteArray(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 }, 0);

				}

				return _white;
			}
		}

		private static Texture _planenormal;

		public static Texture planeNormal
		{
			get
			{
				if (_planenormal == null)
				{
					_planenormal = new Texture(2, 2);

					_planenormal.uploadFromByteArray(new byte[] { 128, 128, 255, 255, 128, 128, 255, 255, 128, 128, 255, 255, 128, 128, 255, 255 }, 0);

				}

				return _planenormal;
			}
		}

		internal MipLevel[] miplevels;

		internal Texture(int width,int height)
		{
			miplevels = MipLevel.calMipLevels(width, height);

		}

		internal override MipLevel[] getMipLevels()
		{
			return miplevels;
		}

		/// <summary>
		/// Uploads a texture from a ByteArray. 
		/// </summary>
		/// <param name="data">a byte array that is contains enough bytes in the textures internal format to fill the texture. rgba textures are read as bytes per texel component (1 or 4). float textures are read as floats per texel component (1 or 4). The ByteArray object must use the little endian format. </param>
		/// <param name="byteArrayOffset">the position in the byte array object at which to start reading the texture data. </param>
		/// <param name="miplevel"> the mip level to be loaded, level zero is the top-level, full-resolution image. </param>
		public void uploadFromByteArray(byte[] data, int byteArrayOffset, int miplevel = 0)
		{
			if (miplevel > 0)
			{
				if (miplevels[miplevel - 1].texuteData == null)
				{
					throw new InvalidOperationException("先上传上一级mipmap");
				}
			}



			var mip = miplevels[miplevel];
			mip.texuteData = new float4[mip.width][];
			for (int i = 0; i < mip.width; i++)
			{
				mip.texuteData[i] = new float4[mip.height];

				for (int j = 0; j < mip.height; j++)
				{
					int pos = j * mip.width * 4 + i * 4;

					byte r = data[pos + byteArrayOffset + 0];
					byte g = data[pos + byteArrayOffset + 1];
					byte b = data[pos + byteArrayOffset + 2];
					byte a = data[pos + byteArrayOffset + 3];

					mip.texuteData[i][j] = new float4(
						r * 1.0f /255,
						g * 1.0f /255,
						b * 1.0f /255,
						a * 1.0f /255
						);

				}

			}

			uploadedmiplevels = miplevel;

		}


		public void AutoGenMipMap()
		{
			if (miplevels[0].texuteData == null)
			{
				throw new InvalidOperationException("先上传0级的图像");
			}

			for (int k = 1; k < miplevels.Length; k++)
			{
				var mip = miplevels[k];
				mip.texuteData = new float4[mip.width][];

				var perMip = miplevels[k - 1];
				int pwfactor = perMip.width / mip.width;
				int phfactor = perMip.height / mip.height;

				


				for (int i = 0; i < mip.width; i++)
				{
					mip.texuteData[i] = new float4[mip.height];
					for (int j = 0; j < mip.height; j++)
					{
						float4 pc0 = perMip.texuteData[i*pwfactor][j*phfactor];
						float4 pc1 = perMip.texuteData[i * pwfactor + pwfactor-1 ][j * phfactor];
						float4 pc2 = perMip.texuteData[i * pwfactor][j * phfactor + phfactor-1];
						float4 pc3 = perMip.texuteData[i * pwfactor + pwfactor - 1][j * phfactor + phfactor - 1];

						mip.texuteData[i][j] = (pc0 + pc1 + pc2 + pc3) * 0.25;

					}
				}

			}

			uploadedmiplevels = miplevels.Length-1;

		}

		private int uploadedmiplevels;
		internal override int maxMipLevel
		{
			get
			{
				return uploadedmiplevels;
			}
		}

	}
}
