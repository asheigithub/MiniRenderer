using MiniRender.geom;
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


		//private ZBuffer zBuffer;
		//private ScreenFrameBuffer backbuffer;
		private RenderTargetBind backBufferRenderTarget;

		private ProgramConstants programConstants;



		private Context3DTriangleFace _culling;
		private bool _depthtest_depthMask;
		private Context3DCompareMode _depthtest_passCompareMode;



		public debugger.FrameDebugBuffer DebugBuffer { get; private set; }
		public IRenderTargetAdapter debugLayerAdapter;

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
			this.quadFragementUnit = new QuadFragementUnit();
			this.renderBufferAdapter = renderBufferAdapter;

			backBufferWidth = width;
			backBufferHeight = height;


			backBufferRenderTarget = new RenderTargetBind();
			var backbuffer=new ScreenFrameBuffer(width,height ,1,1);
			backBufferRenderTarget.renderTarget = backbuffer;
			backBufferRenderTarget.zBuffer = new ZBuffer(backbuffer.rt_width, backbuffer.rt_height);

			currentRenderTarget = backBufferRenderTarget;
			needclearflag = true;

			_culling = Context3DTriangleFace.NONE;
			_depthtest_depthMask = true;
			_depthtest_passCompareMode = Context3DCompareMode.ALWAYS;


			if (backbuffer.rt_width == width && backbuffer.rt_height == height)
			{
				DebugBuffer = new debugger.FrameDebugBuffer(width, height);
			}
			else
			{
				DebugBuffer = null;
			}
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

		
		public void setProgramConstantsVector4(int index, geom.Vector4 vector4)
		{
			if (index < ProgramConstants.USERDEFINE_STARTIDX )
			{
				throw new ArgumentException("之前行被保留");
			}
			_setProgramConstantsVector4(index,vector4);
		}

		private void _setProgramConstantsVector4(int index, geom.Vector4 vector4)
		{
			programConstants.registers[index] = new float4(vector4.x, vector4.y, vector4.z, vector4.w);
		}

		/// <summary>
		/// 设置预定义矩阵
		/// </summary>
		/// <param name="_ObjectToWorld"></param>
		/// <param name="_WorldToObject"></param>
		/// <param name="_MatrixV"></param>
		/// <param name="_matrix_projection"></param>
		/// <param name="_MatrixVP"></param>
		/// <param name="_MatrixInvV"></param>
		/// <param name="transpose"></param>
		public void setProgramConstants_Matrices(
			geom.Matrix3D _ObjectToWorld,
			geom.Matrix3D _WorldToObject,
			geom.Matrix3D _MatrixV,
			geom.Matrix3D _matrix_projection,
			geom.Matrix3D _MatrixVP,
			geom.Matrix3D _MatrixInvV,

			bool transpose

			)
		{
			_setProgramConstants_Matrix(_ObjectToWorld, ProgramConstants._ObjectToWorld_ROW0, transpose);
			_setProgramConstants_Matrix(_WorldToObject, ProgramConstants._WorldToObject_ROW0, transpose);
			_setProgramConstants_Matrix(_MatrixV, ProgramConstants._MatrixV_ROW0, transpose);
			_setProgramConstants_Matrix(_matrix_projection, ProgramConstants._matrix_projection_ROW0, transpose);
			_setProgramConstants_Matrix(_MatrixVP, ProgramConstants._MatrixVP_ROW0, transpose);
			_setProgramConstants_Matrix(_MatrixInvV, ProgramConstants._MatrixInvV_ROW0, transpose);
		}

		/// <summary>
		/// 设置预定义的参数
		/// </summary>
		public void setProgramVariables(geom.Vector4 WorldSpaceCameraPos)
		{
			_setProgramConstantsVector4(ProgramConstants._WorldSpaceCameraPos_ROW, WorldSpaceCameraPos);

		}


		private void _setProgramConstants_Matrix(geom.Matrix3D matrix3D, int startindex , bool transpose )
		{
			if (transpose)
				matrix3D.transpose();

			programConstants.registers[startindex] = new float4(matrix3D.M00, matrix3D.M01, matrix3D.M02, matrix3D.M03);
			programConstants.registers[startindex + 1] = new float4(matrix3D.M10, matrix3D.M11, matrix3D.M12, matrix3D.M13);
			programConstants.registers[startindex + 2] = new float4(matrix3D.M20, matrix3D.M21, matrix3D.M22, matrix3D.M23);
			programConstants.registers[startindex + 3] = new float4(matrix3D.M30, matrix3D.M31, matrix3D.M32, matrix3D.M33);
		}
		
		public void setProgramConstantsFromMatrix(geom.Matrix3D matrix3D,int startindex , bool transpose)
		{
			if (startindex < ProgramConstants.USERDEFINE_STARTIDX )
			{
				throw new ArgumentException("之前行被保留");
			}

			if (transpose)
			{
				matrix3D.transpose();
			}

			programConstants.registers[startindex] = new float4(matrix3D.M00, matrix3D.M01, matrix3D.M02, matrix3D.M03);
			programConstants.registers[startindex + 1] = new float4(matrix3D.M10, matrix3D.M11, matrix3D.M12, matrix3D.M13);
			programConstants.registers[startindex + 2] = new float4(matrix3D.M20, matrix3D.M21, matrix3D.M22, matrix3D.M23);
			programConstants.registers[startindex + 3] = new float4(matrix3D.M30, matrix3D.M31, matrix3D.M32, matrix3D.M33);
		}


		/// <summary>
		/// 设置三角形剔除模式。 
		/// 可基于其相对于视图平面的方向，提前在呈现管道流程中从场景中排除三角形。如模型外部所示，一致地指定顶点顺序（顺时针或逆时针）以正确剔除。
		/// </summary>
		/// <param name="triangleFaceToCull"></param>
		public void setCulling(Context3DTriangleFace triangleFaceToCull)
		{
			_culling = triangleFaceToCull;
		}

		/// <summary>
		/// 设置用于深度测试的比较类型。 
		///像素着色器程序的源像素输出的深度将与深度缓冲区中的当前值进行比较。如果比较计算结果为 false，则丢弃源像素。
		///如果为 true，则呈现管道中的下一步“印模测试”将处理源像素。此外，只要 depthMask 参数设置为 true，就会使用源像素的深度更新深度缓冲区。
		///设置用于比较源像素和目标像素的深度值的测试。当比较为 true 时，源像素与目标像素合成。将比较运算符按该顺序作为源像素值和目标像素值之间的中缀运算符应用。
		/// </summary>
		/// <param name="depthMask"></param>
		/// <param name="passCompareMode"></param>
		public void setDepthTest(bool depthMask,Context3DCompareMode passCompareMode)
		{
			_depthtest_depthMask = depthMask;
			_depthtest_passCompareMode = passCompareMode;
		}

		public void clear(float red=0,float green=0,float blue=0,float alpha=1,float depth =1,byte stencil=0,uint mask = Context3DClearMask.ALL)
		{
			needclearflag = false;

			var currentRenderBuffer = currentRenderTarget.renderTarget;

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


			depth = Mathf.clamp01(depth);
			currentRenderTarget.zBuffer.clear(depth);


			if (DebugBuffer != null)
			{
				DebugBuffer.Clear();
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
		private RenderTargetBind currentRenderTarget;
		public void present()
		{
			if (needclearflag)
			{
				clear(0.5f, 1);
				needclearflag = false;
			}

			//***最后，重置渲染目标****
			currentRenderTarget = backBufferRenderTarget;
			renderBufferAdapter.SetRenderBuffer(currentRenderTarget.renderTarget);
		}

		private QuadFragementUnit quadFragementUnit;
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


			if (rasters == null || rasters.Length < currentRenderTarget.renderTarget.rt_width * currentRenderTarget.renderTarget.rt_height)
			{
				rasters = new Rasterizer.Raster[currentRenderTarget.renderTarget.rt_width * currentRenderTarget.renderTarget.rt_height];
				for (int i = 0; i < rasters.Length; i++)
				{
					rasters[i] = new Rasterizer.Raster();
				}
			}

			current_program3D.vertexShader.constants = programConstants;
			current_program3D.fragementShader.constants = programConstants;

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

				#region clip and cull
				{
					float3 xyz1 = v1.SV_POSITION.xyz / v1.SV_POSITION.w;
					float3 xyz2 = v2.SV_POSITION.xyz / v2.SV_POSITION.w;
					float3 xyz3 = v3.SV_POSITION.xyz / v3.SV_POSITION.w;

					#region 视景体外裁剪
					{


						if ((xyz1.z > 1 || xyz1.z < 0 || xyz1.x < -1 || xyz1.x > 1 || xyz1.y < -1 || xyz1.y > 1 || double.IsNaN(xyz1.x) || double.IsNaN(xyz1.y) || double.IsNaN(xyz1.z))
							&&
							(xyz2.z > 1 || xyz2.z < 0 || xyz2.x < -1 || xyz2.x > 1 || xyz2.y < -1 || xyz2.y > 1 || double.IsNaN(xyz2.x) || double.IsNaN(xyz2.y) || double.IsNaN(xyz2.z))
							&&
							(xyz3.z > 1 || xyz3.z < 0 || xyz3.x < -1 || xyz3.x > 1 || xyz3.y < -1 || xyz3.y > 1 || double.IsNaN(xyz3.x) || double.IsNaN(xyz3.y) || double.IsNaN(xyz3.z))
							)
						{
							continue;
						}

					}
					#endregion

					//背面剔除
					{
						float area = Rasterizer.TriangleDoubleArea(xyz1.xy, xyz2.xy, xyz3.xy);

						switch (_culling)
						{
							case Context3DTriangleFace.BACK:
								if (area < 0)
									continue;
								break;
							case Context3DTriangleFace.FRONT:
								if (area > 0)
									continue;
								break;
							case Context3DTriangleFace.FRONT_AND_BACK:
								continue;								
							case Context3DTriangleFace.NONE:
								break;
							default:
								continue;
						}

						
					}
					
				}
				#endregion

				int totalraises;
				Rasterizer.Triangle(currentRenderTarget.renderTarget,
					v1, v2, v3
					, rasters
					, out totalraises
					);

				var pixels = currentRenderTarget.renderTarget.getRealPixels();

				for (int j = 0; j < totalraises; j++)
				{
					var r = rasters[j];
					if (r.vsout.SV_POSITION.z < 0 || r.vsout.SV_POSITION.z > r.vsout.SV_POSITION.w || double.IsNaN(r.vsout.SV_POSITION.z))
					{
						//在远近裁剪面外，去掉
						continue;
					}
					r.isclippass = true;
				}

				var zBuffer = currentRenderTarget.zBuffer;
				var currentRenderBuffer = currentRenderTarget.renderTarget;

				for (int j = 0; j < totalraises; j+=4)
				{
					var r0 = rasters[j];
					var r1 = rasters[j + 1];
					var r2 = rasters[j + 2];
					var r3 = rasters[j + 3];

					quadFragementUnit.setQuadUnit(current_program3D.fragementShader, r0.vsout, r1.vsout, r2.vsout, r3.vsout);

					for (int k = 0; k < 4; k++)
					{
						var r = rasters[j + k];
						if (r.isclippass && r.rasterize)
						{							
							var unit = quadFragementUnit[k];

							debugger.FrameDebugData debugData = null;
							if (DebugBuffer !=null && current_program3D.fragementShader.HasDebug)
							{
								debugData = DebugBuffer.buffer[r.x][r.y];
								debugData.inputdata = unit.input;
								debugData.i = r.x;
								debugData.j = r.y;
								debugData.Ready();
							}

							float4 oc = pixels[r.x][r.y];
							current_program3D.fragementShader.Run(unit, debugData);
							float4 color = unit.output;

							if (unit.isdiscard)
							{
								continue;
							}

							#region 深度检测处理
							{
								//float depth = r.vsout.SV_POSITION.z / r.vsout.SV_POSITION.w;
								ushort depth = zBuffer.Convert(r.vsout.SV_POSITION.z / r.vsout.SV_POSITION.w);

								//读出zBuffer
								//var buffer = zBuffer.buffer;
								var di = (int)(r.x * 1.0f * zBuffer.width / currentRenderBuffer.rt_width);
								var dj = (int)(r.y * 1.0f * zBuffer.height / currentRenderBuffer.rt_height);
								//float depth_buffer = buffer[di][dj];
								ushort depth_buffer = zBuffer[di, dj];
								#region 深度检测
								switch (_depthtest_passCompareMode)
								{
									case Context3DCompareMode.ALWAYS:
										break;
									case Context3DCompareMode.EQUAL:
										if (!(depth == depth_buffer))
										{
											continue;
										}
										break;
									case Context3DCompareMode.GREATER:
										if (!(depth > depth_buffer))
										{
											continue;
										}
										break;
									case Context3DCompareMode.GREATER_EQUAL:
										if (!(depth >= depth_buffer))
										{
											continue;
										}
										break;
									case Context3DCompareMode.LESS:
										if (!(depth < depth_buffer))
										{
											continue;
										}
										break;
									case Context3DCompareMode.LESS_EQUAL:
										if (!(depth <= depth_buffer))
										{
											continue;
										}
										break;
									case Context3DCompareMode.NEVER:
										//永远测试不通过
										continue;
									case Context3DCompareMode.NOT_EQUAL:
										if (!(depth != depth_buffer))
										{
											continue;
										}
										break;
									default:
										continue;
								}
								#endregion

								//深度测试通过

								if (_depthtest_depthMask) //需要写深度
								{
									//buffer[di][dj] = depth;
									zBuffer.WriteDepth(di, dj, depth);
								}

							}
							#endregion

							
							//***Blend***
							float4 final = oc * (1 - color.a) + color * color.a;
							pixels[r.x][r.y] = final;

						}

					}

					
				}


				#region 线框

				//var linepos1 = v1.SV_POSITION / v1.SV_POSITION.w;
				//var linepos2 = v2.SV_POSITION / v2.SV_POSITION.w;
				//var linepos3 = v3.SV_POSITION / v3.SV_POSITION.w;

				//Rasterizer.Line(currentRenderBuffer,
				//	linepos1.xy, linepos2.xy, new float4(0, 0, 0, 1));

				//Rasterizer.Line(currentRenderBuffer,
				//	linepos2.xy, linepos3.xy, new float4(0, 0, 0, 1));

				//Rasterizer.Line(currentRenderBuffer,
				//	linepos3.xy, linepos1.xy, new float4(0, 0, 0, 1));

				#endregion

				#region 显示经着色器计算后的法线和切线
				{
					
					//DrawDirLine(currentRenderBuffer, 0.2f, v1.worldPos, v1.worldNormal, new float4(0, 0, 1, 1));
					//DrawDirLine(currentRenderBuffer, 0.2f, v1.worldPos, v1.worldTangent, new float4(1, 1, 0, 1));

				}

				#endregion
			}
		}

		private void DrawDirLine(IRenderTarget currentRenderBuffer, float linelength, float4 worldpos,float3 dir, float4 color)
		{
			float4 sv1;
			float4 sv2;

			calDirectionPos(worldpos, dir, linelength, out sv1, out sv2);
			Rasterizer.Line(currentRenderBuffer,
				sv1.xy / sv1.w, sv2.xy / sv2.w, color);

		}




		private void calDirectionPos(float4 worldpos, float3 dir, float length, out float4 np1,out float4 np2)
		{
			
			var vp = programConstants.MATRIX_VP;
			var normalpoint = worldpos + Shader.normalize( dir) * length;

			np1 = Shader.mul(
				vp,
				new float4( worldpos.xyz,1)
				);
			np2 = Shader.mul(
				vp,
				new float4(normalpoint.xyz, 1)
				);
			
		}

		public void DrawDebugVisualization(int x,int y,float edgeline)
		{
			if (DebugBuffer != null)
			{
				var data = DebugBuffer.buffer[x][y];
				if (!data.hasdraw && !data.isEmpty)
				{
					for (int i = 0; i < data.debugInfos.Count; i++)
					{
						var info = data.debugInfos[i];

						if (info.type== debugger.DebugInfoType.Vector)
						{
							DrawDirLine(DebugBuffer, edgeline, data.inputdata.worldPos, info.data.xyz, info.visualizationColor);
						}
					}

					data.hasdraw = true;

					if (debugLayerAdapter != null)
					{
						debugLayerAdapter.SetRenderBuffer(DebugBuffer);
					}

				}
			}
		}





		private VertexBuffer3D currentVertexBuffer;
		class RenderTargetBind
		{
			public IRenderTarget renderTarget;
			public ZBuffer zBuffer;

		}

	}
}
