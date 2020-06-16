using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public enum TriggerMode
    {
        Default = 0, // 默认模式
        Transmit = 1 // 转发触发事件到ITriggerDelegate
    }
    public class TriggerController : MyMonoBehaviour
    {
        [SerializeField] private ITriggerDelegate itriggerDelegate;
        virtual public ITriggerDelegate ITriggerDelegate
        {
            get
            {
                if (itriggerDelegate == null)
                {
                    itriggerDelegate = GetComponentInParent<ITriggerDelegate>();
                    if (itriggerDelegate == null)
                    {
                        itriggerDelegate = GetComponent<ITriggerDelegate>();
                        if (itriggerDelegate == null)
                        {
                            if (transform.parent != null && transform.parent.parent != null)
                            {
                                itriggerDelegate = transform.parent.parent.GetComponent<ITriggerDelegate>();
                            }
                        }
                    }
                }
                return itriggerDelegate;
            }
            set { itriggerDelegate = value; }
        }

        public string id = Tools.getOnlyId().ToString();

        public bool needKeepingGameObject = false;
        private List<GameObject> keepingGameObjectList = new List<GameObject>();
        private bool isReady = true;
        public TriggerMode triggerMode = TriggerMode.Default;
        public Collider triggerCollider;

        public event Action<TriggerEvent> OnTriggerInEvent;
        public event Action<TriggerEvent> OnTriggerOutEvent;
       
        
        
        public void Init()
        {
            triggerCollider = GetComponent<Collider>();
            isReady = true;
        }

        public virtual void OnEnable()
        {
            Init();

            TriggerManager.Instance.Add(this);
        }

        public virtual void OnDisable()
        {
            TriggerManager.Instance.Remove(this);
        }

        // 移除所有记录的游戏对象 add by TangJian 2018/05/11 17:18:21
        public void RemoveAllKeepingGameObject()
        {
            keepingGameObjectList.Clear();
        }

        public T GetFirstComponent<T>() where T : class
        {
            List<TriggerController> triggerControllers = TriggerManager.Instance.GetRecordList(this);
            T retComponent = null;
                
            foreach (var triggerController in triggerControllers)
            {
                if (triggerController.ITriggerDelegate != null && triggerController.ITriggerDelegate.gameObject != null)
                {
                    retComponent = triggerController.ITriggerDelegate.gameObject.GetComponent<T>();
                    if (retComponent != null)
                        break;   
                }
            }
            return retComponent;
        }

        public TriggerController GetFirstKeepingTriggerController()
        {
            List<TriggerController> triggerControllers = TriggerManager.Instance.GetRecordList(this);
            if (triggerControllers != null)
            {
                return triggerControllers.Find(controller => controller != null);
            }
            return null;
        }

        // 得到第一个保持的对象 add by TangJian 2017/07/27 18:03:47
        public GameObject GetFirstKeepingGameObject()
        {
            TriggerController triggerController = GetFirstKeepingTriggerController();

            if (triggerController != null && triggerController.ITriggerDelegate != null)
                return triggerController.ITriggerDelegate.GetGameObject();

            return null;
        }

        // 进入触发器 add by TangJian 2017/07/27 17:52:41
        public void OnTriggerEnter(Collider other)
        {
            
            if (triggerMode == TriggerMode.Default)
            {
                if (isReady == false)
                    return;

                if (TagTriggerManager.Instance.FindTirggerBool(this.gameObject.tag, other.gameObject.tag))
                {
                    //Debug.Log("OnTriggerEnter");
                    

                    TriggerController otherTriggerController = other.gameObject.GetComponent<TriggerController>();

                    // 记录触发器 add by TangJian 2018/12/2 15:41
                    TriggerManager.Instance.AddRecord(this, otherTriggerController);
                    
                    // 获取双方代理 add by TangJian 2019/3/19 18:21
                    ITriggerDelegate selfITriggerDelegate = ITriggerDelegate;
                    ITriggerDelegate otherITriggerDelegate = otherTriggerController.ITriggerDelegate;
                    
                    // 获得碰撞位置 add by TangJian 2019/3/19 18:21
                    Bounds inbounds = other.bounds.IntersectsExtend(triggerCollider.bounds);
                    Vector3 collidePoint = inbounds.center;
                    
                    // 组织触发器事件 add by TangJian 2019/3/19 18:22
                    TriggerEvent triggerEvent = new TriggerEvent();
                    triggerEvent.type = TriggerEventType.NewTrigger;
                    triggerEvent.colider = other;
                    triggerEvent.colidePoint = collidePoint;
                    triggerEvent.selfTriggerController = this;
                    triggerEvent.otherTriggerController = otherTriggerController;
                    
//                    if (other.gameObject.tag == gameObject.tag&& other.gameObject.GetComponentInParent<RoleController>().RoleData.AttrData.isCrash)
//                    {
//                        gameObject.GetComponent<Rigidbody>().AddForce(collidePoint-gameObject.GetComponent<Collider>().bounds.center);
//                    }
                    
                    if(selfITriggerDelegate != null)
                        selfITriggerDelegate.OnTriggerIn(triggerEvent);
                    
                    if (OnTriggerInEvent != null)
                    {
                        OnTriggerInEvent(triggerEvent);
                    }
                }
            }
            else if (triggerMode == TriggerMode.Transmit)
            {
                TriggerEvent triggerEvent = new TriggerEvent();
                triggerEvent.colider = other;
                if (ITriggerDelegate != null)
                {
                    ITriggerDelegate.OnTriggerIn(triggerEvent);
                    
                    if (OnTriggerInEvent != null)
                    {
                        OnTriggerInEvent(triggerEvent);
                    }
                }
            }
        }

        // 离开触发器 add by TangJian 2017/07/27 17:52:49        
        public void OnTriggerExit(Collider other)
        {
            if (triggerMode == TriggerMode.Default)
            {
                if (isReady == false)
                    return;

                if (TagTriggerManager.Instance.FindTirggerBool(this.gameObject.tag, other.gameObject.tag))
                {
                    //Debug.Log("OnTriggerExit");

                    TriggerController otherTriggerController = other.gameObject.GetComponent<TriggerController>();

                    // 移除触发器 add by TangJian 2018/12/2 15:41
                    TriggerManager.Instance.RemoveRecord(this, otherTriggerController);
                    
                    // 获取双方代理 add by TangJian 2019/3/19 18:21
                    ITriggerDelegate selfITriggerDelegate = ITriggerDelegate;
                    ITriggerDelegate otherITriggerDelegate = otherTriggerController.ITriggerDelegate;
                    
                    // 组织触发器事件 add by TangJian 2019/3/19 18:22
                    TriggerEvent triggerEvent = new TriggerEvent();
                    triggerEvent.type = TriggerEventType.NewTrigger;
                    triggerEvent.colider = other;
                    triggerEvent.selfTriggerController = this;
                    triggerEvent.otherTriggerController = otherTriggerController;
                    
                    if(selfITriggerDelegate != null)
                        selfITriggerDelegate.OnTriggerOut(triggerEvent);
                            
                    if (OnTriggerOutEvent != null)
                    {
                        OnTriggerOutEvent(triggerEvent);
                    }
                }
            }
            else if (triggerMode == TriggerMode.Transmit)
            {
                TriggerEvent triggerEvent = new TriggerEvent();
                triggerEvent.colider = other;
                ITriggerDelegate.OnTriggerOut(triggerEvent);
                
                if (OnTriggerOutEvent != null)
                {
                    OnTriggerOutEvent(triggerEvent);
                }
            }
        }
    }
}