using UnityEngine;
using FairyGUI;

namespace Tang
{
    public class RolePromptController : MonoBehaviour
    {
        public GameObject parent;
        ValueMonitorPool valueMonitorPool = new ValueMonitorPool();
        GLoader prompt;
        TriggerController selfTriggerController;
        TriggerController otherTriggerController;
        TreasureBoxController treasureBox;


        void Start()
        {
            var ui = this.GetComponent<UIPanel>().ui;
            prompt = ui.GetChild("n0").asLoader;
            prompt.rotation = -40f;
            Prompt();
            //parent=GameObject.Find("Player1");


        }
        public void Init(){
            //parent=GameObject.Find("Player1");
        }
        void Prompt()
        {
            selfTriggerController = parent.GetComponentInChildren<TriggerController>();

            valueMonitorPool.Clear();
            valueMonitorPool.AddMonitor(() =>
            {
                otherTriggerController = selfTriggerController.GetFirstKeepingTriggerController();

                if (otherTriggerController != null)
                {
                    DoorController doorController = otherTriggerController.ITriggerDelegate as DoorController;
                    NpcController npcController = otherTriggerController.ITriggerDelegate as NpcController;
                    TreasureBoxController treasureBox = otherTriggerController.ITriggerDelegate as TreasureBoxController;
                    JoystickController joystickController = otherTriggerController.ITriggerDelegate as JoystickController;
                    DropItemController dropItemController = otherTriggerController.ITriggerDelegate as DropItemController;
                    MagicPortalController magicPortalController =
                        otherTriggerController.ITriggerDelegate as MagicPortalController;
                    
                    if (doorController != null)
                    {
                        if (doorController.IsClosed())
                            return 0;
                        else
                            return 1;
                    }
                    else if (npcController != null)
                    {
                        return 2;
                    }
                    else if (treasureBox != null)
                    {
                        if (treasureBox.IsOpened())
                            return 3;
                        else
                            return 4;
                    }else if (joystickController != null)
                    {
                        return 5;
                    }
                    else if(dropItemController!=null)
                    {
                        return 6;
                    }
                    else if(magicPortalController != null)
                    {
                        return 7;
                    }
                }
                return -1;
            }, (int from, int to) =>
            {
                if (otherTriggerController != null)
                {
                    GameObject interactObject = otherTriggerController.ITriggerDelegate.GetGameObject();
                    if (interactObject != null)
                    {
                        DoorController doorController = interactObject.GetComponent<DoorController>();
                        NpcController npcController = interactObject.GetComponent<NpcController>();
                        TreasureBoxController treasureBox = interactObject.GetComponent<TreasureBoxController>();
                        JoystickController joystickController = interactObject.GetComponent<JoystickController>();
                        DropItemController dropItemController = interactObject.GetComponent<DropItemController>();
                        MagicPortalController magicPortalController = interactObject.GetComponent<MagicPortalController>();

                        if (doorController != null)
                        {
                            if (doorController.IsClosed())
                                ShowPrompt("ui://UI/key");
                            else
                                HidePrompt();
                        }
                        else if (npcController != null)
                        {
                            ShowPrompt("ui://UI/talkmark");
                        }
                        else if (treasureBox != null)
                        {
                            if (treasureBox.IsOpened())
                                HidePrompt();
                            else
                                ShowPrompt("ui://UI/Exclamatory mark");
                        }else if (joystickController)
                        {
                            ShowPrompt("ui://UI/key");
                        }else if (dropItemController != null)
                        {
                            if(dropItemController.ItemData.pickUpMethod == PickUpMethod.Interact)
                                ShowPrompt("ui://UI/Exclamatory mark");
                        }
                        else if (magicPortalController != null)
                        {
                            ShowPrompt("ui://UI/Exclamatory mark");
                        }
                        else
                        {
                            HidePrompt();
                        }
                    }
                }
                else
                {
                    HidePrompt();
                }
            });

        }

        // 

        public void ShowPrompt(string url)
        {
            prompt.visible = true;
            prompt.url = url;
            AnimManager.Instance.Grotation(prompt, 0f, 0.2f, 0.2f, true);
        }

        public void HidePrompt()
        {
            AnimManager.Instance.Grotation(prompt, 0f, 0.1f, 0.1f, false);
        }

        public void killtalk()
        {
            AnimManager.Instance.Grotation(prompt, 0f, 0.2f, 0.2f, false);
        }


        void Update()
        {
            valueMonitorPool.Update();
        }
    }
}