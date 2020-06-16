using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using FairyGUI;
using DG.Tweening;

namespace Tang
{
    public class PanelDownUI : MonoBehaviour, UIInterface
    {
        private UIPanel uiPanel;
        private GComponent ui;
        private GComponent paneldownComponent;
        private Transition transition;


        private void Start()
        {
            uiPanel = GetComponent<UIPanel>();
            ui = uiPanel.ui;
            Debug.Assert(ui != null);

            paneldownComponent = ui.GetChild("paneldown").asCom;

            Init();
        }

        public void Init()
        {
            Hide(false);

            SetTitle("");
            SetDesc("");


            SetLV("");

        }
            
        public bool IsVisible()
        {
            return paneldownComponent.visible;
        }
        
        //装备种类
        public void SetKind(string equip)
        {
            paneldownComponent.GetChild("kind").asTextField.text = equip;
        }
        
        //使用等级
        public void SetLV(string level)
        {
            paneldownComponent.GetChild("LV").asTextField.text = "使用等级" + level;
        }
        
        //装备图标
        public void SetIcon(string icon)
        {
            paneldownComponent.GetChild("icon").asCom.GetChild("icon").asLoader.url = "Textures/Icon/" + icon;
        }
        
        //装备名称
        public void SetTitle(string title)
        {
            paneldownComponent.GetChild("name").asTextField.text = "<font size = 32>" + title + "</font>";
        }
        
        //装备简介
        public void SetDesc(string desc)
        {
            paneldownComponent.GetChild("text").asTextField.text = desc;
        }
        
        //
        public void SetATK(string attribute, string value, string changevalue)
        {
            paneldownComponent.GetChild("n26").asRichTextField.text =
                attribute + " " + "[color=#ffffff]" + value + "[/color] "+ changevalue;
        }
        
        //
        public void SetACC(string attribute, string value, string changevalue)
        {
            paneldownComponent.GetChild("n27").asRichTextField.text =
                attribute + " " + "[color=#ffffff]" + value + "[/color] "+ changevalue;
        }
        
        //
        public void SetEND(string attribute, string value, string changevalue)
        {
            paneldownComponent.GetChild("n28").asRichTextField.text =
                attribute + " " + "[color=#ffffff]" + value + "[/color] " + changevalue;
        }

        int showState = 0;
        
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

                    paneldownComponent.visible = true;

                    transition = ui.GetTransition("Show");
                    transition.Play(() =>
                    {
                        showState = 2;
                    });
                }
            }
            else
            {
                paneldownComponent.visible = true;
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

                    paneldownComponent.visible = true;

                    transition = ui.GetTransition("Hide");
                    transition.Play(() =>
                    {
                        showState = 0;
                        paneldownComponent.visible = false;
                    });
                }
            }
            else
            {
                paneldownComponent.visible = false;
                showState = 0;
            }
        }

        public bool IsShow()
        {
            if(showState == 2)
                return true;
            else
                return false;
        }
    }
}