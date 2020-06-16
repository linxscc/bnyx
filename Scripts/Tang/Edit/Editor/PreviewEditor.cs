using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Tang.Editor
{
    public class PreviewEditor : UnityEditor.Editor
    {
        PreviewRenderUtility m_PreviewUtility;
        float previewScale = 1.0f;
        // Vector2 previewDir = new Vector2(180, 90);
        Vector2 previewDir = new Vector2(0, 0);

        private bool targetIsInstance = false;
        private List<GameObject> m_PreviewInstances;
        private GameObject m_PreviewInstance;
        private List<bool> rendererEnableList = new List<bool>();
        protected bool canScale = true;
        public PreviewEditor()
        {
            // Init("GameObjectInspector");
        }

        private void InitPreview()
        {
            if (this.m_PreviewUtility == null)
            {
                this.m_PreviewUtility = new PreviewRenderUtility(true);
                this.m_PreviewUtility.cameraFieldOfView = 30f;
                this.m_PreviewUtility.camera.clearFlags = CameraClearFlags.SolidColor;

                int PreviewCullingLayer = (int)Reflection.Instance.GetPropertyInfoValue("Camera", "PreviewCullingLayer", null, null);
                this.m_PreviewUtility.camera.cullingMask = ((int)1) << PreviewCullingLayer;
            }
        }

        private GameObject InstantiateForAnimatorPreview(object original)
        {
            return Reflection.Instance.Invoke("EditorUtility", "InstantiateForAnimatorPreview", null, new object[] { original }) as GameObject;
        }

        private void CreatePreviewInstance()
        {
            this.DestroyPreviewInstance();

            PrefabType prefabType = PrefabUtility.GetPrefabType(target as GameObject);
            if (((prefabType != PrefabType.None) && (prefabType != PrefabType.Prefab)) && (prefabType != PrefabType.ModelPrefab))
            {
                targetIsInstance = true;
                m_PreviewInstance = InstantiateForAnimatorPreview(target);
                // m_PreviewInstance.transform.position = new Vector3(999999, 999999, 999999);
                // m_PreviewInstance.transform.eulerAngles = new Vector3(m_PreviewInstance.transform.eulerAngles.x, m_PreviewInstance.transform.eulerAngles.y, m_PreviewInstance.transform.eulerAngles.z);
            }
            else
            {
                targetIsInstance = false;
                m_PreviewInstance = InstantiateForAnimatorPreview(target);
                // m_PreviewInstance.transform.position = new Vector3(999999, 999999, 999999);
                // m_PreviewInstance.transform.eulerAngles = new Vector3(m_PreviewInstance.transform.eulerAngles.x, m_PreviewInstance.transform.eulerAngles.y, m_PreviewInstance.transform.eulerAngles.z);
            }

            var renderers = m_PreviewInstance.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                rendererEnableList.Add(renderers[i].enabled);
            }
        }

        private void DestroyPreviewInstance()
        {
            Tools.Destroy(m_PreviewInstance);
        }

        void OnEnable()
        {
            InitPreview();
            CreatePreviewInstance();
        }

        void OnDisable()
        {
            DestroyPreviewInstance();
        }

        public override void OnInspectorGUI()
        {
            var r = GUILayoutUtility.GetRect(100, 300);
            var background = GUIStyle.none;
            DrawGameObjectPreview(r, background);
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            DrawGameObjectPreview(r, background);
        }

        void DrawGameObjectPreview(Rect r, GUIStyle background)
        {
            if (canScale)
            {
                previewScale += MouseScroll(GUILayoutUtility.GetLastRect());
            }

            previewDir = Drag2D(previewDir, r);

            Vector2 extraEuler = new Vector2(0, 0);
            // if (targetIsInstance)
            // {
            //     extraEuler.y += 180;
            // }

            InitPreview();
            if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint)
            {
                this.m_PreviewUtility.BeginPreview(r, background);

                GameObject go = m_PreviewInstance;
                Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
                GetRenderableBoundsRecurse(ref bounds, go);
                float num = Mathf.Max(bounds.extents.magnitude, 0.0001f);
                float num2 = num * 3.8f * previewScale;
                Quaternion quaternion = Quaternion.Euler(-(this.previewDir.y + extraEuler.y), -(this.previewDir.x + extraEuler.x), 0f);
                Vector3 vector2 = bounds.center - ((Vector3)(quaternion * (Vector3.forward * num2)));
                this.m_PreviewUtility.camera.transform.position = vector2;
                this.m_PreviewUtility.camera.transform.rotation = quaternion;
                this.m_PreviewUtility.camera.nearClipPlane = num2 - (num * 1.1f);
                this.m_PreviewUtility.camera.farClipPlane = num2 + (num * 1.1f);
                // this.m_PreviewUtility.m_Light[0].intensity = 0.7f;
                // this.m_PreviewUtility.m_Light[0].transform.rotation = quaternion * Quaternion.Euler(40f, 40f, 0f);
                // this.m_PreviewUtility.m_Light[1].intensity = 0.7f;
                // this.m_PreviewUtility.m_Light[1].transform.rotation = quaternion * Quaternion.Euler(340f, 218f, 177f);
                // Color ambient = new Color(0.1f, 0.1f, 0.1f, 0f);
                // UnityEditorInternal.InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, ambient);
                // bool fog = RenderSettings.fog;
                // Unsupported.SetRenderSettingsUseFogNoDirty(false);

                SetEnabledRecursive(go, true);
                this.m_PreviewUtility.camera.Render();
                SetEnabledRecursive(go, false);
                // Unsupported.SetRenderSettingsUseFogNoDirty(fog);
                // UnityEditorInternal.InternalEditorUtility.RemoveCustomLighting();

                this.m_PreviewUtility.EndAndDrawPreview(r);
            }
        }

        public void SetEnabledRecursive(GameObject go, bool enabled)
        {
            var renderers = go.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                var renderer = renderers[i];
                if (enabled && rendererEnableList[i])
                {
                    // renderer.material.shader = Shader.Find("Sprites/Default");
                    // renderer.material.renderQueue = 2450;
                    renderer.enabled = enabled;
                }
                else
                {
                    renderer.enabled = false;
                }
            }
        }

        public static void GetRenderableBoundsRecurse(ref Bounds bounds, GameObject go)
        {
            MeshRenderer component = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            MeshFilter filter = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
            if (((component != null) && (filter != null)) && (filter.sharedMesh != null))
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = component.bounds;
                }
                else
                {
                    bounds.Encapsulate(component.bounds);
                }
            }
            SkinnedMeshRenderer renderer2 = go.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
            if ((renderer2 != null) && (renderer2.sharedMesh != null))
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = renderer2.bounds;
                }
                else
                {
                    bounds.Encapsulate(renderer2.bounds);
                }
            }
            SpriteRenderer renderer3 = go.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            if ((renderer3 != null) && (renderer3.sprite != null))
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = renderer3.bounds;
                }
                else
                {
                    bounds.Encapsulate(renderer3.bounds);
                }
            }
            IEnumerator enumerator = go.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform)enumerator.Current;
                    GetRenderableBoundsRecurse(ref bounds, current.gameObject);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
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
                        scrollPosition -= current.delta * (float)((!current.shift) ? 1 : 3) / Mathf.Min(position.width, position.height) * 140f;
                        scrollPosition.y = Mathf.Clamp(scrollPosition.y, -90f, 90f);
                        current.Use();
                        GUI.changed = true;
                    }
                    break;
            }
            return scrollPosition;
        }

        public static float MouseScroll(Rect rect)
        {
            UnityEngine.Event current = UnityEngine.Event.current;
            if ((current.type == UnityEngine.EventType.ScrollWheel) && rect.Contains(current.mousePosition))
            {
                return current.delta.y * 0.1f;
            }
            return 0;
        }
    }

    public class PreviewEditorWithoutScale : PreviewEditor
    {
        PreviewEditorWithoutScale()
        : base()
        {
            canScale = false;
        }
    }
}