using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniRender.textures;

namespace MiniRender
{
	/// <summary>
	/// 纹理采样器
	/// </summary>
	class Sampler
	{
		public Sampler()
		{
			wrap = Context3DWrapMode.CLAMP;
			filter = Context3DTextureFilter.LINEAR;
			mipFilter = Context3DMipFilter.MIPNEAREST;

			texture = null;
			mode = 0;
		}


		public float4 getData(float3 uvw,FragementUnit unit,float lodbias=0)
		{
			
			var miplevels = texture.getMipLevels();

			if (mipFilter == Context3DMipFilter.MIPNONE )
			{
				return sampleTex(miplevels[0], uvw, unit);
			}
			else if (mipFilter == Context3DMipFilter.MIPNEAREST)
			{
				float3 dxuv = (unit.dpdx_v2.input.uv - unit.dpdx_v1.input.uv) * new float3(miplevels[0].width, miplevels[0].height, 1);
				float3 dyuv = (unit.dpdy_v2.input.uv - unit.dpdy_v1.input.uv) * new float3(miplevels[0].width, miplevels[0].height, 1);

				float len1 = dxuv.x * dxuv.x + dxuv.y * dxuv.y;
				float len2 = dyuv.x * dyuv.x + dyuv.y * dyuv.y;

				float l = Mathf.max(len1, len2);
				float p = Mathf.sqrt(l);
				float d = (float)Math.Log(p, 2) + lodbias;

				int level = (int)Mathf.floor(Mathf.clamp(d + 0.5f, 0, texture.maxMipLevel));


				//return new float4((level+1.0f)/(texture.maxMipLevel+1), (level + 1.0f) / (texture.maxMipLevel + 1), (level + 1.0f) / (texture.maxMipLevel + 1), 1);

				return sampleTex(miplevels[level], uvw, unit);

			}
			else
			{



				float3 dxuv = (unit.dpdx_v2.input.uv - unit.dpdx_v1.input.uv) * new float3(miplevels[0].width, miplevels[0].height, 1);
				float3 dyuv = (unit.dpdy_v2.input.uv - unit.dpdy_v1.input.uv) * new float3(miplevels[0].width, miplevels[0].height, 1);

				float len1 = dxuv.x * dxuv.x + dxuv.y * dxuv.y;
				float len2 = dyuv.x * dyuv.x + dyuv.y * dyuv.y;

				float l = Mathf.max(len1, len2);
				float p = Mathf.sqrt(l);
				float d = (float)Math.Log(p, 2)+lodbias;


				//if (filter == Context3DTextureFilter.ANISOTROPIC)
				//	d -= 1;

				float level = Mathf.clamp(d, 0, texture.maxMipLevel);

				

				//return new float4( level * 1.0f / texture.maxMipLevel ,level * 1.0f / texture.maxMipLevel, level * 1.0f / texture.maxMipLevel, 1 );

				int l1 = (int)Mathf.floor(level);
				int l2 = l1 + 1;
				if (l2 > texture.maxMipLevel)
					l2 = texture.maxMipLevel;

				float a = Shader.frac(level);

				float4 c1 = sampleTex(miplevels[l1], uvw, unit);
				float4 c2 = sampleTex(miplevels[l2], uvw, unit);

				//c1.r = dxuv.x;
				//c1.g = dyuv.x;
				//c2.r = dxuv.x;
				//c2.g = dyuv.x;

				return c1 * (1-a) + c2 * ( a);
			}
			






			

		}


