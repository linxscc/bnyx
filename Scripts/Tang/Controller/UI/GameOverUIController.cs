using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Spine.Unity;

namespace Tang
{
    public class GameOverUIController : MonoBehaviour,UIInterface
    {
        ValueMonitorPool valueMonitorPool = new ValueMonitorPool();
        GButton RestartG;
        GComponent ui;
        GComponent GameOverChoiceframe;
        GComponent swordchoice;
        GRichTextField choice0;
        GRichTextField choice1;
        public int panduanindex;
		List<GRichTextField> choicelist=new List<GRichTextField>();

        // Use this for initialization
        public void Init()
        {
            panduanindex = 0;
            valueMonitorPool.Clear();
            gameObject.SetActive(false);
            ui = gameObject.GetComponent<UIPanel>().ui;
            RestartG = ui.GetChildWithPath("n1").asButton;
            RestartG.GetChild("text").text = "重新开始";
            RestartG.onClick.Add(() =>
                    {
                        GameStart.Instance.ReloadGame(); 
                    });
            

            GameOverChoiceframe = ui.GetChild("GameOverChoiceframe").asCom;
            swordchoice = GameOverChoiceframe.GetChildWithPath("swordchoice").asCom;
            choice0 = GameOverChoiceframe.GetChild("n4").asRichTextField;
            choice1 = GameOverChoiceframe.GetChild("n5").asRichTextField;
            choicelist.Add(choice0);
            choicelist.Add(choice1);
            valueMonitorPool.AddMonitor(() =>
                {
                    return panduanindex;
                }, (int from, int to) =>
                {
                    FocusIndex(panduanindex);
                });
            alphachoice(0);  
			choice0.onClick.Add(() => {FocusIndex(0);Restart();});
            choice1.onClick.Add(() =>{FocusIndex(1);Debug.Log("out");});

            #region //关闭you die界面

            UIManager.Instance.CreatSUIObj<SkeletonAnimation>("OverSpine01").gameObject.SetActive(false);
            UIManager.Instance.CreatSUIObj<SkeletonAnimation>("OverSpine02").gameObject.SetActive(false);
            
            #endregion
            
        }
        void FocusIndex(int choice)
        {
            switch (choice)
            {
                case 0:
                    swordchoice.position = new Vector2(40, choice0.position.y - 4);
					alphachoice(0);
                    break;
                case 1:
                    swordchoice.position = new Vector2(40, choice1.position.y - 4);
					alphachoice(1);
                    break;
            }
        }
		void alphachoice(int cindex){
			int i;
			for(i=0;i<2;i++){
				if(cindex==i){choicelist[i].alpha=1f;}else{choicelist[i].alpha=0.3f;}
			}
			//choicelist[cindex].alpha=1f;

		}
        public void up()
        {
            panduanindex--;
            if (panduanindex < 0) { panduanindex = 1; }
        }
        public void down()
        {
            panduanindex++;
            if (panduanindex > 1) { panduanindex = 0; }
        }
        public void Restart()
        {
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var activeSceneName = activeScene.name;
            UnityEngine.SceneManagement.SceneManager.LoadScene(activeSceneName);
        }

        // Update is called once per frame
        void Update()
        {
            valueMonitorPool.Update();
            //RRestart();
        }

        public void Show(bool withAnim = true)
        {

        }

        public void Hide(bool withAnim = true)
        {
            
        }

        public bool IsShow()
        {
            throw new System.NotImplementedException();
        }
    }
}