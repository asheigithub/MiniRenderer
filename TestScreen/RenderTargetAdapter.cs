using MiniRender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestScreen
{
	class RenderTargetAdapter : MiniRender.IRenderTargetAdapter
	{
		IRenderTarget renderBuffer;

		System.Drawing.Bitmap bitmap;//用于显示RenderBuffer的位图

		System.Windows.Forms.Control control;

		public RenderTargetAdapter(System.Drawing.Bitmap bitmap,System.Windows.Forms.Control control)
		{
			this.bitmap = bitmap;
			this.control = control;
		}

		public void SetRenderBuffer(IRenderTarget renderBuffer)
		{
			this.renderBuffer = renderBuffer;


			UpdateBitmap();

		}


		byte[] bytes;
		int stride;
		private void UpdateBitmap()
		{
			var bitmapdata = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, renderBuffer.width, renderBuffer.height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			if (bytes == null)
			{
				stride = Math.Abs(bitmapdata.Stride);
				bytes = new byte[ stride * bitmapdata.Height];
			}

			for (int i = 0; i < renderBuffer.width; i++)
			{
				for (int j = 0; j < renderBuffer.height; j++)
				{
					float4 color = renderBuffer.getPixel(i, j);

					int destidx = j * stride + i * 4;

					bytes[destidx + 0] =  floatToByte(color.b);
					bytes[destidx + 1] =  floatToByte(color.g);
					bytes[destidx + 2] =  floatToByte(color.r);
					bytes[destidx + 3] =  floatToByte(color.a);

				}
			}




			System.Runtime.InteropServices.Marshal.Copy( bytes,0,bitmapdata.Scan0,bytes.Length );
			bitmap.UnlockBits(bitmapdata);
			control.Invalidate();

		}

		private byte floatToByte(float v)
		{
			int bv = (int)(v * 255);

			if (bv < 0)
				return 0;
			else if (bv > 255)
				return 255;
			else
				return (byte)bv;

		}

		
	}
}
