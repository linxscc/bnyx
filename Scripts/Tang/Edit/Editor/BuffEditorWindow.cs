using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


namespace Tang.Editor
{
    public class BuffEditorWindow : EditorWindow
    {
        [MenuItem("Window/增益编辑器")]
        static void Init()
        {
            BuffEditorWindow window = (BuffEditorWindow)EditorWindow.GetWindow(typeof(BuffEditorWindow));
            window.Show();
        }

        private string buffDataFile = "Resources_moved/Scripts/Buff/Buff.json";
        private string prefabPath = "Assets/Resources_moved/Prefabs/Item/Equip/Buff";

        Dictionary<string, BuffData> buffDataDic = new Dictionary<string, BuffData>();
        List<BuffData> buffDataList = new List<BuffData>();

        BuffData currBuffData;
        
        Vector2 listScrollViewPos = Vector2.zero;
        Vector2 editScrollViewPos = Vector2.zero;


        Rect windowRect = new Rect(100, 100, 500, 500);

        BuffEventType currAddBuffEventType = BuffEventType.OnBuffBegin;

        void OnGUI()
        {
            GUIStyle boxStyle = new GUIStyle("box");

            var width = position.size.x - boxStyle.border.horizontal;
            var height = position.size.y - boxStyle.border.vertical;

            var innerBoxWidth = width - (boxStyle.padding.horizontal + boxStyle.border.horizontal);
            var innerBoxHeight = height - (boxStyle.padding.vertical + boxStyle.border.vertical);
            
            MyGUIExtend.Instance.ToolbarButton(new Dictionary<string, Action>
            {
                {
                    "读取",(() => { loadBuff(); })
                },
                {
                    "保存",(() => { saveBuff();})
                }
            });
            
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(width), GUILayout.Height(height));

            // 设置路径, 以及存取数据 add by TangJian 2017/11/15 16:22:45
            EditorGUILayout.BeginHorizontal();
            
            MyGUIExtend.Instance.Foldout("BuffEditor","路径信息",(() =>
            {
                EditorGUILayout.LabelField(buffDataFile);
            }));
            
            EditorGUILayout.EndHorizontal();

            // EditorGUILayout.BeginHorizontal();
            // EditorGUILayout.LabelField(prefabPath);
            // if (MyGUI.Button("制作预制体"))
            // {
            //     saveBuff();
            // }
            // EditorGUILayout.EndHorizontal();

            // 编辑区域 add by TangJian 2017/11/15 16:28:19
            EditorGUILayout.BeginHorizontal();

            // // 列表框 add by TangJian 2017/11/15 16:27:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            listScrollViewPos = EditorGUILayout.BeginScrollView(listScrollViewPos);

            for (int i = buffDataList.Count - 1; i >= 0; i--)
            {
                var item = buffDataList[i];
                
                EditorGUILayout.BeginHorizontal();
                int Index = MyGUIExtend.Instance.ListSingleButton("BuffEditor",item.id,i,(() => { currBuffData = item; }) );
                
                MyGUIExtend.Instance.Mouse_RightDrop(new Dictionary<string, Action>
                {
                    {
                        "删除",(() => { buffDataList.RemoveAt(Index); })
                    },
                    {
                        "复制",(() => 
                        { 
                            var buffData = Tools.Json2Obj<BuffData>(Tools.Obj2Json(buffDataList[Index], true));
                            buffDataList.Add(buffData); 
                        })
                    }
                });
                EditorGUILayout.EndHorizontal();
            }

            if (MyGUI.Button("+"))
            {
                buffDataList.Add(new BuffData());
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            // 编辑框 add by TangJian 2017/11/15 16:28:46
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(innerBoxWidth / 2), GUILayout.ExpandHeight(true));
            editScrollViewPos = EditorGUILayout.BeginScrollView(editScrollViewPos);
            if (currBuffData != null)
            {
                currBuffData.id = MyGUI.TextFieldWithTitle("id", currBuffData.id);
                currBuffData.name = MyGUI.TextFieldWithTitle("name", currBuffData.name);
                currBuffData.duration = MyGUI.FloatFieldWithTitle("duration", currBuffData.duration);
                currBuffData.updateInterval = MyGUI.FloatFieldWithTitle("updateInterval", currBuffData.updateInterval);

                currBuffData.attrData = MyGUI.AttrDataField(currBuffData.attrData);

                // 
                currAddBuffEventType = (BuffEventType)MyGUI.EnumPopupWithTitle("事件类型", currAddBuffEventType);
                if (MyGUI.Button("添加"))
                {
                    currBuffData.buffEvents.Add(currAddBuffEventType, new List<ActionData>());
                }

                if (MyGUI.Button("移除"))
                {
                    if (currBuffData.buffEvents.ContainsKey(currAddBuffEventType))
                        currBuffData.buffEvents.Remove(currAddBuffEventType);
                }

                List<BuffEventType> needRemoveBuffEventList = new List<BuffEventType>();
                foreach (var buffEvent in currBuffData.buffEvents)
                {
                    List<ActionData> actionDataList = buffEvent.Value;
                    DrawActionList(buffEvent.Key.ToString(), ref actionDataList);
                }

                foreach (var item in needRemoveBuffEventList)
                {
                    currBuffData.buffEvents.Remove(item);
                }
            }

            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        public bool actionListVisable = false;

