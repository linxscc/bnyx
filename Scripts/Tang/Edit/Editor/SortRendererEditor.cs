using UnityEngine;
using UnityEditor;


namespace Tang.Editor
{
    [CustomEditor(typeof(SortRenderer))]
    public class SortRendererEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            SortRenderer sortRenderer = target as SortRenderer;

            if (MyGUI.Button("从碰撞区域生成SortRendererPos"))
            {
                Bounds bounds = sortRenderer.gameObject.GetColliderBounds();
                sortRenderer.SetSortRendererPos(new Vector3(bounds.center.x, bounds.center.y, bounds.max.z));
            }

            if (MyGUI.Button("从渲染区域生成SortRendererPos"))
            {
                Bounds bounds = sortRenderer.gameObject.GetRendererBounds();
                sortRenderer.SetSortRendererPos(new Vector3(bounds.center.x, bounds.center.y, bounds.max.z));
            }

            base.OnInspectorGUI();
        }
    }
}