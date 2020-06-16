using System;
using System.Collections.Generic;


namespace Tang
{

    public class ValueMonitorPool
    {
        List<IValueMonitor> valueMonitorList;

        public ValueMonitorPool()
        {
            valueMonitorList = new List<IValueMonitor>();
        }

        public void AddMonitor<T>(Func<T> valueGetter, Action<T, T> action, bool callAtOnce = false)
        {
            valueMonitorList.Add(new ValueMonitor<T>(valueGetter, action, callAtOnce));
        }

        public void Clear()
        {
            valueMonitorList.Clear();
        }

        public void Reset()
        {
            foreach (var item in valueMonitorList)
            {
                item.Reset();
            }
        }

        public void Update()
        {
//            if (item != Soul)
            foreach (var item in valueMonitorList)
            {
                item.Update();
            }
        }
    }
}