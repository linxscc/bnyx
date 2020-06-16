using UnityEngine;
using FairyGUI;

namespace Tang
{
    public class PaneldownUIController : MonoBehaviour, UIInterface
    {
        GTweener showAndHideTweeneralphadown;
        GTweener showAndHideTweenerdown;
		GComponent paneldown;
		GTextField namename;
        GTextField miaoshu;
		float paneldownY;
        void Start() {
            Init();
        }
		public void Init(){
			var ui = this.GetComponent<UIPanel>().ui;
			// paneldown = ui.GetChild("paneldown").asCom;
            paneldown = ui.GetChild("RoleInfodown").asCom;
			namename = paneldown.GetChild("name").asTextField;
            miaoshu = paneldown.GetChild("n2").asTextField;
			paneldownY=paneldown.y;
			paneldown.visible = false;
            Hide(true);
		}
		
        public void paneldowntext(string name, string desc)
        {
            namename.text = "" + name.ToString();
            miaoshu.text = "" + desc.ToString();
        }
		void killtweener(GTweener tweener)
        {
            if (tweener != null)
            {
                GTween.Kill(tweener);
                tweener = null;
            }
            else
            {

            }

        }

        int showState = 0;
        private Transition transition;
        GComponent ui;

        public void Show(bool withAnim = true)
        {
            if (transition != null)
            {
                transition.Stop();
            }

            if (withAnim)
            {
                // if (showState == 0)
                {
                    showState = 1;

                    paneldown.visible = true;

                    transition = ui.GetTransition("Show");
                    transition.Play(() =>
                    {
                        showState = 2;
                    });
                }
            }
            else
            {
                paneldown.visible = true;
                showState = 2;
            }


        }

        public void Hide(bool withAnim = true)
        {
            if (transition != null)
            {
                transition.Stop();
            }

            if (withAnim)
            {
                if (showState == 2 || showState == 1)
                {
                    showState = 1;

                    paneldown.visible = true;

                    transition = ui.GetTransition("Hide");
                    transition.Play(() =>
                    {
                        showState = 0;
                        paneldown.visible = false;
                    });
                }
            }
            else
            {
                paneldown.visible = false;
                showState = 0;
            }
        }

        public bool IsShow()
        {
            throw new System.NotImplementedException();
        }
    }
}