		private float4 sampleTex(MipLevel level,float3 uvw,FragementUnit unit)
		{
			switch (filter)
			{
				case Context3DTextureFilter.NEAREST:
					//break;				
					{
						return readpixel(uvw, level);
					}
					
				case Context3DTextureFilter.LINEAR:
					{
						return readliner(uvw, level);
					}
				case Context3DTextureFilter.ANISOTROPIC:
					{
						
						var quadunit = unit.quadunit;
						var center = (quadunit.unit0.input.uv + quadunit.unit1.input.uv + quadunit.unit2.input.uv + quadunit.unit3.input.uv) * 0.25f;
						var t_center = (quadunit.unit0.input.uv + quadunit.unit1.input.uv) * 0.5f;
						var b_center = (quadunit.unit2.input.uv + quadunit.unit3.input.uv) * 0.5f;
						var l_center = (quadunit.unit0.input.uv + quadunit.unit2.input.uv) * 0.5f;
						var r_center = (quadunit.unit1.input.uv + quadunit.unit3.input.uv) * 0.5f;

						float3 p0, p1, p2, p3;
						if (unit.index == 0)
						{
							p0 = unit.input.uv-(center-unit.input.uv);
							p1 = t_center-(center-t_center);
							p2 = l_center-(center-l_center);
							p3 = center;
						}
						else if (unit.index == 1)
						{
							p0 = t_center-(center-t_center);
							p1 = unit.input.uv - (center - unit.input.uv);
							p2 = center;
							p3 = r_center -(center-r_center) ;
						}
						else if (unit.index == 2)
						{
							p0 = l_center-(center-l_center);
							p1 = center;
							p2 = unit.input.uv - (center - unit.input.uv);
							p3 = b_center - (center-b_center);
						}
						else
						{
							p0 = center;
							p1 = r_center - (center - r_center);
							p2 = b_center - (center - b_center);
							p3 = unit.input.uv - (center - unit.input.uv);
						}

						//float3 lp0, lp1, lp2, lp3;
						//lp0 = (p0 + p1) * 0.5f;
						//lp1 = (p1 + p2) * 0.5f;
						//lp2 = (p2 + p3) * 0.5f;
						//lp3 = (p3 + p0) * 0.5f;


						return (readliner(p0, level) + readliner(p1, level) + readliner(p2, level) + readliner(p3, level)
							+ readliner(unit.input.uv, level)
							//+ readliner(lp0, level)
							//+ readliner(lp1, level)
							//+ readliner(lp2, level)
							//+ readliner(lp3, level)


							) * 0.2f;


						//float3 dxuv = (unit.dpdx_v2.input.uv - unit.dpdx_v1.input.uv);
						//float3 dyuv = (unit.dpdy_v2.input.uv - unit.dpdy_v1.input.uv);

						//float l1 =  dxuv.x * dxuv.x + dxuv.y * dxuv.y;
						//float l2 =  dyuv.x * dyuv.x + dyuv.y * dyuv.y;

						//float3 duv = (dxuv * l1 + dyuv * l2) / (l1 + l2);


						//float4 c = new float4();

						//float3 st = uvw - new float3(-duv.x, -duv.y, uvw.z) * 0.5;
						//float3 ed = uvw + new float3(duv.x, duv.y, uvw.z) * 0.5;

						//float stepx = (ed.x - st.x);
						//float stepy = (ed.y - st.y);

						//for (int i = 0; i < 2; i++)
						//{
						//	for (int j = 0; j < 2; j++)
						//	{
						//		float3 uvpos = uvw + new float3(i * stepx, j * stepy, 0);
						//		c += readliner(uvpos, level);
						//	}
						//}

						//return c * 1f / 4;


					}
					throw new NotImplementedException();
				default:

					throw new NotImplementedException();
			}
		}

		private float4 readpixel(float3 uvw, MipLevel level)
		{
			float2 uv1 = warp(uvw.xy * new float2(level.width, level.height) + 0.5, level);
			int i0 = (int)Mathf.floor(uv1.x) % level.width;
			int j0 = (int)Mathf.floor(uv1.y) % level.height;
			return level.texuteData[i0][j0];
		}

