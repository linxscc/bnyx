using UnityEngine;

namespace Tang
{
    public class DoubleSideMeshShadowReceiver : MonoBehaviour
    {
        void Start()
        {
            GameObject doubleSideMeshGameObject = this.gameObject.GetChild("DoubleSideMeshGameObject", true);
            doubleSideMeshGameObject.layer = this.gameObject.layer;

            var mesh1 = this.gameObject.GetComponent<MeshFilter>().sharedMesh;
            var mesh2 = Instantiate(mesh1);

            var normals = mesh2.normals;
            for (int i = 0; i < normals.Length; ++i)
            {
                normals[i] = -normals[i];
            }
            mesh2.normals = normals;

            for (int i = 0; i < mesh2.subMeshCount; ++i)
            {
                int[] triangles = mesh2.GetTriangles(i);
                for (int j = 0; j < triangles.Length; j += 3)
                {
                    int temp = triangles[j];
                    triangles[j] = triangles[j + 1];
                    triangles[j + 1] = temp;
                }
                mesh2.SetTriangles(triangles, i);
            }

            doubleSideMeshGameObject.AddComponent<MeshFilter>().mesh = mesh2;

            doubleSideMeshGameObject.transform.localScale = new Vector3(1, 1, 1);
            doubleSideMeshGameObject.transform.localPosition = new Vector3(0, 0, 0);

            doubleSideMeshGameObject.AddComponentIfNone<MeshRenderer>();

            gameObject.AddComponentIfNone<ShadowReceiver>();
            doubleSideMeshGameObject.AddComponentIfNone<ShadowReceiver>();
        }
    }
}