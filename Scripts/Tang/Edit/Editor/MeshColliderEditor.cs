using System;
using UnityEngine;
using UnityEditor;

namespace Tang.Editor
{
    [Serializable]
    public class MeshColliderEditorData : MonoBehaviour
    {
        public Bounds bounds;
    }

    [CustomEditor(typeof(MeshCollider))]
    public class MeshColliderEditor : UnityEditor.Editor
    {
        MeshColliderEditor()
        {
        }

        [SerializeField]
        MeshColliderEditorData MeshColliderEditorData;
        GameObject MeshColliderEditorDataObject;

        MeshCollider meshCollider;
        SerializedObject mainSerializedObject;

        void createData()
        {            
            if (Application.isPlaying)
            {
                MeshColliderEditorDataObject = new GameObject();
                MeshColliderEditorDataObject.name = "TmpEditorObject";
                MeshColliderEditorData = Tools.AddComponent<MeshColliderEditorData>(MeshColliderEditorDataObject);
            }
        }

        void destoryData()
        {
            if (Application.isPlaying)
            {
                DestroyImmediate(MeshColliderEditorDataObject);
            }
        }

        void OnEnable()
        {
            if (Application.isPlaying)
            {
                createData();

                meshCollider = target as MeshCollider;

                MeshColliderEditorData.bounds = meshCollider.bounds;

                mainSerializedObject = new SerializedObject(MeshColliderEditorData);
            }
        }

        void OnDisable()
        {
            destoryData();
        }

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                if (MeshColliderEditorData != null)
                {
                    MeshColliderEditorData.bounds = meshCollider.bounds;
                    Reflection.Instance.Invoke("Editor", "DoDrawDefaultInspector", null, new object[] { mainSerializedObject });
                }
            }

            base.DrawDefaultInspector();
        }

        // void OnSceneGUI()
        // {
        //     if (Application.isPlaying)
        //     {
        //         Handles.color = Color.red;
        //         Handles.DrawWireCube(MeshColliderEditorData.bounds.center, MeshColliderEditorData.bounds.size);
        //     }
        // }
    }
}