// using System;
// using System.Linq;
// using System.Reflection;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
// 
// using System.IO;
// using Tang;

// namespace Tang.Editor
// {
//     [CustomEditor(typeof(Wall))]
//     public class WallEditor : Editor
//     {
//         Wall wall;

//         /// <summary>
//         /// This function is called when the object becomes enabled and active.
//         /// </summary>
//         void OnEnable()
//         {
//             wall = target as Wall;
//         }

//         public override void OnInspectorGUI()
//         {
//             if (MyGUI.Button("生成mesh"))
//             {
//                 Vector3 p2 = Vector2.zero;
//                 Vector3 p1 = wall.wallData.GetGridVertexVec3(0, wall.wallData.cols);
//                 Vector3 p3 = wall.wallData.GetGridVertexVec3(wall.wallData.rows, wall.wallData.cols);
//                 Vector3 p4 = wall.wallData.GetGridVertexVec3(wall.wallData.rows, 0);

//                 Texture2D texture = Resources.Load<Texture2D>("Prefabs/SceneObject/Stage01/Textures/3d/Wall");

//                 float width = 10;
//                 float height = 10;

//                 MeshFilter meshFilter = wall.gameObject.AddComponentUnique<MeshFilter>();
//                 MeshRenderer meshRenderer = wall.gameObject.AddComponentUnique<MeshRenderer>();

//                 // 初始化mesh uv 等add by TangJian 2017/12/20 17:04:45
//                 {
//                     // Shader shader = Shader.Find("Custom/Unlit/Transparent");
//                     // Shader shader = Shader.Find("Spine/W_Sprite");
//                     Shader shader = Shader.Find("Sprites/Default");

//                     {
//                         Mesh mesh = new Mesh();

//                         // 为网格创建顶点数组
//                         // Vector3[] vertices = new Vector3[4]{
//                         //     new Vector3(width / 2, height / 2, 0),
//                         //     new Vector3(-width / 2, height / 2, 0),
//                         //     new Vector3(width / 2, -height / 2, 0),
//                         //     new Vector3(-width / 2, -height / 2, 0)
//                         // };

//                         Vector3[] vertices = new Vector3[4]{
//                                 p1, p2, p3, p4
//                             };

//                         // for (int i = 0; i < vertices.Length; i++)
//                         // {
//                         //     vertices[i].y += height / 2;
//                         // }

//                         mesh.vertices = vertices;

//                         // 通过顶点为网格创建三角形
//                         int[] triangles = new int[2 * 3]{
//                                 0, 3, 1, 0, 2, 3
//                             };
//                         // int[] triangles = new int[2 * 3]{
//                         //     0, 1, 3, 0, 3, 2
//                         // };

//                         mesh.triangles = triangles;

//                         mesh.uv = new Vector2[]{
//                                 new Vector2(1, 1),
//                                 new Vector2(0, 1),
//                                 new Vector2(1, 0),
//                                 new Vector2(0, 0)
//                             };
//                         meshFilter.mesh = mesh;
//                     }
//                     Material material = new Material(shader);
//                     material.mainTexture = texture;
//                     material.renderQueue = 3000;

//                     material.SetFloat("_Cutoff", 0.2f);

//                     meshRenderer.material = material;
//                 }
//             }

//             DrawDefaultInspector();
//         }
//     }
// }