﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	/// <summary>
	/// 光栅化器
	/// </summary>
	class Rasterizer
	{
		/// <summary>
		/// 三角形的一条边
		/// </summary>
		class TriangleEdge
		{
			//public v2f point1;
			//public v2f point2;

			public float2 rtpos1;
			public float2 rtpos2;

			public float2 anotherrtpos;


			public bool istopedge;
			public bool isleftedge;


			public void checkedgeattribue()
			{
				if (rtpos1.y == rtpos2.y)
				{
					if (anotherrtpos.y > rtpos1.y)
					{
						istopedge = true;
					}
				}
				else
				{
					//确定是否左侧边
					var top = rtpos1;
					var bottom = rtpos2;

					if (rtpos1.y > rtpos2.y) //先确定边的顺序
					{
						top = rtpos2;
						bottom = rtpos1;
					}

					//然后求叉积判断对面点是否在右侧
					float area = (top.x - bottom.x) * (anotherrtpos.y - bottom.y) - (anotherrtpos.x - bottom.x) * (top.y - bottom.y);
					if (area > 0)
					{
						isleftedge = true;
					}
				}

			}

		}


		/*
		 * 正规化设备坐标（NDC, Normalized Device Coordinate)
			                              -1,1 ---- 1,1 
			                               |         |
                                          -1,-1 ----1,-1
		*/

		/*
		 Render Target
		 由一个二维矩形区域和一维的z范围构成，表示要显示渲染输出的区域，它是NDC立方体所映射的区域。
		 例如RenderTarget大小为64*64，当设置Viewport左上角为(32,32)，大小为32*32时，
		 我们将在Render Target的右下角1/4的区域画图。
		*/



		private static float2 toRenderTargetPos(IRenderTarget buffer, float2 pos)
		{
			float fx = pos.x * 0.5f + 0.5f;
			float fy = 0.5f - pos.y * 0.5f;

			//float fy = pos.y * 0.5f+0.5f;

			return new float2((buffer.rt_width) * fx , (buffer.rt_height) * fy);
		}

		/// <summary>
		/// 叉乘求三角形面积。
		/// </summary>
		/// <param name="p0"></param>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <returns></returns>
		public static float TriangleDoubleArea(float2 p0,float2 p1,float2 p2)
		{
			return (p1.x - p0.x) * (p2.y - p0.y) 
				- (p2.x - p0.x) * (p1.y - p0.y);
		}

		public static void Triangle(IRenderTarget renderBuffer, v2f p1,v2f p2,v2f p3 ,Raster[] rasters , out int totalrasters )
		{
			/*
			 D3D将点按z字形分成两个三角形做光栅化，而三角形的光栅化遵守TOP-LEFT规则：
			在屏幕上，若某条边位于三角形的左侧，则这条边称为LEFT边；若某条边是平行边，且位于三角形的上侧，则这条边称为TOP边。简单地讲，LEFT边是“左侧的边”，TOP边是“上面的平行边”。
			D3D规定：
			（1）如果一个像素中心刚好落在三角形的一条边上，则仅当这条边为TOP或LEFT边时才画该像素；
			（2）如果一个像素中心刚好落在三角形两条边的交点，则仅当两条边分别为TOP和LEFT边时才画该像素。
			TOP-LEFT规则保证了当两个三角形有重合边时，像素不会被重复渲染。对于拆分成两个三角形的点，则保证了一个顶点只覆盖一个像素。
			 */
			totalrasters = 0;

			float2 pos1 = toRenderTargetPos(renderBuffer, p1.SV_POSITION.xy/p1.SV_POSITION.w);
			float2 pos2 = toRenderTargetPos(renderBuffer, p2.SV_POSITION.xy/p2.SV_POSITION.w);
			float2 pos3 = toRenderTargetPos(renderBuffer, p3.SV_POSITION.xy/p3.SV_POSITION.w);

			float area = TriangleDoubleArea(pos1, pos2, pos3);
			if (area == 0)
				return;


			//****确定矩形区域****
			int top = (int)Math.Floor( Mathf.min( Mathf.min(pos1.y, pos2.y),pos3.y));
			int bottom = (int)Math.Ceiling( Mathf.max(Mathf.max(pos1.y, pos2.y), pos3.y));
			int left = (int)Math.Floor(Mathf.min(Mathf.min(pos1.x, pos2.x), pos3.x));
			int right= (int)Math.Ceiling(Mathf.max(Mathf.max(pos1.x, pos2.x), pos3.x));

			TriangleEdge edge1 = new TriangleEdge() {  rtpos1 = pos1, rtpos2 = pos2, anotherrtpos = pos3 };
			TriangleEdge edge2 = new TriangleEdge() {  rtpos1 = pos2, rtpos2 = pos3, anotherrtpos = pos1 };
			TriangleEdge edge3 = new TriangleEdge() {  rtpos1 = pos3, rtpos2 = pos1, anotherrtpos = pos2 };

            //如果用定义判断，需要处理顶边和左边，这里不用定义判断，所以不需要执行了
			//edge1.checkedgeattribue();
			//edge2.checkedgeattribue();
			//edge3.checkedgeattribue();


			if (left < 0) left = 0;
			if (top < 0) top = 0;

			//***将矩形区域扩展成2的倍数
			if (left % 2 == 1)
				left--;
			if (right % 2 == 1)
				right++;
			if (top % 2 == 1)
				top--;
			if (bottom % 2 == 1)
				bottom++;

			
			
			for (int i = left; i < right; i+=2)
			{
				if (i < 0 || i >= renderBuffer.rt_width)
					continue;

				for (int j = top; j < bottom; j+=2)
				{
					if (j < 0 || j >= renderBuffer.rt_height)
						continue;

					float2 pixelpositon = new float2( i+0.5f,j+0.5f );

					//***检测相邻的四个像素。片段着色器阶段四个相邻像素Z形执行

					float[] A = new float[4];
					float[] B = new float[4];
					float[] C = new float[4];
					bool[] r_pass = new bool[4];

					r_pass[0] = need_rasterize(edge1, edge2, edge3, new float2(pixelpositon.x, pixelpositon.y), area, out A[0], out B[0], out C[0]);
					r_pass[1] = need_rasterize(edge1, edge2, edge3, new float2(pixelpositon.x + 1, pixelpositon.y ), area, out A[1], out B[1], out C[1]);
					r_pass[2] = need_rasterize(edge1, edge2, edge3, new float2(pixelpositon.x, pixelpositon.y+1), area, out A[2], out B[2], out C[2]);
					r_pass[3] = need_rasterize(edge1, edge2, edge3, new float2(pixelpositon.x + 1, pixelpositon.y+1), area, out A[3], out B[3], out C[3]);

					if (!(r_pass[0] || r_pass[1] || r_pass[2] || r_pass[3]))
					{
						continue;
					}

					for (int jj = 0; jj < 2; jj++)
					{
						for (int ii = 0; ii < 2; ii++)
						{
							int idx = jj * 2 + ii;

							//重心坐标系
							float a = A[idx] / area;
							float b = B[idx] / area;
							float c = C[idx] / area;

							//返回光栅化结果

							var r = rasters[totalrasters];
							r.isclippass = false;
							r.rasterize = r_pass[idx];
							r.x = i+ii;
							r.y = j+jj;

							v2f v2f = new v2f();
							interpolation(ref v2f, ref p1, ref p2, ref p3, a, b, c);

							r.vsout = v2f;

							totalrasters++;



						}
					}

				}
			}
		}

		private static void interpolation(ref v2f v2f,ref v2f p1,ref v2f p2,ref v2f p3,float a,float b,float c)
		{
			v2f.SV_POSITION = (p1.SV_POSITION * a / p1.SV_POSITION.w +
												p2.SV_POSITION * b / p2.SV_POSITION.w +
												p3.SV_POSITION * c / p3.SV_POSITION.w)
												/
												(
												a / p1.SV_POSITION.w + b / p2.SV_POSITION.w + c / p3.SV_POSITION.w
												)
												;

			v2f.objPos = (p1.objPos * a / p1.SV_POSITION.w +
								p2.objPos * b / p2.SV_POSITION.w +
								p3.objPos * c / p3.SV_POSITION.w)
								/
								(
								a / p1.SV_POSITION.w + b / p2.SV_POSITION.w + c / p3.SV_POSITION.w
								)
								;

			v2f.worldPos = (p1.worldPos * a / p1.SV_POSITION.w +
								p2.worldPos * b / p2.SV_POSITION.w +
								p3.worldPos * c / p3.SV_POSITION.w)
								/
								(
								a / p1.SV_POSITION.w + b / p2.SV_POSITION.w + c / p3.SV_POSITION.w
								)
								;

			v2f.color = (p1.color * a / p1.SV_POSITION.w +
								p2.color * b / p2.SV_POSITION.w +
								p3.color * c / p3.SV_POSITION.w)
								/
								(
								a / p1.SV_POSITION.w + b / p2.SV_POSITION.w + c / p3.SV_POSITION.w
								)
								;

			v2f.uv = (p1.uv * a / p1.SV_POSITION.w +
								p2.uv * b / p2.SV_POSITION.w +
								p3.uv * c / p3.SV_POSITION.w)
								/
								(
								a / p1.SV_POSITION.w + b / p2.SV_POSITION.w + c / p3.SV_POSITION.w
								)
								;

			v2f.worldNormal = (p1.worldNormal * a / p1.SV_POSITION.w +
								p2.worldNormal * b / p2.SV_POSITION.w +
								p3.worldNormal * c / p3.SV_POSITION.w)
								/
								(
								a / p1.SV_POSITION.w + b / p2.SV_POSITION.w + c / p3.SV_POSITION.w
								)
								;

			v2f.objNormal = (p1.objNormal * a / p1.SV_POSITION.w +
								p2.objNormal * b / p2.SV_POSITION.w +
								p3.objNormal * c / p3.SV_POSITION.w)
								/
								(
								a / p1.SV_POSITION.w + b / p2.SV_POSITION.w + c / p3.SV_POSITION.w
								)
								;



			v2f.worldTangent = (p1.worldTangent * a / p1.SV_POSITION.w +
								p2.worldTangent * b / p2.SV_POSITION.w +
								p3.worldTangent * c / p3.SV_POSITION.w)
								/
								(
								a / p1.SV_POSITION.w + b / p2.SV_POSITION.w + c / p3.SV_POSITION.w
								)
								;


			v2f.tSpace0 = (p1.tSpace0 * a / p1.SV_POSITION.w +
								p2.tSpace0 * b / p2.SV_POSITION.w +
								p3.tSpace0 * c / p3.SV_POSITION.w)
								/
								(
								a / p1.SV_POSITION.w + b / p2.SV_POSITION.w + c / p3.SV_POSITION.w
								)
								;
			v2f.tSpace1 = (p1.tSpace1 * a / p1.SV_POSITION.w +
								p2.tSpace1 * b / p2.SV_POSITION.w +
								p3.tSpace1 * c / p3.SV_POSITION.w)
								/
								(
								a / p1.SV_POSITION.w + b / p2.SV_POSITION.w + c / p3.SV_POSITION.w
								)
								;
			v2f.tSpace2 = (p1.tSpace2 * a / p1.SV_POSITION.w +
								p2.tSpace2 * b / p2.SV_POSITION.w +
								p3.tSpace2 * c / p3.SV_POSITION.w)
								/
								(
								a / p1.SV_POSITION.w + b / p2.SV_POSITION.w + c / p3.SV_POSITION.w
								)
								;



		}

        private static bool need_rasterize(float2 ap1,float2 ap2,float2 ap3,float2 pixelposition)
        {
            
            //float2 p1 = new float2((int)(ap1.x + 0.5f), (int)(ap1.y + 0.5f));
            //float2 p2 = new float2((int)(ap2.x + 0.5f), (int)(ap2.y + 0.5f));
            //float2 p3 = new float2((int)(ap3.x + 0.5f), (int)(ap3.y + 0.5f));
            float2 p1 = ap1;
            float2 p2 = ap2;
            float2 p3 = ap3;

            if (p1.y == p2.y && p1.y == p3.y)
                return false;

            float2[] three = new float2[3];
            three[0] = p1;
            three[1] = p2;
            three[2] = p3;
            #region 按y排序
            if (three[1].y < three[0].y)
            {
                var temp = three[0];
                three[0] = three[1];
                three[1] = temp;
            }
            if (three[2].y < three[0].y)
            {
                var temp = three[0];
                three[0] = three[2];
                three[2] = temp;
            }

            if (three[2].y < three[1].y)
            {
                var temp = three[1];
                three[1] = three[2];
                three[2] = temp;
            }
            #endregion

            float px = pixelposition.x;
            float py = pixelposition.y;

            float dx = 0.0000001f;                                  //float为7位有效数字
            float tx = px; while (tx > 1) { tx /= 10; dx *= 10; } //动态调节浮点数有效位
            


            if (pixelposition.y < three[1].y)
            {
                //上三角
                //int sty = (int)Math.Floor(three[0].y );
                //int edy = (int)Math.Floor(three[1].y );

                float sty = three[0].y;
                float edy = three[1].y;

                if (py < sty || py >= edy)
                    return false;

                float x1 = three[0].x + (three[1].x - three[0].x) * (py - three[0].y) / (three[1].y - three[0].y);
                float x2 = three[0].x + (three[2].x - three[0].x) * (py - three[0].y) / (three[2].y - three[0].y);

                //int ix1 = (int)Math.Floor(x1);
                //int ix2 = (int)Math.Floor(x2);

                float ix1 = x1;
                float ix2 = x2;


                if (ix1 > ix2)
                {
                    var temp = ix1;
                    ix1 = ix2;
                    ix2 = temp;
                }

                if (px < ix1-dx|| px >= ix2+dx)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                //下三角

                //int sty = (int)Math.Floor(three[1].y );
                //int edy = (int)Math.Floor(three[2].y );

                float sty = three[1].y;
                float edy = three[2].y;

                if (py < sty || py >= edy)
                    return false;

                float x1 = three[2].x + (three[1].x - three[2].x) * (py - three[2].y) / (three[1].y - three[2].y);
                float x2 = three[2].x + (three[0].x - three[2].x) * (py - three[2].y) / (three[0].y - three[2].y);

                //int ix1 = (int)Math.Floor(x1);
                //int ix2 = (int)Math.Floor(x2);

                float ix1 = x1;
                float ix2 = x2;

                if (ix1 > ix2)
                {
                    var temp = ix1;
                    ix1 = ix2;
                    ix2 = temp;
                }

                //if (px < ix1 || px >= ix2)
                if( px <ix1 -dx  || px >=ix2 + dx)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }

            
        }


		private static bool need_rasterize(TriangleEdge edge1, TriangleEdge edge2, TriangleEdge edge3,float2 pixelpositon,float area ,out float A,out float B,out float C )
		{
			A = TriangleDoubleArea(edge2.rtpos1, edge2.rtpos2, pixelpositon);
			B = TriangleDoubleArea(edge3.rtpos1, edge3.rtpos2, pixelpositon);
			C = TriangleDoubleArea(edge1.rtpos1, edge1.rtpos2, pixelpositon);

            return need_rasterize(edge1.rtpos1, edge1.rtpos2, edge1.anotherrtpos,pixelpositon);

            //按照定义判断，会由于浮点数出现错误的点和缝隙

			if (area * A < 0 || area * B < 0 || area * C < 0)
			{
				//不在三角形内
				return false;
			}
            //应用TOP-LEFT规则
            
			if (A == 0)
			{
				if (B == 0)
				{
					if (!((edge2.istopedge && edge3.isleftedge) || (edge2.isleftedge && edge3.istopedge)))
					{
						return false;
					}
				}
				else if (C == 0)
				{
					if (!((edge2.istopedge && edge1.isleftedge) || (edge2.isleftedge && edge1.istopedge)))
					{
						return false;
					}
				}
				else
				{
					if (!(edge2.istopedge || edge2.isleftedge))
						return false;
				}
			}
			if (B == 0)
			{
				if (A == 0)
				{
					if (!((edge2.istopedge && edge3.isleftedge) || (edge2.isleftedge && edge3.istopedge)))
					{
						return false;
					}
				}
				else if (C == 0)
				{
					if (!((edge1.istopedge && edge3.isleftedge) || (edge1.isleftedge && edge3.istopedge)))
					{
						return false;
					}
				}
				else
				{
					if (!(edge3.istopedge || edge3.isleftedge))
						return false;
				}
			}
			if (C == 0)
			{
				if (A == 0)
				{
					if (!((edge2.istopedge && edge1.isleftedge) || (edge2.isleftedge && edge1.istopedge)))
					{
						return false;
					}
				}
				else if (B == 0)
				{
					if (!((edge1.istopedge && edge2.isleftedge) || (edge1.isleftedge && edge2.istopedge)))
					{
						return false;
					}
				}
				else
				{
					if (!(edge1.istopedge || edge1.isleftedge))
						return false;
				}
			}

			return true;
		}



		public static void Line( IRenderTarget renderBuffer,float2 stpos,float2 edpos , float4 linecolor )
		{
			///https://blog.csdn.net/patient16/article/details/50194993 将来按此修改

			var p1 = toRenderTargetPos(renderBuffer, stpos);
			var p2 = toRenderTargetPos(renderBuffer, edpos);

			

			var pd = p2 - p1;
			float distance = Mathf.sqrt(pd.x * pd.x + pd.y * pd.y);

			if (distance == 0)
				return;

			float addx = pd.x / distance;
			float addy = pd.y / distance;

			int cd = 0;
			while (cd < distance)
			{
				float2 p = new float2( p1.x + cd * addx,p1.y + cd * addy );

				float left = p.x - 0.5f;
				float top = p.y - 0.5f;

				int x1 = (int)left;
				int y1 = (int)top;

				float area1 = (x1 + 1 - left) * (y1+1-top);
				
				int x2 = x1 + 1;
				int y2 = y1;

				float area2 = ( p.x+0.5f-x2 ) * (y1 + 1 - top);

				int x3 = (int)left;
				int y3 = y1+1;

				float area3 = (x1 + 1 - left) * (p.y + 0.5f - y3);

				int x4 = x1 + 1;
				int y4 = y1 + 1;

				float area4 = (p.x + 0.5f - x4) * (p.y + 0.5f - y4);

				{
					int i = x1;
					int j = y1;

					if (!(i < 0 || i >= renderBuffer.rt_width || j < 0 || j >= renderBuffer.rt_height))
					{
						float4 oc = renderBuffer.getRealPixels()[i][j];
						float4 final = oc * (1 - area1) + linecolor * area1;

						renderBuffer.getRealPixels()[i][j] = final;
					}
				}
				{
					int i = x2;
					int j = y2;

					if (!(i < 0 || i >= renderBuffer.rt_width || j < 0 || j >= renderBuffer.rt_height))
					{
						float4 oc = renderBuffer.getRealPixels()[i][j];
						float4 final = oc * (1 - area2) + linecolor * area2;

						renderBuffer.getRealPixels()[i][j] = final;
					}
				}
				{
					int i = x3;
					int j = y3;

					if (!(i < 0 || i >= renderBuffer.rt_width || j < 0 || j >= renderBuffer.rt_height))
					{
						float4 oc = renderBuffer.getRealPixels()[i][j];
						float4 final = oc * (1 - area3) + linecolor * area3;

						renderBuffer.getRealPixels()[i][j] = final;
					}
				}
				{
					int i = x4;
					int j = y4;

					if (!(i < 0 || i >= renderBuffer.rt_width || j < 0 || j >= renderBuffer.rt_height))
					{
						float4 oc = renderBuffer.getRealPixels()[i][j];
						float4 final = oc * (1 - area4) + linecolor * area4;

						renderBuffer.getRealPixels()[i][j] = final;
					}
				}


				cd++;
			}



		}


		public class Raster
		{
			public int x;
			public int y;
			public v2f vsout;

			//是否通过CVV裁剪
			internal bool isclippass;
			
			//是否需要被光栅化
			internal bool rasterize;
		}

	}
}
