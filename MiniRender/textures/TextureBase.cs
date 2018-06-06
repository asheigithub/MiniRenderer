using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender.textures
{
	/// <summary>
	/// The TextureBase class is the base class for Context3D texture objects. 
	///	Note: You cannot create your own texture classes using TextureBase. To add functionality to a texture class, extend either Texture or CubeTexture instead.
	/// </summary>
	public abstract class TextureBase
	{

		internal abstract MipLevel[] getMipLevels();


		internal abstract int maxMipLevel { get; }
	}
}
