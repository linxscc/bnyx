using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace Tang.Editor
{
    public class TagTirggerMatrixEditorWindow : EditorWindow
    {
        [MenuItem("Window/tag触发器触发判断编辑")]
        static void Init()
        {
            TagTirggerMatrixEditorWindow window = (TagTirggerMatrixEditorWindow)EditorWindow.GetWindow(typeof(TagTirggerMatrixEditorWindow));
            window.Show();
        }
        public string path = "Resources_moved/Scripts/Tagtrigger/";
        public string TagTirggerListPath = "Resources_moved/Scripts/Tagtrigger/TagTirggerList.json";
        private string TagTirggerListDataPath = "Assets/Resources_moved/Manager/TagTriggerListData.asset";
        public TagTriggerListData tagTriggerListData;
        public delegate bool GetValueFunc(int layerA, int layerB);
        public delegate void SetValueFunc(int layerA, int layerB, bool val);
        public bool windowbool = true;
        public bool taglistbool = false;
        Vector2 scrollPos;
        Vector2 ScrollPos2;
        //public List<string> Oldtaglist = new List<string>();
        public List<string> Newtaglist = new List<string>();
        string tagname;
        bool addremove;
        //public bool[,] OldTagTirggerList;
        public bool[,] NewTagTirggerList;
        protected SerializedProperty m_Tags;
        ReorderableList m_TagsList;
        public void OnGUI()
        {
            GUIStyle boxStyle = new GUIStyle("box");

            var width = position.size.x - boxStyle.border.horizontal;
            var height = position.size.y - boxStyle.border.vertical;
            var innerBoxWidth = width - (boxStyle.padding.horizontal + boxStyle.border.horizontal);
            var innerBoxHeight = height - (boxStyle.padding.vertical + boxStyle.border.vertical);
            MyGUI.Layout(position, boxStyle, new int[,]
            {
                { 0 ,1, 1 },
                { 2 ,1, 1 },
                { 2, 1, 1 },
                //{ 1, 1, 1 },
            },
            (Rect pos) =>
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(TagTirggerListPath);
                EditorGUILayout.BeginHorizontal();
                if (MyGUI.Button("保存"))
                {
                    savetagTirgger();
                }
                if (MyGUI.Button("读取"))
                {
                    LoadtagTirgger();
                }
                if (MyGUI.Button("增加Tag"))
                {

                    if (tagname != "" && tagname != null)
                    {
                        AddTag(tagname);
                    }
                    tagname = "";
                }
                if (MyGUI.Button("删除Tag"))
                {

                    if (tagname != "" && tagname != null)
                    {
                        RemoveTag(tagname);
                    }
                    tagname = "";
                }
                EditorGUILayout.EndHorizontal();
                tagname = MyGUI.TextFieldWithTitle("tagName:", tagname);
                EditorGUILayout.EndVertical();
            },
            (Rect pos) =>
            {
                //EditorGUILayout.BeginHorizontal();
                //if (MyGUI.Button("保存"))
                //{

                //}
                //if (MyGUI.Button("读取"))
                //{

                //}
                //EditorGUILayout.EndHorizontal();
                DoGUI("hhh", ref windowbool, ref scrollPos, getValue, setValue, pos.width, pos.height, pos.x, pos.y);
            },
            (Rect pos) =>
            {
                taglistdogui(ref taglistbool, "dsa", pos.width - 10, pos.height - 10);
            },
            (Rect pos) =>
            {
                //DoGUI("hhh", ref windowbool, ref scrollPos, getValue, setValue, pos.width, pos.height,pos.x,pos.y);
            }
            );
            //EditorGUILayout.BeginHorizontal();
            //taglistdogui(ref taglistbool, "dsa");
            //DoGUI("hhh", ref windowbool, ref scrollPos, getValue, setValue, width, height);
            //EditorGUILayout.EndHorizontal();

        }

        public bool getValue(int layerA, int layerB)
        {
            if (NewTagTirggerList != null)
            {
                return NewTagTirggerList[layerA, layerB];
            }
            else
            {
                return false;
            }
            //return NewTagTirggerList[layerA,layerB];
        }
        public void setValue(int layerA, int layerB, bool val)
        {
            if (layerA != layerB)
            {
                NewTagTirggerList[layerA, layerB] = val;
                NewTagTirggerList[layerB, layerA] = val;
            }
            else
            {
                NewTagTirggerList[layerA, layerB] = val;
            }
            //NewTagTirggerList[layerA, layerB] = val;
        }
        public void taglistdogui(ref bool show, string title, float width, float height)
        {
            show = EditorGUILayout.Foldout(show, title, true);
            if (show)
            {
                ScrollPos2 = EditorGUILayout.BeginScrollView(ScrollPos2, GUILayout.Width(width), GUILayout.Height(height));
                //EditorGUILayout.BeginVertical();
                for (int ix = InternalEditorUtility.tags.Length - 1; ix >= 0; ix--)
                {
                    EditorGUILayout.LabelField(InternalEditorUtility.tags[ix]);
                }
                //EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
        }
        public void listclear()
        {
            int count = InternalEditorUtility.tags.Length;
            NewTagTirggerList = new bool[count, count];
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    NewTagTirggerList[i, j] = false;
                }
            }
            //Array.Clear(NewTagTirggerList, 0, count);
        }
        void OnEnable()
        {
            titleContent = new GUIContent("tag触发器触发判断编辑");
            listclear();
            //Newtaglist.AddArray(InternalEditorUtility.tags);
            LoadtagTirgger();
            // loadWeapon();
            //NewTagTirggerList = new bool[InternalEditorUtility.tags.Length, InternalEditorUtility.tags.Length];
        }
        public static void DoGUI(string title, ref bool show, ref Vector2 scrollPos, GetValueFunc getValue, SetValueFunc setValue, float width, float height, float xdasd, float ydsda)
        {
            int kMaxLayers = InternalEditorUtility.tags.Length;
            const int checkboxSize = 16;
            int labelSize = 100;
            const int indent = 30;

            int numLayers = 0;
            for (int i = 0; i < kMaxLayers; i++)
                //if (LayerMask.LayerToName(i) != "")
                numLayers++;

            // find the longest label
            for (int i = 0; i < kMaxLayers; i++)
            {
                var textDimensions = GUI.skin.label.CalcSize(new GUIContent(InternalEditorUtility.tags[i]));
                if (labelSize < textDimensions.x)
                    labelSize = (int)textDimensions.x;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(0);
            show = EditorGUILayout.Foldout(show, title, true);
            GUILayout.EndHorizontal();
            if (show)
            {
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(labelSize + 20), GUILayout.MaxHeight(labelSize + (numLayers + 1) * checkboxSize));
                Rect topLabelRect = GUILayoutUtility.GetRect(checkboxSize * numLayers + labelSize, labelSize);
                //topLabelRect.position = new Vector2(xasd , yasd);
                Rect scrollArea = new Rect(0, 5, width - 10, height - 10);
                Vector2 topLeft = new Vector2(topLabelRect.x, topLabelRect.y);
                int y = 0;
                for (int i = 0; i < kMaxLayers; i++)
                {
                    //if (LayerMask.LayerToName(i) != "")
                    {
                        // Need to do some shifting around here, so the rotated text will still clip correctly
                        float clipOffset = (labelSize + indent + (numLayers - y) * checkboxSize) - (scrollArea.width + scrollPos.x);
                        if (clipOffset < 0)
                            clipOffset = 0;

                        Vector3 translate = new Vector3(labelSize + indent + checkboxSize * (numLayers - y) + topLeft.y + topLeft.x + scrollPos.y - clipOffset, topLeft.y + scrollPos.y, 0);
                        GUI.matrix = Matrix4x4.TRS(translate, Quaternion.identity, Vector3.one) * Matrix4x4.TRS(new Vector3(xdasd + 48, ydsda - 360, 0), Quaternion.Euler(0, 0, 90), Vector3.one);

                        GUI.Label(new Rect(2 - topLeft.x - scrollPos.y, scrollPos.y - clipOffset, labelSize, checkboxSize), InternalEditorUtility.tags[i], "RightLabel");
                        y++;
                    }
                }
                GUI.matrix = Matrix4x4.identity;
                y = 0;
                for (int i = 0; i < kMaxLayers; i++)
                {
                    //if (LayerMask.LayerToName(i) != "")
                    {
                        int x = 0;
                        Rect r = GUILayoutUtility.GetRect(indent + checkboxSize * numLayers + labelSize, checkboxSize);
                        GUI.Label(new Rect(r.x + indent, r.y, labelSize, checkboxSize), InternalEditorUtility.tags[i], "RightLabel");
                        for (int j = kMaxLayers - 1; j >= 0; j--)
                        {
                            //if (LayerMask.LayerToName(j) != "")
                            {
                                if (x < numLayers - y)
                                {
                                    GUIContent tooltip = new GUIContent("", InternalEditorUtility.tags[i] + "/" + InternalEditorUtility.tags[j]);
                                    bool val = getValue(i, j);
                                    bool toggle = GUI.Toggle(new Rect(labelSize + indent + r.x + x * checkboxSize, r.y, checkboxSize, checkboxSize), val, tooltip);
                                    if (toggle != val)
                                        setValue(i, j, toggle);
                                }
                                x++;
                            }
                        }
                        y++;
                    }
                }
                GUILayout.EndScrollView();
            }
        }
        void savetagTirgger()
        {
            string jsonString = Tools.Obj2Json(NewTagTirggerList, true);
            Tools.WriteStringFromFile(Application.dataPath + "/" + TagTirggerListPath, jsonString);
            string jsonlist = Tools.Obj2Json(Newtaglist, true);
            Tools.WriteStringFromFile(Application.dataPath + "/" + path + "TagList.json", jsonlist);

            tagTriggerListData.TagList = Newtaglist;
            tagTriggerListData.tagTriggerBoolList.Clear();
            int coutn = Newtaglist.Count;
            for(int t = 0; t < coutn; t++)
            {
                tagTriggerListData.tagTriggerBoolList.Add(new TagTriggerBoolList());
                for (int xt = 0; xt < coutn; xt++)
                {
                    tagTriggerListData.tagTriggerBoolList[t].list.Add(NewTagTirggerList[t,xt]);
                }
            }
            //tagTriggerListData.TagTriggerListString = jsonString;
            EditorUtility.SetDirty(tagTriggerListData);
            AssetDatabase.SaveAssets();

            Debug.Log("Save");
        }
        void LoadtagTirgger()
        {
            bool onoff = false;
            string jsonString = Tools.ReadStringFromFile(Application.dataPath + "/" + TagTirggerListPath);
            string jsonlist = Tools.ReadStringFromFile(Application.dataPath + "/" + path + "TagList.json");
            //NewTagTirggerList = Tools.Json2Obj<bool[,]>(jsonString);
            bool[,] OldTagTirggerList = Tools.Json2Obj<bool[,]>(jsonString);
            List<string> Oldtaglist = new List<string>();
            Oldtaglist = Tools.Json2Obj<List<string>>(jsonlist);
            Newtaglist.Clear();
            Newtaglist.AddArray<string>(InternalEditorUtility.tags);
            if (Newtaglist.Count == Oldtaglist.Count)
            {
                for (int i = 0; i < Oldtaglist.Count; i++)
                {
                    if (Newtaglist[i] != Oldtaglist[i])
                    {
                        onoff = true;
                        break;
                    }
                }
            }
            else
            {
                onoff = true;
            }
            if (onoff)
            {
                CompareCreate(Oldtaglist, Newtaglist, OldTagTirggerList);
            }
            else
            {
                NewTagTirggerList = OldTagTirggerList;
            }
            tagTriggerListData = AssetDatabase.LoadAssetAtPath<TagTriggerListData>(TagTirggerListDataPath);
            if (tagTriggerListData == null)
            {
                tagTriggerListData = ScriptableObject.CreateInstance<TagTriggerListData>();
                AssetDatabase.CreateAsset(tagTriggerListData, TagTirggerListDataPath);
            }
        }
        void AddTag(string tagname)
        {
            bool[,] OldTagTirggerList = Tools.Json2Obj<bool[,]>(Tools.Obj2Json(NewTagTirggerList));
            List<string> Oldtaglist = new List<string>();
            Oldtaglist.AddList(Newtaglist);
            //Oldtaglist = Tools.Json2Obj<List<string>>(Tools.Obj2Json(Newtaglist));
            InternalEditorUtility.AddTag(tagname);
            listclear();
            Newtaglist.Clear();
            Newtaglist.AddArray<string>(InternalEditorUtility.tags);
            CompareCreate(Oldtaglist, Newtaglist, OldTagTirggerList);
        }
        void RemoveTag(string tagname)
        {
            bool[,] OldTagTirggerList = Tools.Json2Obj<bool[,]>(Tools.Obj2Json(NewTagTirggerList));
            List<string> Oldtaglist = new List<string>();
            Oldtaglist.AddList(Newtaglist);
            //Oldtaglist = Tools.Json2Obj<List<string>>(Tools.Obj2Json(Newtaglist));
            InternalEditorUtility.RemoveTag(tagname);
            listclear();
            Newtaglist.Clear();
            Newtaglist.AddArray<string>(InternalEditorUtility.tags);
            CompareCreate(Oldtaglist, Newtaglist, OldTagTirggerList);
        }

        void CompareCreate(List<string> Oldtaglist, List<string> Newtaglist, bool[,] OldTagTirggerList)
        {
            Dictionary<string, int> newindex = new Dictionary<string, int>();
            foreach (var item in Newtaglist)
            {
                if (Oldtaglist.Contains(item))
                {
                    newindex.Add(item, Oldtaglist.IndexOf(item));
                }
                else
                {
                    newindex.Add(item, -1);
                }
            }

            for (int i = 0; i < Newtaglist.Count; i++)
            {
                if (newindex[Newtaglist[i]] != -1)
                {
                    for (int j = 0; j < Newtaglist.Count; j++)
                    {
                        if (newindex[Newtaglist[j]] != -1)
                        {
                            try
                            {
                                NewTagTirggerList[i, j] = OldTagTirggerList[newindex[Newtaglist[i]], newindex[Newtaglist[j]]];
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e);
                            }

                        }
                        else
                        {
                            NewTagTirggerList[i, j] = false;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < Newtaglist.Count; j++)
                    {
                        NewTagTirggerList[i, j] = false;
                    }
                }
            }
        }

    }


}

