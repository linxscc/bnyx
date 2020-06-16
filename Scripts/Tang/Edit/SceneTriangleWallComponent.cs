








using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tang
{
    public enum TriangleWallType
    {
        None = 0,
        Left = 1,
        Right = 2,
    }

    [System.Serializable]
    public class TriangleWallData
    {
        public int widthX = 1;
        public int widthY = 1;

        public int x = 0;
        public int y = 0;
        public int z = 0;

        public bool isMirror = false;


        public Vector3 GetPoint(int index)
        {
            if (index == 0)
            {
                return new Vector3(0, 0, 0);
            }
            else if (index == 1)
            {
                return new Vector3(SceneComponentConstant.GapX * widthX, SceneComponentConstant.GapY * widthY, 0);
            }
            else if (index == 2)
            {
                return new Vector3(SceneComponentConstant.GapX * widthX, 0, 0);
            }

            Debug.LogError("");
            return Vector3.zero;
        }

        public Vector2 GetUV(int index)
        {
            if (index == 0)
            {
                return new Vector2(0, 0);
            }
            else if (index == 1)
            {
                return new Vector2(1, 1);
            }
            else if (index == 2)
            {
                return new Vector3(1, 0);
            }

            Debug.LogError("");
            return Vector3.zero;
        }
    }

    public class SceneTriangleWallComponent : SceneComponent
    {
        [SerializeField] public TriangleWallData triangleWallData = new TriangleWallData();

        public override string ComponentType
        {
            get
            {
                return "TriangleWall_" + triangleWallData.widthX + "x" + triangleWallData.widthY;
            }
        }

        public override object Data
        {
            get
            {
                return triangleWallData;
            }

            set
            {
                triangleWallData = (TriangleWallData)value;
            }
        }


        void Awake()
        {
            GetComponent<Collider>().enabled = false;
        }

        public override Vector3 GridPos
        {
            set
            {
                triangleWallData.x = (int)value.x;
                triangleWallData.y = (int)value.y;
                triangleWallData.z = (int)value.z;
            }
            get
            {
                return new Vector3(triangleWallData.x, triangleWallData.y, triangleWallData.z);
            }
        }

        public override Vector3 GridPosToLocalPos(Vector3 gridPos)
        {
            return new Vector3(gridPos.x * 3 + gridPos.z * 3 / 2, gridPos.y * SceneComponentConstant.GapY, gridPos.z * 3);
        }

        public override Vector3 LocalPosToGridPos(Vector3 localPos)
        {
            return new Vector3((localPos.x - localPos.z / 2.0f) / 3.0f, localPos.y / SceneComponentConstant.GapY, localPos.z / 3.0f);
        }

#if UNITY_EDITOR
        
        public override void OnDrawGizmos()
        {
            MoveByGridPos(Vector3.zero);

            Gizmos.color = color;

            Vector3 point1 = transform.TransformPoint(triangleWallData.GetPoint(0));
            Vector3 point2 = transform.TransformPoint(triangleWallData.GetPoint(1));
            Vector3 point3 = transform.TransformPoint(triangleWallData.GetPoint(2));

            Gizmos.DrawLine(point1, point2);
            Gizmos.DrawLine(point2, point3);
            Gizmos.DrawLine(point3, point1);
        }

        public override void CreateMesh(string materialPath)
        {

#if UNITY_EDITOR
            Vector3 p1 = triangleWallData.GetPoint(0);
            Vector3 p2 = triangleWallData.GetPoint(1);
            Vector3 p3 = triangleWallData.GetPoint(2);

            Vector2 uv1 = triangleWallData.GetUV(0);
            Vector2 uv2 = triangleWallData.GetUV(1);
            Vector2 uv3 = triangleWallData.GetUV(2);

            //Vector2 uv1 = p1;
            //Vector2 uv2 = p2;
            //Vector2 uv3 = p3;

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Materials/Shaders/tang/Wall.jpg");

            MeshFilter meshFilter = gameObject.AddComponentUnique<MeshFilter>(true);
            MeshRenderer meshRenderer = gameObject.AddComponentUnique<MeshRenderer>(true);
            MeshCollider meshCollider = gameObject.AddComponentUnique<MeshCollider>(true);

            // 初始化mesh uv 等add by TangJian 2017/12/20 17:04:45
            {
                Shader shader = Shader.Find("Tang/Unlit/PureColor");

                // mesh
                {
                    Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(materialPath + ".asset");

                    if (mesh == null)
                    {
                        mesh = new Mesh();
                        AssetDatabase.CreateAsset(mesh, materialPath + ".asset");
                        mesh = AssetDatabase.LoadAssetAtPath<Mesh>(materialPath + ".asset");
                    }

                    // 为网格创建顶点数组
                    Vector3[] vertices = new Vector3[3]{
                                             p1, p2, p3
                                         };

                    mesh.vertices = vertices;

                    // 通过顶点为网格创建三角形
                    int[] triangles = new int[3]{
                                             0, 1, 2
                                         };

                    mesh.triangles = triangles;

                    mesh.uv = new Vector2[]
                    {
                        uv1,
                        uv2,
                        uv3
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

                material.SetColor("MulColor", new Color(222 / 255f, 129 / 255f, 0 / 255f, 1f));

                meshRenderer.sharedMaterial = material;
                meshRenderer.enabled = false;

                //AssetDatabase.SaveAssets();
            }

            // 创建渲染 add by TangJian 2018/10/15 17:39
            {
                GameObject rendererGameObject = gameObject.GetChild("Renderer", true);

                GameObject renderer1GameObject = rendererGameObject.GetChild("Renderer1", true);

                renderer1GameObject.layer = LayerMask.NameToLayer("Wall");

                MeshFilter rendererMeshFilter = renderer1GameObject.AddComponentUnique<MeshFilter>(true);
                MeshRenderer rendererMeshRenderer = renderer1GameObject.AddComponentUnique<MeshRenderer>(true);

                // 设置Renderer网格 add by TangJian 2018/10/15 16:34
                rendererMeshFilter.sharedMesh = meshFilter.sharedMesh;

                // 设置Renderer材质 add by TangJian 2018/10/15 16:34
                rendererMeshRenderer.sharedMaterial = meshRenderer.sharedMaterial;

                // 添加阴影 add by TangJian 2018/10/15 16:36
                //renderer1GameObject.AddComponentUnique<ShadowReceiver>();

                rendererMeshRenderer.enabled = true;
            }

            // 创建碰撞 add by TangJian 2018/10/15 17:39
            {
                GameObject colliderGameObject = gameObject.GetChild("Collider", true);
                GameObject collider1GameObject = colliderGameObject.GetChild("Collider1", true);

                collider1GameObject.layer = LayerMask.NameToLayer("Wall");

                MeshFilter colliderMeshFilter = collider1GameObject.AddComponentUnique<MeshFilter>();
                colliderMeshFilter.sharedMesh = meshCollider.sharedMesh;

                MeshCollider colliderMeshCollider = collider1GameObject.AddComponentUnique<MeshCollider>();
                colliderMeshCollider.convex = true;
            }
#endif
        }

        public void CreateTexture2D()
        {
            Vector3 p1 = triangleWallData.GetPoint(0);
            Vector3 p2 = triangleWallData.GetPoint(1);
            Vector3 p3 = triangleWallData.GetPoint(2);

            p1 = SceneView.lastActiveSceneView.camera.transform.TransformPoint(p1);
            p2 = SceneView.lastActiveSceneView.camera.transform.TransformPoint(p2);
            p3 = SceneView.lastActiveSceneView.camera.transform.TransformPoint(p3);

            float minX = Mathf.Min(p1.x, p2.x, p3.x);
            float minY = Mathf.Min(p1.y, p2.y, p3.y);

            p1.x -= minX;
            p2.x -= minX;
            p3.x -= minX;

            p1.y -= minY;
            p2.y -= minY;
            p3.y -= minY;

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
            texture.DrawLine(center + p3 * 100f, center + p1 * 100f, Color.red);

            texture.Apply();

            Tools.SaveTextureToFile(texture, Application.dataPath + "/Output/out.png");

            // AssetDatabase.CreateAsset(texture, "Assets/Output/out.png");
        }
#endif
    }
}