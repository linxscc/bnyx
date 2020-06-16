using UnityEngine;
using Tang.Interfaces;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tang
{
    [System.Serializable]
    public class SideWallData
    {
        public int x = 0;
        public int y = 0;
        public int z = 0;

        public Vector3 offset;
        
        [SerializeField] public float widthZ = 3;
        [SerializeField] public float widthY = 3;

        public Vector3[] GetGrids()
        {
            return new Vector3[4] {
                new Vector3(0, 0, 0),
                new Vector3(0, widthY , 0),
                new Vector3(widthZ / 2, widthY, widthZ),
                new Vector3(widthZ / 2, 0, widthZ),
            };
        }

        public Vector3[] GetVertices()
        {
            return new Vector3[4] {
                new Vector3(0, 0, 0),
                new Vector3(0, SceneComponentConstant.GapY * widthY , 0),
                new Vector3(SceneComponentConstant.GapZ * widthZ / 2, SceneComponentConstant.GapY * widthY, SceneComponentConstant.GapZ * widthZ),
                new Vector3(SceneComponentConstant.GapZ * widthZ / 2, 0, SceneComponentConstant.GapZ * widthZ),
            };
        }
    }

    public class SceneSideWallComponent : SceneComponent, ISideWall
    {
        [SerializeField] public SideWallData wallData = new SideWallData();

        public override string ComponentType
        {
            get
            {
                return "Wall_Side_" + wallData.widthZ + "x" + wallData.widthY;
            }
        }

        public override string PathInScene
        {
            get
            {
                return "Terrains/Walls";
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

        public override object Data
        {
            get
            {
                return wallData;
            }

            set
            {
                wallData = (SideWallData)value;
            }
        }

        public Vector3 LinkBeginPos
        {
            get
            {
                return GridPos;
            }
        }

        public Vector3 LinkEndPos
        {
            get
            {
                if (isMirror)
                {
                    return GridPos + new Vector3(-wallData.widthZ, 0, wallData.widthZ);
                }
                else
                {
                    return GridPos + new Vector3(0, 0, wallData.widthZ);
                }
            }
        }

        public bool IsMirror { get { return isMirror; } }

        public virtual bool IsBreak { get { return false; } }


        public virtual void SetBackSortRendererPos(Vector3 vector3)
        {
            gameObject.RecursiveComponent((SortRenderer sr, int depth) =>
            {
                sr.SetSortRendererPos(vector3);
            }, 1, 999);
        }

        public virtual Vector3 GetBackSortRendererPos()
        {
            Collider collider = GetComponent<Collider>();
            return new Vector3(collider.bounds.center.x, collider.bounds.center.y, collider.bounds.max.z);
        }

        public virtual Vector3 GetFrontSortRendererPos()
        {
            Collider collider = GetComponent<Collider>();
            return new Vector3(collider.bounds.center.x, collider.bounds.center.y, collider.bounds.max.z);
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
            gameObject.layer = LayerMask.NameToLayer("SceneComponent");

            Vector3[] vertexes = wallData.GetVertices();

            MeshFilter meshFilter;
            MeshRenderer meshRenderer;
            MeshCollider meshCollider;

            Vector3 p1 = vertexes[0];
            Vector3 p2 = vertexes[1];

            Vector3 p3 = vertexes[2];
            Vector3 p4 = vertexes[3];

            CreateMesh(materialPath
                , new Vector3[4] { p1, p2, p3, p4 }
                , new int[] { 0, 1, 2, 0, 2, 3 }
                , new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) }
                , Shader.Find("Tang/Unlit/PureColor")
                , AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Materials/Shaders/tang/Wall.jpg")
                , out meshFilter, out meshRenderer, out meshCollider);

            // 创建Renderer add by TangJian 2018/10/15 16:34
            AddMeshRenderer("Template", meshFilter.sharedMesh, meshRenderer.sharedMaterial, LayerMask.NameToLayer("Default"));

            // 创建碰撞体 add by TangJian 2018/11/30 19:23
            AddMeshCollider("Template", meshFilter.sharedMesh, LayerMask.NameToLayer("Wall"));

            AssetDatabase.SaveAssets();
        }

        public void CreateTexture2D()
        {
            Vector3[] vertices = wallData.GetVertices();
            Vector3 p1 = vertices[0];
            Vector3 p2 = vertices[1];
            Vector3 p3 = vertices[2];
            Vector3 p4 = vertices[3];

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