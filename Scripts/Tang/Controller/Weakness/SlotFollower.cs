using UnityEngine;
using Spine.Unity;
namespace Tang
{
    public class SlotFollower : MonoBehaviour
    {
        public SkeletonAnimator skeletonAnimator;
        public string slotName;
        public Collider mainCollider;
	    // Use this for initialization
	    void Start ()
        {
            mainCollider = GetComponent<Collider>();
        }
	
	    // Update is called once per frame
	    void Update () {
            Follow();
        }
        void Follow()
        {

            if (skeletonAnimator != null && slotName != null)
            {
                Vector3 worldPostion, worldScale;
                Quaternion worldRotation;
                if (skeletonAnimator.TryGetSlotAttachmentCube(slotName, out worldPostion, out worldScale, out worldRotation))
                {
                    transform.position = worldPostion;
                    transform.localScale = new Vector3(worldScale.x, worldScale.y, transform.localScale.z); // z保持不变 add by TangJian 2018/12/5 17:43
                    transform.localRotation = worldRotation;
                    if (mainCollider != null)
                    {
                        mainCollider.enabled = true;
                    }
                    
                }
            }
            else
            {
                if (mainCollider != null)
                {
                    mainCollider.enabled = false;
                }
                
            }

        }
    
    }

}
