using System;
using System.Collections.Generic;









namespace Tang
{    
    public class FunctionPool
    {
        List<Func<bool>> funcList = new List<Func<bool>>();

        public void AddFunc(Func<bool> func)
        {
            funcList.Add(func);
        }

        public void CallFuncs()
        {
            for (int i = funcList.Count - 1; i >= 0; i--)
            {
                var func = funcList[i];
                if (func())
                {
                    funcList.RemoveAt(i);
                }
            }
        }
    }
}