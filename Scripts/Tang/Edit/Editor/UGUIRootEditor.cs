using UnityEngine;
using UnityEditor;


namespace Tang.Editor
{
    [CustomEditor(typeof(UGUIRoot))]
    public class UGUIRootEditor : UnityEditor.Editor
    {
        UGUIRoot uguiRoot;

        void OnEnable()
        {
            uguiRoot = target as UGUIRoot;
        }

        void Start()
        {
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("更具宽度按比例自适应图片"))
            {
                Debug.Log("更具宽度按比例自适应图片");
                uguiRoot.AutoSizeImageWithWidth();
            }
            base.DrawDefaultInspector();
        }
    }
}