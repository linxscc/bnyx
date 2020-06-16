using UnityEngine;
#if UNITY_EDITOR

#endif




namespace Tang
{
    [System.Serializable]
    public class SceneDecorationData
    {
        public int x = 0;
        public int y = 0;
        public int z = 0;

        public Vector3 offset;
    }
    public class SceneDecorationEditorwindowData
    {
        public string texturePath;
        public SceneDecorationPosition sceneDecorationPosition = SceneDecorationPosition.Positive;
        public string id;
        public tian.ScaleType scaleType = tian.ScaleType.Whole;
        public float AloneScale = 1;
    }
    public enum SceneDecorationPosition
    {
        Positive = 0,
        LeftSide = 1,
        RightSide = 2,
        ground = 3,
    }
    public class SceneDecorationComponent : SceneComponent
    {
        public SceneDecorationData data = new SceneDecorationData();

        public override object Data
        {
            get
            {
                return data;
            }

            set
            {
                data = (SceneDecorationData)value;
            }
        }

        public override Vector3 GridPos
        {
            get
            {
                return new Vector3(data.x, data.y, data.z);
            }
            set
            {
                data.x = (int)value.x;
                data.y = (int)value.y;
                data.z = (int)value.z;
            }
        }

        public override Vector3 Offset
        {
            set
            {
                data.offset = value;
            }
            get
            {
                return data.offset;
            }
        }

        void Awake()
        {
            if (GetComponent<Collider>())
                GetComponent<Collider>().enabled = false;
        }

        public override Vector3 GridPosToLocalPos(Vector3 gridPos)
        {
            return gridPos + new Vector3(0, 0, -0.03f);
        }

        public override Vector3 LocalPosToGridPos(Vector3 localPos)
        {
            return localPos;
        }

#if UNITY_EDITOR
        public override void MoveByGridPos(Vector3 gridPos)
        {
            GridPos = GridPos + gridPos;
            gameObject.transform.localPosition = GridPosToLocalPos(GridPos) + Offset;
        }

        public override void CreateMesh(string materialPath = "")
        {
        }

        public override void OnDrawGizmos()
        {
            MoveByGridPos(Vector3.zero);
        }
        #endif
    }
}