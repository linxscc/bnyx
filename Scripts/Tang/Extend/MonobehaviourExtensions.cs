using UnityEngine;

namespace Tang
{
    public static class MonobehaviourExtensions
    {
        public static GameObject GetRendererObject(this MonoBehaviour target)
        {
            return target.gameObject.GetChild("Renderer", true);
        }
    }
}