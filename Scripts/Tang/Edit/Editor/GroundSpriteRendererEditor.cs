using UnityEngine;
using UnityEditor;

namespace Tang.Editor
{
    [CustomEditor(typeof(GroundSpriteRenderer))]
    public class GroundSpriteRendererEditor : UnityEditor.Editor
    {
        GroundSpriteRenderer sideRenderer;
        SpriteRenderer spriteRenderer;

        public override void OnInspectorGUI()
        {
            LazyInit();

            sideRenderer.mainRenderer = spriteRenderer;
            if (sideRenderer.mainRenderer.sharedMaterial == null)
            {
                sideRenderer.mainRenderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Tang/GroundSpriteRenderer.mat");
            }
            sideRenderer.mainRenderer.drawMode = SpriteDrawMode.Sliced;
            sideRenderer.UpdateTextureSize();

            DrawDefaultInspector();
        }

        void LazyInit()
        {
            sideRenderer = target as GroundSpriteRenderer;
            spriteRenderer = sideRenderer.gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = sideRenderer.gameObject.AddComponent<SpriteRenderer>();
            }
        }
    }
}