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
	public partial class Form2 : Form
	{
		public Form2()
		{
			InitializeComponent();
		}

		private Assimp.Scene scene;

		Context3D context3D;
		private void Form1_Load(object sender, EventArgs e)
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
			
			scene= importer.ImportFile("../../../models/Robot2.fbx", 
				Assimp.PostProcessSteps.MakeLeftHanded 
				| Assimp.PostProcessSteps.Triangulate 
				//| Assimp.PostProcessSteps.GenerateSmoothNormals
				| Assimp.PostProcessSteps.CalculateTangentSpace
				//| Assimp.PostProcessSteps.PreTransformVertices
				);

			lst_indexList = new List<IndexBuffer3D>();
			lst_vertexes = new List<VertexBuffer3D>();


			//var texture = context3D.createTexture(474, 474);
			//texture.uploadFromByteArray(SceneUtils.LoadBitmapData("../../../models/texs/th.jpg"), 0);
			var texture = //MiniRender.textures.Texture.white; 
						SceneUtils.MakeAndUploadTexture(context3D, "../../../models/texs/Robot_Color.png");
			texture.AutoGenMipMap();
			context3D.setTextureAt(0, texture);
			context3D.setSamplerStateAt(0, Context3DWrapMode.REPEAT, Context3DTextureFilter.LINEAR, Context3DMipFilter.MIPLINEAR);

			//设置matcap
			var matcap = SceneUtils.MakeAndUploadTexture(context3D, "../../../models/texs/MaCrea_6.png");
			matcap.AutoGenMipMap();
			context3D.setTextureAt(1, matcap);
			context3D.setSamplerStateAt(1, Context3DWrapMode.CLAMP, Context3DTextureFilter.LINEAR, Context3DMipFilter.MIPLINEAR);

			//设置法线
			var normalmap = //MiniRender.textures.Texture.planeNormal;
				SceneUtils.MakeAndUploadTexture(context3D, "../../../models/texs/Robot_Normal.png");
			normalmap.AutoGenMipMap();
			context3D.setTextureAt(2, normalmap);
			context3D.setSamplerStateAt(2, Context3DWrapMode.REPEAT, Context3DTextureFilter.LINEAR, Context3DMipFilter.MIPNEAREST);


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

		List< VertexBuffer3D> lst_vertexes;
		List<IndexBuffer3D> lst_indexList;


		private void button1_Click(object sender, EventArgs e)
		{
			//timerFrame.Enabled = !timerFrame.Enabled;
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
			
			while (nodes.Count > 0)
			{
				var n = nodes.Pop();
				for (int i = 0; i < n.ChildCount; i++)
				{			
					nodes.Push(n.Children[i]);
					dictNodeM.Add(n.Children[i],  dictNodeM[n] * n.Children[i].Transform );

				}

				//if (n.Name.IndexOf("LOD") > 0 && !n.Name.EndsWith("3"))
				//{

				//}
				//else 
				if(n.MeshIndices.Count>0)
				{

					var m = dictNodeM[n];
					renderMesh(m, n.MeshIndices);
				}

			}



			context3D.present();

		}

		private void renderMesh(Assimp.Matrix4x4 matrix,List<int> meshindices)
		{

			Matrix3D _ObjectToWorld;
			Matrix3D _WorldToObject;
			Matrix3D _MatrixV;
			Matrix3D _matrix_projection;
			Matrix3D _MatrixVP;
			Matrix3D _MatrixInvV;



			Matrix3D m = Matrix3D.Identity;

			Matrix3D mt = new Matrix3D(
				
				matrix[1, 1], matrix[1, 2], matrix[1, 3], matrix[1, 4],
				matrix[2, 1], matrix[2, 2], matrix[2, 3], matrix[2, 4],
				matrix[3, 1], matrix[3, 2], matrix[3, 3], matrix[3, 4],
				matrix[4, 1], matrix[4, 2], matrix[4, 3], matrix[4, 4]
				);

			//mt.transpose();
			//m.append(mt);


			//time = 0.096f;

			float angle = time * 3.14f / 2;

			//angle = 0.32656005f;

			//m.appendRotation(angle, Vector3.X_AXIS);
			m.appendRotation(angle, Vector3.Y_AXIS);
			//m.appendScale(Mathf.sin(1), 1, 2);


			_ObjectToWorld = m;
			_WorldToObject = m.getInvert();

			Vector4 camerpos = new Vector4(0, 2, -3.3f, 1);

			Matrix3D mcamera = Matrix3D.Identity.appendRotation(angle, Vector3.Y_AXIS);
			//camerpos = camerpos * mcamera;


			var camera = Matrix3D.lookAtLH(camerpos.x, camerpos.y, camerpos.z,
											0f, 1f, 0,
											0, 1, 0);

			_MatrixV = camera;
			_MatrixInvV = _MatrixV.getInvert();

			var perspective = Matrix3D.perspectiveOffCenterLH(-1, 1, -1.0f * 600 / 800, 1.0f * 600 / 800, 2f, 50f);

			_matrix_projection = perspective;
			_MatrixVP = _MatrixV.append(perspective);

			context3D.setProgramConstants_Matrices(_ObjectToWorld, _WorldToObject, camera, _matrix_projection, _MatrixVP, _MatrixInvV, true);
			context3D.setProgramVariables(camerpos);


			
			for (int i = 0; i < meshindices.Count; i++)
			{
				context3D.bindVertexBuffer(lst_vertexes[meshindices[i]]);
				context3D.drawTriangles(lst_indexList[meshindices[i]]);
			}

		}



		float time = 0;
		private void timerFrame_Tick(object sender, EventArgs e)
		{
			time += 0.016f;
			render();
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

		private void button2_Click(object sender, EventArgs e)
		{
			timerFrame.Enabled = !timerFrame.Enabled;
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			fShader.Roughness = trackBar1.Value / 100.0f;
			render();
		}

		private void trackBar2_Scroll(object sender, EventArgs e)
		{
			fShader._Metallic = trackBar2.Value / 100.0f;
			render();
		}
	}
}
