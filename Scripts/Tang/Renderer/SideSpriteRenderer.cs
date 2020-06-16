


using UnityEngine;



namespace Tang
{
    public class SideSpriteRenderer : MonoBehaviour
    {
        public SpriteRenderer mainRenderer;

        private void OnEnable()
        {
            UpdateTextureSize();
        }

        public void UpdateTextureSize()
        {
            if (mainRenderer != null && mainRenderer.sprite != null)
            {
                float width = mainRenderer.sprite.texture.width;
                float height = mainRenderer.sprite.texture.height;
                mainRenderer.size = new Vector3(width / mainRenderer.sprite.pixelsPerUnit * Mathf.Sqrt(5), (height - width) / mainRenderer.sprite.pixelsPerUnit);


                MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                mainRenderer.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetFloat("_TexWidth", width);
                materialPropertyBlock.SetFloat("_TexHeight", height);
                mainRenderer.SetPropertyBlock(materialPropertyBlock);
            }
        }
    }
}