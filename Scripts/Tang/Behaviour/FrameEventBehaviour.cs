using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject;
using UnityEngine;
using Spine.Unity;
using Tang;
using Tang.FrameEvent;

public class FrameEventBehaviour : StateMachineBehaviour
{
    SkeletonAnimator skeletonAnimator;
    RoleController roleController;
    BaseSkillController baseSkillController;
    int currAnimEventIndex = 0;
    float loopTimes = 0;
    private bool enterNewAnim = false;

    [SerializeField] public string Parameter;
    [SerializeField] public int CurrnameIndex;
    [SerializeField] public List<string> namelist = new List<string>();
    [SerializeField] public List<float> floatList = new List<float>();
    [SerializeField] public string animName = "";

    [SerializeField] public float beginTime = 0;
    [SerializeField] public float endTime = 1;
    
    bool valid = false;

    private List<CastSkillEventBehaviour> castSkillEventBehaviourList = new List<CastSkillEventBehaviour>();

    private void LazyInit(Animator animator)
    {
        baseSkillController = animator.GetComponentInParent<Tang.BaseSkillController>();
        roleController = animator.GetComponentInParent<Tang.RoleController>();
        skeletonAnimator = animator.gameObject.GetComponent<SkeletonAnimator>();

        valid = skeletonAnimator != null && skeletonAnimator.FrameEventDic != null;
    }

  
    string GetCurrAnimName(Animator animator)
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            var ci = clipInfo[0];
            var animName = ci.clip.name;
            return animName;
        }
        return "";
    }
    
    void OnAnimBegin(Animator animator, string animName)
    {
        if (!valid) return;

        if (animName == null)
            return;

        // Debug.Log("开始动画:" + animName);


        currAnimEventIndex = 0;
        loopTimes = 0;
        enterNewAnim = true;
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        ExecuteAnimFrameEvent(animator, animName);
    }

    void OnAnimUpdate(Animator animator, string animName)
    {
        if (!valid) return;

        if (animName == null || GetCurrAnimName(animator) != animName)
            return;

        // 每次进入新的动画 增加动画改变次数 add by TangJian 2018/12/19 17:07
        if (enterNewAnim)
        {
            enterNewAnim = false;
            skeletonAnimator.FrameEventBatchId++;
        }

        // Debug.Log("刷新动画:" + animName);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        ExecuteAnimFrameEvent(animator, animName);

        if (stateInfo.loop && stateInfo.normalizedTime >= loopTimes + 1)
        {
            currAnimEventIndex = 0;
            loopTimes++;
        }
    }

    void OnAnimEnd(Animator animator, string animName)
    {
        if (!valid) return;

        if (animName == null)
            return;

        ForceExecuteAnimFrameEvent(animator, animName);
        //Debug.Log("结束动画:" + animName);
    }

    public void ExecuteAnimFrameEvent(Animator animator, string animName)
    {
        if (!valid) return;

        if (skeletonAnimator == null) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        List<FrameEventData> events;
        if (skeletonAnimator.FrameEventDic.TryGetValue(animName, out events))
        {
            // 帧事件排序
            events.Sort((data, eventData) =>
            {
                if (data.time > eventData.time)
                {
                    return 1;
                }
                else if (data.time < eventData.time)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            });
            
            if (events.Count > 0)
            {
                if (currAnimEventIndex < events.Count)
                {
                    for (int i = currAnimEventIndex; i < events.Count; i++)
                    {
                        if (currAnimEventIndex == i)
                        {
                            var e = events[i];
                            if (e != null)
                            {
                                if (e.time >= beginTime && e.time <= endTime && stateInfo.normalizedTime >= loopTimes + e.time)
                                {
                                    currAnimEventIndex++;
                                    if (e.type == "String")
                                    {
                                        if (e.EventData != null)
                                        {
                                            animator.gameObject.SendMessage("JObject", e);
                                        }
                                        else
                                        {
                                            animator.gameObject.SendMessage(e.name, e.String);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void ForceExecuteAnimFrameEvent(Animator animator, string animName)
    {
        if (!valid) return;

        if (skeletonAnimator == null) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        List<FrameEventData> events;
        if (skeletonAnimator.FrameEventDic.TryGetValue(animName, out events))
        {
            if (events.Count > 0)
            {
                if (currAnimEventIndex < events.Count)
                {
                    for (int i = currAnimEventIndex; i < events.Count; i++)
                    {
                        var e = events[i];
                        if (e != null)
                        {
                            if (e.time >= beginTime && e.time <= endTime)
                            {
                                if (e.forceExecute)
                                {
                                    if (e.type == "String")
                                    {
                                        if (e.EventData != null)
                                        {
                                            animator.gameObject.SendMessage("JObject", e);
                                        }
                                        else
                                        {
                                            animator.gameObject.SendMessage(e.name, e.String);
                                        }
                                    }
                                }
                                else if(stateInfo.normalizedTime >= loopTimes + e.time)
                                {
                                    if (e.type == "String")
                                    {
                                        if (e.EventData != null)
                                        {
                                            animator.gameObject.SendMessage("JObject", e);
                                        }
                                        else
                                        {
                                            animator.gameObject.SendMessage(e.name, e.String);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    override public void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        LazyInit(animator);

        if (skeletonAnimator == null) return;

        if (roleController != null)
        {
            roleController.DamageOnlyID = Tang.Tools.getOnlyId().ToString();
        }
        if (baseSkillController != null)
        {
            baseSkillController.DamageOnlyID = Tang.Tools.getOnlyId().ToString();
        }



        if (Parameter != null && Parameter != "" && namelist.Count != 0)
        {
            float findex = animator.GetFloat(Parameter);
            int index = (int)Mathf.Round(findex);


            ConfirmKey(ref  index);
            CurrnameIndex = index;
            if (namelist[index] != "null")
            {
                OnAnimBegin(animator, namelist[index]);
            }
        }
        else
        {

            OnAnimBegin(animator, animName);
        }


        animupdate(animator);

    }

    private void ConfirmKey(ref int index)
    {
        
        for (int i = 0; i < floatList.Count; i++)
        {
            if (Math.Truncate(floatList[i])== index)
            {
                index = i;
                break;
            }
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        LazyInit(animator);
        if (Parameter != null && Parameter != "" && namelist.Count != 0)
        {
            if (namelist[CurrnameIndex].IsNullOrEntry() == false)
            {
                OnAnimEnd(animator, namelist[CurrnameIndex]);
            }
        }
        else
        {
            OnAnimEnd(animator, animName);
        }
    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (Parameter != null && Parameter != "" && namelist.Count != 0)
        {
            float findex = animator.GetFloat(Parameter);
            int index = (int)Mathf.Round(findex);
            ConfirmKey(ref  index);
            if (CurrnameIndex == index)
            {
                if (namelist[index] != "null")
                {
                    OnAnimUpdate(animator, namelist[index]);
                }
            }
            else
            {
                if (namelist[CurrnameIndex] != "null")
                {
                    OnAnimEnd(animator, namelist[CurrnameIndex]);
                }
                if (namelist[index] != "null")
                {
                    OnAnimBegin(animator, namelist[index]);
                }

                CurrnameIndex = index;
            }

        }
        else
        {
            OnAnimUpdate(animator, animName);
        }

    }
    void animupdate(Animator animator)
    {
        if (Parameter != null && Parameter != "" && namelist.Count != 0)
        {
            float findex = animator.GetFloat(Parameter);
            int index = (int)Mathf.Round(findex);
            ConfirmKey(ref  index);
            if (CurrnameIndex == index)
            {
                if (namelist[index] != "null")
                {
                    OnAnimUpdate(animator, namelist[index]);
                }
            }
            else
            {
                if (namelist[CurrnameIndex] != "null")
                {
                    OnAnimEnd(animator, namelist[CurrnameIndex]);
                }
                if (namelist[index] != "null")
                {
                    OnAnimBegin(animator, namelist[index]);
                }

                CurrnameIndex = index;
            }

        }
        else
        {
            OnAnimUpdate(animator, animName);
        }
    }
}