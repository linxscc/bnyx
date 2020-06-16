namespace Tang.Editor
{
    // [CustomEditor(typeof(GameObject))]
    public class GameObjectEditor : DecoratorEditor
    {
        GameObjectEditor()
        {
            Init("GameObjectInspector");
        }

        public override void OnInspectorGUI()
        {
            EditorInstance.OnInspectorGUI();
        }
    }
}