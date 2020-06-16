using UnityEngine;
using FairyGUI;
namespace Tang
{
    public class TalkBubbleUIController : MonoBehaviour
    {
		GRichTextField textField;
		GImage bg;
		GImage jiao;
		void Start() 
		{
			var ui=GetComponent<UIPanel>().ui;
			textField=ui.GetChild("n2").asRichTextField;
			bg=ui.GetChild("n1").asImage;
			jiao=ui.GetChild("n0").asImage;
			tiaozheng();
		}
		void tiaozheng(){
			bg.SetSize(textField.size.x+60,textField.size.y+60);
			jiao.SetXY(bg.size.x/2,bg.size.y-4);
		}
    }

}
