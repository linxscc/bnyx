using System;
using System.Collections.Generic;









namespace Tang
{
    public class ActionPool
    {
        List<Action> actionList = new List<Action>();

        public void AddAction(Action action)
        {
            actionList.Add(action);
        }

        public void CallActions()
        {
            for (int i = actionList.Count - 1; i >= 0; i--)
            {
                var action = actionList[i];
                action();
                actionList.RemoveAt(i);
            }
        }
    }
}