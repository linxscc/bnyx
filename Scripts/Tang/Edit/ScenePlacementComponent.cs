using UnityEngine;
#if UNITY_EDITOR

#endif




namespace Tang
{
    [System.Serializable]
    public class PlacementData
    {
        public int x = 0;
        public int y = 0;
        public int z = 0; // 后添加, 实际为y轴 add by TangJian 2018/8/16 15:37
        public float width = 1;
        public float height = 1;

        public Vector3 offset;
        public bool isMirror = false;
        public bool hasGravity = false;
    }

    public class ScenePlacementComponent : SceneComponent
    {
        public override string ComponentType { get { return "Placement"; } }
        public override string PathInScene { get { return "Placements"; } }

        public PlacementData data = new PlacementData();

        public override object Data
        {
            get
            {
                return data;
            }

            set
            {
                data = (PlacementData)value;
            }
        }

        public override Vector3 GridPos
        {
            get
            {
                return new Vector3(data.x, data.z, data.y);
            }
            set
            {
                data.x = (int)value.x;
                data.y = (int)value.z;
                data.z = (int)value.y;

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

        public override void MoveByGridPos(Vector3 gridPos)
        {
            if (Application.isPlaying)
            { }
            else
            {
                GridPos = GridPos + gridPos;
                gameObject.transform.localPosition = GridPosToLocalPos(GridPos) + Offset;
            }
        }

#if UNITY_EDITOR
        
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