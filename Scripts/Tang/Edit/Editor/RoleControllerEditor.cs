


using UnityEngine;
using UnityEditor;

namespace Tang.Editor
{
    [CustomEditor(typeof(RoleController), true)]
    // [CanEditMultipleObjects]
    public class RoleControllerEditor : UnityEditor.Editor
    {
        RoleController roleController;

        DamageData damageData = new DamageData();

        int hitPointSelectedIndex = 0;
        string[] hitPointStringList = null;

        ValueMonitorPool valueMonitorPool = new ValueMonitorPool();

        private void OnEnable()
        {
            valueMonitorPool.AddMonitor<UnityEngine.Object>(() =>
            {
                return target;
            }, (UnityEngine.Object from, UnityEngine.Object to) =>
            {
                roleController = target as RoleController;
            });
        }

        public override void OnInspectorGUI()
        {
            valueMonitorPool.Update();

            if (roleController)
            {
                if (Application.isPlaying)
                {
                    //if (roleController.SkeletonAnimator != null && roleController.SkeletonAnimator.HitPointList != null)
                    //{
                    //    if (hitPointStringList == null || hitPointStringList.Length != roleController.SkeletonAnimator.HitPointList.Count)
                    //    {
                    //        hitPointStringList = new string[roleController.SkeletonAnimator.HitPointList.Count];

                    //        for (int i = 0; i < hitPointStringList.Length; i++)
                    //        {
                    //            hitPointStringList[i] = i + "_" + roleController.SkeletonAnimator.HitPointList[i].hitType;
                    //        }
                    //    }

                    //    hitPointSelectedIndex = MyGUI.PopupWithTitle("打击点", hitPointSelectedIndex, hitPointStringList);
                    //}

                    //MyGUI.DamageDataField(damageData);

                    //if (MyGUI.Button("测试打击效果"))
                    //{
                    //    var hitPoint = roleController.SkeletonAnimator.HitPointList[hitPointSelectedIndex];

                    //    damageData.collideBounds = new Bounds(roleController.transform.TransformPoint(new Vector3(hitPoint.pos.x * roleController.GetDirectionInt(), hitPoint.pos.y, hitPoint.pos.z)), Vector3.zero);

                    //    //damageData.collideBounds = new Bounds(Vector3.zero, Vector3.zero);
                    //    roleController.OnHurt(damageData);
                    //}
                }
            }
            
            DrawDefaultInspector();
        }
    }
}