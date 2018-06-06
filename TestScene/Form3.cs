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
	public partial class Form3 : Form
	{
		public Form3()
		{
			InitializeComponent();
		}

		private Assimp.Scene scene;
		Context3D context3D;

		private void Form3_Load(object sender, EventArgs e)
		{
			context3D = new Context3D();
			this.pictureBox1.MouseMove += pictureBox1_MouseMove;

			System.Drawing.Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			RenderTargetAdapter renderBufferAdapter = new RenderTargetAdapter(bitmap, pictureBox1);
			context3D.configureBackBuffer(pictureBox1.Width, pictureBox1.Height, renderBufferAdapter);
			pictureBox1.BackgroundImage = bitmap;


			System.Drawing.Bitmap debuglayer = new Bitmap(pictureBox1.Width, pictureBox1.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			RenderTargetAdapter debuggerAdapter = new RenderTargetAdapter(debuglayer, pictureBox1);
			pictureBox1.Image = debuglayer;
			context3D.debugLayerAdapter = debuggerAdapter;


			Assimp.AssimpContext importer = new Assimp.AssimpContext();

			scene = importer.ImportFile("../../../models/Sphere.fbx",
				Assimp.PostProcessSteps.MakeLeftHanded
				| Assimp.PostProcessSteps.Triangulate
				//| Assimp.PostProcessSteps.GenerateSmoothNormals
				| Assimp.PostProcessSteps.CalculateTangentSpace
				//| Assimp.PostProcessSteps.PreTransformVertices
				);

			lst_indexList = new List<IndexBuffer3D>();
			lst_vertexes = new List<VertexBuffer3D>();

			var texture = MiniRender.textures.Texture.white;
						//SceneUtils.MakeAndUploadTexture(context3D, "../../../models/texs/sf_conc_floor_01_d.png");
			texture.AutoGenMipMap();
			context3D.setTextureAt(0, texture);
			context3D.setSamplerStateAt(0, Context3DWrapMode.REPEAT, Context3DTextureFilter.LINEAR, Context3DMipFilter.MIPLINEAR);

			//设置matcap
			var matcap = SceneUtils.MakeAndUploadTexture(context3D, "../../../models/texs/MaCrea_6.png");
			matcap.AutoGenMipMap();
			context3D.setTextureAt(1, matcap);
			context3D.setSamplerStateAt(1, Context3DWrapMode.CLAMP, Context3DTextureFilter.LINEAR, Context3DMipFilter.MIPLINEAR);

			//设置法线
			var normalmap = MiniRender.textures.Texture.planeNormal;
			normalmap.AutoGenMipMap();
			context3D.setTextureAt(2, normalmap);
			context3D.setSamplerStateAt(2, Context3DWrapMode.REPEAT, Context3DTextureFilter.NEAREST, Context3DMipFilter.MIPNEAREST);


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
			fShader = new programs.test3.FShader();
			program3d.upload(new programs.test3.VShader(),
				//new programs.test4.FShader_Bump()
				fShader
				);

			context3D.setProgram(program3d);
		}

		programs.test3.FShader fShader;

		List<VertexBuffer3D> lst_vertexes;
		List<IndexBuffer3D> lst_indexList;

		private void button1_Click(object sender, EventArgs e)
		{
			render();
		}



		private void render()
		{
			//fShader._Metallic = (Mathf.sin(time) + 1) / 2;

			context3D.setCulling(Context3DTriangleFace.BACK);
			context3D.setDepthTest(true, Context3DCompareMode.LESS);


			Stack<Assimp.Node> nodes = new Stack<Assimp.Node>();
			nodes.Push(scene.RootNode);

			Dictionary<Assimp.Node, Assimp.Matrix4x4> dictNodeM = new Dictionary<Assimp.Node, Assimp.Matrix4x4>();
			dictNodeM.Add(scene.RootNode, scene.RootNode.Transform);

			context3D.clear(49 / 255f, 77 / 255f, 121 / 255f);


			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					var m = Matrix3D.Identity;

					m.appendScale(0.5f, 0.5f, 0.5f);

					m.appendTranslation((i-5) * 0.6f,  (j-5) * 0.6f,0);

					fShader.Roughness = 1-i * 1.0f/10;
					fShader._Metallic = 1-j * 1.0f/10;

					renderMesh(m);
				}
			}

			

			context3D.present();

		}

		private void renderMesh(Matrix3D modelmatrix )
		{

			Matrix3D _ObjectToWorld;
			Matrix3D _WorldToObject;
			Matrix3D _MatrixV;
			Matrix3D _matrix_projection;
			Matrix3D _MatrixVP;
			Matrix3D _MatrixInvV;



			Matrix3D m = Matrix3D.Identity;


			m.append(modelmatrix);

			_ObjectToWorld = m;
			_WorldToObject = m.getInvert();

			Vector4 camerpos = new Vector4(0, 0, -9f, 1);

			var camera = Matrix3D.lookAtLH(camerpos.x, camerpos.y, camerpos.z,
											0f, 0f, 0,
											0, 1, 0);

			_MatrixV = camera;
			_MatrixInvV = _MatrixV.getInvert();

			var perspective = Matrix3D.perspectiveOffCenterLH(-1, 1, -1.0f * 600 / 800, 1.0f * 600 / 800, 2f, 50f);

			_matrix_projection = perspective;
			_MatrixVP = _MatrixV.append(perspective);

			context3D.setProgramConstants_Matrices(_ObjectToWorld, _WorldToObject, camera, _matrix_projection, _MatrixVP, _MatrixInvV, true);
			context3D.setProgramVariables(camerpos);



			
			context3D.bindVertexBuffer(lst_vertexes[0]);
			context3D.drawTriangles(lst_indexList[0]);
			

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

				if (debugdata.isEmpty)
				{
					toolStripStatusLabel1.Text = e.Location.ToString();

				}
				else
				{
					toolStripStatusLabel1.Text = e.Location.ToString() + "调试信息:\n" + debugdata.ToString();

				}

			}


		}


	}
}
