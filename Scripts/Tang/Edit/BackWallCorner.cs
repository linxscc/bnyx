

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tang
{
    public enum BackWallCornerType
    {
        None = 0,
        Left = 1,
        Center = 2,
        Right = 3
    }

    [System.Serializable]
    public class BackWallCornerData
    {
        public float h { get { return rowH; } }

        public BackWallCornerType wallType = BackWallCornerType.Center;

        public int x = 0;
        public int y = 0;
        public int lenth = 1;

        public static float rowH = 3f;
        public static float colW = 3f;

        public Vector3 offset;

        public Vector2 Position
        {
            get
            {
                return new Vector2(x * colW + y * rowH / 2, y * rowH);
            }
        }

        public Vector2 GetNodePos(int n)
        {
            Vector2 retPos = Vector2.zero;
            if (wallType == BackWallCornerType.Center)
            {
                float x = n * colW;
                float y = 0;

                retPos.y = y;
                retPos.x = x;

                retPos.y = -retPos.y;
            }
            else if (wallType == BackWallCornerType.Left)
            {
                float x = -n * colW / 2;
                float y = n * colW;

                retPos.y = y;
                retPos.x = x;

                retPos.x = -retPos.x;
            }
            else if (wallType == BackWallCornerType.Right)
            {
                float x = n * colW / 2;
                float y = n * colW;

                retPos.y = y;
                retPos.x = x;

                retPos.y = -retPos.y;

            }
            return retPos;
        }

        public Vector3 GetNodeVec3(int n)
        {
            Vector2 pos = GetNodePos(n);
            return new Vector3(pos.x, 0, pos.y);
        }
    }

    public class BackWallCorner : SceneComponent
    {
        [SerializeField] public BackWallCornerData wallData = new BackWallCornerData();

        public override object Data
        {
            get
            {
                return wallData;
            }

            set
            {
                wallData = (BackWallCornerData)value;
            }
        }

        public override Vector3 GridPos
        {
            set
            {
                wallData.x = (int)value.x;
                wallData.y = (int)value.z;
            }
            get
            {
                return new Vector3(wallData.x, 0, wallData.y);
            }
        }
        
        public override Vector3 Offset
        {
            get => wallData.offset;
            set => wallData.offset=value;
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

            Vector2 p1 = Vector2.zero;
            Vector2 p2 = p1;
            Vector2 p4 = wallData.GetNodePos(wallData.lenth);
            Vector2 p3 = p4;

            Vector3 vec3_p1 = gameObject.transform.position + new Vector3(p1.x, 0, p1.y);
            Vector3 vec3_p2 = gameObject.transform.position + new Vector3(p2.x, 0, p2.y) + new Vector3(0, wallData.h, 0);
            Vector3 vec3_p4 = gameObject.transform.position + new Vector3(p4.x, 0, p4.y);
            Vector3 vec3_p3 = vec3_p4 + new Vector3(0, wallData.h, 0);

            Gizmos.DrawLine(vec3_p1, vec3_p2);
            Gizmos.DrawLine(vec3_p2, vec3_p3);
            Gizmos.DrawLine(vec3_p3, vec3_p4);
            Gizmos.DrawLine(vec3_p4, vec3_p1);
        }

        public override void CreateMesh(string materialPath = "")
        {
#if UNITY_EDITOR
            gameObject.layer = LayerMask.NameToLayer("SceneComponent");

            Vector3 p1 = Vector2.zero;
            Vector3 p2 = wallData.GetNodeVec3(wallData.lenth);

            Vector3 p3 = p2 + new Vector3(0, wallData.h, 0);
            Vector3 p4 = p1 + new Vector3(0, wallData.h, 0);

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources_moved/Prefabs/SceneObject/Stage01/Textures/3d/Wall.jpg");

            MeshFilter meshFilter = gameObject.AddComponentUnique<MeshFilter>(true);
            MeshRenderer meshRenderer = gameObject.AddComponentUnique<MeshRenderer>(true);
            MeshCollider meshCollider = gameObject.AddComponentUnique<MeshCollider>(true);
            // meshCollider.convex = true;

            // 初始化mesh uv 等add by TangJian 2017/12/20 17:04:45
            {
                // Shader shader = Shader.Find("Custom/Unlit/Transparent");
                Shader shader = Shader.Find("Sprites/Default");

                {
                    Mesh mesh = new Mesh();

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

                    if (materialPath != "")
                    {
                        AssetDatabase.CreateAsset(mesh, materialPath + ".asset");
                        mesh = AssetDatabase.LoadAssetAtPath(materialPath + ".asset", typeof(Mesh)) as Mesh;
                    }

                    meshCollider.sharedMesh = mesh;

                    meshFilter.mesh = mesh;
                }
                Material material = new Material(shader);
                material.mainTexture = texture;
                material.renderQueue = 2000;
                // material.SetFloat("_IsScale", 0f);
                // material.SetFloat("_Cutoff", 0.2f);


                if (materialPath != "")
                {
                    AssetDatabase.CreateAsset(material, materialPath + ".mat");
                    material = AssetDatabase.LoadAssetAtPath(materialPath + ".mat", typeof(Material)) as Material;
                }

                meshRenderer.material = material;

                meshRenderer.enabled = false;
            }
#endif
        }
        #endif
    }
}