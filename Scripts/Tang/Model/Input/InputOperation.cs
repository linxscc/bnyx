using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    [System.Serializable]
    public class InputOperation
    {
        string name;
        int currIndex = 0;
        float time = 0;
        float interval = 0;
        List<InputOperation> unitOperationList = new List<InputOperation>();

        public bool tryOperation(string operationName)
        {
            // 如果没有单元操作列表, 操作名正确则返回true
            if (unitOperationList == null || unitOperationList.Count == 0)
            {
                time = Time.time;
                return name == operationName;
            }

            if (currIndex >= unitOperationList.Count)
            {
                currIndex = 0;
            }

            InputOperation operation = unitOperationList[currIndex];
            if (operation.tryOperation(operationName))
            {
                if (tryInterval())
                {
                    return true;
                }
            }

            currIndex++;

            return false;
        }

        public bool tryInterval()
        {
            if (this.interval <= 0)
            {
                return true;
            }
            if (unitOperationList.Count >= 2)
            {
                var begin = unitOperationList[0];
                var end = unitOperationList[unitOperationList.Count - 1];
                var interval = end.time - begin.time;
                if (this.interval >= interval)
                {
                    return true;
                }
            }
            return false;
        }
    }
}