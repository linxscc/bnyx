using UnityEngine;

namespace Tang
{
    public static class BoundsExtend
    {
        public static Bounds IntersectsExtend(this Bounds target, Bounds other)
        {
            Vector3 center = new Vector3();
            Vector3 size = new Vector3();
            if (target.Intersects(other))
            {
                float Xmin =target.min.x<other.min.x? other.min.x:target.min.x ;
                float Xmax = target.max.x > other.max.x ? other.max.x : target.max.x ;
                float Ymin = target.min.y < other.min.y ? other.min.y : target.min.y ;
                float Ymax = target.max.y > other.max.y ? other.max.y : target.max.y ;
                float Zmin = target.min.z < other.min.z ? other.min.z : target.min.z ;
                float Zmax = target.max.z > other.max.z ? other.max.z : target.max.z;

                size = new Vector3(Xmax-Xmin,Ymax-Ymin,Zmax-Zmin);
                center = new Vector3(Xmin + (size.x / 2f), Ymin+(size.y / 2f), Zmin+(size.z / 2f));
            }
            else
            {
            }
            Bounds bounds = new Bounds(center,size);
            return bounds;
        }
    }
}

