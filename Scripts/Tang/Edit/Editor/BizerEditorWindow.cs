using UnityEngine;
using UnityEditor;

namespace Tang.Editor
{
    public enum diantype
    {
        point=1,
        parabola =2,
    }
	public class BizerEditorWindow : EditorWindow 
	{
		[MenuItem("Window/Bizer参数编辑器")]
		static void Init()
        {
            BizerEditorWindow window = (BizerEditorWindow)EditorWindow.GetWindow(typeof(BizerEditorWindow));
            window.Show();
        }
        public diantype diantype = diantype.parabola;
        public bool windowbool=false;
		public float longlong=3;
		public float heightf=3;
		public float wwwidth;
		public float hheight;
		public float t=1;
		public float sort=3;
		public Vector3 P0= new Vector3(500,500,0);
		public Vector3 P1;
		public Vector3 P2;
        public Vector3 P1put;
        public Vector3 P2put;
        public void OnGUI() 
		{
			GUIStyle boxStyle = new GUIStyle("box");

            var width = position.size.x - boxStyle.border.horizontal;
            var height = position.size.y - boxStyle.border.vertical;
			wwwidth=width;
			hheight=height;
            var innerBoxWidth = width - (boxStyle.padding.horizontal + boxStyle.border.horizontal);
            var innerBoxHeight = height - (boxStyle.padding.vertical + boxStyle.border.vertical);

			Rect mSecWindowRect=new Rect(5,5,width-10,height-10);

            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(width), GUILayout.Height(height));

                diantype =(diantype) MyGUI.EnumPopupWithTitle("点或抛物线", diantype);
                if (diantype == diantype.parabola)
                {
                    longlong = MyGUI.FloatFieldWithTitle("weight", longlong);
                    heightf = MyGUI.FloatFieldWithTitle("height", heightf);
                }
                else
                {
                P1put = MyGUI.Vector3WithTitle("P1", P1put);
                P2put = MyGUI.Vector3WithTitle("P2", P2put);
                }
               
				sort=MyGUI.FloatFieldWithTitle("点",sort);
				t=MyGUI.FloatFieldWithTitle("t",t);
				// windowbool=MyGUI.Button("开启窗口");
				if (GUILayout.Button("开启窗口"))
            	{
					GUIUtility.keyboardControl = 0;
					windowbool=true;
            	}
				if (windowbool)
				{
					P0=new Vector3((width/2),(height/2),0);
                    if(diantype == diantype.parabola)
                    {
                        P2 = new Vector3(P0.x + longlong, P0.y, P0.z);
                        P1 = new Vector3(P0.x + longlong / 2, P0.y - heightf, P0.z);
                    }
                    else
                    {
                        P2 = new Vector3(P0.x+P2put.x * 50, P0.y - P2put.y*50, P0.z+P2put.z * 50);
                        P1 = new Vector3(P0.x + P1put.x * 50, P0.y - P1put.y * 50, P0.z + P1put.z * 50);
                    }
					BeginWindows();
					mSecWindowRect = GUILayout.Window(354888, mSecWindowRect, DrawGraphWindow, "图形绘制", GUI.skin.window);
					EndWindows();
				}

				

				

			EditorGUILayout.EndVertical();
		}
		private void DrawGraphWindow(int varWindowID)
		{
			
			if (GUILayout.Button("关闭窗口"))
			{
				GUIUtility.keyboardControl = 0;
				windowbool=false;
				
			}
				// Handles.color = Color.red;
				// Handles.DrawLine(new Vector3(0,0,0),new Vector3(500,500,0));
				
				// Vector3[] fa=new Vector3[(int)sort];

				
			for (int i = 1; i < sort+1; i++)
			{
				// Vector3 faa=new Vector3(200,200,0);
				// Vector3 beginpos=Tools.BezierCurve(P0,P1,P2,(i-1)/sort);
				// Vector3 endpos=Tools.BezierCurve(P0,P1,P2,i/sort);
				// Vector3 DDSSS=new Vector3();
				float dsa=(float)i/sort;
				float dsada=(float)(i-1)/sort;
				Vector3 beginpos=Tools.BezierCurve(P0,P1,P2,dsada);
				Vector3 endpos=Tools.BezierCurve(P0,P1,P2,dsa);
				Handles.color = Color.red;
				Handles.DrawLine(beginpos,endpos);
				// fa[i]=Tools.BezierCurve(P0,P1,P2,dsa);
				// fa[i]=DDSSS;
				
			}
            if (diantype == diantype.parabola)
            {
                Handles.DrawLine(new Vector3(P0.x+(longlong*t),100,0),new Vector3(P0.x+(longlong*t),hheight,0));
            }
            else
            {
                Handles.DrawLine(new Vector3(P0.x + ((P2.x - P0.x) * t), 100, 0), new Vector3(P0.x + ((P2.x-P0.x )* t), hheight, 0));
            }
                
			// foreach (var item in fa)
			// {
			// 	Debug.Log(item);
			// }
			// Handles.color = Color.red;
			// Handles.DrawLines(fa);
				// Handles.DrawLines(fa);
				GUI.DragWindow();
			// 
		}
		void OnEnable()
        {
            titleContent = new GUIContent("Bizer参数编辑器");
            // loadWeapon();
        }

	}


}

