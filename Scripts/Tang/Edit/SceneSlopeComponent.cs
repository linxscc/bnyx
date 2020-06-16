








using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tang
{
    public enum SlopeType
    {
        None = 0,
        Left = 1,
        Right = 2,
    }

    [System.Serializable]
    public class SlopeData
    {
        public List<Vector2> digList = new List<Vector2>();

        public int widthZ = 1;
        // public int widthX = 1;
        public int length = 1;

        public int x = 0;
        public int y = 0;
        public int z = 0;

        public Vector3 GetPoint(int x, int z)
        {
            Vector3 vecX = x * (new Vector3(1 * SceneComponentConstant.GapX, 1 * SceneComponentConstant.GapY, 0)) * length;
            Vector3 vecZ = z * (new Vector3(-1 * SceneComponentConstant.GapX, 0, 2 * SceneComponentConstant.GapZ)) / 2.0f * widthZ;
            return vecX + vecZ;
        }
    }

    public class SceneSlopeComponent : SceneComponent
    {
        [SerializeField] public SlopeData slopData = new SlopeData();

        public override string ComponentType
        {
            get
            {
                return "Slope_" + slopData.widthZ + "x" + slopData.length;
            }
        }

        public override object Data
        {
            get
            {
                return slopData;
            }

            set
            {
                slopData = (SlopeData)value;
            }
        }

        public override Vector3 GridPos
        {
            set
            {
                slopData.x = (int)value.x;
                slopData.y = (int)value.y;
                slopData.z = (int)value.z;
            }
            get
            {
                return new Vector3(slopData.x, slopData.y, slopData.z);
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

            Vector3 point1 = transform.TransformPoint(slopData.GetPoint(0, 0));
            Vector3 point2 = transform.TransformPoint(slopData.GetPoint(1, 0));
            Vector3 point3 = transform.TransformPoint(slopData.GetPoint(1, 1));
            Vector3 point4 = transform.TransformPoint(slopData.GetPoint(0, 1));

            Gizmos.DrawLine(point1, point2);
            Gizmos.DrawLine(point2, point3);
            Gizmos.DrawLine(point3, point4);
            Gizmos.DrawLine(point4, point1);
        }

        public override void CreateMesh(string materialPath)
        {
#if UNITY_EDITOR
            Vector3 p1 = slopData.GetPoint(0, 0);
            Vector3 p2 = slopData.GetPoint(1, 0);
            Vector3 p3 = slopData.GetPoint(1, 1);
            Vector3 p4 = slopData.GetPoint(0, 1);

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Materials/Shaders/tang/Floor.png");

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
                    Vector3[] vertices = new Vector3[4]{
                                             p1, p2, p3, p4
                                         };

                    mesh.vertices = vertices;

                    // 通过顶点为网格创建三角形
                    int[] triangles = new int[2 * 3]{
                                             0,2,1, 0,3,2
                                         };

                    mesh.triangles = triangles;

                    mesh.uv = new Vector2[]
                    {
                        new Vector2(0, 0),
                        new Vector2(1, 0),
                        new Vector2(1, 1),
                        new Vector2(0, 1)
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

                material.SetColor("MulColor", new Color(159f / 255f, 240f / 255f, 72f / 255f, 1f));

                meshRenderer.sharedMaterial = material;
                meshRenderer.enabled = false;
            }

            // 创建渲染 add by TangJian 2018/10/15 17:39
            {
                GameObject rendererGameObject = gameObject.GetChild("Renderer", true);

                GameObject renderer1GameObject = rendererGameObject.GetChild("Renderer1", true);

                renderer1GameObject.layer = LayerMask.NameToLayer("Ground");

                MeshFilter rendererMeshFilter = renderer1GameObject.AddComponentUnique<MeshFilter>(true);
                MeshRenderer rendererMeshRenderer = renderer1GameObject.AddComponentUnique<MeshRenderer>(true);

                // 设置Renderer网格 add by TangJian 2018/10/15 16:34
                rendererMeshFilter.sharedMesh = meshFilter.sharedMesh;

                // 设置Renderer材质 add by TangJian 2018/10/15 16:34
                rendererMeshRenderer.sharedMaterial = meshRenderer.sharedMaterial;

                // 添加阴影 add by TangJian 2018/10/15 16:36
                renderer1GameObject.AddComponentUnique<ShadowReceiver>();

                rendererMeshRenderer.enabled = true;
            }

            // 创建碰撞 add by TangJian 2018/10/15 17:39
            {
                GameObject colliderGameObject = gameObject.GetChild("Collider", true);
                GameObject collider1GameObject = colliderGameObject.GetChild("Collider1", true);

                collider1GameObject.tag = "Ground";
                collider1GameObject.layer = LayerMask.NameToLayer("Ground");

                MeshFilter colliderMeshFilter = collider1GameObject.AddComponentUnique<MeshFilter>();
                colliderMeshFilter.sharedMesh = meshCollider.sharedMesh;

                MeshCollider colliderMeshCollider = collider1GameObject.AddComponentUnique<MeshCollider>();
            }
#endif
        }

        public void CreateTexture2D()
        {
            Vector3 p1 = slopData.GetPoint(0, 0);
            Vector3 p2 = slopData.GetPoint(1, 0);
            Vector3 p3 = slopData.GetPoint(1, 1);
            Vector3 p4 = slopData.GetPoint(0, 1);

            float factorY = Mathf.Sqrt(3) / 2f;
            float factorZ = 1f / 2f;

            p1.y = p1.y * factorY + p1.z * factorZ;
            p2.y = p2.y * factorY + p2.z * factorZ;
            p3.y = p3.y * factorY + p3.z * factorZ;
            p4.y = p4.y * factorY + p4.z * factorZ;

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
        }
        #endif
    }
}