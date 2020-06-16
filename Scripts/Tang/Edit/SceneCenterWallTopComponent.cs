

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tang
{
    [System.Serializable]
    public class CenterWallTopData
    {
        public int x = 0;
        public int y = 0;
        public int z = 0;

        [SerializeField] public float widthZ = 3;
        [SerializeField] public float widthX = 3;


        public Vector3[] GetVertices()
        {
            return new Vector3[4] {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, SceneComponentConstant.GapZ * widthZ),
            new Vector3(SceneComponentConstant.GapX * widthX, 0, SceneComponentConstant.GapZ * widthZ),
            new Vector3(SceneComponentConstant.GapX * widthX, 0, 0),
            };
        }
    }

    public class SceneCenterWallTopComponent : SceneComponent
    {
        [SerializeField] public CenterWallTopData wallData = new CenterWallTopData();

        public override string ComponentType
        {
            get
            {
                return "CenterWallTop_" + wallData.widthX + "x" + wallData.widthZ;
            }
        }

        public override Vector3 GridPos
        {
            set
            {
                wallData.x = (int)value.x;
                wallData.y = (int)value.y;
                wallData.z = (int)value.z;
            }

            get
            {
                return new Vector3(wallData.x, wallData.y, wallData.z);
            }
        }

        public override object Data
        {
            get
            {
                return wallData;
            }

            set
            {
                wallData = (CenterWallTopData)value;
            }
        }

#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            MoveByGridPos(Vector3.zero);

            Gizmos.color = color;

            Gizmos.color = color;

            Vector3[] vertices = wallData.GetVertices();

            Vector3 point1 = transform.TransformPoint(vertices[0]);
            Vector3 point2 = transform.TransformPoint(vertices[1]);
            Vector3 point3 = transform.TransformPoint(vertices[2]);
            Vector3 point4 = transform.TransformPoint(vertices[3]);

            Gizmos.DrawLine(point1, point2);
            Gizmos.DrawLine(point2, point3);
            Gizmos.DrawLine(point3, point4);
            Gizmos.DrawLine(point4, point1);
        }

        public override void CreateMesh(string materialPath = "")
        {
#if UNITY_EDITOR
            gameObject.layer = LayerMask.NameToLayer("SceneComponent");

            Vector3[] vertexes = wallData.GetVertices();

            Vector3 p1 = vertexes[0]; // Vector2.zero;
            Vector3 p2 = vertexes[1]; // wallData.GetNodeVec3(wallData.lenth);

            Vector3 p3 = vertexes[2]; // p2 + new Vector3(0, wallData.h, 0);
            Vector3 p4 = vertexes[3]; // p1 + new Vector3(0, wallData.h, 0);

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Materials/Shaders/tang/Wall.jpg");

            MeshFilter meshFilter = gameObject.AddComponentUnique<MeshFilter>(true);
            MeshRenderer meshRenderer = gameObject.AddComponentUnique<MeshRenderer>(true);
            MeshCollider meshCollider = gameObject.AddComponentUnique<MeshCollider>(true);

            // 初始化mesh uv 等add by TangJian 2017/12/20 17:04:45
            {
                // Shader shader = Shader.Find("Custom/Unlit/Transparent");
                Shader shader = Shader.Find("Tang/Unlit/PureColor");

                {
                    Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(materialPath + ".asset");

                    if (mesh == null)
                    {
                        mesh = new Mesh();
                        AssetDatabase.CreateAsset(mesh, materialPath + ".asset");
                        mesh = AssetDatabase.LoadAssetAtPath<Mesh>(materialPath + ".asset");
                    }

                    // 为网格创建顶点数组
                    Vector3[] vertices = new Vector3[4]{
                                p1, p2, p3, p4
                            };

                    mesh.vertices = vertices;

                    // 通过顶点为网格创建三角形
                    int[] triangles = new int[2 * 3]{
                                0,1,2, 0,2,3
                            };

                    mesh.triangles = triangles;

                    mesh.uv = new Vector2[]{
                                new Vector2(0, 0),
                                new Vector2(0, 1),
                                new Vector2(1, 1),
                                new Vector2(1, 0)
                            };



                    meshCollider.sharedMesh = mesh;
                    meshFilter.sharedMesh = mesh;
                }

                Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath + ".mat");

                if (material == null)
                {
                    material = new Material(shader);
                    AssetDatabase.CreateAsset(material, materialPath + ".mat");
                    material = AssetDatabase.LoadAssetAtPath<Material>(materialPath + ".mat");
                }
                else
                {
                    material.shader = shader;
                }

                material.mainTexture = texture;
                material.renderQueue = 2000;


                material.SetColor("MulColor", new Color(59f / 255f, 32f / 255f, 12f / 255f, 1f));

                meshRenderer.sharedMaterial = material;

                meshRenderer.enabled = false;

                //AssetDatabase.SaveAssets();
            }

            // 创建Renderer add by TangJian 2018/10/15 16:34
            {
                GameObject rendererGameObject = gameObject.GetChild("Renderer", true);
                // rendererGameObject.DestoryChildren();

                GameObject renderer1GameObject = rendererGameObject.GetChild("Renderer1", true);

                renderer1GameObject.layer = LayerMask.NameToLayer("Default");

                MeshFilter rendererMeshFilter = renderer1GameObject.AddComponentUnique<MeshFilter>(true);
                MeshRenderer rendererMeshRenderer = renderer1GameObject.AddComponentUnique<MeshRenderer>(true);
                MeshCollider rendererMeshCollider = renderer1GameObject.AddComponentUnique<MeshCollider>(true);

                // 设置Renderer网格 add by TangJian 2018/10/15 16:34
                rendererMeshCollider.sharedMesh = meshCollider.sharedMesh;
                rendererMeshFilter.sharedMesh = meshFilter.sharedMesh;

                // 设置Renderer材质 add by TangJian 2018/10/15 16:34
                rendererMeshRenderer.sharedMaterial = meshRenderer.sharedMaterial;

                // 添加阴影 add by TangJian 2018/10/15 16:36
                // renderer1GameObject.AddComponentUnique<ShadowReceiver>();
            }


#endif
        }

        public void CreateTexture2D()
        {
            Vector3[] vertices = wallData.GetVertices();
            Vector3 p1 = vertices[0];
            Vector3 p2 = vertices[1];
            Vector3 p3 = vertices[2];
            Vector3 p4 = vertices[3];

            float factorY = Mathf.Sqrt(3) / 2f;
            float factorZ = 1f / 2f;

            p1.y = p1.y * factorY + p1.z * factorZ;
            p2.y = p2.y * factorY + p2.z * factorZ;
            p3.y = p3.y * factorY + p3.z * factorZ;
            p4.y = p4.y * factorY + p4.z * factorZ;

            //p1 = SceneView.lastActiveSceneView.camera.transform.TransformPoint(p1);

            int width = 10000;
            int height = 10000;

            Vector3 center = new Vector3(width / 2f, height / 2f, 0);

            Texture2D texture = new Texture2D(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }

            texture.DrawLine(center + p1 * 100f, center + p2 * 100f, Color.red);
            texture.DrawLine(center + p2 * 100f, center + p3 * 100f, Color.red);
            texture.DrawLine(center + p3 * 100f, center + p4 * 100f, Color.red);
            texture.DrawLine(center + p4 * 100f, center + p1 * 100f, Color.red);

            texture.Apply();

            Tools.SaveTextureToFile(texture, Application.dataPath + "/Output/out.png");

            // AssetDatabase.CreateAsset(texture, "Assets/Output/out.png");
        }
        #endif
    }
}