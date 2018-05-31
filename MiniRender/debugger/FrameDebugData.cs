using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender.debugger
{
	/// <summary>
	/// 为每个光栅提供一个Debug数据
	/// </summary>
	public class FrameDebugBuffer : IRenderTarget
	{
		public FrameDebugData[][] buffer;

		private float4[][] visualization;

		public FrameDebugBuffer(int width,int height)
		{
			buffer = new FrameDebugData[width][];

			visualization = new float4[width][];

			for (int i = 0; i < width; i++)
			{
				buffer[i] = new FrameDebugData[height];
				visualization[i] = new float4[height];
				for (int j = 0; j < height; j++)
				{
					buffer[i][j] = new FrameDebugData();
				}

			}

			this.rt_width = width;
			this.rt_height = height;

		}

		public int rt_width { get; }

		public int rt_height { get; }

		int IRenderTarget.width => rt_width;

		int IRenderTarget.height => rt_height;

		public float4 getPixel(int x, int y)
		{
			return visualization[x][y];
		}

		public float4[][] getRealPixels()
		{
			return visualization;
		}

		public void ClearVisualization()
		{
			for (int i = 0; i < rt_width; i++)
			{

				for (int j = 0; j < rt_height; j++)
				{
					visualization[i][j] = new float4(0, 0, 0, 0);
				}

			}
		}


		internal void Clear()
		{
			for (int i = 0; i < rt_width; i++)
			{
				
				for (int j = 0; j < rt_height; j++)
				{
					buffer[i][j].Empty();
					visualization[i][j] =new float4(0, 0, 0, 0);
				}

			}
		}
	}




	public class FrameDebugData
	{
		public v2f inputdata;

		public int i;

		public int j;

		public List<DebugInfo> debugInfos;

		internal bool hasdraw;

		public FrameDebugData()
		{
			debugInfos = new List<DebugInfo>();
			Empty();
		}

		public void Ready()
		{
			isEmpty = false;
			hasdraw = false;
			debugInfos.Clear();
		}

		public bool isEmpty;

		internal void Empty()
		{
			isEmpty = true;
			debugInfos.Clear();
		}


		public override string ToString()
		{
			if (isEmpty)
			{
				return "";
			}
			else if (debugInfos.Count == 0)
			{
				return "[NODATA]";
			}
			else
			{
				string result = "";

				for (int i = 0; i < debugInfos.Count; i++)
				{
					result += debugInfos[i].ToString();
				}

				return result;
			}
		}

		
	}

	public class DebugInfo
	{
		public string label;
		public float4 data;
		public DebugInfoType type;
		public float3 visualizationColor;


		public override string ToString()
		{
			return string.Format("{0}: {1},type:{2} ;",label,data,type);
		}
	}

	public enum DebugInfoType
	{
		Vector,
		Color,
		Numeric
	}

}
