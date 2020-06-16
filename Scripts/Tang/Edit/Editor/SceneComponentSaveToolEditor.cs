using UnityEditor;

namespace Tang.Editor
{
    [CustomEditor(typeof(SceneComponentSaveTool), true)]
    [CanEditMultipleObjects]
    public class SceneComponentSaveToolEditor : UnityEditor.Editor
    {
        SceneComponent terrainComponent;

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            //terrainComponent = target as SceneComponent;
            //terrainComponent.color = new Color(UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f));

            //// 设置资源路径 add by TangJian 2018/03/13 15:37:42
            //{
            //    if (PrefabUtility.GetPrefabType(terrainComponent.gameObject) == PrefabType.Prefab)
            //    {
            //        terrainComponent.FilePath = Tools.GetPrefabPath(terrainComponent.gameObject) ?? terrainComponent.FilePath;
            //    }

            //    //else if (PrefabUtility.GetPrefabType(terrainComponent.gameObject) == PrefabType.PrefabInstance) // 如果类型是预制体实例 add by TangJian 2018/8/14 20:42
            //    //{
            //    //    GameObject root = Tools.GetPrefabRoot(terrainComponent.gameObject) as GameObject;
            //    //    if (root.layer == LayerMask.NameToLayer("SceneComponent"))
            //    //    {
            //    //        terrainComponent.FilePath = AssetDatabase.GetAssetPath(root);// root.GetComponent<SceneComponent>().FilePath;
            //    //    }
            //    //    else
            //    //    {
            //    //        Debug.LogError("组件 " + terrainComponent.name + " 丢失预制体");
            //    //    }
            //    //}

            //    terrainComponent.Prefab = AssetDatabase.LoadAssetAtPath<GameObject>(terrainComponent.FilePath);
            //}

            //if (terrainComponent.GetComponent<Cask>())
            //{
            //    Cask cask = terrainComponent.GetComponent<Cask>();
            //    Bounds bounds = cask.gameObject.GetRendererBounds(2, 99);
            //    cask.data.width = bounds.size.x;
            //    cask.data.height = bounds.size.y;
            //}
        }

        public override void OnInspectorGUI()
        {
            //terrainComponent.MoveByGridPos(Vector3.zero);

            //if (MyGUI.Button("保存子节点预制体"))
            //{
            //    //EditorUtility.DisplayDialog("wodasd","asdad","ok");
            //    foreach (SceneComponentSaveTool sceneComponentSaveTool in targets)
            //    {
            //        List<GameObject> childrenlist = new List<GameObject>();
            //        childrenlist = sceneComponentSaveTool.gameObject.GetChildren();
            //        foreach (GameObject children in childrenlist)
            //        {
            //            SceneComponent sceneComponent = children.GetComponent<SceneComponent>();
            //            if (sceneComponent != null && PrefabUtility.GetPrefabType(children) != PrefabType.None)
            //            {
            //                string Path = sceneComponent.filePath;
            //                Tools.UpdatePrefab(children, Path);
            //            }
            //        }
            //    }
            //}


            DrawDefaultInspector();
        }
    }
}