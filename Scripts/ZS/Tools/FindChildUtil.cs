using UnityEngine;

public class FindChildUtil : MonoBehaviour
{
    /// <summary>
    /// 通过名字找子物体
    /// </summary>
    /// <param name="name">子物体的名字</param>
    /// <param name="parent">子物体的父物体</param>
    /// <returns>子物体</returns>
    public static GameObject FindChild(string name, Transform parent)
    {
        GameObject target = null;
        for (int i = 0; i < parent.childCount; i++)
        {
            //先在parent的子物体中找，如果有名字一致的直接返回
            if (parent.GetChild(i).name == name)
            {
                return parent.GetChild(i).gameObject;
            }
            //如果parent的子物体中没有名字一致的，先判断parent的子物是否有子物体，如果有，递归从子物体里面找
            target = FindChild(name, parent.GetChild(i));
            if (target != null)
            {
                return target;
            }
        }
        return null;
    }
}
