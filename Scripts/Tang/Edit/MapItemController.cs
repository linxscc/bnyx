using UnityEngine;

namespace Tang
{
    public class MapItemController : MonoBehaviour
    {
        void OnCollisionEnter(Collision other)
        {
            var objTag = other.gameObject.tag;
            var layerName = LayerMask.LayerToName(other.gameObject.layer);
            if (layerName == "Terrain" && objTag != "Floor")
            {
                Destroy(gameObject, 0);
            }
        }

    }
}