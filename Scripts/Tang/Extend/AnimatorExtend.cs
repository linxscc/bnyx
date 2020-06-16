using System;
using Spine;
using UnityEngine;
using Spine.Unity;





namespace Tang
{
    public static class AnimatorExtend
    {
        public static bool GetBonePos(this SkeletonRenderer target, string BoneName,out Vector3 worldPos)
        {
           
            worldPos = Vector3.zero;
            Bone bone = target.skeleton.FindBone(BoneName);
            if (bone == null) return false;
                
            Vector3 boneLocalPos = new Vector3(bone.worldX,bone.worldY,0); 
            worldPos = target.transform.TransformPoint(boneLocalPos);
//                worldPos += new Vector3(0, 0, -0.1f);
            return true;
                
        }
        
        public static void SetBool(this Animator target, string name, bool value)
        {

        }

        public static bool IsCurrTag(this Animator target, string tag)
        {
            return target.GetCurrentAnimatorStateInfo(0).IsTag(tag);
        }

        public static bool IsCurrName(this Animator target, string name)
        {
            return target.GetCurrentAnimatorStateInfo(0).IsName(name);
        }

        public static int GetCurrAnimNameHash(this Animator target, int layerIndex = 0)
        {
            return target.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash;
        }
        
        public static int GetCurrAnimTagHash(this Animator target, int layerIndex = 0)
        {
            return target.GetCurrentAnimatorStateInfo(layerIndex).tagHash;
        }
        
        public static bool TryGetSlotAttachmentCube(this SkeletonRenderer target, string slotName, out Vector3 worldPosition, out Vector3 worldScale, out Quaternion worldRotation)
        {
            worldPosition = Vector3.zero;
            worldScale = Vector3.zero;
            worldRotation = new Quaternion();

            var slot = target.skeleton.FindSlot(slotName);
            if (slot == null)
                return false;

            var bone = slot.bone;
            if (bone == null)
                return false;

            var attachment = slot.attachment as Spine.RegionAttachment;

            if (attachment == null)
                return false;

            // 骨骼平移矩阵 add by TangJian 2018/12/4 19:09
            Matrix4x4 boneMoveMatrix = new Matrix4x4();
            boneMoveMatrix.SetRow(0, new Vector4(1, 0, 0, bone.worldX));
            boneMoveMatrix.SetRow(1, new Vector4(0, 1, 0, bone.worldY));
            boneMoveMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            boneMoveMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 骨骼旋转矩阵 add by TangJian 2018/12/4 19:09
            Matrix4x4 boneRotationMatrix = new Matrix4x4();
            boneRotationMatrix.SetRow(0, new Vector4(Mathf.Cos(-bone.WorldRotationX / 180f * Mathf.PI), Mathf.Sin(-bone.WorldRotationX / 180f * Mathf.PI), 0, 0));
            boneRotationMatrix.SetRow(1, new Vector4(-Mathf.Sin(-bone.WorldRotationX / 180f * Mathf.PI), Mathf.Cos(-bone.WorldRotationX / 180f * Mathf.PI), 0, 0));
            boneRotationMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            boneRotationMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 骨骼缩放矩阵 add by TangJian 2018/12/4 21:50
            Matrix4x4 boneScaleMatrix = new Matrix4x4();
            boneScaleMatrix.SetRow(0, new Vector4(bone.WorldScaleX, 0, 0, 0));
            boneScaleMatrix.SetRow(1, new Vector4(0, bone.WorldScaleY, 0, 0));
            boneScaleMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            boneScaleMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 附件平移矩阵 add by TangJian 2018/12/4 21:35
            Matrix4x4 attachmentMoveMatrix = new Matrix4x4();
            attachmentMoveMatrix.SetRow(0, new Vector4(1, 0, 0, attachment.X));
            attachmentMoveMatrix.SetRow(1, new Vector4(0, 1, 0, attachment.Y));
            attachmentMoveMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            attachmentMoveMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 附件缩放矩阵 add by TangJian 2018/12/4 21:50
            Matrix4x4 attachmentScaleMatrix = new Matrix4x4();
            attachmentScaleMatrix.SetRow(0, new Vector4(attachment.ScaleX, 0, 0, 0));
            attachmentScaleMatrix.SetRow(1, new Vector4(0, attachment.ScaleY, 0, 0));
            attachmentScaleMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            attachmentScaleMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 附件旋转组件 add by TangJian 2018/12/4 21:35
            Matrix4x4 attachmentRotationMatrix = new Matrix4x4();
            attachmentRotationMatrix.SetRow(0, new Vector4(Mathf.Cos(-attachment.rotation / 180f * Mathf.PI), Mathf.Sin(-attachment.rotation / 180f * Mathf.PI), 0, 0));
            attachmentRotationMatrix.SetRow(1, new Vector4(-Mathf.Sin(-attachment.rotation / 180f * Mathf.PI), Mathf.Cos(-attachment.rotation / 180f * Mathf.PI), 0, 0));
            attachmentRotationMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            attachmentRotationMatrix.SetRow(3, new Vector4(0, 0, 0, 1));


            Matrix4x4 matrix = target.transform.localToWorldMatrix;
            matrix = matrix * boneMoveMatrix;
            matrix = matrix * boneScaleMatrix;
            matrix = matrix * boneRotationMatrix;

            matrix = matrix * attachmentMoveMatrix;
            matrix = matrix * attachmentScaleMatrix;
            matrix = matrix * attachmentRotationMatrix;

            worldPosition = matrix.MultiplyPoint(new Vector3(0, 0, 0));

            worldScale = boneScaleMatrix.MultiplyPoint(new Vector3(attachment.Width, attachment.Height, 1));
            worldScale = attachmentScaleMatrix.MultiplyPoint(worldScale);

            worldRotation = Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
            return true;
        }
        
