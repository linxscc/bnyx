using System.Collections.Generic;
using UnityEngine;

namespace Tang
{
    public class InputController : MonoBehaviour
    {
        protected RoleController roleController;
        protected RoleInputData roleInputData = new RoleInputData();
        protected List<KeyActionAndRoleActionBinding> keyActionAndRoleActionBindingList = new List<KeyActionAndRoleActionBinding>();
        protected Dictionary<KeyAction, RoleAction> keyActionAndRoleActionMap = new Dictionary<KeyAction, RoleAction>();
        public List<KeyAction> currKeyActionList = new List<KeyAction>();
        // 得到按键状态 add by TangJian 2017/08/28 18:08:20
        // public List<KeyAction> getCurrentKeyActionList()
        // {
        //     int recordCount = 0;
        //     foreach (var keyActionAndRoleActionBinding in keyActionAndRoleActionBindingList)
        //     {
        //         KeyCode keyCode = keyActionAndRoleActionBinding.Key.keyCode;
        //         KeyActionType keyActionType = keyActionAndRoleActionBinding.Key.actionType;
        //         if (keyActionType == KeyActionType.Press)
        //         {
        //             if (Input.GetKeyDown(keyCode))
        //             {
        //                 recordCount++;

        //                 if (currKeyActionList.Count >= recordCount)
        //                 {
        //                     var keyAction = currKeyActionList[recordCount - 1];
        //                     keyAction.keyCode = keyCode;
        //                     keyAction.actionType = KeyActionType.Press;
        //                 }
        //                 else
        //                 {
        //                     currKeyActionList.Add(new KeyAction(keyCode, KeyActionType.Press));
        //                 }
        //             }
        //             else if (Input.GetKeyUp(keyCode))
        //             {
        //                 recordCount++;

        //                 if (currKeyActionList.Count >= recordCount)
        //                 {
        //                     var keyAction = currKeyActionList[recordCount - 1];
        //                     keyAction.keyCode = keyCode;
        //                     keyAction.actionType = KeyActionType.Release;
        //                 }
        //                 else
        //                 {
        //                     currKeyActionList.Add(new KeyAction(keyCode, KeyActionType.Release));
        //                 }
        //             }
        //         }
        //     }
        //     // 移除多余的按键 add by TangJian 2017/08/30 15:31:07
        //     if (currKeyActionList.Count > recordCount)
        //     {
        //         currKeyActionList.RemoveRange(recordCount, currKeyActionList.Count - recordCount);
        //     }
        //     return currKeyActionList;
        // }

        // protected List<KeyAction> getKeyActionList()
        // {
        //     return getCurrentKeyActionList();
        // }

        // List<RoleAction> currRoleActionList = new List<RoleAction>();
        // protected List<RoleAction> getCurrRoleActionList()
        // {
        //     // 按键 add by TangJian 2017/08/28 19:37:28
        //     int recordCount = 0;
        //     var keyActionList = getKeyActionList();
        //     if (keyActionList != null)
        //     {
        //         foreach (var keyAction in keyActionList)
        //         {
        //             if (keyActionAndRoleActionMap.ContainsKey(keyAction))
        //             {
        //                 recordCount++;

        //                 if (currRoleActionList.Count > recordCount)
        //                 {
        //                     RoleAction roleAction = currRoleActionList[recordCount - 1];
        //                     roleAction.roleActionType = keyActionAndRoleActionMap[keyAction].roleActionType;
        //                 }
        //                 else
        //                 {
        //                     currRoleActionList.Add(new RoleAction(keyActionAndRoleActionMap[keyAction].roleActionType));
        //                 }
        //             }
        //         }
        //     }
        //     if (currRoleActionList.Count > recordCount)
        //     {
        //         currRoleActionList.RemoveRange(recordCount, currRoleActionList.Count - recordCount);
        //     }
        //     return currRoleActionList;
        // }
    }
}