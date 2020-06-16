

using UnityEngine;
#if UNITY_EDITOR

#endif

namespace Tang
{
    public class SceneSideDoorComponent : SceneSideWallComponent
    {
        public override string PathInScene
        {
            get { return "Portals"; }
        }

        public override bool IsBreak { get { return true; } }

        public override void SetBackSortRendererPos(Vector3 vector3)
        {
            gameObject.RecursiveComponent((SortRenderer sr, int depth) =>
            {
                if (sr.name.IndexOf("$back") >= 0)
                {
                    sr.SetSortRendererPos(vector3);
                }
            }, 1, 999);
        }

        //public override Vector3 GetBackSortRendererPos()
        //{
        //    Vector3 pos = Vector3.zero;
        //    gameObject.RecursiveComponent((SortRenderer sr, int depth) =>
        //    {
        //        if (sr.name.IndexOf("$front") >= 0)
        //        {
        //            pos = sr.MainTransform.position;
        //        }
        //    }, 1, 999);

        //    return pos;
        //}

        public override Vector3 GetFrontSortRendererPos()
        {
            Vector3 pos = Vector3.zero;
            gameObject.RecursiveComponent((SortRenderer sr, int depth) =>
            {
                if (sr.name.IndexOf("$front") >= 0)
                {
                    pos = sr.MainTransform.position;
                }
            }, 1, 999);

            return pos;
        }
    }
}