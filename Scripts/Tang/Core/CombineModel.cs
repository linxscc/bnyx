using UnityEngine;
using System.Collections.Generic;

namespace Tang
{
    public class CombineModel : MonoBehaviour
    {
        void Start()
        {
            //获取MeshRender;  
            MeshRenderer[] meshRenders = GetComponentsInChildren<MeshRenderer>();

            //材质;  
            List<Material> mats = new List<Material>();
            for (int i = 0; i < meshRenders.Length; i++)
            {
                var meshRender = meshRenders[i];
                foreach (var material in meshRender.sharedMaterials)
                {
                    mats.Add(material);
                }
            }

            //合并Mesh;  
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            for (int i = 0; i < meshFilters.Length; i++)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
            }

            MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
            MeshFilter mf = gameObject.AddComponent<MeshFilter>();
            mf.mesh = new Mesh();
            mf.mesh.CombineMeshes(combine, true);
            gameObject.SetActive(true);
            mr.sharedMaterials = mats.ToArray();
        }
    }
}