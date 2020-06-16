using UnityEngine;
using UnityEditor;

namespace Tang.Editor
{
    public class PreviewWindow : EditorWindow
    {
        [MenuItem("Window/PreviewWindow")]
        static void Init()
        {
            PreviewWindow window = (PreviewWindow)EditorWindow.GetWindow(typeof(PreviewWindow));
            window.name = "PreviewWindow";
            window.Show();
        }

        void OnEnable()
        {
            title = "PreviewWindow";
        }

        void OnDisable()
        {
            Tools.Destroy(previewEditor);
        }

        GameObject target;
        UnityEditor.Editor previewEditor;

        void OnSelectionChange()
        {
            target = Tools.GetFirstSelectGameObject();

            if (previewEditor != null)
                Tools.Destroy(previewEditor);
            previewEditor = UnityEditor.Editor.CreateEditor(target, typeof(PreviewEditor));

            base.Repaint();
        }

        void OnGUI()
        {
            if (previewEditor != null)
            {
                if (previewEditor.HasPreviewGUI())
                {

                    Rect r = GUILayoutUtility.GetRect(100, 100, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    previewEditor.OnPreviewGUI(r, GUIStyle.none);
                }
            }
        }

        void Update()
        {
            if (mouseOverWindow) // 如果鼠标在窗口上, 则重绘界面 add by TangJian 2017/09/28 21:57:44
            {
                base.Repaint();
            }
        }
    }
}