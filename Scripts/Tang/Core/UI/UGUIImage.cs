using UnityEngine;
using UnityEngine.UI;



namespace Tang
{
    public class UGUIImage : MyMonoBehaviour
    {
        public void AutoSizeWithWidth()
        {
            Image img = GetComponent<Image>();
            float hdw = img.sprite.rect.height / img.sprite.rect.width;
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x * hdw);
        }
    }
}