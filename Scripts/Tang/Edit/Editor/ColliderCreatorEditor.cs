using UnityEngine;
using UnityEditor;

namespace Tang.Editor
{
    [CustomEditor(typeof(ColliderCreator))]
    public class ColliderCreatorEditor : UnityEditor.Editor
    {
        private string colliderName = "ColliderObject";

        public override void OnInspectorGUI()
        {
            {
                EditorGUILayout.BeginHorizontal();

                MyGUI.TextFieldWithTitle("碰撞区域名称", colliderName);
                if (MyGUI.Button("根据Renderer生成碰撞区域"))
                {
                    foreach (var item in targets)
                    {
                        ColliderCreator itriggerDelegate = item as ColliderCreator;
                        GameObject gameObject = itriggerDelegate.gameObject;
                        Debug.Assert(gameObject != null);

                        Bounds bounds = gameObject.GetRendererBounds();
                        bounds.center = new Vector3(bounds.center.x - gameObject.transform.position.x, bounds.center.y - gameObject.transform.position.y, bounds.center.z - gameObject.transform.position.z);

                        // 创建交互区域 add by TangJian 2018/01/02 21:26:35
                        {
                            float scale = 1f;

                            GameObject interactObject = gameObject.GetChild(colliderName, true);
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

                if (MyGUI.Button("根据Collider生成碰撞区域"))
                {
                    foreach (var item in targets)
                    {
                        ColliderCreator itriggerDelegate = item as ColliderCreator;
                        GameObject gameObject = itriggerDelegate.gameObject;
                        Debug.Assert(gameObject != null);

                        Bounds bounds = gameObject.GetColliderBounds();
                        bounds.center = new Vector3(bounds.center.x - gameObject.transform.position.x, bounds.center.y - gameObject.transform.position.y, bounds.center.z - gameObject.transform.position.z);

                        // 创建交互区域 add by TangJian 2018/01/02 21:26:35
                        {
                            float scale = 1f;

                            GameObject interactObject = gameObject.GetChild(colliderName, true);
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
                EditorGUILayout.EndHorizontal();
            }

            DrawDefaultInspector();
        }
    }
}