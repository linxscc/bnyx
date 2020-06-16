using UnityEngine;
using FairyGUI;





namespace Tang
{
    public static class GObjectExtend
    {
        public static Vector2 WorldToScreenPoint(this GObject target, Vector3 worldPos)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            //原点位置转换
            screenPos.y = Screen.height - screenPos.y;
            Vector2 pt = GRoot.inst.GlobalToLocal(screenPos);
            return pt;
        }
    }
 
}