using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tang
{
    public enum SlopeWallType
    {
        None = 0,
        Left = 1,
        Right = 2,
    }

    [System.Serializable]
    public class SlopeWallData
    {
        public int widthY = 1;

        public int slopeX = 1;
        public int slopeY = 1;

        public int x = 0;
        public int y = 0;
        public int z = 0;

        public Vector3[] GetVertices()
        {
            return new Vector3[] {
                new Vector3(0, 0, 0),
                new Vector3(0, SceneComponentConstant.GapY * widthY, 0),
                new Vector3(SceneComponentConstant.GapX * slopeX, SceneComponentConstant.GapY * widthY + SceneComponentConstant.GapY * slopeY, 0),
                new Vector3(SceneComponentConstant.GapX * slopeX, SceneComponentConstant.GapY * slopeY, 0),
            };
        }
    }

    public class SceneSlopeWallComponent : SceneComponent
    {
        [SerializeField] public SlopeWallData slopeData = new SlopeWallData();

        public override string ComponentType
        {
            get
            {
                return "SlopeWall_" + slopeData.slopeX + "x" + slopeData.slopeY + "x" + slopeData.widthY;
            }
        }

        public override object Data
        {
            get
            {
                return slopeData;
            }

            set
            {
                slopeData = (SlopeWallData)value;
            }
        }

        public override Vector3 GridPos
        {
            set
            {
                slopeData.x = (int)value.x;
                slopeData.y = (int)value.y;
                slopeData.z = (int)value.z;
            }
            get
            {
                return new Vector3(slopeData.x, slopeData.y, slopeData.z);
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

            Vector3[] vertices = slopeData.GetVertices();

            Vector3 point1 = transform.TransformPoint(vertices[0]);
            Vector3 point2 = transform.TransformPoint(vertices[1]);
            Vector3 point3 = transform.TransformPoint(vertices[2]);
            Vector3 point4 = transform.TransformPoint(vertices[3]);

            Gizmos.DrawLine(point1, point2);
            Gizmos.DrawLine(point2, point3);
            Gizmos.DrawLine(point3, point4);
            Gizmos.DrawLine(point4, point1);
        }

        public override void CreateMesh(string materialPath)
        {
#if UNITY_EDITOR
            gameObject.layer = LayerMask.NameToLayer("SceneComponent");

            Vector3[] vertexes = slopeData.GetVertices();

            Vector3 p1 = vertexes[0]; // Vector2.zero;
            Vector3 p2 = vertexes[1]; // wallData.GetNodeVec3(wallData.lenth);

            Vector3 p3 = vertexes[2]; // p2 + new Vector3(0, wallData.h, 0);
            Vector3 p4 = vertexes[3]; // p1 + new Vector3(0, wallData.h, 0);

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Materials/Shaders/tang/Wall.jpg");
            // Texture2D texture = Resources.Load<Texture2D>("Prefabs/SceneObject/Stage01/Textures/3d/Wall");

            MeshFilter meshFilter = gameObject.AddComponentUnique<MeshFilter>(true);
            MeshRenderer meshRenderer = gameObject.AddComponentUnique<MeshRenderer>(true);
            MeshCollider meshCollider = gameObject.AddComponentUnique<MeshCollider>(true);

            // 初始化mesh uv 等add by TangJian 2017/12/20 17:04:45
            {
                // Shader shader = Shader.Find("Custom/Unlit/Transparent");
                Shader shader = Shader.Find("Tang/Unlit/BackWall");

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


                material.SetFloat("_IsScale", 0);
                material.SetFloat("_TestFactor", 10);

                meshRenderer.sharedMaterial = material;

                meshRenderer.enabled = false;
            }

            // 创建Renderer add by TangJian 2018/10/15 16:34
            {
                GameObject rendererGameObject = gameObject.GetChild("Renderer", true);
                // rendererGameObject.DestoryChildren();

                GameObject renderer1GameObject = rendererGameObject.GetChild("Renderer1", true);

                renderer1GameObject.layer = LayerMask.NameToLayer("Wall");

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
            Vector3[] points = slopeData.GetVertices();

            Vector3 p1 = points[0];
            Vector3 p2 = points[1];
            Vector3 p3 = points[2];
            Vector3 p4 = points[3];

            p1 = SceneView.lastActiveSceneView.camera.transform.TransformPoint(p1);
            p2 = SceneView.lastActiveSceneView.camera.transform.TransformPoint(p2);
            p3 = SceneView.lastActiveSceneView.camera.transform.TransformPoint(p3);
            p4 = SceneView.lastActiveSceneView.camera.transform.TransformPoint(p4);

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

            Tools.SaveTextureToFile(texture, Application.dataPath + "/Output/out.png");

            // AssetDatabase.CreateAsset(texture, "Assets/Output/out.png");
        }
#endif

    }

}