using UnityEngine;
using UnityEditor;

namespace Tang.Editor
{
    [CustomEditor(typeof(MeshFilter))]
    public class MeshFilterEditor : UnityEditor.Editor
    {
        void OnEnable()
        {

        }

        void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            MeshFilter meshFilter = target as MeshFilter;

            //MyGUI.AutoSerializedObjectField(meshFilter.mesh.normals);
            DrawDefaultInspector();
        }
    }
}