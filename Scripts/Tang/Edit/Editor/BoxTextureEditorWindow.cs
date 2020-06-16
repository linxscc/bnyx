using UnityEngine;
using UnityEditor;
using Spine.Unity;


namespace Tang.Editor
{
    public class BoxTextureEditorWindow : EditorWindow
    {
        public tian.PlacementConfig placementConfig;
        public Material material;
        public float zoomLevel = 1;


        Vector2 scrollPosition = Vector2.zero;
        public float textureWidth;
        public float textureHeight;
        public Spine.Unity.Editor.SkeletonPreviewEditor skeletonPreviewEditor;

        public static void Open(tian.PlacementConfig placementConfig)
        {
            BoxTextureEditorWindow window = (BoxTextureEditorWindow)EditorWindow.GetWindow(typeof(BoxTextureEditorWindow));
            window.title = "UV Editor";

            window.placementConfig = placementConfig;

            window.Show();
            if (placementConfig.randerChoice == tian.RanderChoice.Anim)
            {
                Selection.activeObject = (Spine.Unity.SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(placementConfig.SkeletonDataAssetpath, typeof(Spine.Unity.SkeletonDataAsset));
                window.skeletonPreviewEditor= UnityEditor.Editor.CreateEditor(Selection.activeObject, typeof(Spine.Unity.Editor.SkeletonPreviewEditor)) as Spine.Unity.Editor.SkeletonPreviewEditor;
            }

            window.material = new Material(Shader.Find("Sprites/Default"));
        }

        void OnGUI()
        {
            GUIStyle boxStyle = new GUIStyle("box");
            if (placementConfig != null)
            {
                switch (placementConfig.randerChoice)
                {
                    case tian.RanderChoice.Image:
                        EditorGUILayout.LabelField("正面");
                        placementConfig.frontRect = EditorGUILayout.RectField(placementConfig.frontRect);

                        EditorGUILayout.LabelField("顶面");
                        placementConfig.topRect = EditorGUILayout.RectField(placementConfig.topRect);
                        zoomLevel = MyGUI.FloatFieldWithTitle("缩放比例", zoomLevel);
                        Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(placementConfig.ImagePath, typeof(Texture2D));
                        textureHeight = texture.height;
                        textureWidth = texture.width;

                        Vector2 imagePos = new Vector2(20, 150);
                        Rect imageRect = new Rect(imagePos.x, imagePos.y, textureWidth * zoomLevel, textureHeight * zoomLevel);

                        EditorGUI.DrawRect(imageRect, new Color(0.7f, 0.7f, 0.7f, 0.7f));
                        EditorGUI.DrawPreviewTexture(imageRect, texture, material, ScaleMode.ScaleToFit);

                        DrawZones(new Rect(placementConfig.frontRect.x, 1.0f - placementConfig.frontRect.y, placementConfig.frontRect.width, placementConfig.frontRect.height), imagePos, Color.red, texture);
                        DrawZones(new Rect(placementConfig.topRect.x, 1.0f - placementConfig.topRect.y, placementConfig.topRect.width, placementConfig.topRect.height), imagePos, Color.blue,texture);
                        break;
                    case tian.RanderChoice.Anim:
                        placementConfig.colliderBounds.center = EditorGUILayout.Vector3Field("center", placementConfig.colliderBounds.center);
                        placementConfig.colliderBounds.size = EditorGUILayout.Vector3Field("size",placementConfig.colliderBounds.size);
                        SkeletonAnimOnGUI(boxStyle, position.width,position.height-50f);
                        skeletonPreviewEditor.AddDrawExtraGuideLineAction("colliderBounds", (Camera camera) =>
                        {
                            Handles.color = Color.green;
                            Handles.DrawWireCube(placementConfig.colliderBounds.center, placementConfig.colliderBounds.size);
                        });
                        break;
                }
                
            }
        }

        void DrawZones(Rect rect, Vector2 offset, Color color,Texture tex=null)
        {
            float x1 = rect.x;
            float x2 = rect.x + rect.width;

            float y1 = rect.y;
            float y2 = rect.y - rect.height;

            if ((Mathf.Floor(x1) == Mathf.Floor(x2)) && (Mathf.Floor(y1) == Mathf.Floor(y2)))
            {
                Rect newRect = new Rect(rect);
                x1 = x1 - Mathf.Floor(x1);
                y1 = y1 - Mathf.Floor(y1);
                newRect.x = x1;
                newRect.y = y1;

                DrawRect(tex, newRect, offset, color);
            }
            else
            {
                if ((Mathf.Floor(x1) != Mathf.Floor(x2)) && (Mathf.Floor(y1) != Mathf.Floor(y2)))
                {
                    float newRectY1 = Mathf.Min(y1 - Mathf.Floor(y1), y2 - Mathf.Floor(y2));
                    float newRectY2 = Mathf.Max(y1 - Mathf.Floor(y1), y2 - Mathf.Floor(y2));

                    float newRectX1 = Mathf.Min(x1 - Mathf.Floor(x1), x2 - Mathf.Floor(x2));
                    float newRectX2 = Mathf.Max(x1 - Mathf.Floor(x1), x2 - Mathf.Floor(x2));

                    Rect rect1 = new Rect(rect);
                    rect1.y = newRectY1;
                    rect1.x = 0;
                    rect1.height = newRectY1;
                    rect1.width = newRectX1;

                    DrawRect(tex, rect1, offset, color);

                    rect1.y = newRectY1;
                    rect1.x = newRectX2;
                    rect1.height = newRectY1;
                    rect1.width = 1.0f - newRectX2; ;

                    DrawRect(tex, rect1, offset, color);

                    Rect rect2 = new Rect(rect);
                    rect2.y = 1.0f;
                    rect2.x = newRectX2;
                    rect2.height = 1.0f - newRectY2;
                    rect2.width = 1.0f - newRectX2;

                    DrawRect(tex, rect2, offset, color);

                    rect2.y = 1.0f;
                    rect2.x = 0.0f;
                    rect2.height = 1.0f - newRectY2;
                    rect2.width = newRectX1;

                    DrawRect(tex, rect2, offset, color);
                }
                else
                {

                    if (Mathf.Floor(x1) != Mathf.Floor(x2))
                    {
                        float newRectX1 = Mathf.Min(x1 - Mathf.Floor(x1), x2 - Mathf.Floor(x2));
                        float newRectX2 = Mathf.Max(x1 - Mathf.Floor(x1), x2 - Mathf.Floor(x2));

                        float newY = y1 - Mathf.Floor(y1);

                        Rect rect1 = new Rect(rect);
                        rect1.x = 0;
                        rect1.y = newY;
                        rect1.width = newRectX1;

                        DrawRect(tex, rect1, offset, color);

                        Rect rect2 = new Rect(rect);
                        rect2.x = newRectX2;
                        rect2.y = newY;
                        rect2.width = 1.0f - newRectX2;

                        DrawRect(tex, rect2, offset, color);
                    }

                    if (Mathf.Floor(y1) != Mathf.Floor(y2))
                    {
                        float newRectY1 = Mathf.Min(y1 - Mathf.Floor(y1), y2 - Mathf.Floor(y2));
                        float newRectY2 = Mathf.Max(y1 - Mathf.Floor(y1), y2 - Mathf.Floor(y2));


                        float newX = x1 - Mathf.Floor(x1);

                        Rect rect1 = new Rect(rect);
                        rect1.y = newRectY1;
                        rect1.x = newX;
                        rect1.height = newRectY1;

                        DrawRect(tex, rect1, offset, color);

                        Rect rect2 = new Rect(rect);
                        rect2.y = 1.0f;
                        rect2.x = newX;
                        rect2.height = 1.0f - newRectY2;

                        DrawRect(tex, rect2, offset, color);
                    }
                }
            }
        }
        void Update()
        {
            if (mouseOverWindow) // 如果鼠标在窗口上, 则重绘界面 add by TangJian 2017/09/28 21:57:44
            {
                base.Repaint();
            }
        }

        void OnDisable()
        {
            skeletonPreviewEditor.ClearDrawExtraGuideLineActions();
        }

        void DrawRect(Texture tex, Rect rect, Vector2 offset, Color color)
        {
            Rect clampedRect = new Rect(rect);

            clampedRect.x = Mathf.Clamp(clampedRect.x, 0.0f, 1.0f);
            clampedRect.y = Mathf.Clamp(clampedRect.y, 0.0f, 1.0f);
            clampedRect.width = Mathf.Clamp(clampedRect.width, 0.0f, 1.0f);
            clampedRect.height = Mathf.Clamp(clampedRect.height, 0.0f, 1.0f);

            float endCoordX = clampedRect.x + clampedRect.width;
            if (endCoordX > 1.0f)
            {
                clampedRect.width = 1.0f - clampedRect.x;
            }

            float endCoordY = clampedRect.y - clampedRect.height;
            if (endCoordY < 0.0f)
            {
                clampedRect.height = clampedRect.y;
            }

            EditorGUI.DrawRect(new Rect(clampedRect.x * textureWidth * zoomLevel + offset.x,
                                        clampedRect.y * textureHeight * zoomLevel + offset.y,
                                        clampedRect.width * textureWidth * zoomLevel,
                                        1.0f),
                                           color);


            EditorGUI.DrawRect(new Rect(clampedRect.x * textureWidth * zoomLevel + clampedRect.width * textureWidth * zoomLevel + offset.x,
                                        clampedRect.y * textureHeight * zoomLevel + offset.y,
                                        1.0f,
                                        -clampedRect.height * textureHeight * zoomLevel), color);


            EditorGUI.DrawRect(new Rect(clampedRect.x * textureWidth * zoomLevel + offset.x,
                                        clampedRect.y * textureHeight * zoomLevel - clampedRect.height * textureHeight * zoomLevel + offset.y,
                                        clampedRect.width * textureWidth * zoomLevel,
                                        -1.0f),
                                           color);

            EditorGUI.DrawRect(new Rect(clampedRect.x * textureWidth * zoomLevel + offset.x,
                                        clampedRect.y * textureHeight * zoomLevel + offset.y,
                                        1.0f,
                                        -clampedRect.height * textureHeight * zoomLevel), color);
        }
        void SkeletonAnimOnGUI(GUIStyle boxStyle, System.Single innerBoxWidth, float height)
        {
            var Target = Selection.activeObject;
            EditorGUILayout.BeginHorizontal();
            if (Target is SkeletonDataAsset && Target == Selection.activeObject)
            {
                // 动画选择列表 add by TangJian 2018/8/24 16:05
                {
                    EditorGUILayout.BeginVertical(boxStyle, GUILayout.Width(((innerBoxWidth / 3) * 2 - 32) * 0.3f), GUILayout.ExpandHeight(true));

                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                    // PreviewEditor.OnInspectorGUI ();
                    EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
                    skeletonPreviewEditor.DrawAnimationList();
                    // EditorGUILayout.Space();
                    //PreviewEditor.DrawSlotList();
                    // EditorGUILayout.Space();
                    // PreviewEditor.DrawUnityTools();
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                }

                // 动画播放界面 add by TangJian 2018/8/24 16:05
                {
                    EditorGUILayout.BeginVertical(boxStyle);

                    if (skeletonPreviewEditor.HasPreviewGUI())
                    {
                        Rect r = GUILayoutUtility.GetRect(((innerBoxWidth / 3) * 2 - 32) * 0.7f, height);
                        GUIStyle style = new GUIStyle("PreBackground");
                        skeletonPreviewEditor.OnInteractivePreviewGUI(r, style);
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
