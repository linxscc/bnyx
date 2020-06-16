using UnityEngine;
#if UNITY_EDITOR

#endif




namespace Tang
{
    [System.Serializable]
    public class SceneRoleComponentData
    {
        public int x = 0;
        public int y = 0;
        public float width = 1;
        public float height = 1;
    }

    public class SceneRoleComponent : SceneComponent
    {
        public SceneRoleComponentData data = new SceneRoleComponentData();

        public override object Data
        {
            get
            {
                return data;
            }

            set
            {
                data = (SceneRoleComponentData)value;
            }
        }

        public override Vector3 GridPos
        {
            get
            {
                return new Vector3(data.x, 0, data.y);
            }
            set
            {
                data.x = (int)value.x;
                data.y = (int)value.z;
            }
        }
        private void OnEnable()
        {
            GetComponent<Collider>().enabled = false;
        }

        public override void MoveByGridPos(Vector3 gridPos)
        {
            GridPos += gridPos;

            gameObject.transform.localPosition = new Vector3(GridPos.x, GridPos.y, GridPos.z);
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