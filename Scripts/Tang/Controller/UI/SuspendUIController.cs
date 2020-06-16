using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
namespace Tang
{
    public class SuspendUIController : MonoBehaviour, UIInterface
    {
		ValueMonitorPool valueMonitorPool = new ValueMonitorPool();
		GComponent ui;
		GComponent choiceframe;
		GComponent swordchoice;
		GRichTextField choice0;
		GRichTextField choice1;
		GRichTextField choice2;
		GRichTextField choice3;
		List<GRichTextField> choicelist=new List<GRichTextField>();
		GRichTextField choice4;
		public int choiceindex;

		public void Init(){
			valueMonitorPool.Clear();
			ui = gameObject.GetComponent<UIPanel>().ui;
			choiceframe=ui.GetChild("choiceframe").asCom;
			swordchoice=choiceframe.GetChild("swordchoice").asCom;
			choice0=choiceframe.GetChild("n0").asRichTextField;
			choice1=choiceframe.GetChild("n1").asRichTextField;
			choice2=choiceframe.GetChild("n2").asRichTextField;
			choice3=choiceframe.GetChild("n3").asRichTextField;
			choice4=choiceframe.GetChild("n4").asRichTextField;
			choicelist.Add(choice0);
			choicelist.Add(choice1);
			choicelist.Add(choice2);
			choicelist.Add(choice3);
			choicelist.Add(choice4);
			choiceframe.visible=false;
			choiceindex=0;
			valueMonitorPool.AddMonitor(() =>
                {
                    return choiceindex;
                }, (int from, int to) =>
                {
                    FocusIndex(choiceindex);

                });
				alphachoice(0);
		}
		void FocusIndex(int choice)
        {
            switch (choice)
            {
                case 0:
                    swordchoice.position = new Vector2(40, choice0.position.y - 6);
					alphachoice(0);
                    break;
                case 1:
                    swordchoice.position = new Vector2(40, choice1.position.y - 6);
					alphachoice(1);
                    break;
				case 2:
                    swordchoice.position = new Vector2(40, choice2.position.y - 6);
					alphachoice(2);
                    break;
				case 3:
                    swordchoice.position = new Vector2(40, choice3.position.y - 6);
					alphachoice(3);
                    break;
				case 4:
                    swordchoice.position = new Vector2(40, choice4.position.y - 6);
					alphachoice(4);
                    break;
            }
        }
		void alphachoice(int cindex){
			int i;
			for(i=0;i<5;i++){
				if(cindex==i){choicelist[i].alpha=1f;}else{choicelist[i].alpha=0.3f;}
			}
		}
		public void up()
        {
            choiceindex--;
            if (choiceindex < 0) { choiceindex = 4; }
        }
        public void down()
        {
            choiceindex++;
            if (choiceindex > 4) { choiceindex = 0; }
        }
		// public void Open(){
		// 	choiceframe.visible=true;
		// }
		// public void Close(){
		// 	choiceframe.visible=false;
		// }

		void Update() {
			valueMonitorPool.Update();
		}

        public void Show(bool withAnim = true)
        {
			choiceframe.visible=true;
        }

        public void Hide(bool withAnim = true)
        {
			choiceframe.visible=false;
        }

        public bool IsShow()
        {
            throw new System.NotImplementedException();
        }
    }

}

