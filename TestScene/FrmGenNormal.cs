using MiniRender;
using MiniRender.geom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestScreen
{
	public partial class FrmGenNormal : Form
	{
		public FrmGenNormal()
		{
			InitializeComponent();
			this.pictureBox1.MouseMove += pictureBox1_MouseMove;
		}

		float2 texsize;
		private void btnOpen_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				using (var bitmap = System.Drawing.Bitmap.FromFile(openFileDialog1.FileName))
				{
					pictureBox1.Width = bitmap.Width;
					pictureBox1.Height = bitmap.Height;


					var img = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
					RenderTargetAdapter renderBufferAdapter = new RenderTargetAdapter(img, pictureBox1);
					context3D.configureBackBuffer(pictureBox1.Width, pictureBox1.Height, renderBufferAdapter);
					pictureBox1.BackgroundImage = img;





					texsize = new float2(bitmap.Width, bitmap.Height);
					var texture = context3D.createTexture(bitmap.Width, bitmap.Height);
					texture.uploadFromByteArray(SceneUtils.LoadBitmapData((Bitmap)bitmap), 0);
					context3D.setTextureAt(0, texture);

				}


				refreshCtl();

			}
		}

		private float getBumpiness()
		{
			return this.trackBumpiness.Value * 0.03f / this.trackBumpiness.Maximum;
		}

		private void refreshCtl()
		{
			this.lblBumpiness.Text = getBumpiness().ToString("F4");
			if (context3D != null)
			{
				context3D.setProgramConstantsVector4(MiniRender.ProgramConstants.USERDEFINE_STARTIDX, new Vector4(
					1 / texsize.x, 1 / texsize.y,
					getBumpiness(), 0
					));

				render();
			}
		}

		private void trackBumpiness_Scroll(object sender, EventArgs e)
		{
			refreshCtl();
		}

		Context3D context3D;
		private void FrmGenNormal_Load(object sender, EventArgs e)
		{

			context3D = new Context3D();

			System.Drawing.Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			RenderTargetAdapter renderBufferAdapter = new RenderTargetAdapter(bitmap, pictureBox1);
			context3D.configureBackBuffer(pictureBox1.Width, pictureBox1.Height, renderBufferAdapter);
			pictureBox1.BackgroundImage = bitmap;


			System.Drawing.Bitmap debuglayer = new Bitmap(pictureBox1.Width, pictureBox1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			RenderTargetAdapter debuggerAdapter = new RenderTargetAdapter(debuglayer, pictureBox1);
			pictureBox1.Image = debuglayer;
			context3D.debugLayerAdapter = debuggerAdapter;


			Assimp.AssimpContext importer = new Assimp.AssimpContext();
			Assimp.Scene scene
				= importer.ImportFile("../../../models/Quad.fbx",
				Assimp.PostProcessSteps.MakeLeftHanded
				|
				Assimp.PostProcessSteps.CalculateTangentSpace
				);

			lst_indexList = new List<IndexBuffer3D>();
			lst_vertexes = new List<VertexBuffer3D>();

			texsize = new float2(1.0f / 2, 1.0f / 2);

			var texture = MiniRender.textures.Texture.white;
			texture.AutoGenMipMap();
			context3D.setTextureAt(0, texture);
			context3D.setSamplerStateAt(0, Context3DWrapMode.REPEAT, Context3DTextureFilter.LINEAR, Context3DMipFilter.MIPNONE);

			for (int k = 0; k < scene.MeshCount; k++)
			{
				var mesh = scene.Meshes[k];
				var vs = mesh.Vertices;
				var indices = mesh.GetUnsignedIndices();



				var normals = mesh.Normals;
				var tangents = mesh.Tangents;
				var coords = mesh.TextureCoordinateChannels[0];

				var indexList = context3D.createIndexBuffer(indices.Length);
				indexList.uploadFromVector(indices);

				lst_indexList.Add(indexList);

				List<Vertex> vertices = new List<Vertex>();
				for (int i = 0; i < vs.Count; i++)
				{
					vertices.Add(
						new Vertex()
						{
							vertex = new float3(vs[i].X, vs[i].Y, vs[i].Z)
						}
						);
				}

				if (mesh.HasNormals)
				{
					for (int i = 0; i < vs.Count; i++)
					{
						vertices[i].normal = (new float3(normals[i].X, normals[i].Y, normals[i].Z));
					}
				}

				if (mesh.HasTangentBasis)
				{
					for (int i = 0; i < vs.Count; i++)
					{
						vertices[i].tangent = new float3(tangents[i].X, tangents[i].Y, tangents[i].Z);
					}
				}

				if (mesh.HasTextureCoords(0))
				{
					for (int i = 0; i < vs.Count; i++)
					{
						vertices[i].uv = new float3(coords[i].X, coords[i].Y, coords[i].Z);
					}
				}

				if (mesh.HasVertexColors(0))
				{
					var color = mesh.VertexColorChannels[0];
					for (int i = 0; i < vs.Count; i++)
					{
						vertices[i].color = new float4(color[i].R, color[i].G, color[i].B, color[i].A);
					}
				}
				var vertexes = context3D.createVertexBuffer(vertices.Count);
				vertexes.uploadFromVector(vertices.ToArray());

				lst_vertexes.Add(vertexes);
			}



			var program3d = context3D.createProgram();

			program3d.upload(new VShader(),
				new FShader()
				);

			context3D.setProgram(program3d);

			//refreshCtl();

			//render();

		}


		List<VertexBuffer3D> lst_vertexes;
		List<IndexBuffer3D> lst_indexList;

		private void render()
		{
			//fShader._Metallic = (Mathf.sin(time) + 1) / 2;

			context3D.setCulling(Context3DTriangleFace.BACK);
			context3D.setDepthTest(true, Context3DCompareMode.LESS);
			context3D.clear(49 / 255f, 77 / 255f, 121 / 255f);


			Matrix3D _ObjectToWorld;
			Matrix3D _WorldToObject;
			Matrix3D _MatrixV;
			Matrix3D _matrix_projection;
			Matrix3D _MatrixVP;
			Matrix3D _MatrixInvV;



			Matrix3D m = Matrix3D.Identity;
			
			_ObjectToWorld = m;
			_WorldToObject = m.getInvert();

			Vector4 camerpos = new Vector4(0, 0, 0.5f, 1);


			var camera = Matrix3D.lookAtLH(camerpos.x, camerpos.y, camerpos.z,
											0f, 0f, 0,
											0, 1, 0);

			_MatrixV = camera;
			_MatrixInvV = _MatrixV.getInvert();

			var perspective = Matrix3D.perspectiveOffCenterLH(-0.5f, 0.5f, -0.5f, 0.5f, 0.5f, 50f);

			_matrix_projection = perspective;
			_MatrixVP = _MatrixV.append(perspective);

			context3D.setProgramConstants_Matrices(_ObjectToWorld, _WorldToObject, camera, _matrix_projection, _MatrixVP, _MatrixInvV, true);
			context3D.setProgramVariables(camerpos);

			context3D.bindVertexBuffer(lst_vertexes[0]);
			context3D.drawTriangles(lst_indexList[0]);

			context3D.present();

		}

		private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
		{


			if (
				context3D != null
				&&
				context3D.DebugBuffer != null
				&&
				e.X >= 0 && e.X < context3D.DebugBuffer.rt_width
				&&
				e.Y >= 0 && e.Y < context3D.DebugBuffer.rt_height

				)
			{


				var debugdata = context3D.DebugBuffer.buffer[e.X][e.Y];

				context3D.DrawDebugVisualization(debugdata.i, debugdata.j, 0.5f);
				context3D.DebugBuffer.ClearVisualization();

			}

		}



		class VShader : VertexShader
		{
			public override v2f Execute()
			{
				v2f v2f = __init_v2f();

				v2f.SV_POSITION = mul(MATRIX_MVP, appdata.vertex);
				v2f.worldPos = mul(_ObjectToWorld, appdata.vertex).xyz;
				v2f.uv = appdata.uv;

				v2f.objPos = appdata.vertex.xyz;
				v2f.color = appdata.color;
				v2f.uv = appdata.uv;

				// Declares 3x3 matrix 'rotation', filled with tangent space basis
				float3 worldNormal = ObjectToWorldNormal(normalize(appdata.normal));
				float3 worldTangent = ObjectToWorldDir(appdata.tangent.xyz);
				float3 worldBinormal = cross(v2f.worldNormal, worldTangent) * appdata.tangent.w;

				//创建切线空间
				v2f.tSpace0 = float3(worldTangent.x, worldBinormal.x, worldNormal.x);
				v2f.tSpace1 = float3(worldTangent.y, worldBinormal.y, worldNormal.y);
				v2f.tSpace2 = float3(worldTangent.z, worldBinormal.z, worldNormal.z);


				return v2f;
			}
		}

		class FShader : FragementShader
		{
			public override bool HasDebug => false;
	
			float GetGrayColor(float3 color)
			{
				return color.r * 0.2126f + color.g * 0.7152f + color.b * 0.0722f;
			}

			private float _HeightScale
			{
				get
				{
					return constants.registers[ProgramConstants.USERDEFINE_STARTIDX].z;
				}
			}

			private float2 _MainTex_TexelSize
			{
				get
				{
					return constants.registers[ProgramConstants.USERDEFINE_STARTIDX].xy;
				}
			}

			float3 GetNormalByGray(float2 uv)
			{
				float _DeltaScale = 0.5f;
				//代码分为 3 段，前两段为计算 uv 各自方向的高度函数切线，最后一段计算最终法线。
				//先看第一段，计算 u 方向的高度函数切线。首先，确定步长 的大小。MainTexTexelSize 是 unity shader 内置的一个变量，
				//保存着纹理大小相关的信息，是一个 float4 类型的值，具体为(1 / width, 1 / height, width, height)。
				//_DeltaScale 是一个控制步长缩放的变量，在这个例子中为 0.5，乘以 _DeltaScale 是用来控制法线生成的精确度的，就如之前所说，  越小，
				//生成的法线就越精确。通常我们会向当前采样点两侧去采样，以获得更精准的结果，这个方法叫做中心差分法。
				//然后可以根据步长分别取当前像素左右两侧的高度值（在这个例子里就是灰度值），在按照上面提到的计算方法计算切线即可。
				//注释掉的代码是原始代码，下面没注释的是优化后的代码，这个也是上面提到的。
				//有一个问题是，为什么计算出来的切线向量是(x, 0, z) 的形式，而不是其他？这是因为前面提到整张纹理是处于 XOY 平面的，
				//而高度是第三个维度，因为 u 和 v 自然是按照 x 和 y 轴处理方便，所以高度 h 就按照 z 轴来处理了。
				//还有一个可能的疑问是，当 _DeltaScale 特别小的时候，取两侧的像素实际上都是单前像素，则高度差都是 0 了。
				//但实际上这个情况只有在采样过滤方式为 point 采样时才会出现，具体采样过滤方式是如何处理的可以查阅其他资料。
				//同理，第二段可以计算出 v 方向的高度函数切线，两个切线向量，做叉积，再归一化，即可获得当前像素点表面的法线向量。
				//叉积的顺序很重要，因为纹理是朝向 - z 轴的，所以一般来说会让法线也顺着表面所在的朝向，这就是为什么是 cross(tangentv, tangentu) 而不是 cross(tangentu, tangentv) 的原因。
				//现在将法线当作颜色输出出来看一下，当然不能直接输出，因为法线向量可能包含着负值，可能看到的都是黑色，所以需要转换一下，这个转换对于了解过法线贴图的读者应该很熟悉了。


				float2 deltaU = float2(_MainTex_TexelSize.x * _DeltaScale, 0);
				float h1_u = GetGrayColor(tex2D(0, uv - deltaU).rgb);
				float h2_u = GetGrayColor(tex2D(0, uv + deltaU).rgb);
				// float3 tangent_u = float3(1, 0, (h2_u - h1_u) / deltaU.x);
				float3 tangent_u = float3(deltaU.x, 0, _HeightScale * (h2_u - h1_u));

				float2 deltaV = float2(0, _MainTex_TexelSize.y * _DeltaScale);
				float h1_v = GetGrayColor(tex2D(0, uv - deltaV).rgb);
				float h2_v = GetGrayColor(tex2D(0, uv + deltaV).rgb);
				// float3 tangent_v = float3(0, 1, (h2_v - h1_v) / deltaV.y);
				float3 tangent_v = float3(0, deltaV.y, _HeightScale * (h2_v - h1_v));

				float3 normal = normalize(cross(tangent_v, tangent_u));

				
				normal.z *= -1;
				normal = normal * 0.5 + 0.5;

				return normal;
			}








			protected override float4 Execute(v2f IN)
			{
				return GetNormalByGray(IN.uv.xy);

				//float3 lightdir = normalize(float3(-5, 5, 8));


				////从法线贴图中取出法线
				//float3 normal = GetNormalByGray(IN.uv.xy) * 2-1;



				////将法线从切线空间旋转到世界空间 Rotate normals from tangent space to world space
				//float3 worldNorm = float3(1,1,1);
				//worldNorm.x = dot(IN.tSpace0, normal);
				//worldNorm.y = dot(IN.tSpace1, normal);
				//worldNorm.z = dot(IN.tSpace2, normal);

				////AddDebugInfo(worldNorm, "normal", MiniRender.debugger.DebugInfoType.Vector, float3(0, 0, 1));
				////AddDebugInfo(lightdir, "lightdir", MiniRender.debugger.DebugInfoType.Vector, float3(1, 1, 0));

				////AddDebugInfo(IN.worldNormal, "normal", MiniRender.debugger.DebugInfoType.Vector, float3(0, 0, 1));

				////float4 tex = tex2D(0, IN.uv);

				//float4 texColor = float4(1, 1, 1, 1);
				//float diffuse = clamp(dot(worldNorm, lightdir ), 0, 1);
				//float4 color = texColor;
				//color.rgb = texColor.rgb * diffuse;

				//return color;





				//return color;

				//return tex;
			}

		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				pictureBox1.BackgroundImage.Save(saveFileDialog1.FileName);
			}
		}
	}

}
