using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
namespace Tang.Editor
{
	public class EffectEditorWindow : EditorWindow 
	{
		[MenuItem("Window/特效动画资源路径存储")]
		static void Init()
        {
            EffectEditorWindow window = (EffectEditorWindow)EditorWindow.GetWindow(typeof(EffectEditorWindow));
            window.Show();
        }
		void OnEnable()
        {
            titleContent = new GUIContent("特效动画资源路径存储");
            // loadWeapon();
			loadskeletonAsstes();
        }
		private string EffectFile = Definition.SpineAssetPath;
		private string EffectFilejson = Definition.SpineAssetPath + "/Effects/";
		List<string> EffectFileList = new List<string> ();
		void OnGUI()
		{
			GUIStyle boxStyle = new GUIStyle("box");

            var width = position.size.x - boxStyle.border.horizontal;
            var height = position.size.y - boxStyle.border.vertical;

            var innerBoxWidth = width - (boxStyle.padding.horizontal + boxStyle.border.horizontal);
            var innerBoxHeight = height - (boxStyle.padding.vertical + boxStyle.border.vertical);

			EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(width), GUILayout.Height(height));

				EditorGUILayout.BeginHorizontal();
					MyGUI.TextFieldWithTitle("动画资源路径:",EffectFile);
					if (MyGUI.Button("动画资源列表刷新"))
					{
						load();
						saveskeletonAsstes();
					}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
					MyGUI.TextFieldWithTitle("动画资源列表json路径:",EffectFilejson);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginVertical();
					for (int i = EffectFileList.Count - 1; i >= 0; i--)
					{
						EditorGUILayout.LabelField(EffectFileList[i]);
					}
				EditorGUILayout.EndVertical();

			EditorGUILayout.EndVertical();
		}

		void load()
		{
			List<string> Effectzhan=new List<string>();
			EffectFileList.Clear();
			Effectzhan=Tools.GetFilesInFolder(EffectFile,".asset");
			foreach (var item in Effectzhan)
			{
				if (item.IndexOf("SkeletonData")!=-1)
				{
                    var str=item.Replace( '\\', '/');
					EffectFileList.Add(str);
				}
			}
		}

		void saveskeletonAsstes()
		{
			string tostr = Tools.Obj2Json(EffectFileList, true);
            Tools.WriteStringFromFile(EffectFilejson+"effectFile.json", tostr);
		}

		void loadskeletonAsstes()
		{
			if (File.Exists(EffectFilejson+"effectFile.json"))
			{
				string jsonString = Tools.ReadStringFromFile(EffectFilejson+"effectFile.json");
				EffectFileList=Tools.Json2Obj<List<string>>(jsonString);
			}
			
		}
	}
}

