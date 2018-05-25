using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	/// <summary>
	/// Context3D 类提供了用于呈现几何定义图形的上下文。 
	/// </summary>
	public class Context3D
	{

		public int backBufferWidth { get; private set; }

		public int backBufferHeight { get; private set; }

		public int driverInfo { get; private set; }



		private ScreenFrameBuffer backbuffer;

		private ProgramConstants programConstants;


		/// <summary>
		/// 设置渲染缓冲区的视口尺寸和其他属性。 
		/// 渲染是双缓冲的。当调用 present() 方法时，后台缓冲区与可见的前台缓冲区交换。
		/// 缓冲区的最小大小为 32 x 32 像素。
		/// 配置缓冲区是一个缓慢的操作。在正常渲染操作期间，请避免更改缓冲区大小或属性。
		/// 
		/// </summary>
		public void configureBackBuffer(int width,int height,IRenderTargetAdapter renderBufferAdapter)
		{
			if (width < 32 || height < 32 || width % 2 != 0 || height % 2 != 0)
			{
				throw new ArgumentException("缓冲区的最小大小为 32 x 32 像素,并且必须是偶数");
			}

			this.programConstants = new ProgramConstants();
			this.renderBufferAdapter = renderBufferAdapter;

			backBufferWidth = width;
			backBufferHeight = height;
			backbuffer = new ScreenFrameBuffer(width,height ,1,1);


			currentRenderBuffer = backbuffer;
			needclearflag = true;
		}


		/// <summary>
		/// 创建 Program3D 对象。
		/// </summary>
		/// <returns></returns>
		public Program3D createProgram()
		{
			return new Program3D();
		}

		private Program3D current_program3D;
		/// <summary>
		/// 设置用于后续渲染的顶点和片段着色器程序。 
		/// </summary>
		/// <param name="program"></param>
		public void setProgram(Program3D program)
		{
			current_program3D = program;
		}

		public void setProgramConstantsFromMatrix()
		{
			programConstants.registers[0] = new float4(1, 0, 0, -0.3);
			programConstants.registers[1] = new float4(0, 1, 0, -0.2);
			programConstants.registers[2] = new float4(0, 0, 1, 0);
			programConstants.registers[3] = new float4(0, 0, 0, 1);
		}


		public void clear(float red=0,float green=0,float blue=0,float alpha=1,float depth =0,byte stencil=0,uint mask = Context3DClearMask.ALL)
		{
			needclearflag = false;
			var pixels = currentRenderBuffer.getRealPixels();
			for (int i = 0; i < currentRenderBuffer.rt_width; i++)
			{
				for (int j = 0; j < currentRenderBuffer.rt_height; j++)
				{
					var color = pixels[i][j];
					color.r = Mathf.clamp01( red);
					color.g = Mathf.clamp01( green);
					color.b = Mathf.clamp01( blue);
					color.a = Mathf.clamp01( alpha);
					pixels[i][j] = color;
				}
			}
		}
		private bool needclearflag;



		public VertexBuffer3D createVertexBuffer(int numVertices)
		{
			return new VertexBuffer3D(numVertices);
		}


		public IndexBuffer3D createIndexBuffer(int numIndices)
		{
			return new IndexBuffer3D(numIndices);
		}

		public void bindVertexBuffer(VertexBuffer3D vertexBuffer3D)
		{
			currentVertexBuffer = vertexBuffer3D;
		}

		private IRenderTargetAdapter renderBufferAdapter;
		private IRenderTarget currentRenderBuffer;
		public void present()
		{
			if (needclearflag)
			{
				clear(0.5f, 1);
				needclearflag = false;
			}

			//***最后，重置渲染目标****
			currentRenderBuffer = backbuffer;
			renderBufferAdapter.SetRenderBuffer(backbuffer);
		}

		private Rasterizer.Raster[] rasters;
		public void drawTriangles(IndexBuffer3D indexBuffer,int firstIndex = 0,int numTriangles= -1)
		{
			if (currentVertexBuffer == null)
				throw new InvalidOperationException("VertexBuffer无效");

			if (current_program3D == null)
				throw new InvalidOperationException("尚未设置着色器");

			if (!current_program3D.Validate())
				throw new InvalidOperationException("着色器校验失败");


			if (numTriangles == -1)
				numTriangles = (indexBuffer.data.Count-firstIndex) / 3;


			if (rasters == null || rasters.Length < currentRenderBuffer.rt_width * currentRenderBuffer.rt_height)
			{
				rasters = new Rasterizer.Raster[currentRenderBuffer.rt_width * currentRenderBuffer.rt_height];
				for (int i = 0; i < rasters.Length; i++)
				{
					rasters[i] = new Rasterizer.Raster();
				}
			}

			current_program3D.vertexShader.constants = programConstants;

			for (int i = 0; i < numTriangles; i++)
			{
				if (firstIndex + i * 3 > indexBuffer.data.Count - 3 )
				{
					throw new ArgumentException("index数量不足");
				}

				uint idx1 = indexBuffer.data[firstIndex + i*3];
				uint idx2 = indexBuffer.data[firstIndex + i*3+1];
				uint idx3 = indexBuffer.data[firstIndex + i * 3 + 2];

				#region 顶点着色器运算
				v2f v1 ;
				v2f v2 ;
				v2f v3 ;

				var vs = current_program3D.vertexShader;
				{
					vs.appdata = currentVertexBuffer.vertices[(int)idx1];
					v1=vs.Execute();
				}
				{
					vs.appdata = currentVertexBuffer.vertices[(int)idx2];
					v2=vs.Execute();
				}
				{
					vs.appdata = currentVertexBuffer.vertices[(int)idx3];
					v3=vs.Execute();
				}

				#endregion


				float area = Rasterizer.TriangleDoubleArea(v1.SV_POSITION.xy, v2.SV_POSITION.xy, v3.SV_POSITION.xy);



				//int totalraises;
				//Rasterizer.Triangle(currentRenderBuffer,
				//	v1,v2,v3
				//	,rasters
				//	,out totalraises
				//	);

				//var pixels= currentRenderBuffer.getRealPixels();
				//for (int j = 0; j < totalraises; j++)
				//{
				//	var r = rasters[j];

				//	float4 oc = pixels[r.x][r.y];
				//	float4 color = r.vsout.color;

				//	float4 final = oc * (1 - color.a) + color * color.a;
				//	pixels[r.x][r.y] = final;

				//}

				#region 线框

				Rasterizer.Line(currentRenderBuffer,
					v1.SV_POSITION.xy, v2.SV_POSITION.xy, new float4(1, 1, 1, 1));

				Rasterizer.Line(currentRenderBuffer,
					v2.SV_POSITION.xy, v3.SV_POSITION.xy, new float4(1, 1, 1, 1));

				Rasterizer.Line(currentRenderBuffer,
					v3.SV_POSITION.xy, v1.SV_POSITION.xy, new float4(1, 1, 1, 1));

				#endregion

			}


		}

		private VertexBuffer3D currentVertexBuffer;


	}
}
