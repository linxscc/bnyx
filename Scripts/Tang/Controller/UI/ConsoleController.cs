using UnityEngine;
using FairyGUI;


namespace Tang
{
    public class ConsoleController : MyMonoBehaviour
    {
        GComponent console;
        GComponent scrollText;
        GTextField text;

        string textString;

        void Start()
        {
            console = gameObject.GetChild("Console").GetComponent<UIPanel>().ui;
            scrollText = console.GetChild("scrollText").asCom;
            text = scrollText.GetChild("text").asTextField;

            text.textFormat.color = Color.white;
            text.textFormat.size = 36;
        }

        public void Print(object str)
        {
            
            textString += str;
            if (text != null)
            {
                text.text = textString;
            }
        }
        public void TypingEffect(GTextField textui,string text,int Length,float time){

        }
    }
}