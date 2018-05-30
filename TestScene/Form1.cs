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
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		Context3D context3D;
		private void Form1_Load(object sender, EventArgs e)
		{
			context3D = new Context3D();

			System.Drawing.Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height,  System.Drawing.Imaging.PixelFormat.Format32bppArgb );

			RenderTargetAdapter renderBufferAdapter = new RenderTargetAdapter(bitmap,pictureBox1);

			context3D.configureBackBuffer(pictureBox1.Width, pictureBox1.Height, renderBufferAdapter);

			pictureBox1.Image = bitmap;

			var triangles = new uint[] {
				  2,1,0, //front face
					3,2,0,
					4,7,5, //bottom face
					7,6,5,
					8,11,9, //back face
					9,11,10,
					12,15,13, //top face
					13,15,14,
					16,19,17, //left face
					17,19,18,
					20,23,21, //right face
					21,23,22
											};

			indexList = context3D.createIndexBuffer(triangles.Length);
			indexList.uploadFromVector(triangles);
			
			var vertexData = new Vertex[] {

				// x,y,z r,g,b format
     //               0,0,0, 1,0,0, //front face
     //               0,1,0, 1,0,0,
					//1,1,0, 1,0,0,
					//1,0,0, 1,0,0,

					//0,0,0, 0,1,0, //bottom face
     //               1,0,0, 0,1,0,
					//1,0,1, 0,1,0,
					//0,0,1, 0,1,0,

					//0,0,1, 1,0,0, //back face
     //               1,0,1, 1,0,0,
					//1,1,1, 1,0,0,
					//0,1,1, 1,0,0,

					//0,1,1, 0,1,0, //top face
     //               1,1,1, 0,1,0,
					//1,1,0, 0,1,0,
					//0,1,0, 0,1,0,

					//0,1,1, 0,0,1, //left face
     //               0,1,0, 0,0,1,
					//0,0,0, 0,0,1,
					//0,0,1, 0,0,1,

					//1,1,0, 0,0,1, //right face
     //               1,1,1, 0,0,1,
					//1,0,1, 0,0,1,
					//1,0,0, 0,0,1




				new Vertex(){ vertex=new float3(0,0,0), color=new float3(1,0,0) }, //front face
				new Vertex(){ vertex=new float3(0,1,0), color=new float3(1,0,0) },
				new Vertex(){ vertex=new float3(1,1,0), color=new float3(1,0,0) },
				new Vertex(){ vertex=new float3(1,0,0), color=new float3(1,0,0) },

				new Vertex(){ vertex=new float3(0,0,0), color=new float3(0,1,0) },//bottom face
				new Vertex(){ vertex=new float3(1,0,0), color=new float3(0,1,0) },
				new Vertex(){ vertex=new float3(1,0,1), color=new float3(0,1,0) },
				new Vertex(){ vertex=new float3(0,0,1), color=new float3(0,1,0) },

				new Vertex(){ vertex=new float3(0,0,1), color=new float3(1,0,0) },//back face
				new Vertex(){ vertex=new float3(1,0,1), color=new float3(1,0,0) },
				new Vertex(){ vertex=new float3(1,1,1), color=new float3(1,0,0) },
				new Vertex(){ vertex=new float3(0,1,1), color=new float3(1,0,0) },

				new Vertex(){ vertex=new float3(0,1,1), color=new float3(0,1,0) },//top face
				new Vertex(){ vertex=new float3(1,1,1), color=new float3(0,1,0) },
				new Vertex(){ vertex=new float3(1,1,0), color=new float3(0,1,0) },
				new Vertex(){ vertex=new float3(0,1,0), color=new float3(0,1,0) },

				new Vertex(){ vertex=new float3(0,1,1), color=new float3(0,0,1) },//left face
				new Vertex(){ vertex=new float3(0,1,0), color=new float3(0,0,1) },
				new Vertex(){ vertex=new float3(0,0,0), color=new float3(0,0,1) },
				new Vertex(){ vertex=new float3(0,0,1), color=new float3(0,0,1) },

				new Vertex(){ vertex=new float3(1,1,0), color=new float3(0,0,1) },//right face
				new Vertex(){ vertex=new float3(1,1,1), color=new float3(0,0,1) },
				new Vertex(){ vertex=new float3(1,0,1), color=new float3(0,0,1) },
				new Vertex(){ vertex=new float3(1,0,0), color=new float3(0,0,1) },

			};

			vertexes = context3D.createVertexBuffer(24);
			vertexes.uploadFromVector(vertexData);


			var program3d = context3D.createProgram();
			program3d.upload(new programs.test1.VShader(),new programs.test1.FShader());
			context3D.setProgram(program3d);
		}

		VertexBuffer3D vertexes;
		IndexBuffer3D indexList;

		
		private void button1_Click(object sender, EventArgs e)
		{
			timerFrame.Enabled = !timerFrame.Enabled;
			//render();
		}

		private void render()
		{
			Matrix3D _ObjectToWorld;
			Matrix3D _WorldToObject;
			Matrix3D _MatrixV;
			Matrix3D _matrix_projection;
			Matrix3D _MatrixVP;
			Matrix3D _MatrixInvV;

			context3D.bindVertexBuffer(vertexes);

			context3D.setCulling(Context3DTriangleFace.NONE);
			context3D.setDepthTest(true, Context3DCompareMode.LESS);

			Matrix3D mv = new Matrix3D();
			mv.identity();


			_ObjectToWorld = mv;
			_WorldToObject = mv.getInvert();

			float angle = time * 3.14f / 2;
			var camera = Matrix3D.lookAtLH( Mathf.sin(angle)*-5f +0.5f, 2f, Mathf.cos(angle)*-5f+0.5f,
											0.5f, 0.5f, 0,
											0, 1, 0);

			_MatrixV = camera;
			_MatrixInvV = _MatrixV.getInvert();

			var perspective = Matrix3D.perspectiveOffCenterLH(-1, 1, -1.0f * 600 / 800, 1.0f * 600 / 800, 2f, 8f);
			mv.append(perspective);

			_matrix_projection = perspective;
			_MatrixVP = _MatrixV.append(perspective);

			context3D.setProgramConstants_Matrices(_ObjectToWorld, _WorldToObject, _MatrixV, _matrix_projection, _MatrixVP, _MatrixInvV, true);


			context3D.clear(49 / 255f, 77 / 255f, 121 / 255f);
			context3D.drawTriangles(indexList);

			context3D.present();
			
		}

		float time = 0;
		private void timerFrame_Tick(object sender, EventArgs e)
		{
			time += 0.016f;
			render();
		}
	}
}
