using UnityEditor;


namespace Tang.Editor
{
    public class MyEditorWindow : EditorWindow
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