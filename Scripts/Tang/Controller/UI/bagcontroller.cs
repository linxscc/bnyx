using UnityEngine;
using FairyGUI;

namespace Tang{
	public class bagcontroller :MonoBehaviour
	{
		GList list;
		//GList sd;
		GComponent fdas;
		void Start() {
			var ui=GetComponent<UIPanel>().ui;
			//fdas=ui.GetChild("n3").asCom;
			//sd=fdas.GetChild("n0").asList;
			list=ui.GetChild("rightlist").asList;
			list.itemRenderer = RenderListItem;
			//sd.itemRenderer=dfsd;
			list.numItems = 24;
			//sd.numItems = 10;
			list.onClickItem.Add(_onclickitem);
			//sd.onClickItem.Add(_onclicki);
		}
		void RenderListItem(int index, GObject obj){
			GButton button = obj.asButton;
		}
		void dfsd(int index, GObject obj){
			GButton button = obj.asButton;
		}
		void _onclickitem(EventContext eventContext){
			GObject sdfsad=(GObject)eventContext.data;
            int inda =list.GetChildIndex(sdfsad);
            Debug.Log(""+inda);
		}
		void _onclicki(EventContext eventContext){
			GObject sdfsad=(GObject)eventContext.data;
            //int inda =sd.GetChildIndex(sdfsad);
            //Debug.Log(""+inda);
		}
	}
}