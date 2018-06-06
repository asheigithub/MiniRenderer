using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender.textures
{
	class MipLevel
	{
		public int level;

		public int width;
		public int height;
		

		public float4[][] texuteData;

		public override string ToString()
		{
			return string.Format("level {0}:({1},{2})", level, width, height);
		}


		public static MipLevel[] calMipLevels(int width,int height)
		{
			List<MipLevel> mipLevels = new List<MipLevel>();

			mipLevels.Add(new MipLevel() { level = 0, width = width, height = height });

			while (true)
			{
				if (width == 1 && height == 1)
				{
					break;
				}


				width = width >> 1;
				height = height >> 1;

				if (width < 1) width = 1;
				if (height < 1) height = 1;
				mipLevels.Add(new MipLevel() { level = 0, width = width, height = height });




			}

			return mipLevels.ToArray();
		}


	}
}
