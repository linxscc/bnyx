using UnityEngine;
using UnityEditor;

namespace Tang.Editor
{
    public enum ShakeXYEnum
    {
        ShakeX = 0,
        ShakeY = 1,
        FastShake = 2,
        ShakeV3 = 3,

    }
    [CustomEditor(typeof(AnimManager))]
    public class AnimManagerEditor : UnityEditor.Editor
    {
        bool shakeEffectVisable = false;
        bool CameraScaleVisable = false;
        float CameraScale = 0.1f;
        float duration = 1.0f;
        float strength = 1;
        Vector3 strengthV3 = new Vector3(0, 0, 0);
        int vibrato = 10;
        float randomness = 90;
        bool snapping = false;
        bool fadeOut = true;
        public ShakeXYEnum shakeXYEnum = ShakeXYEnum.FastShake;
        GameObject game = null;
        float longlong = 3;
        float heitht = 3;
        float t = 1;


        string animEffectId = "";

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                animEffectId = MyGUI.TextFieldWithTitle("animEffectId", animEffectId);
                if (MyGUI.Button("测试特效"))
                {
                    var playerObject = GameObject.Find("Player1");
                    AnimManager.Instance.PlayAnimEffect(animEffectId, playerObject.transform.position);
                }

                shakeEffectVisable = MyGUI.FoldoutWithTitle("Shake", shakeEffectVisable);
                if (shakeEffectVisable)
                {
                    shakeXYEnum = (ShakeXYEnum)MyGUI.EnumPopupWithTitle("震动类型", shakeXYEnum);
                    EditorGUILayout.BeginVertical();

                    duration = MyGUI.FloatFieldWithTitle("持续时间(duration)", duration);
                    if (shakeXYEnum == ShakeXYEnum.ShakeV3)
                    {
                        strengthV3 = MyGUI.Vector3WithTitle("震动幅度(strength)Vector3", strengthV3);
                    }
                    else
                    {
                        strength = MyGUI.FloatFieldWithTitle("震动幅度(strength)", strength);
                    }

                    vibrato = MyGUI.IntFieldWithTitle("震动强度(vibrato)", vibrato);
                    randomness = MyGUI.FloatFieldWithTitle("随机性震动(randomness)", randomness);
                    snapping = MyGUI.ToggleWithTitle("snapping", snapping);
                    fadeOut = MyGUI.ToggleWithTitle("fadeOut", fadeOut);

                    EditorGUILayout.EndVertical();

                    if (MyGUI.Button("测试震动"))
                    {
                        switch (shakeXYEnum)
                        {
                            case ShakeXYEnum.FastShake:
                                AnimManager.Instance.Shake(duration, strength, vibrato, randomness, snapping, fadeOut);
                                break;
                            case ShakeXYEnum.ShakeX:
                                AnimManager.Instance.ShakeX(duration, strength, vibrato, randomness, snapping, fadeOut);
                                break;
                            case ShakeXYEnum.ShakeY:
                                AnimManager.Instance.ShakeY(duration, strength, vibrato, randomness, snapping, fadeOut);
                                break;
                            case ShakeXYEnum.ShakeV3:
                                AnimManager.Instance.ShakeV3(duration, strengthV3, vibrato, randomness, snapping, fadeOut);
                                break;
                            default:
                                break;
                        }

                    }
                }
                CameraScaleVisable = MyGUI.FoldoutWithTitle("CameraScale", CameraScaleVisable);
                if (CameraScaleVisable)
                {
                    CameraScale = MyGUI.FloatFieldWithTitle("Camerasize", CameraScale);
                    if (MyGUI.Button("测试摄像机放大"))
                    {
                        AnimManager.Instance.CameraScaleUp(CameraScale);
                    }
                }
                game = (GameObject)EditorGUILayout.ObjectField(new GUIContent("role"), game, typeof(GameObject), true);
                if (game != null)
                {
                    longlong = MyGUI.FloatFieldWithTitle("long", longlong);
                    heitht = MyGUI.FloatFieldWithTitle("height", heitht);
                    t = MyGUI.FloatFieldWithTitle("", t);
                    if (MyGUI.Button("测试伤害数值"))
                    {
                        Vector3 v3 = new Vector3(0, 3, 0) + game.transform.position;
                        AnimManager.Instance.cebezierInterrupt("13", v3, 1f, false, longlong, heitht, t);
                    }
                }
            }

            DrawDefaultInspector();
        }
    }
}