        Dictionary<object, bool> actionListVisableDic = new Dictionary<object, bool>();

        void DrawActionList(string title, ref List<ActionData> actionDataList)
        {
            EditorGUILayout.BeginHorizontal();

            if (actionListVisableDic.TryGetValue(actionDataList, out actionListVisable) == false)
                actionListVisable = false;
            actionListVisable = MyGUI.FoldoutWithTitle(title, actionListVisable);
            actionListVisableDic[actionDataList] = actionListVisable;

            if (actionListVisable)
            {
                if (MyGUI.Button("添加"))
                {
                    actionDataList.Add(new ActionData());
                }
            }
            EditorGUILayout.EndHorizontal();
            if (actionListVisable)
            {
                GUIStyle boxStyle = new GUIStyle("box");
                EditorGUILayout.BeginVertical(boxStyle, GUILayout.ExpandWidth(true));

                for (int i = actionDataList.Count - 1; i >= 0; i--)
                {
                    EditorGUILayout.BeginHorizontal();


                    var actionData = actionDataList[i];
                    DrawActionData(ref actionData);

                    if (MyGUI.Button("删除"))
                    {
                        actionDataList.RemoveAt(i);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }
        }

        void DrawActionData(ref ActionData actionData)
        {

            GUIStyle boxStyle = new GUIStyle("box");
            EditorGUILayout.BeginVertical(boxStyle, GUILayout.ExpandWidth(true));

            actionData.type = (ActionType)MyGUI.EnumPopupWithTitle("type", actionData.type);
            actionData.chance = MyGUI.IntFieldWithTitle("概率",actionData.chance);
            switch (actionData.type)
            {
                case ActionType.AddAttr:
                    AttrType attrType = (AttrType)actionData.int1;
                    attrType = (AttrType)MyGUI.EnumPopupWithTitle("属性", attrType);
                    actionData.int1 = (int)attrType;

                    actionData.float1 = MyGUI.FloatFieldWithTitle("数值", actionData.float1);
                    break;
                case ActionType.AddAnim:
                    actionData.string1 = MyGUI.TextFieldWithTitle("动画名", actionData.string1);
                    actionData.bool1 = MyGUI.ToggleWithTitle("是否循环", actionData.bool1);
                    break;
                case ActionType.RemoveAnim:
                    actionData.string1 = MyGUI.TextFieldWithTitle("动画名", actionData.string1);
                    break;
                case ActionType.SetColor:
                    actionData.float1 = MyGUI.FloatFieldWithTitle("红", actionData.float1);
                    actionData.float2 = MyGUI.FloatFieldWithTitle("绿", actionData.float2); 
                    actionData.float3 = MyGUI.FloatFieldWithTitle("蓝", actionData.float3); 
                    break;
                case ActionType.CreateDamage:
                    // AttrType asd = (AttrType)actionData.int1;
                    actionData.vector3_1=MyGUI.Vector3WithTitle("pos",actionData.vector3_1);
                    actionData.vector3_2=MyGUI.Vector3WithTitle("size",actionData.vector3_2);
                    actionData.vector3_3=MyGUI.Vector3WithTitle("force",actionData.vector3_3);
                    HitType hittype = (HitType)actionData.int1;
                    hittype = (HitType)MyGUI.EnumPopupWithTitle("Hit Type", hittype);
                    actionData.int1 = (int)hittype;
                    Direction direction=(Direction)actionData.int2;
                    direction=(Direction)MyGUI.EnumPopupWithTitle("Direction", direction);
                    actionData.int2=(int)direction;
                    break;
            }

            EditorGUILayout.EndVertical();


        }

        void OnEnable()
        {
            title = "增益编辑器";
            loadBuff();
        }

        void loadBuff()
        {
            //string jsonString = Tools.ReadStringFromFile(Application.dataPath + "/" + buffDataFile);
            buffDataDic = Tools.LoadOneData<BuffData>(Application.dataPath + "/" + "Resources_moved/Scripts/Buff/BuffList");

            buffDataList = buffDataDic.Values.ToList();
            currBuffData = buffDataList[0];
        }

        void saveBuff()
        {
            buffDataDic = buffDataList.ToDictionary(item => item.id, item => item);

            //string jsonString = Tools.Obj2Json(buffDataDic, true);
            //Debug.Log("jsonString = " + jsonString);
            //Tools.WriteStringFromFile(Application.dataPath + "/" + buffDataFile, jsonString);
            Tools.SaveOneData<BuffData>(buffDataDic, Application.dataPath + "/" + "Resources_moved/Scripts/Buff/BuffList");
        }
    }
}