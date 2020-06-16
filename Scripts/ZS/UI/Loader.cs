using System;
using System.Collections;
using System.Collections.Generic;

namespace Tang
{
    public class Loader
    {
        public struct Item
        {
            public int stepCount;
            public IEnumerator action;
        }

        public List<Item> Items;

        public IEnumerator Update()
        {
            int totalStepCount = 0;
            int currStep = 0;
            
            // 计算步数总和
            foreach (var item in Items)
            {
                totalStepCount += item.stepCount;
            }
            
            // 加载
            foreach (var item in Items)
            {
                int step = 0;
                while (item.action.MoveNext())
                {
                    if (item.action.Current is int current)
                    {
                        step = current;
                    }
                    yield return (float)(currStep + step) / totalStepCount;
                }
                
                currStep += item.stepCount;
            }
        }
    }
}