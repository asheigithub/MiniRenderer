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
			
			var scene= importer.ImportFile("../../../models/dwarf2.b3d", 
				Assimp.PostProcessSteps.MakeLeftHanded 
				| Assimp.PostProcessSteps.Triangulate 
				| Assimp.PostProcessSteps.GenerateSmoothNormals
				| Assimp.PostProcessSteps.CalculateTangentSpace
				);

			lst_indexList = new List<IndexBuffer3D>();
			lst_vertexes = new List<VertexBuffer3D>();

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
							vertex = new float3(vs[i].X, vs[i].Y, vs[i].Z) * 0.05
						}
						);
				}

				if (mesh.HasNormals)
				{
					for (int i = 0; i < vs.Count; i++)
					{
						vertices[i].normal = -(new float3(normals[i].X, normals[i].Y, normals[i].Z));
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
			program3d.upload(new programs.test2.VShader(), new programs.test2.FShader());
			context3D.setProgram(program3d);
		}

		List< VertexBuffer3D> lst_vertexes;
		List<IndexBuffer3D> lst_indexList;


		private void button1_Click(object sender, EventArgs e)
		{
			//timerFrame.Enabled = !timerFrame.Enabled;
			render();
		}

		

		private void render()
		{
			Matrix3D _ObjectToWorld;
			Matrix3D _WorldToObject;
			Matrix3D _MatrixV;
			Matrix3D _matrix_projection;
			Matrix3D _MatrixVP;
			Matrix3D _MatrixInvV;


			
			context3D.setCulling(Context3DTriangleFace.BACK);
			context3D.setDepthTest(true, Context3DCompareMode.LESS);

			Matrix3D mv = Matrix3D.Identity;
			
			float angle = time * 3.14f / 2;

			mv.appendRotation(angle, Vector3.Y_AXIS);
			//mv.appendScale(Mathf.sin(1), 1, 2);


			_ObjectToWorld = mv;
			_WorldToObject = mv.getInvert();

			Vector4 camerpos = new Vector4(0, 3, -7, 1);

			Matrix3D mcamera = Matrix3D.Identity.appendRotation(angle, Vector3.Y_AXIS);
			//camerpos = camerpos * mcamera;


			var camera = Matrix3D.lookAtLH(camerpos.x,camerpos.y,camerpos.z,
											0f, 0f, 0,
											0, 1, 0);

			_MatrixV = camera;
			_MatrixInvV = _MatrixV.getInvert();

			var perspective = Matrix3D.perspectiveOffCenterLH(-1, 1, -1.0f * 600 / 800, 1.0f * 600 / 800, 2f, 8f);

			_matrix_projection = perspective;
			_MatrixVP = _MatrixV.append(perspective);

			context3D.setProgramConstants_Matrices(_ObjectToWorld, _WorldToObject, _MatrixV, _matrix_projection, _MatrixVP, _MatrixInvV, true);
			context3D.setProgramVariables(camerpos);


			context3D.clear(49 / 255f, 77 / 255f, 121 / 255f);

			for (int i = 0; i < lst_vertexes.Count; i++)
			{
				context3D.bindVertexBuffer(lst_vertexes[i]);
				context3D.drawTriangles(lst_indexList[i]);
			}

			context3D.present();

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
				context3D !=null 
				&&
				context3D.DebugBuffer != null)
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
					toolStripStatusLabel1.Text = e.Location.ToString() + " 当前片段着色器调试信息: " + debugdata.ToString();
				}
			}

		}

		private void button2_Click(object sender, EventArgs e)
		{
			timerFrame.Enabled = !timerFrame.Enabled;
		}
	}
}
