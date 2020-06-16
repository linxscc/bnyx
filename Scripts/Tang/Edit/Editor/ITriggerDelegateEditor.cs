using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Tang.Editor
{
    [CustomEditor(typeof(DoorController))]
    public class ITriggerDelegateEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("生成交互区域"))
            {
                foreach (var item in targets)
                {
                    DoorController itriggerDelegate = item as DoorController;
                    GameObject gameObject = itriggerDelegate.gameObject;
                    Debug.Assert(gameObject != null);
                    Bounds bounds = gameObject.GetColliderBounds(new List<string>() { "Interact" });
                    bounds.center = new Vector3(bounds.center.x - gameObject.transform.position.x, bounds.center.y - gameObject.transform.position.y, bounds.center.z - gameObject.transform.position.z);

                    // 创建交互区域 add by TangJian 2018/01/02 21:26:35
                    {
                        float scale = 1f;

                        GameObject interactObject = gameObject.GetChild("Interact", true);
                        interactObject.transform.localScale = new Vector3(1, 1, 1) * scale;
                        interactObject.transform.localPosition = Vector3.zero;
                        interactObject.transform.localEulerAngles = Vector3.zero;

                        interactObject.layer = LayerMask.NameToLayer("Interaction");

                        BoxCollider boxCollider = interactObject.AddComponentUnique<BoxCollider>();
                        boxCollider.isTrigger = true;

                        boxCollider.size = bounds.size;
                        boxCollider.center = bounds.center;

                        TriggerController triggerController = interactObject.AddComponentUnique<TriggerController>();
                    }

                }
            }
        }
    }
}