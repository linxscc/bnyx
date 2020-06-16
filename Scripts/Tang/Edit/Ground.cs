using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tang
{
    public enum GroundType
    {
        None = 0,
        Left = 1,
        Center = 2,
        Right = 3,
    }

    [System.Serializable]
    public class GroundData
    {
        public GroundType groundType = GroundType.Center;

        public List<Vector2> digList = new List<Vector2>();

        public int rows = 1;
        public int cols = 1;

        public static float rowH = 3;
        public static float colW = 3;

        public int x = 0;
        public int y = 0;
        public int z = 0;

        public Vector3 offset;
        
        public Vector2 Position
        {
            get
            {
                return new Vector2(x * colW + y * rowH / 2, y * rowH);
            }
        }
        
        public Vector2 InnerOffset
        {
            get
            {
                int row = this.rows;
                int col = 0;

                Vector2 retPos = Vector2.zero;
                if (groundType == GroundType.Center)
                {
                    var rows = row;

                    float totalHeight = rows * rowH;
                    float topWidth = cols * colW;
                    float bottomWidth = topWidth + totalHeight * 2 / 2;

                    float topGridWidth = topWidth / cols;
                    float bottomGridWidth = bottomWidth / cols;

                    retPos = new Vector2(bottomGridWidth * col - rows * rowH / 2, totalHeight);
                }
                else if (groundType == GroundType.Left)
                {
                    var rows = row;

                    float totalHeight = rows * rowH;
                    float topWidth = cols * colW;
                    float bottomWidth = topWidth;

                    float topGridWidth = topWidth / cols;
                    float bottomGridWidth = bottomWidth / cols;

                    retPos = new Vector2(bottomGridWidth * col - rows * rowH / 2, totalHeight);
                }
                else if (groundType == GroundType.Right)
                {
                    var rows = row;

                    float totalHeight = rows * rowH;
                    float topWidth = cols * colW;
                    float bottomWidth = topWidth;

                    float topGridWidth = topWidth / cols;
                    float bottomGridWidth = bottomWidth / cols;

                    retPos = new Vector2(bottomGridWidth * col + rows * rowH / 2, totalHeight);
                }

                retPos.y = -retPos.y;
                return -retPos;
            }
        }

        public Vector2 GetGridVertexPos(int row, int col)
        {
            Vector2 retPos = Vector2.zero;
            if (groundType == GroundType.Center)
            {
                var rows = row;

                float totalHeight = rows * rowH;
                float topWidth = cols * colW;
                float bottomWidth = topWidth + totalHeight * 2 / 2;

                float topGridWidth = topWidth / cols;
                float bottomGridWidth = bottomWidth / cols;

                retPos = new Vector2(bottomGridWidth * col - rows * rowH / 2, totalHeight);
            }
            else if (groundType == GroundType.Left)
            {
                var rows = row;

                float totalHeight = rows * rowH;
                float topWidth = cols * colW;
                float bottomWidth = topWidth;

                float topGridWidth = topWidth / cols;
                float bottomGridWidth = bottomWidth / cols;

                retPos = new Vector2(bottomGridWidth * col - rows * rowH / 2, totalHeight);
            }
            else if (groundType == GroundType.Right)
            {
                var rows = row;

                float totalHeight = rows * rowH;
                float topWidth = cols * colW;
                float bottomWidth = topWidth;

                float topGridWidth = topWidth / cols;
                float bottomGridWidth = bottomWidth / cols;

                retPos = new Vector2(bottomGridWidth * col + rows * rowH / 2, totalHeight);
            }

            retPos.y = -retPos.y;
            return retPos + InnerOffset;
        }

        public Vector3 GetGridVertexVec3(float row, float col)
        {
            return GetGridVertexVec3((int)row, (int)col);
        }

        public Vector3 GetGridVertexVec3(int row, int col)
        {
            Vector2 retPos = Vector2.zero;
            if (groundType == GroundType.Center)
            {
                var rows = row;

                float totalHeight = rows * rowH;
                float topWidth = cols * colW;
                float bottomWidth = topWidth + totalHeight * 2 / 2;

                float topGridWidth = topWidth / cols;
                float bottomGridWidth = bottomWidth / cols;

                retPos = new Vector2(bottomGridWidth * col - rows * rowH / 2, totalHeight);
            }
            else if (groundType == GroundType.Left)
            {
                var rows = row;

                float totalHeight = rows * rowH;
                float topWidth = cols * colW;
                float bottomWidth = topWidth;

                float topGridWidth = topWidth / cols;
                float bottomGridWidth = bottomWidth / cols;

                retPos = new Vector2(bottomGridWidth * col - rows * rowH / 2, totalHeight);
            }
            else if (groundType == GroundType.Right)
            {
                var rows = row;

                float totalHeight = rows * rowH;
                float topWidth = cols * colW;
                float bottomWidth = topWidth;

                float topGridWidth = topWidth / cols;
                float bottomGridWidth = bottomWidth / cols;

                retPos = new Vector2(bottomGridWidth * col + rows * rowH / 2, totalHeight);
            }

            retPos.y = -retPos.y;
            return new Vector3(retPos.x, 0, retPos.y) + new Vector3(InnerOffset.x, 0, InnerOffset.y);
        }

        public Vector2 WorldPosToGridPos(Vector2 pos)
        {
            for (int row = 0; row <= rows; row++)
            {
                for (int col = 0; col <= cols; col++)
                {
                    var currGridPos = GetGridVertexPos(row, col);
                    if (currGridPos.x > pos.x && currGridPos.y < pos.y)
                    {
                        return new Vector2(row - 1, col - 1);
                    }
                }
            }
            return Vector2.zero;
        }
    }

    public class Line
    {
        public Vector3 beginPos;
        public Vector3 endPos;
    }

    public class Ground : SceneComponent
    {
        [SerializeField] public GroundData groundData = new GroundData();

        public override string ComponentType
        {
            get
            {
                return groundData.groundType.ToString() + "Ground_" + groundData.cols + "x" + groundData.rows;
            }
        }

        public override string PathInScene
        {
            get
            {
                return "Terrains/Ground";
            }
        }

        public override object Data
        {
            get
            {
                return groundData;
            }

            set
            {
                groundData = (GroundData)value;
            }
        }


        ShadowReceiver shadowReceiver;

        void Awake()
        {
            GameObject rendererGameObject = gameObject.GetChild("Renderer", true);
            GameObject renderer1GameObject = rendererGameObject.GetChild("Renderer1", true);
            renderer1GameObject.tag = "Ground";
        }

        public Vector2 WorldPosToGridPos(Vector3 worldPos)
        {
            Vector3 localPos = gameObject.transform.InverseTransformPoint(worldPos);
            return groundData.WorldPosToGridPos(new Vector2(localPos.x, localPos.z));
        }

        public override Vector3 GridPos
        {
            set
            {
                groundData.x = (int)value.x;
                groundData.y = (int)value.z;
                groundData.z = (int)value.y;
            }
            get
            {
                return new Vector3(groundData.x, groundData.z, groundData.y);
            }
        }

        public override Vector3 Offset
        {
            get => groundData.offset;
            set => groundData.offset=value;
        }
        
        public override void MoveByGridPos(Vector3 gridPos)
        {
            GridPos = GridPos + gridPos;
            gameObject.transform.localPosition = GridPosToLocalPos(GridPos) + Offset;
        }
#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            MoveByGridPos(Vector3.zero);

            Gizmos.color = color;
            // 横线 add by TangJian 2018/02/28 17:26:25
            for (int row = 0; row <= groundData.rows; row++)
            {
                Vector2 beginVec2 = groundData.GetGridVertexPos(row, 0);
                Vector2 endPosVec2 = groundData.GetGridVertexPos(row, groundData.cols);

                Vector3 beginVec3 = new Vector3(beginVec2.x, 0, beginVec2.y);
                Vector3 endVec3 = new Vector3(endPosVec2.x, 0, endPosVec2.y);

                Gizmos.DrawLine(transform.TransformPoint(beginVec3), transform.TransformPoint(endVec3));
            }

            // 竖线 add by TangJian 2018/02/28 17:55:34
            for (int col = 0; col <= groundData.cols; col++)
            {
                Vector2 beginVec2 = groundData.GetGridVertexPos(0, col);
                Vector2 endPosVec2 = groundData.GetGridVertexPos(groundData.rows, col);

                Vector3 beginVec3 = gameObject.transform.position + new Vector3(beginVec2.x, 0, beginVec2.y);
                Vector3 endVec3 = gameObject.transform.position + new Vector3(endPosVec2.x, 0, endPosVec2.y);

                Gizmos.DrawLine(transform.TransformPoint(beginVec3), transform.TransformPoint(endVec3));
            }
        }

        public override void CreateMesh(string materialPath)
        {
#if UNITY_EDITOR
            gameObject.layer = LayerMask.NameToLayer("SceneComponent");

            // 得到定点 add by TangJian 2018/10/15 16:29
            Vector3 p1 = groundData.GetGridVertexVec3(groundData.rows, 0);
            Vector3 p2 = groundData.GetGridVertexVec3(groundData.rows, groundData.cols);
            Vector3 p3 = groundData.GetGridVertexVec3(0, groundData.cols);
            Vector3 p4 = groundData.GetGridVertexVec3(0, 0);

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Materials/Shaders/tang/Floor.png");

            MeshFilter meshFilter = gameObject.AddComponentUnique<MeshFilter>(true);
            MeshRenderer meshRenderer = gameObject.AddComponentUnique<MeshRenderer>(true);
            MeshCollider meshCollider = gameObject.AddComponentUnique<MeshCollider>(true);

            // 初始化mesh uv 等add by TangJian 2017/12/20 17:04:45
            {
                Shader shader = Shader.Find("Tang/Unlit/Ground");

                // 生成Mesh add by TangJian 2018/10/15 16:28
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
                        0,2,1, 0,3,2
                    };

                    mesh.triangles = triangles;

                    mesh.uv = new Vector2[]{
                        new Vector2(0, 0),
                        new Vector2(1, 0),
                        new Vector2(1, 1),
                        new Vector2(0, 1)
                    };

                    meshCollider.sharedMesh = mesh;
                    meshFilter.sharedMesh = mesh;
                }


                // 生成材质 add by TangJian 2018/10/15 16:29
                {
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

                    material.SetFloat("_TestFactor", 50);

                    meshRenderer.sharedMaterial = material;
                }
            }

            // 关闭渲染 add by TangJian 2018/10/15 16:47
            meshRenderer.enabled = false;

            // 创建Renderer add by TangJian 2018/10/15 16:34
            {
                GameObject rendererGameObject = gameObject.GetChild("Renderer", true);
                rendererGameObject.DestoryChildren();

                GameObject renderer1GameObject = rendererGameObject.GetChild("Renderer1", true);

                renderer1GameObject.layer = LayerMask.NameToLayer("Ground");
                renderer1GameObject.tag = "Ground";

                MeshFilter rendererMeshFilter = renderer1GameObject.AddComponentUnique<MeshFilter>(true);
                MeshRenderer rendererMeshRenderer = renderer1GameObject.AddComponentUnique<MeshRenderer>(true);
                MeshCollider rendererMeshCollider = renderer1GameObject.AddComponentUnique<MeshCollider>(true);

                // 设置Renderer网格 add by TangJian 2018/10/15 16:34
                rendererMeshCollider.sharedMesh = meshCollider.sharedMesh;
                rendererMeshFilter.sharedMesh = meshFilter.sharedMesh;

                // 设置Renderer材质 add by TangJian 2018/10/15 16:34
                rendererMeshRenderer.sharedMaterial = meshRenderer.sharedMaterial;

                // 添加阴影 add by TangJian 2018/10/15 16:36
                renderer1GameObject.AddComponentUnique<ShadowReceiver>();

                // 添加排序节点 add by TangJian 2018/11/6 21:23
                SortRenderer sortRenderer = renderer1GameObject.AddComponentUnique<SortRenderer>(true);
                GameObject sortRenderPos = sortRenderer.gameObject.GetChild("SortRenderPos", true);
                Bounds bounds = sortRenderer.gameObject.GetRendererBounds();
                sortRenderPos.transform.position = new Vector3(0, bounds.min.y, bounds.max.z);
            }

            // // 创建碰撞 add by TangJian 2018/10/15 17:19
            // {
            //     GameObject colliderGameObject = gameObject.GetChild("Collider", true);
            //     GameObject collider1GameObject = colliderGameObject.GetChild("Collider1", true);

            //     collider1GameObject.layer = LayerMask.NameToLayer("Ground");

            //     MeshFilter colliderMeshFilter = collider1GameObject.AddComponentUnique<MeshFilter>();
            //     colliderMeshFilter.sharedMesh = meshCollider.sharedMesh;
            // }
