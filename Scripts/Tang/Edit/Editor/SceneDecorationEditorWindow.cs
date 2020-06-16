using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


namespace Tang.Editor
{
    public class SceneDecorationEditorWindow : EditorWindow
    {
        [MenuItem("Window/场景简单组件编辑器")]
        static void Init()
        {
            SceneDecorationEditorWindow window = (SceneDecorationEditorWindow)EditorWindow.GetWindow(typeof(SceneDecorationEditorWindow));
            window.Show();
        }

        bool UsePathorImg = true;
        private string SceneDecorationFile = "Resources_moved/Scripts/SceneDecoration/sceneDecoration.json";
        private string prefabPath = "Assets/Resources_moved/Prefabs/Placement";

        Dictionary<string, SceneDecorationEditorwindowData> SceneDecorationDic = new Dictionary<string, SceneDecorationEditorwindowData>();
        List<SceneDecorationEditorwindowData> SceneDecorationList = new List<SceneDecorationEditorwindowData>();

        SceneDecorationEditorwindowData currSceneDecoration;

        Vector2 listScrollViewPos = Vector2.zero;
        Vector2 editScrollViewPos = Vector2.zero;

        Rect windowRect = new Rect(100, 100, 500, 500);
        float imgsclae = 1;
        void OnGUI()
        {
            GUIStyle boxStyle = new GUIStyle("box");

            var width = position.size.x - boxStyle.border.horizontal;
            var height = position.size.y - boxStyle.border.vertical;

            var innerBoxWidth = width - (boxStyle.padding.horizontal + boxStyle.border.horizontal);
            var innerBoxHeight = height - (boxStyle.padding.vertical + boxStyle.border.vertical);

            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(width), GUILayout.Height(height));

            // 设置路径, 以及存取数据 add by TangJian 2017/11/15 16:22:45
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(SceneDecorationFile);
            if (MyGUI.Button("读取"))
            {
                loadSceneDecoration();
            }
            if (MyGUI.Button("保存"))
            {
                saveSceneDecoration();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(prefabPath);
            if (MyGUI.Button("制作预制体"))
            {
                saveSceneDecoration();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            imgsclae = MyGUI.FloatFieldWithTitle("图片整体缩放大小", imgsclae);
            EditorGUILayout.EndHorizontal();

            // 编辑区域 add by TangJian 2017/11/15 16:28:19
            EditorGUILayout.BeginHorizontal();

            // // 列表框 add by TangJian 2017/11/15 16:27:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            listScrollViewPos = EditorGUILayout.BeginScrollView(listScrollViewPos);

            for (int i = SceneDecorationList.Count - 1; i >= 0; i--)
            {
                var item = SceneDecorationList[i];
                EditorGUILayout.BeginHorizontal();

                if (MyGUI.Button("删除", GUILayout.Width(50)))
                {
                    SceneDecorationList.RemoveAt(i);
                }

                if (MyGUI.Button(item.id))
                {
                    currSceneDecoration = item;
                }

                if (MyGUI.Button("复制", GUILayout.Width(50)))
                {
                    var consumableData = Tools.Json2Obj<SceneDecorationEditorwindowData>(Tools.Obj2Json(item, true));
                    SceneDecorationList.Add(consumableData);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (MyGUI.Button("+"))
            {
                SceneDecorationList.Add(new SceneDecorationEditorwindowData());
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            // 编辑框 add by TangJian 2017/11/15 16:28:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            editScrollViewPos = EditorGUILayout.BeginScrollView(editScrollViewPos);
            if (currSceneDecoration != null)
            {
                currSceneDecoration.id = MyGUI.TextFieldWithTitle("ID", currSceneDecoration.id);
                currSceneDecoration.sceneDecorationPosition = (SceneDecorationPosition)MyGUI.EnumPopupWithTitle("显示位置", currSceneDecoration.sceneDecorationPosition);
                UsePathorImg = EditorGUILayout.ToggleLeft(new GUIContent("使用路径还是贴图"), UsePathorImg);
                if (UsePathorImg)
                {
                    Texture2D img = (Texture2D)AssetDatabase.LoadAssetAtPath(currSceneDecoration.texturePath, typeof(Texture2D));
                    img = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("贴图"), img, typeof(Texture2D), UsePathorImg);
                    currSceneDecoration.texturePath = AssetDatabase.GetAssetPath(img);
                    EditorGUILayout.LabelField("图片路径:", currSceneDecoration.texturePath);
                }
                else
                {
                    currSceneDecoration.texturePath = MyGUI.TextFieldWithTitle("图片路径:", currSceneDecoration.texturePath);
                    Texture2D img = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("贴图"), AssetDatabase.LoadAssetAtPath(currSceneDecoration.texturePath, typeof(Texture2D)), typeof(Texture2D), UsePathorImg);
                }
                currSceneDecoration.scaleType = (tian.ScaleType)MyGUI.EnumPopupWithTitle("使用整体缩放大小还是单独缩放大小", currSceneDecoration.scaleType);
                if (currSceneDecoration.scaleType == tian.ScaleType.Alone)
                {
                    currSceneDecoration.AloneScale = MyGUI.FloatFieldWithTitle("单独缩放大小", currSceneDecoration.AloneScale);
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        void OnEnable()
        {
            title = "场景简单组件编辑器";
            loadSceneDecoration();
        }

        void loadSceneDecoration()
        {
            string jsonString = Tools.ReadStringFromFile(Application.dataPath + "/" + SceneDecorationFile);
            SceneDecorationDic = Tools.Json2Obj<Dictionary<string, SceneDecorationEditorwindowData>>(jsonString);

            SceneDecorationList = SceneDecorationDic.Values.ToList();
            currSceneDecoration = SceneDecorationList[0];

            string scalejson = Tools.ReadStringFromFile(Application.dataPath + "/Resources_moved/Scripts/SceneDecoration/scale.json");
            imgsclae = Tools.Json2Obj<float>(scalejson);
        }

        void saveSceneDecoration()
        {
            SceneDecorationDic = SceneDecorationList.ToDictionary(item => item.id, item => item);

            string jsonString = Tools.Obj2Json(SceneDecorationDic, true);
            Debug.Log("jsonString = " + jsonString);
            Tools.WriteStringFromFile(Application.dataPath + "/" + SceneDecorationFile, jsonString);
            string scalejson = Tools.Obj2Json(imgsclae, true);
            Tools.WriteStringFromFile(Application.dataPath + "/Resources_moved/Scripts/SceneDecoration/scale.json", scalejson);

            // 生成预制体 add by TangJian 2017/11/16 17:42:05
            {
                foreach (var item in SceneDecorationDic)
                {
                    var itemObject = PrefabCreator.CreateSceneDecoration(item.Value, prefabPath, imgsclae);
                    Tools.UpdatePrefab(itemObject, prefabPath + "/" + item.Value.id + ".prefab");
                    DestroyImmediate(itemObject);
                }
            }
        }

    }
}