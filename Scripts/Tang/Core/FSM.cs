// #define FSM_Debug

using System;
using System.Collections.Generic;

namespace Tang
{
    public class FSM
    {
        public class State
        {
            public State(string name, Action beginAction, Action updateAction, Action endAction)
            {
                Id = name.GetHashCode();
                Name = name;
                OnBegin = beginAction;
                OnUpdate = updateAction;
                OnEnd = endAction;
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public Action OnBegin { get; set; }
            public Action OnUpdate { get; set; }
            public Action OnEnd { get; set; }
        }

        public class Event
        {
            public Event(string name, string from, string to, System.Func<bool> condition, System.Action action)
            {
                Id = name.GetHashCode();
                Name = name;
                FromId = from.GetHashCode();
                ToId = to.GetHashCode();
                FromName = from;
                ToName = to;
                Condition = condition;
                Action = action;
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public int FromId { get; set; }
            public int ToId { get; set; }
            public string FromName { get; set; }
            public string ToName { get; set; }
            public System.Func<bool> Condition { get; set; }
            public System.Action Action { get; set; }
        }

        int sendEventName = 0;
        int allStateName = "All".GetHashCode();
        int currStateId;
        string currStateName;
        public string CurrStateName { get { return currStateName; } }
        public void SetCurrStateName(string name)
        {
            currStateId = name.GetHashCode();
            currStateName = name;
        }

        State CurrState
        {
            get
            {
                State ret;
                if (StateDic.TryGetValue(currStateId, out ret))
                    return ret;
                else
                    return null;
            }
        }
        List<Event> EventList { get; set; }
        Dictionary<int, State> StateDic { get; set; }

        ActionPool actionPool = new ActionPool();

        public FSM()
        {
            EventList = new List<Event>();
            StateDic = new Dictionary<int, State>();
        }

        public State AddState(string stateName, Action onBegin = null, Action onUpdate = null, Action onEnd = null)
        {
            var ret = new State(stateName, onBegin, onUpdate, onEnd);
            StateDic.Add(ret.Id, ret);

            // 自动添加事件 add by TangJian 2018/01/12 23:54:26
            AddEvent("AllTo" + stateName, "All", stateName, () => { return false; });
            AddEvent("To" + stateName, "All", stateName, () => { return false; });
            return ret;
        }

        public Event AddEvent(string eventName, string fromStateName, string toStateName, System.Func<bool> condition = null, System.Action action = null)
        {
            if (condition == null)
            {
                condition = () => { return false; };
            }
            if (action == null)
            {
                action = () => { };
            }

            var ret = new Event(eventName, fromStateName, toStateName, condition, action);
            EventList.Add(ret);
            return ret;
        }

        public void SendEvent(string eventName)
        {
            sendEventName = eventName.GetHashCode();
        }

        public void Clear()
        {
            EventList.Clear();
        }

        public void Update()
        {
            var currState = CurrState;
            if (currState != null)
            {
                // 刷新当前状态 add by TangJian 2017/11/04 18:22:09
                if (currState.OnUpdate != null)
                {
#if FSM_Debug
                // Debug.Log(currStateName + ": OnUpdate");
#endif
                    actionPool.AddAction(currState.OnUpdate);
                }

                foreach (var e in EventList)
                {
                    if ((currStateId.Equals(e.FromId) || e.FromId == allStateName) && currStateId != e.ToId)
                    {
                        if (e.Condition() || e.Id == sendEventName)
                        {
#if FSM_Debug
                        Debug.Log(e.Name + ": Action");
#endif
                            actionPool.AddAction(e.Action);

                            // 当前状态结束 add by TangJian 2017/11/04 18:21:08
                            if (currState.OnEnd != null)
                            {
#if FSM_Debug
                            Debug.Log(currStateName + ": OnEnd");
#endif
                                actionPool.AddAction(currState.OnEnd);
                            }
                            currStateId = e.ToId;
                            currStateName = e.ToName;

                            // 进入新的状态 add by TangJian 2017/11/04 18:22:37
                            if (CurrState.OnBegin != null)
                            {
#if FSM_Debug
                            Debug.Log(currStateName + ": OnBegin");
#endif
                                actionPool.AddAction(CurrState.OnBegin);
                            }

                            // 成功一个条件跳出 add by TangJian 2017/11/09 14:38:30
                            break;
                        }
                    }
                }

                // 清空事件 add by TangJian 2017/11/06 16:01:19
                sendEventName = 0;

                actionPool.CallActions();
            }
        }
    }
}