#endif
        }


        public void CreateTexture2D()
        {
            Vector3 p1 = groundData.GetGridVertexVec3(groundData.rows, 0);
            Vector3 p2 = groundData.GetGridVertexVec3(groundData.rows, groundData.cols);
            Vector3 p3 = groundData.GetGridVertexVec3(0, groundData.cols);
            Vector3 p4 = groundData.GetGridVertexVec3(0, 0);

            p1 = SceneView.lastActiveSceneView.camera.worldToCameraMatrix.MultiplyPoint(p1);
            p2 = SceneView.lastActiveSceneView.camera.worldToCameraMatrix.MultiplyPoint(p2);
            p3 = SceneView.lastActiveSceneView.camera.worldToCameraMatrix.MultiplyPoint(p3);
            p4 = SceneView.lastActiveSceneView.camera.worldToCameraMatrix.MultiplyPoint(p4);

            float minX = Mathf.Min(p1.x, p2.x, p3.x, p4.x);
            float minY = Mathf.Min(p1.y, p2.y, p3.y, p4.y);

            p1.x -= minX;
            p2.x -= minX;
            p3.x -= minX;
            p4.x -= minX;

            p1.y -= minY;
            p2.y -= minY;
            p3.y -= minY;
            p4.y -= minY;

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

            Tools.SaveTextureToFile(texture, Application.dataPath + "/Output/" + name + ".png");

            // AssetDatabase.CreateAsset(texture, "Assets/Output/out.png");
        }
#endif

    }
}