        public static bool TryGetSlotAttachmentPos(this SkeletonRenderer target, string slotName, out Vector3 worldPosition)
        {
            worldPosition = Vector3.zero;

            var slot = target.skeleton.FindSlot(slotName);
            if (slot == null)
                return false;

            var bone = slot.bone;
            if (bone == null)
                return false;

            float attachmentX = 0;
            float attachmentY = 0;
            float attachmentScaleX = 0;
            float attachmentScaleY = 0;
            float attachmentRotation = 0;

            // 骨骼平移矩阵 add by TangJian 2018/12/4 19:09
            Matrix4x4 boneMoveMatrix = new Matrix4x4();
            boneMoveMatrix.SetRow(0, new Vector4(1, 0, 0, bone.worldX));
            boneMoveMatrix.SetRow(1, new Vector4(0, 1, 0, bone.worldY));
            boneMoveMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            boneMoveMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 骨骼旋转矩阵 add by TangJian 2018/12/4 19:09
            Matrix4x4 boneRotationMatrix = new Matrix4x4();
            boneRotationMatrix.SetRow(0, new Vector4(Mathf.Cos(-bone.WorldRotationX / 180f * Mathf.PI), Mathf.Sin(-bone.WorldRotationX / 180f * Mathf.PI), 0, 0));
            boneRotationMatrix.SetRow(1, new Vector4(-Mathf.Sin(-bone.WorldRotationX / 180f * Mathf.PI), Mathf.Cos(-bone.WorldRotationX / 180f * Mathf.PI), 0, 0));
            boneRotationMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            boneRotationMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 骨骼缩放矩阵 add by TangJian 2018/12/4 21:50
            Matrix4x4 boneScaleMatrix = new Matrix4x4();
            boneScaleMatrix.SetRow(0, new Vector4(bone.WorldScaleX, 0, 0, 0));
            boneScaleMatrix.SetRow(1, new Vector4(0, bone.WorldScaleY, 0, 0));
            boneScaleMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            boneScaleMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 附件平移矩阵 add by TangJian 2018/12/4 21:35
            Matrix4x4 attachmentMoveMatrix = new Matrix4x4();
            attachmentMoveMatrix.SetRow(0, new Vector4(1, 0, 0, attachmentX));
            attachmentMoveMatrix.SetRow(1, new Vector4(0, 1, 0, attachmentY));
            attachmentMoveMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            attachmentMoveMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 附件缩放矩阵 add by TangJian 2018/12/4 21:50
            Matrix4x4 attachmentScaleMatrix = new Matrix4x4();
            attachmentScaleMatrix.SetRow(0, new Vector4(attachmentScaleX, 0, 0, 0));
            attachmentScaleMatrix.SetRow(1, new Vector4(0, attachmentScaleY, 0, 0));
            attachmentScaleMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            attachmentScaleMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 附件旋转组件 add by TangJian 2018/12/4 21:35
            Matrix4x4 attachmentRotationMatrix = new Matrix4x4();
            attachmentRotationMatrix.SetRow(0, new Vector4(Mathf.Cos(-attachmentRotation / 180f * Mathf.PI), Mathf.Sin(-attachmentRotation / 180f * Mathf.PI), 0, 0));
            attachmentRotationMatrix.SetRow(1, new Vector4(-Mathf.Sin(-attachmentRotation / 180f * Mathf.PI), Mathf.Cos(-attachmentRotation / 180f * Mathf.PI), 0, 0));
            attachmentRotationMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            attachmentRotationMatrix.SetRow(3, new Vector4(0, 0, 0, 1));


            Matrix4x4 matrix = target.transform.localToWorldMatrix;
            matrix = matrix * boneMoveMatrix;
            matrix = matrix * boneScaleMatrix;
            matrix = matrix * boneRotationMatrix;

            matrix = matrix * attachmentMoveMatrix;
            matrix = matrix * attachmentScaleMatrix;
            matrix = matrix * attachmentRotationMatrix;

            worldPosition = matrix.MultiplyPoint(new Vector3(0, 0, 0));
            return true;
        }
        
        
        public static bool TryGetBonePos(this SkeletonRenderer target, string slotName, out Vector3 worldPosition)
        {
            worldPosition = Vector3.zero;

            var bone = target.skeleton.FindBone(slotName);
            if (bone == null)
                return false;

//            float attachmentX = 0;
//            float attachmentY = 0;
//            float attachmentScaleX = 0;
//            float attachmentScaleY = 0;
//            float attachmentRotation = 0;

            // 骨骼平移矩阵 add by TangJian 2018/12/4 19:09
            Matrix4x4 boneMoveMatrix = new Matrix4x4();
            boneMoveMatrix.SetRow(0, new Vector4(1, 0, 0, bone.worldX));
            boneMoveMatrix.SetRow(1, new Vector4(0, 1, 0, bone.worldY));
            boneMoveMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            boneMoveMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 骨骼旋转矩阵 add by TangJian 2018/12/4 19:09
            Matrix4x4 boneRotationMatrix = new Matrix4x4();
            boneRotationMatrix.SetRow(0, new Vector4(Mathf.Cos(-bone.WorldRotationX / 180f * Mathf.PI), Mathf.Sin(-bone.WorldRotationX / 180f * Mathf.PI), 0, 0));
            boneRotationMatrix.SetRow(1, new Vector4(-Mathf.Sin(-bone.WorldRotationX / 180f * Mathf.PI), Mathf.Cos(-bone.WorldRotationX / 180f * Mathf.PI), 0, 0));
            boneRotationMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            boneRotationMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 骨骼缩放矩阵 add by TangJian 2018/12/4 21:50
            Matrix4x4 boneScaleMatrix = new Matrix4x4();
            boneScaleMatrix.SetRow(0, new Vector4(bone.WorldScaleX, 0, 0, 0));
            boneScaleMatrix.SetRow(1, new Vector4(0, bone.WorldScaleY, 0, 0));
            boneScaleMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            boneScaleMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

//            // 附件平移矩阵 add by TangJian 2018/12/4 21:35
//            Matrix4x4 attachmentMoveMatrix = new Matrix4x4();
//            attachmentMoveMatrix.SetRow(0, new Vector4(1, 0, 0, attachmentX));
//            attachmentMoveMatrix.SetRow(1, new Vector4(0, 1, 0, attachmentY));
//            attachmentMoveMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
//            attachmentMoveMatrix.SetRow(3, new Vector4(0, 0, 0, 1));
//
//            // 附件缩放矩阵 add by TangJian 2018/12/4 21:50
//            Matrix4x4 attachmentScaleMatrix = new Matrix4x4();
//            attachmentScaleMatrix.SetRow(0, new Vector4(attachmentScaleX, 0, 0, 0));
//            attachmentScaleMatrix.SetRow(1, new Vector4(0, attachmentScaleY, 0, 0));
//            attachmentScaleMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
//            attachmentScaleMatrix.SetRow(3, new Vector4(0, 0, 0, 1));
//
//            // 附件旋转组件 add by TangJian 2018/12/4 21:35
//            Matrix4x4 attachmentRotationMatrix = new Matrix4x4();
//            attachmentRotationMatrix.SetRow(0, new Vector4(Mathf.Cos(-attachmentRotation / 180f * Mathf.PI), Mathf.Sin(-attachmentRotation / 180f * Mathf.PI), 0, 0));
//            attachmentRotationMatrix.SetRow(1, new Vector4(-Mathf.Sin(-attachmentRotation / 180f * Mathf.PI), Mathf.Cos(-attachmentRotation / 180f * Mathf.PI), 0, 0));
//            attachmentRotationMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
//            attachmentRotationMatrix.SetRow(3, new Vector4(0, 0, 0, 1));


            Matrix4x4 matrix = target.transform.localToWorldMatrix;
            matrix = matrix * boneMoveMatrix;
            matrix = matrix * boneScaleMatrix;
            matrix = matrix * boneRotationMatrix;

//            matrix = matrix * attachmentMoveMatrix;
//            matrix = matrix * attachmentScaleMatrix;
//            matrix = matrix * attachmentRotationMatrix;

            worldPosition = matrix.MultiplyPoint(new Vector3(0, 0, 0));
            return true;
        }