		private float4 readliner(float3 uvw,MipLevel level)
		{
			float2 uv = uvw.xy * new float2(level.width, level.height);
			float2 uv1 = warp(uv - 0.5, level);
			float2 uv2 = warp(uv + 0.5, level);

			int i0 = (int)Mathf.floor(uv1.x) % level.width;
			int j0 = (int)Mathf.floor(uv1.y) % level.height;

			int i1 = (int)Mathf.floor(uv2.x) % level.width;
			int j1 = (int)Mathf.floor(uv2.y) % level.height;

			float alpha = Shader.frac(uv.x - 0.5f);
			float beta = Shader.frac(uv.y - 0.5f);

			float4 Ti0j0 = level.texuteData[i0][j0];
			float4 Ti1j0 = level.texuteData[i1][j0];
			float4 Ti0j1 = level.texuteData[i0][j1];
			float4 Ti1j1 = level.texuteData[i1][j1];


			return (1 - alpha) * (1 - beta) * Ti0j0
					+ alpha * (1 - beta) * Ti1j0
					+ (1 - alpha) * beta * Ti0j1 + alpha * beta * Ti1j1;
		}


		private float2 warp(float2 uv,MipLevel level)
		{
			
			switch (wrap)
			{
				case Context3DWrapMode.CLAMP:
					{
						return Shader.clamp(uv, new float2(0,0), new float2( level.width-1,level.height-1 ));
					}
				case Context3DWrapMode.CLAMP_U_REPEAT_V:
					{
						float v = uv.y % level.height;if (v < 0) v += level.height;
						float u = Shader.clamp(uv.x, 0, level.width - 1);
						return new float2(u, v);
					}
				case Context3DWrapMode.REPEAT:
					{
						float u = uv.x % level.width; if (u < 0) u += level.width;
						float v = uv.y % level.height; if (v < 0) v += level.height;

						return new float2(u, v); 
					}
				case Context3DWrapMode.REPEAT_U_CLAMP_V:
					{
						float u = uv.x % level.width; if (u< 0) u += level.width;
						float v = Shader.clamp(uv.y, 0, level.width - 1);
						return new float2(u, v);
					}
				default:
					throw new NotImplementedException();
			}


		}



		private textures.TextureBase _texture;

		/// <summary>
		/// 0--2D
		/// 1--Cube
		/// 2--3D
		/// </summary>
		private int mode;

		public Context3DWrapMode wrap;

		public Context3DTextureFilter filter;

		public Context3DMipFilter mipFilter;


		public TextureBase texture {
			get { return _texture; }
			set {
				_texture = value;
				if (value is textures.Texture || value == null)
				{
					mode = 0;
				}
				else
				{
					//***
				}
			}
		}
	}


	public enum Context3DWrapMode
	{
		/// <summary>
		/// 将纹理坐标锁定在 0..1 范围之外。
		/// </summary>
		CLAMP,
		/// <summary>
		/// 固定于 U 轴，但在 V 轴重复。
		/// </summary>
		CLAMP_U_REPEAT_V,
		/// <summary>
		/// 重复（平铺）0..1 范围之外的纹理坐标。
		/// </summary>
		REPEAT,
		/// <summary>
		/// 在 U 轴重复，但固定于 V 轴。
		/// </summary>
		REPEAT_U_CLAMP_V
	}


	public enum Context3DTextureFilter
	{
		/// <summary>
		/// 对纹理进行向上取样时，使用最接近的相邻采样，这样可产生像素化的锐化马赛克效果。
		/// </summary>
		NEAREST,
		/// <summary>
		/// 对纹理进行向上取样时，使用线性插值，这样可产生平滑的模糊效果。 
		/// </summary>
		LINEAR,
		/// <summary>
		/// 各项异性过滤。。
		/// </summary>
		ANISOTROPIC
	}

	/// <summary>
	/// 对用于取样器 mipmap 滤镜模式的值进行定义 
	/// </summary>
	public enum Context3DMipFilter
	{		
		MIPNONE,
		MIPNEAREST,
		MIPLINEAR
	}

}
