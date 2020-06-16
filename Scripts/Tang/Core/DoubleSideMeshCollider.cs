using UnityEngine;

namespace Tang
{
    public class DoubleSideMeshCollider : MonoBehaviour
    {
        void Start()
        {
            GameObject gameObject = this.gameObject.GetChild("DoubleSideMeshGameObject", true);
            gameObject.layer = this.gameObject.layer;

            var mesh1 = this.gameObject.GetComponent<MeshCollider>().sharedMesh;
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

            gameObject.AddComponent<MeshFilter>().mesh = mesh2;
            gameObject.AddComponent<MeshCollider>().sharedMesh = mesh2;

            gameObject.transform.localScale = new Vector3(1, 1, 1);
            gameObject.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
}