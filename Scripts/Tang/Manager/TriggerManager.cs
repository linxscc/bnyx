using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public class TriggerManager : MonoBehaviour
    {
        private static TriggerManager instance;

        public static TriggerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MainManager.GetInstance().GetManager<TriggerManager>();
                }
                return instance;
            }
        }

        public Dictionary<TriggerController, List<TriggerController>> triggerControllerDic = new Dictionary<TriggerController, List<TriggerController>>();
    }



    // 扩展方法 add by TangJian 2018/12/2 17:26
    public static class TriggerManagerExtensions
    {
        public static void Add(this TriggerManager target, TriggerController triggerController)
        {
            target.triggerControllerDic.Add(triggerController, new List<TriggerController>() { });
        }

        public static void Remove(this TriggerManager target, TriggerController selfTriggerController)
        {
            {
                // 获得将要移除的triggerController所有记录的triggerController add by TangJian 2018/12/2 16:53
                var triggerControllers = target.GetRecordList(selfTriggerController);
                // 依次执行OnTriggerExit方法 add by TangJian 2018/12/2 16:53
                List<System.Action> actions = new List<System.Action>();
                foreach (var otherTriggerController in triggerControllers)
                    actions.Add(() =>
                    {
                        if (selfTriggerController != null && otherTriggerController != null)
                            selfTriggerController.OnTriggerExit(otherTriggerController.triggerCollider);

                        if (otherTriggerController != null && selfTriggerController != null)
                            otherTriggerController.OnTriggerExit(selfTriggerController.triggerCollider);
                    });

                foreach (var action in actions)
                    action();
            }

            target.triggerControllerDic.Remove(selfTriggerController);
        }

        public static List<TriggerController> GetRecordList(this TriggerManager target, TriggerController triggerController)
        {
            List<TriggerController> recordList;
            if (target.triggerControllerDic.TryGetValue(triggerController, out recordList))
            {
                return recordList;
            }
            return null;
        }

        public static void AddRecord(this TriggerManager target, TriggerController selfTriggerController, TriggerController otherTriggerController)
        {
            List<TriggerController> gameObjects;
            if (target.triggerControllerDic.TryGetValue(selfTriggerController, out gameObjects))
            {
                gameObjects.Add(otherTriggerController);
            }
            else
            {
                target.triggerControllerDic.Add(selfTriggerController, new List<TriggerController>() { otherTriggerController });
            }
        }

        public static void RemoveRecord(this TriggerManager target, TriggerController selfTriggerController, TriggerController otherTriggerController)
        {
            List<TriggerController> gameObjects;
            if (target.triggerControllerDic.TryGetValue(selfTriggerController, out gameObjects))
            {
                gameObjects.Remove(otherTriggerController);
            }
            else
            {
                Debug.Log("没有记录的触发器: name = " + selfTriggerController.name);
            }
        }
    }
}