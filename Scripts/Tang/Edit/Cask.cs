using UnityEngine;
#if UNITY_EDITOR

#endif




namespace Tang
{
    [System.Serializable]
    public class CaskData
    {
        public int x = 0;
        public int y = 0;
        public float width = 1;
        public float height = 1;
    }

    public class Cask : SceneComponent
    {
        public CaskData data = new CaskData();

        public override object Data
        {
            get
            {
                return data;
            }

            set
            {
                data = (CaskData)value;
            }
        }

        public override Vector3 GridPos
        {
            get
            {
                return new Vector3(data.x, 0, data.y);
            }
            set
            {
                data.x = (int)value.x;
                data.y = (int)value.z;
            }
        }

        private void OnEnable()
        {
            GetComponent<BoxCollider>().enabled = false;
        }

        public override void MoveByGridPos(Vector3 gridPos)
        {
            GridPos += gridPos;

            gameObject.transform.localPosition = new Vector3(GridPos.x, GridPos.y, GridPos.z);

        }
#if UNITY_EDITOR
        public override void CreateMesh(string materialPath = "")
        {
            // gameObject.layer = LayerMask.NameToLayer("SceneComponent");

            // Vector3 p1 = Vector2.zero;
            // Vector3 p2 = p1 + new Vector3(data.width, 0, 0);

            // Vector3 p3 = p2 + new Vector3(0, data.height, 0);
            // Vector3 p4 = p1 + new Vector3(0, data.height, 0);

            // Texture2D texture = Resources.Load<Texture2D>("Prefabs/SceneObject/Stage01/Textures/Stage01_Plac02A");

            // MeshFilter meshFilter = gameObject.AddComponentUnique<MeshFilter>(true);
            // MeshRenderer meshRenderer = gameObject.AddComponentUnique<MeshRenderer>(true);
            // MeshCollider meshCollider = gameObject.AddComponentUnique<MeshCollider>(true);
            // // meshCollider.convex = true;

            // // 初始化mesh uv 等add by TangJian 2017/12/20 17:04:45
            // {
            //     // Shader shader = Shader.Find("Custom/Unlit/Transparent");
            //     Shader shader = Shader.Find("Sprites/Default");

            //     {
            //         Mesh mesh = new Mesh();

            //         // 为网格创建顶点数组
            //         Vector3[] vertices = new Vector3[4]{
            //                     p1, p2, p3, p4
            //                 };

            //         mesh.vertices = vertices;

            //         // 通过顶点为网格创建三角形
            //         int[] triangles = new int[2 * 3]{
            //                     0,2,1, 0,3,2
            //                 };

            //         mesh.triangles = triangles;

            //         mesh.uv = new Vector2[]{
            //                     new Vector2(0, 0),
            //                     new Vector2(1, 0),
            //                     new Vector2(1, 1),
            //                     new Vector2(0, 1)
            //                 };

            //         if (materialPath != "")
            //         {
            //             AssetDatabase.CreateAsset(mesh, materialPath + ".asset");
            //             mesh = AssetDatabase.LoadAssetAtPath(materialPath + ".asset", typeof(Mesh)) as Mesh;
            //         }

            //         meshCollider.sharedMesh = mesh;

            //         meshFilter.mesh = mesh;
            //     }
            //     Material material = new Material(shader);
            //     material.mainTexture = texture;
            //     material.renderQueue = 2000;
            //     // material.SetFloat("_IsScale", 0f);
            //     // material.SetFloat("_Cutoff", 0.2f);


            //     if (materialPath != "")
            //     {
            //         AssetDatabase.CreateAsset(material, materialPath + ".mat");
            //         material = AssetDatabase.LoadAssetAtPath(materialPath + ".mat", typeof(Material)) as Material;
            //     }

            //     meshRenderer.material = material;

            //     meshRenderer.enabled = false;
            // }

        }

        public override void OnDrawGizmos()
        {
            MoveByGridPos(Vector3.zero);

            // Gizmos.color = color;

            // Vector2 p1 = Vector2.zero;
            // Vector2 p2 = p1 + new Vector2(data.width, 0);
            // Vector2 p4 = p1 + new Vector2(0, data.height);
            // Vector2 p3 = p2 + new Vector2(0, data.height);

            // Vector3 vec3_p1 = gameObject.transform.position + new Vector3(p1.x, p1.y, 0);
            // Vector3 vec3_p2 = gameObject.transform.position + new Vector3(p2.x, p2.y, 0);
            // Vector3 vec3_p3 = gameObject.transform.position + new Vector3(p3.x, p3.y, 0);
            // Vector3 vec3_p4 = gameObject.transform.position + new Vector3(p4.x, p4.y, 0);

            // Gizmos.DrawLine(vec3_p1, vec3_p2);
            // Gizmos.DrawLine(vec3_p2, vec3_p3);
            // Gizmos.DrawLine(vec3_p3, vec3_p4);
            // Gizmos.DrawLine(vec3_p4, vec3_p1);
        }
        #endif
      
    }
}