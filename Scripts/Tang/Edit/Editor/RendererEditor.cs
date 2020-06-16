


using UnityEngine;
using UnityEditor;

namespace Tang.Editor
{
    [CustomEditor(typeof(MeshRenderer), true)]
    [CanEditMultipleObjects]
    public class RendererEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Renderer renderer = target as Renderer;
            renderer.sortingOrder = MyGUI.IntFieldWithTitle("SortOrder", renderer.sortingOrder);

            DrawDefaultInspector();
        }
    }
}