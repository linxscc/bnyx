// using System;
// using System.Linq;
// using System.Reflection;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;

// namespace Tang.Editor
// {
//     [CustomEditor(typeof(AnimManager))]
//     public class AnimManagerEditor : Editor
//     {
//         bool shakeEffectVisable = false;
//         float duration = 1.0f;
//         float strength = 1;
//         int vibrato = 10;
//         float randomness = 90;
//         bool snapping = false;
//         bool fadeOut = true;

//         public override void OnInspectorGUI()
//         {
//             shakeEffectVisable = MyGUI.FoldoutWithTitle("Shake", shakeEffectVisable);
//             if (shakeEffectVisable)
//             {
//                 EditorGUILayout.BeginVertical();

//                 duration = MyGUI.FloatFieldWithTitle("duration", duration);
//                 strength = MyGUI.FloatFieldWithTitle("strength", strength);
//                 vibrato = MyGUI.IntFieldWithTitle("vibrato", vibrato);
//                 randomness = MyGUI.FloatFieldWithTitle("randomness", randomness);
//                 snapping = MyGUI.ToggleWithTitle("snapping", snapping);
//                 fadeOut = MyGUI.ToggleWithTitle("fadeOut", fadeOut);

//                 EditorGUILayout.EndVertical();

//                 if (MyGUI.Button("测试震动"))
//                 {
//                     AnimManager.Instance.Shake(duration, strength, vibrato, randomness, snapping, fadeOut);
//                 }
//             }

//             DrawDefaultInspector();
//         }
//     }
// }