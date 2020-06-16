using UnityEngine;
using UnityEditor;

namespace Tang.Editor
{
    // [CustomEditor(typeof(MeshFilter))]
    // [CanEditMultipleObjects]
    public class MeshFilterPreview : UnityEditor.Editor
    {
        protected PreviewRenderUtility _previewRenderUtility;
        protected MeshFilter _targetMeshFilter;
        protected MeshRenderer _targetMeshRenderer;

        protected Vector2 _drag = new Vector2(180, 0);


        protected virtual void ValidateData()
        {
            if (_previewRenderUtility == null)
            {
                _previewRenderUtility = new PreviewRenderUtility();

                _previewRenderUtility.camera.transform.position = new Vector3(0, 0, -6);
                _previewRenderUtility.camera.transform.rotation = Quaternion.identity;



                // _previewRenderUtility.m_Camera.near = -1000.0f;
                // _previewRenderUtility.m_Camera.far = 1000.0f;
                // _previewRenderUtility.m_Camera.nearClipPlane = -1000.0f;
                _previewRenderUtility.camera.farClipPlane = 1000.0f;
            }

            _targetMeshFilter = target as MeshFilter;

            {
                var mesh = _targetMeshFilter.sharedMesh;
            }

            _targetMeshRenderer = _targetMeshFilter.GetComponent<MeshRenderer>();
        }

        public override bool HasPreviewGUI()
        {
            ValidateData();

            return true;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            _drag = Drag2D(_drag, r);

            if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint)
            {
                if (_targetMeshRenderer == null)
                {
                    EditorGUI.DropShadowLabel(r, "Mesh Renderer Required");
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        _previewRenderUtility.BeginPreview(r, background);

                        _previewRenderUtility.DrawMesh(_targetMeshFilter.sharedMesh, Matrix4x4.identity, _targetMeshRenderer.sharedMaterial, 0);

                        // _previewRenderUtility.m_Camera.transform.eulerAngles = new Vector3(0, 180, 0);
                        // _previewRenderUtility.m_Camera.transform.position = Vector2.zero;
                        _previewRenderUtility.camera.transform.rotation = Quaternion.Euler(new Vector3(-_drag.y, -_drag.x, 0));
                        _previewRenderUtility.camera.transform.position = _previewRenderUtility.camera.transform.forward * -20;
                        _previewRenderUtility.camera.Render();

                        Texture resultRender = _previewRenderUtility.EndPreview();
                        GUI.DrawTexture(r, resultRender, ScaleMode.StretchToFill, false);
                    }

                    {
                        _previewRenderUtility.BeginPreview(r, background);

                        _previewRenderUtility.DrawMesh(_targetMeshFilter.sharedMesh, Matrix4x4.identity, _targetMeshRenderer.sharedMaterial, 0);

                        // _previewRenderUtility.m_Camera.transform.eulerAngles = new Vector3(0, 180, 0);
                        // _previewRenderUtility.m_Camera.transform.position = Vector2.zero;
                        _previewRenderUtility.camera.transform.rotation = Quaternion.Euler(new Vector3(_drag.y, _drag.x, 0));
                        _previewRenderUtility.camera.transform.position = _previewRenderUtility.camera.transform.forward * -20;
                        _previewRenderUtility.camera.Render();

                        Texture resultRender = _previewRenderUtility.EndPreview();
                        GUI.DrawTexture(r, resultRender, ScaleMode.StretchToFill, false);
                    }
                    EditorGUILayout.EndHorizontal();
                }

            }
        }

        public override void OnPreviewSettings()
        {
            if (GUILayout.Button("Reset Camera", EditorStyles.whiteMiniLabel))
                _drag = Vector2.zero;
        }

        protected virtual void OnDestroy()
        {
            _previewRenderUtility.Cleanup();
        }

        public static Vector2 Drag2D(Vector2 scrollPosition, Rect position)
        {
            int controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
            UnityEngine.Event current = UnityEngine.Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case UnityEngine.EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && position.width > 50f)
                    {
                        GUIUtility.hotControl = controlID;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    break;
                case UnityEngine.EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    EditorGUIUtility.SetWantsMouseJumping(0);
                    break;
                case UnityEngine.EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        scrollPosition -= (current.delta * (float)((!current.shift) ? 1 : 3) / Mathf.Min(position.width, position.height) * 140f);
                        scrollPosition.y = Mathf.Clamp(scrollPosition.y, -90f, 90f);
                        current.Use();
                        GUI.changed = true;
                    }
                    break;
            }
            return scrollPosition;
        }

    }
}