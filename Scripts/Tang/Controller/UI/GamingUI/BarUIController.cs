using UnityEngine.UI;



namespace Tang
{
    public class BarUIController : MyMonoBehaviour
    {
        Slider slider;
        public Slider Slider
        {
            get
            {
                if (slider == null)
                {
                    slider = GetComponent<Slider>();
                }
                return slider;
            }
        }

        Text text;
        public Text Text
        {
            get
            {
                if (text == null)
                {
                    text = Tools.GetChild(gameObject, "Value").GetComponent<Text>();
                }
                return text;
            }
        }

        public void SetValue(float cur, float max)
        {
            Slider.value = Tools.Range(cur / max, 0, 1);
            Text.text = cur + "/" + max;
        }
    }
}