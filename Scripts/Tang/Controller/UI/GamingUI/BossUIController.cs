using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using FairyGUI;
using Tang;
using UnityEngine;
using UnityEngine.Serialization;

namespace ZS
{
    public class BossUIController : MonoBehaviour,UIInterface
    {
        Tweener tweener_progressBar_BossHp;
        Tweener tweener_progressBar_BossHpMax;
        GTweener gTweener_progressBar_fode;
        GProgressBar bossProgressBar;
        GLoader bossicon;
        GTextField bossname;
        GTextField bosshpg;
        GComponent bosshpcomponent;
        GComponent functionlist;
        
        private GComponent ui;

        private GComponent Ui
        {
            get
            {
                if (ui == null)
                {
                    ui = GetComponent<UIPanel>().ui;
                }
                return ui;
            }

        }
        
        private void Awake()
        {
            Init();
        }
        
        public void Init()
        {
            //boss血条监控
            bosshpcomponent = Ui.GetChild("BossHP").asCom;
            bossProgressBar = bosshpcomponent.GetChild("n0").asProgress;
            bossicon = bosshpcomponent.GetChild("n2").asLoader;
            bossname = bosshpcomponent.GetChild("n4").asTextField;
            bosshpg = bosshpcomponent.GetChild("HPName").asRichTextField;
            
            bosshpcomponent.visible = false;
        }
        
        private void OnEnable()
        {
            MessageManager.Instance.Subscribe(MessageName.SHOW_BOSS_HP_UI, ShowBossHp);
            MessageManager.Instance.Subscribe(MessageName.HIDE_BOSS_HP_UI, HideBossHp);
            
            MessageManager.Instance.Subscribe(MessageName.SET_PLAYER1_BOSS_HP, SetBossHp);
        }

        private void OnDisable()
        {
            MessageManager.Instance.Unsubscribe(MessageName.SHOW_BOSS_HP_UI, ShowBossHp);
            MessageManager.Instance.Unsubscribe(MessageName.HIDE_BOSS_HP_UI, HideBossHp);
            
            MessageManager.Instance.Unsubscribe(MessageName.SET_PLAYER1_BOSS_HP, SetBossHp);
        }

        private void ShowBossHp(object[] objects)
        {
            Show(false);
        }
        
        private void HideBossHp(object[] objects)
        {
            Hide(false);
        }

        private void SetBossHp(object[] objects)
        {
            float valueHpMax = ((float)objects[1]).Range(0, 9999999);
            float valueHP =  ((float)objects[0]).Range(0, valueHpMax);
            
            bosshpg.text = valueHP + "/" + valueHpMax;
            
            bossProgressBar.max = valueHpMax;

            if (tweener_progressBar_BossHp != null)
            {
                TweenExtensions.Kill(tweener_progressBar_BossHp);
            }
            tweener_progressBar_BossHp = bossProgressBar.DoValue(valueHP, 0.3f).OnComplete(() =>
            {
                tweener_progressBar_BossHp = null;
            });
        }

        public void Show(bool withAnim = true)
        {
            bosshpcomponent.visible = true;
        }

        public void Hide(bool withAnim = true)
        {
            if (withAnim)
            {
                if (gTweener_progressBar_fode != null)
                {
                    GTween.Kill(gTweener_progressBar_fode);
                }
                gTweener_progressBar_fode = bosshpcomponent.TweenFade(0f, 0.7f).OnComplete(()=> 
                {
                    bosshpcomponent.visible = false;
                    gTweener_progressBar_fode = null;
                });
            }
            else
            {
                bosshpcomponent.visible = false;
            }
        }

        public bool IsShow()
        {
            return bosshpcomponent.visible;
        }
    }
}