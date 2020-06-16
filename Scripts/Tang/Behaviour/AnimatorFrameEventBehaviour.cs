using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Tang.FrameEvent;


public class AnimatorFrameEventBehaviour : StateMachineBehaviour
{
    [SerializeField] public string Parameter;
    [SerializeField] public int CurrnameIndex;
    [SerializeField] public List<string> NameList = new List<string>();
    [SerializeField] public string AnimName = "";

    SkeletonAnimator _skeletonAnimator;
    int _currAnimEventIndex = 0;
    float _loopTimes = 0;

    void LazyInit(Animator animator)
    {
        _skeletonAnimator = animator.gameObject.GetComponent<SkeletonAnimator>();
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
        if (animName == null)
            return;

        // Debug.Log("开始动画:" + animName);


        _currAnimEventIndex = 0;
        _loopTimes = 0;
    }

    void OnAnimUpdate(Animator animator, string animName)
    {
        if (animName == null || GetCurrAnimName(animator) != animName)
            return;

        // Debug.Log("刷新动画:" + animName);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        ExecuteAnimFrameEvent(animator, animName);

        if (stateInfo.loop && stateInfo.normalizedTime >= _loopTimes + 1)
        {
            _currAnimEventIndex = 0;
            _loopTimes++;
        }
    }

    void OnAnimEnd(Animator animator, string animName)
    {
        if (animName == null)
            return;

        ForceExecuteAnimFrameEvent(animator, animName);
        //Debug.Log("结束动画:" + animName);
    }

    public void ExecuteAnimFrameEvent(Animator animator, string animName)
    {
        if (_skeletonAnimator == null) return;


        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        List<FrameEventData> events;
        if (_skeletonAnimator.FrameEventDic.TryGetValue(animName, out events))
        {
            if (events.Count > 0)
            {
                if (_currAnimEventIndex < events.Count)
                {
                    for (int i = _currAnimEventIndex; i < events.Count; i++)
                    {
                        if (_currAnimEventIndex == i)
                        {
                            var e = events[i];
                            if (e != null)
                            {
                                // Debug.Log("stateInfo.normalizedTime = " + stateInfo.normalizedTime + ", e.time = " + e.time + ", loopTimes = " + loopTimes);
                                if (stateInfo.normalizedTime >= _loopTimes + e.time)
                                {
                                    _currAnimEventIndex++;
                                    if (e.type == "String")
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

    public void ForceExecuteAnimFrameEvent(Animator animator, string animName)
    {
        if (_skeletonAnimator == null) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        List<FrameEventData> events;
        if (_skeletonAnimator.FrameEventDic.TryGetValue(animName, out events))
        {
            if (events.Count > 0)
            {
                if (_currAnimEventIndex < events.Count)
                {
                    for (int i = _currAnimEventIndex; i < events.Count; i++)
                    {
                        var e = events[i];
                        if (e != null)
                        {
                            if (e.forceExecute)
                            {
                                if (e.type == "String")
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
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        LazyInit(animator);
        
        if (Parameter != null && Parameter != "" && NameList.Count != 0)
        {
            float findex = animator.GetFloat(Parameter);
            int index = (int)Mathf.Round(findex);
            CurrnameIndex = index;
            if (NameList[index] != "null")
            {
                OnAnimBegin(animator, NameList[index]);
            }
        }
        else
        {

            OnAnimBegin(animator, AnimName);
        }

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        LazyInit(animator);
        
        if (Parameter != null && Parameter != "" && NameList.Count != 0)
        {
            if (NameList[CurrnameIndex] != "null")
            {
                OnAnimEnd(animator, NameList[CurrnameIndex]);
            }
        }
        else
        {
            OnAnimEnd(animator, AnimName);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (Parameter != null && Parameter != "" && NameList.Count != 0)
        {
            float findex = animator.GetFloat(Parameter);
            int index = (int)Mathf.Round(findex);
            if (CurrnameIndex == index)
            {
                if (NameList[index] != "null")
                {
                    OnAnimUpdate(animator, NameList[index]);
                }
            }
            else
            {
                if (NameList[CurrnameIndex] != "null")
                {
                    OnAnimEnd(animator, NameList[CurrnameIndex]);
                }
                if (NameList[index] != "null")
                {
                    OnAnimBegin(animator, NameList[index]);
                }

                CurrnameIndex = index;
            }

        }
        else
        {
            OnAnimUpdate(animator, AnimName);
        }

    }
}