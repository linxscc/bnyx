using UnityEngine;
using FairyGUI;

namespace Tang
{
    public class ChoiceBubbleController : MonoBehaviour, UIInterface
    {
        GComponent ui;
        GTextField n1;
        GTextField n2;
        GTextField n3;
        GImage choice;
        public int xuanzhe;

        GTweener choicebubbletweener;
        void Start()
        {
            Init();
        }
        public void Init()
        {
            ui = gameObject.GetComponent<UIPanel>().ui;
            n1 = ui.GetChild("n1").asTextField;
            n2 = ui.GetChild("n2").asTextField;
            n3 = ui.GetChild("n3").asTextField;
            choice = ui.GetChild("choice").asImage;
            ui.visible = false;
        }
        public void upchoice()
        {
            if (xuanzhe == 0)
            {
                xuanzhe = 2;
                choicemove(xuanzhe);
            }
            else
            {
                xuanzhe--;
                choicemove(xuanzhe);
            }

        }
        public void downchoice()
        {
            if (xuanzhe == 2)
            {
                xuanzhe = 0;
                choicemove(xuanzhe);
            }
            else
            {
                xuanzhe++;
                choicemove(xuanzhe);
            }
        }
        public void textnew(int index, string text)
        {
            switch (index)
            {
                case 0:
                    n1.text = text;
                    break;
                case 1:
                    n2.text = text;
                    break;
                case 2:
                    n3.text = text;
                    break;
            }
        }
        void choicemove(int index)
        {
            switch (index)
            {
                case 0:
                    choice.position = new Vector2(n1.position.x, n1.position.y);
                    break;
                case 1:
                    choice.position = new Vector2(n2.position.x, n2.position.y);
                    break;
                case 2:
                    choice.position = new Vector2(n3.position.x, n3.position.y);
                    break;
            }

        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                upchoice();

            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                downchoice();
            }
        }

        public void Show(bool withAnim = true)
        {
            ui.visible = true;
            if (choicebubbletweener != null)
            {
                GTween.Kill(choicebubbletweener);
            }
            choicebubbletweener = ui.TweenFade(1, 0.2f).OnComplete(() =>
            {
                choicebubbletweener = null;
            });
        }

        public void Hide(bool withAnim = true)
        {
            if (choicebubbletweener != null)
            {
                GTween.Kill(choicebubbletweener);
            }
            choicebubbletweener = ui.TweenFade(0, 0.2f).OnComplete(() =>
            {
                ui.visible = false;
                choicebubbletweener = null;
            });
        }

        public bool IsShow()
        {
            throw new System.NotImplementedException();
        }
    }
}

