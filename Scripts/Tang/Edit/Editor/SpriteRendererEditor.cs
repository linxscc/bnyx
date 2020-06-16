using UnityEngine;
using UnityEditor;


namespace Tang.Editor
{
    [CustomEditor(typeof(SpriteRenderer))]
    public class SpriteRendererEditor : UnityEditor.Editor
    {
        SpriteRenderer spriteRenderer;

        void OnEnable()
        {
            spriteRenderer = target as SpriteRenderer;
        }

        void Start()
        {
        }

        public override void OnInspectorGUI()
        {
            spriteRenderer.sortingOrder = MyGUI.IntFieldWithTitle("SortOrder", spriteRenderer.sortingOrder);

            base.DrawDefaultInspector();
            spriteRenderer.sharedMaterial = (Material)EditorGUILayout.ObjectField("Material", spriteRenderer.sharedMaterial, typeof(Material));

            if (spriteRenderer.sprite != null && spriteRenderer.sprite.texture != null)
            {
                MyGUI.FloatFieldWithTitle("Width", spriteRenderer.sprite.texture.width);
                MyGUI.FloatFieldWithTitle("Height", spriteRenderer.sprite.texture.height);
            }

            spriteRenderer.transform.localPosition = MyGUI.Vector3WithTitle("PixelPosition", spriteRenderer.transform.localPosition * 100f) / 100f;
        }
    }
}