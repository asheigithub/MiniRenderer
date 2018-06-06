using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestScreen
{
	class SceneUtils
	{
		public static MiniRender.textures.Texture MakeAndUploadTexture( MiniRender.Context3D context3D, string path)
		{
			using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(path))
			{
				var texture = context3D.createTexture(bitmap.Width, bitmap.Height);

				texture.uploadFromByteArray(LoadBitmapData(bitmap), 0);

				return texture;
			}
		}

		public static byte[] LoadBitmapData(System.Drawing.Bitmap bitmap)
		{


			byte[] bytes = new byte[bitmap.Width * bitmap.Height * 4];
			var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			int width = bitmap.Width;
			int height = bitmap.Height;
			int stride = bitmapData.Stride;
			byte[] o = new byte[stride * bitmapData.Height];

			System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, o, 0, o.Length);


			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					byte a = o[j * stride + i * 4 + 3];
					byte r = o[j * stride + i * 4 + 2];
					byte g = o[j * stride + i * 4 + 1];
					byte b = o[j * stride + i * 4 + 0];


					bytes[(height - j - 1) * width * 4 + i * 4 + 0] = r;
					bytes[(height - j - 1) * width * 4 + i * 4 + 1] = g;
					bytes[(height - j - 1) * width * 4 + i * 4 + 2] = b;
					bytes[(height - j - 1) * width * 4 + i * 4 + 3] = a;

				}
			}

			return bytes;
		}

		public static byte[] LoadBitmapData(string path)
		{
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(path);

			var bytes = LoadBitmapData(bitmap);
			
			bitmap.Dispose();

			return bytes;
		}


	}
}
