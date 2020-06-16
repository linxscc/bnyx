

using UnityEngine;


namespace Tang.Interfaces
{
    public interface ISideWall
    {
        bool IsBreak { get; }

        Vector3 LinkBeginPos { get; }
        Vector3 LinkEndPos { get; }

        void SetBackSortRendererPos(Vector3 vector3);

        Vector3 GetBackSortRendererPos();
        Vector3 GetFrontSortRendererPos();

        bool IsMirror { get; }

        GameObject gameObject { get; }
        Transform transform { get; }
    }
}