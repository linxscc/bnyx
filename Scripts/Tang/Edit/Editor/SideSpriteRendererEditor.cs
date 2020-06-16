using UnityEngine;
using UnityEditor;

namespace Tang.Editor
{
    [CustomEditor(typeof(SideSpriteRenderer))]
    public class SideSpriteRendererEditor : UnityEditor.Editor
    {
        SideSpriteRenderer sideRenderer;
        SpriteRenderer spriteRenderer;

        public override void OnInspectorGUI()
        {
            LazyInit();

            sideRenderer.mainRenderer = spriteRenderer;
            if (sideRenderer.mainRenderer.sharedMaterial == null)
            {
                sideRenderer.mainRenderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Tang/SideSpriteRenderer.mat");
            }
            sideRenderer.mainRenderer.drawMode = SpriteDrawMode.Sliced;
            sideRenderer.transform.localRotation = Quaternion.Euler(new Vector3(0, -63.43495f, 0));
            sideRenderer.UpdateTextureSize();

            DrawDefaultInspector();
        }

        void LazyInit()
        {
            sideRenderer = target as SideSpriteRenderer;
            spriteRenderer = sideRenderer.gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = sideRenderer.gameObject.AddComponent<SpriteRenderer>();
            }
        }
    }
}