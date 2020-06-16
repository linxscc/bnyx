using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Tang
{

    public class ThreadWork
    {

        Thread thread;

        object mainThreadActionsLock = new object();
        ICollection<Action> mainThreadActions = new LinkedList<Action>();

        object subThreadActionsLock = new object();
        ICollection<Action> subThreadActions = new LinkedList<Action>();


        IEnumerator enumerator;

        public void AddMainThreadAction(Action action)
        {
            lock (mainThreadActionsLock)
            {
                mainThreadActions.Add(action);
            }
        }

        public void ClearMainThreadAction()
        {
            lock (mainThreadActionsLock)
            {
                mainThreadActions.Clear();
            }
        }

        public void AddSubThreadAction(Action action)
        {
            lock (subThreadActions)
            {
                subThreadActions.Add(action);
            }
        }

        public void ClearSubThreadAction()
        {
            lock (subThreadActionsLock)
            {
                subThreadActions.Clear();
            }
        }

        IEnumerator enumerator_update()
        {
            thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        // 执行所有子线程方法 add by TangJian 2018/12/2 18:08
                        foreach (var action in subThreadActions)
                        {
                            action();
                        }

                        // 清除所有子线程方法 add by TangJian 2018/12/2 18:09
                        ClearSubThreadAction();
                    }
                    catch (Exception)
                    {
                    }
                    Thread.Sleep(1);
                }
            });
            thread.Start();

            while (true)
            {
                try
                {
                    // 执行所有主线程方法 add by TangJian 2018/12/2 18:08
                    foreach (var action in mainThreadActions)
                    {
                        action();
                    }

                    // 清除所有主线程方法 add by TangJian 2018/12/2 18:08
                    ClearMainThreadAction();
                }
                catch (Exception)
                {
                }
                yield return 0;
            }
        }

        public void Update()
        {
            if (enumerator != null && enumerator.MoveNext())
            { }
            else
            {
                enumerator = enumerator_update();
            }
        }
    }
}