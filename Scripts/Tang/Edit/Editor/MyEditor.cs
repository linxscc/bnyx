namespace Tang.Editor
{
    public class MyEditor : UnityEditor.Editor
    {
        public ValueMonitorPool valueMonitorPool = new ValueMonitorPool();

        public virtual void Update()
        {
            if (valueMonitorPool != null)
            {
                valueMonitorPool.Update();
            }
        }
    }
}