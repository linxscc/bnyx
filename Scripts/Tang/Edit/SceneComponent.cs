using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Tang
{
    public static class SceneComponentConstant
    {
        public static float GapX = 3;
        public static float GapY = 3 * 1.154700538379252f;
        public static float GapZ = 3;

    }
    
    public abstract class SceneComponent : MonoBehaviour
    {
        [SerializeField] public string filePath;

        [SerializeField] public GameObject prefab;

        [SerializeField] public Color color;

        [SerializeField] public bool isMirror;
        [SerializeField] public bool flipY = false;
        [SerializeField] public bool flipZ = false;

        [SerializeField]
        public virtual string ComponentType { get { return "SceneComponent"; } }
        [SerializeField]
        public virtual string PathInScene { get { return "SceneComponents"; } }

        public void RefreshFlip()
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (isMirror ? -1 : 1)
                , Mathf.Abs(transform.localScale.y) * (flipY ? -1 : 1)
                , Mathf.Abs(transform.localScale.z) * (flipZ ? -1 : 1));
        }

        public abstract object Data
        {
            get; set;
        }

        public virtual Vector3 Offset
        {
            set;
            get;
        }

        public virtual Vector3 GridPosToLocalPos(Vector3 gridPos)
        {
            return new Vector3(gridPos.x * SceneComponentConstant.GapX + gridPos.z * SceneComponentConstant.GapZ / 2, gridPos.y * SceneComponentConstant.GapY, gridPos.z * SceneComponentConstant.GapZ);
        }

        public virtual Vector3 LocalPosToGridPos(Vector3 localPos)
        {
            return new Vector3((localPos.x - localPos.z / 2.0f) / SceneComponentConstant.GapZ, localPos.y / SceneComponentConstant.GapY, localPos.z / SceneComponentConstant.GapZ);
        }

        public virtual void MoveToLocalPos(Vector3 localPos)
        {
            MoveToGridPos(LocalPosToGridPos(localPos));
        }

        public virtual void MoveByLocalPos(Vector3 localPos)
        {
            MoveToGridPos(LocalPosToGridPos(transform.localPosition + localPos));
        }

        public virtual void MoveByGridPos(Vector3 gridPos)
        {
            GridPos = GridPos + gridPos;
            gameObject.transform.localPosition = GridPosToLocalPos(GridPos);
        }

        public virtual void MoveToGridPos(Vector3 gridPos)
        {
            GridPos = gridPos;
            gameObject.transform.localPosition = GridPosToLocalPos(GridPos);
        }

        public abstract Vector3 GridPos { set; get; } // 中心点网格位置 add by TangJian 2018/8/1 17:18

        public virtual void Refresh()
        {
            // 刷新位置
            MoveByGridPos(Vector3.zero);
            RefreshFlip();
        }
        
#if UNITY_EDITOR
        
        public abstract void CreateMesh(string materialPath);

        public abstract void OnDrawGizmos();

        
        public void CreateMesh(
            string materialPath
            , Vector3[] vertices
            , int[] triangles
            , Vector2[] uv
            , Shader shader
            , Texture2D texture
            , out MeshFilter meshFilter
            , out MeshRenderer meshRenderer
            , out MeshCollider meshCollider)
        {
            meshFilter = gameObject.AddComponentUnique<MeshFilter>(true);
            meshRenderer = gameObject.AddComponentUnique<MeshRenderer>(true);
            meshCollider = gameObject.AddComponentUnique<MeshCollider>(true);

            // 初始化mesh uv 等add by TangJian 2017/12/20 17:04:45
            {
                {
                    Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(materialPath + ".asset");

                    if (mesh == null)
                    {
                        mesh = new Mesh();
                        AssetDatabase.CreateAsset(mesh, materialPath + ".asset");
                        mesh = AssetDatabase.LoadAssetAtPath<Mesh>(materialPath + ".asset");
                    }

                    mesh.vertices = vertices;
                    mesh.triangles = triangles;
                    mesh.uv = uv;

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

                material.SetColor("MulColor", new Color(234f / 255f, 240f / 255f, 72f / 255f, 1f));

                meshRenderer.sharedMaterial = material;

                meshRenderer.enabled = false;
            }
        }

        public void AddMeshCollider(string rendererName, Mesh mesh, int layer)
        {
            GameObject colliderGameObject = gameObject.GetChild("Collider", true);


            GameObject collider1GameObject = colliderGameObject.GetChild(rendererName, true);

            collider1GameObject.layer = layer;

            MeshFilter colliderMeshFilter = collider1GameObject.AddComponentUnique<MeshFilter>(true);
            MeshCollider colliderMeshCollider = collider1GameObject.AddComponentUnique<MeshCollider>(true);

            // 设置Renderer网格 add by TangJian 2018/10/15 16:34
            colliderMeshFilter.sharedMesh = mesh;
            colliderMeshCollider.sharedMesh = mesh;

            collider1GameObject.AddComponentUnique<DoubleSideMeshCollider>();
        }

        public void AddMeshRenderer(string rendererName, Mesh mesh, Material material, int layer)
        {
            GameObject rendererGameObject = gameObject.GetChild("Renderer", true);


            GameObject renderer1GameObject = rendererGameObject.GetChild(rendererName, true);

            renderer1GameObject.layer = layer;

            MeshFilter rendererMeshFilter = renderer1GameObject.AddComponentUnique<MeshFilter>(true);
            MeshRenderer rendererMeshRenderer = renderer1GameObject.AddComponentUnique<MeshRenderer>(true);

            // 设置Renderer网格 add by TangJian 2018/10/15 16:34
            rendererMeshFilter.sharedMesh = mesh;

            // 设置Renderer材质 add by TangJian 2018/10/15 16:34
            rendererMeshRenderer.sharedMaterial = material;
        }
        
#endif
    }
}