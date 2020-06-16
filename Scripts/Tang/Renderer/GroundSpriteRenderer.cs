


using UnityEngine;



namespace Tang
{
    public class GroundSpriteRenderer : MonoBehaviour
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
                mainRenderer.transform.rotation = Quaternion.Euler(90, 0, 0);
                mainRenderer.transform.localScale = new Vector3(1, 1, 1);

                float width = mainRenderer.sprite.texture.width;
                float height = mainRenderer.sprite.texture.height;
                mainRenderer.size = new Vector3(width / mainRenderer.sprite.pixelsPerUnit, height * 2f / mainRenderer.sprite.pixelsPerUnit);


                MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                mainRenderer.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetFloat("_TexWidth", width);
                materialPropertyBlock.SetFloat("_TexHeight", height);
                mainRenderer.SetPropertyBlock(materialPropertyBlock);
            }
        }
    }
}