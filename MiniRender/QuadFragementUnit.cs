using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniRender
{
	/// <summary>
	/// 四个片段着色单元。
	/// </summary>
	class QuadFragementUnit
	{
 //     ^
 //     |   <--dpdx0-->
 //     |  ^           ^
 //     t  |      |     |
 //     | dpdy1 - +-   dpdy0
 //     |  |      |     |
 //     |  v            v 
 //     |   <--dpdx1-->
//	     ---------s--------->

//      dpdx_1_0 = dpdx_0_0 = P_1_0 - P_0_0
//      dpdx_1_1 = dpdx_0_1 = P_1_1 - P_0_1
//      dpdy_0_1 = dpdy_0_0 = P_0_1 - P_0_0
//      dpdy_1_1 = dpdy_1_0 = P_1_1 - P_1_0


		
		public FragementUnit unit0;
		public FragementUnit unit1;
		public FragementUnit unit2;
		public FragementUnit unit3;

		public QuadFragementUnit()
		{
			unit0 = new FragementUnit();
			unit1 = new FragementUnit();
			unit2 = new FragementUnit();
			unit3 = new FragementUnit();
		}


		public FragementUnit this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return unit0;
					case 1:
						return unit1;
					case 2:
						return unit2;
					case 3:
						return unit3;
					default:
						return null;
				}
			}
		}
			


		public void setQuadUnit(FragementShader  fragementShader, v2f i0, v2f i1, v2f i2,v2f i3)
		{
			unit0.initData(i0, fragementShader);
			unit1.initData(i1, fragementShader);
			unit2.initData(i2, fragementShader);
			unit3.initData(i3, fragementShader);

			unit0.dpdx_v1 = unit0;
			unit0.dpdx_v2 = unit1;
			unit0.dpdy_v1 = unit0;
			unit0.dpdy_v2 = unit2;

			unit1.dpdx_v1 = unit0;
			unit1.dpdx_v2 = unit1;
			unit1.dpdy_v1 = unit1;
			unit1.dpdy_v2 = unit3;

			unit2.dpdx_v1 = unit2;
			unit2.dpdx_v2 = unit3;
			unit2.dpdy_v1 = unit0;
			unit2.dpdy_v2 = unit2;

			unit3.dpdx_v1 = unit2;
			unit3.dpdx_v2 = unit3;
			unit3.dpdy_v1 = unit1;
			unit3.dpdy_v2 = unit3;

		}
	}

	class FragementUnit
	{
		public v2f input;

		public float4 output;

		public FragementShader fragementShader;

		
		internal FragementUnit dpdx_v1;
		internal FragementUnit dpdx_v2;

		internal FragementUnit dpdy_v1;
		internal FragementUnit dpdy_v2;

		internal void initData(v2f input,FragementShader shader)
		{
			this.input = input;
			this.output =new float4(0, 0, 0, 0);
			fragementShader = shader;
		}
	}


}