        public static void DrawGizmosSlotAttachmentBox(this SkeletonRenderer target, string slotName)
        {
            var slot = target.skeleton.FindSlot(slotName);
            if (slot == null)
                return;

            var bone = slot.bone;
            if (bone == null)
                return;

            var attachment = slot.attachment as Spine.RegionAttachment;

            if (attachment == null)
                return;

            // 骨骼平移矩阵 add by TangJian 2018/12/4 19:09
            Matrix4x4 boneMoveMatrix = new Matrix4x4();
            boneMoveMatrix.SetRow(0, new Vector4(1, 0, 0, bone.worldX));
            boneMoveMatrix.SetRow(1, new Vector4(0, 1, 0, bone.worldY));
            boneMoveMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            boneMoveMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 骨骼旋转矩阵 add by TangJian 2018/12/4 19:09
            Matrix4x4 boneRotationMatrix = new Matrix4x4();
            boneRotationMatrix.SetRow(0, new Vector4(Mathf.Cos(-bone.WorldRotationX / 180f * Mathf.PI), Mathf.Sin(-bone.WorldRotationX / 180f * Mathf.PI), 0, 0));
            boneRotationMatrix.SetRow(1, new Vector4(-Mathf.Sin(-bone.WorldRotationX / 180f * Mathf.PI), Mathf.Cos(-bone.WorldRotationX / 180f * Mathf.PI), 0, 0));
            boneRotationMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            boneRotationMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 骨骼缩放矩阵 add by TangJian 2018/12/4 21:50
            Matrix4x4 boneScaleMatrix = new Matrix4x4();
            boneScaleMatrix.SetRow(0, new Vector4(bone.WorldScaleX, 0, 0, 0));
            boneScaleMatrix.SetRow(1, new Vector4(0, bone.WorldScaleY, 0, 0));
            boneScaleMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            boneScaleMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 附件平移矩阵 add by TangJian 2018/12/4 21:35
            Matrix4x4 attachmentMoveMatrix = new Matrix4x4();
            attachmentMoveMatrix.SetRow(0, new Vector4(1, 0, 0, attachment.X));
            attachmentMoveMatrix.SetRow(1, new Vector4(0, 1, 0, attachment.Y));
            attachmentMoveMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            attachmentMoveMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 附件缩放矩阵 add by TangJian 2018/12/4 21:50
            Matrix4x4 attachmentScaleMatrix = new Matrix4x4();
            attachmentScaleMatrix.SetRow(0, new Vector4(attachment.ScaleX, 0, 0, 0));
            attachmentScaleMatrix.SetRow(1, new Vector4(0, attachment.ScaleY, 0, 0));
            attachmentScaleMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            attachmentScaleMatrix.SetRow(3, new Vector4(0, 0, 0, 1));

            // 附件旋转组件 add by TangJian 2018/12/4 21:35
            Matrix4x4 attachmentRotationMatrix = new Matrix4x4();
            attachmentRotationMatrix.SetRow(0, new Vector4(Mathf.Cos(-attachment.rotation / 180f * Mathf.PI), Mathf.Sin(-attachment.rotation / 180f * Mathf.PI), 0, 0));
            attachmentRotationMatrix.SetRow(1, new Vector4(-Mathf.Sin(-attachment.rotation / 180f * Mathf.PI), Mathf.Cos(-attachment.rotation / 180f * Mathf.PI), 0, 0));
            attachmentRotationMatrix.SetRow(2, new Vector4(0, 0, 1, 0));
            attachmentRotationMatrix.SetRow(3, new Vector4(0, 0, 0, 1));


            Matrix4x4 matrix = target.transform.localToWorldMatrix;
            matrix = matrix * boneMoveMatrix;
            matrix = matrix * boneScaleMatrix;
            matrix = matrix * boneRotationMatrix;

            matrix = matrix * attachmentMoveMatrix;
            matrix = matrix * attachmentScaleMatrix;
            matrix = matrix * attachmentRotationMatrix;

            Vector3 cubePoint = new Vector3(0, 0, 0);
            Vector3 cubeSize = new Vector3(attachment.Width, attachment.Height, 1);

            var oldMatrix = Gizmos.matrix;

            Gizmos.matrix = matrix;

            Gizmos.DrawCube(cubePoint, cubeSize);
            Gizmos.matrix = oldMatrix;
        }
    }
}