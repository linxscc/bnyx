using UnityEngine;
using Spine;
namespace Tang
{
    public class AttackController : MonoBehaviour
    {
        GameObject children;
        RoleController roleController;
        BoxCollider childrenbox;
        int slotindex;
        Slot slot;
        RegionAttachment Attachment;
        System.Type type;
        float x, y, rotation, scaleX = 1, scaleY = 1, width, height;
        private void Start()
        {
            children = gameObject.GetChild("children",true);
            roleController = gameObject.GetComponentInParent<RoleController>();
            slotindex=roleController.SkeletonAnimator.Skeleton.FindSlotIndex("Damage");
            slot= roleController.SkeletonAnimator.Skeleton.FindSlot("Damage");
            Attachment attachment = roleController.SkeletonAnimator.Skeleton.GetAttachment(slotindex, "Damage");
            type = attachment.GetType();

            if (type==typeof(RegionAttachment))
            {
                Attachment = attachment as RegionAttachment;
                if (Attachment != null)
                {
                    
                    x = Attachment.X;
                    y = Attachment.Y;
                    rotation = Attachment.Rotation;
                    scaleX = Attachment.ScaleX;
                    scaleY = Attachment.ScaleY;
                    width = Attachment.Width;
                    height = Attachment.Height;
                    childrenbox = children.AddComponentUnique<BoxCollider>();
                    children.transform.localPosition = new Vector3(x, y, 0);
                    children.transform.localRotation = Quaternion.Euler(0, 0, rotation);
                    children.transform.localScale = new Vector3(scaleX, scaleY, 1f);
                    childrenbox.size = new Vector3(width, height, 0);
                }
            }
            else if (type == typeof(MeshAttachment))
            {
                Debug.Log("MeshAttachment");
                //MeshAttachment Attachment = (MeshAttachment)slot.Attachment;
                //if (Attachment != null)
                //{
                //    x = Attachment.RegionOffsetX;
                //    y = Attachment.RegionOffsetY;
                //    rotation = Attachment.RegionOffsetRotation;
                //    scaleX = Attachment.RegionOffsetScaleX;
                //    scaleY = Attachment.RegionOffsetScaleY;
                //    width = Attachment.Width;
                //    height = Attachment.Height;
                //}
            }
            else if (type == typeof(BoundingBoxAttachment))
            {
                Debug.Log("BoundingBoxAttachment");
            }
            else if (type == typeof (PathAttachment))
            {
                Debug.Log("PathAttachment");
            }
            else
            {

            }
                
        }
        private void Update()
        {
            if (type == typeof(RegionAttachment))
            {
                if (Attachment != null)
                {
                    x = Attachment.X;
                    y = Attachment.Y;
                    rotation = Attachment.Rotation;
                    scaleX = Attachment.ScaleX;
                    scaleY = Attachment.ScaleY;
                    width = Attachment.Width;
                    height = Attachment.Height;

                    children.transform.localPosition = new Vector3(x, y, 0);
                    children.transform.localRotation = Quaternion.Euler(0, 0, rotation);
                    children.transform.localScale = new Vector3(scaleX, scaleY, 1f);
                    childrenbox.size = new Vector3(width, height, 0);
                }
            }
        }


    }
}

