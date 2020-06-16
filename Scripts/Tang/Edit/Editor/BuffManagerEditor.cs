using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Tang.Editor
{        
    [CustomEditor(typeof(BuffManager))]
    public class BuffManagerEditor : UnityEditor.Editor
    {
        BuffManager buffManager;
        SerializedObject m_Object;
        SerializedProperty m_Property;
        string buffMapFileName;
        string buffMapString;
        Dictionary<string, BuffData> buffDataMap = new Dictionary<string, BuffData>();
        List<SerializedObject> serializedObjectList = new List<SerializedObject>();
        void OnEnable()
        {
            buffManager = target as BuffManager;
        }
        void Start()
        {
            buffManager.loadBuffData();
        }
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("载入脚本"))
            {
                buffManager.loadBuffData();
            }
            base.DrawDefaultInspector();
        }
    }
}