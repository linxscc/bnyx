namespace Tang
{
    public class UGUIRoot : MyMonoBehaviour
    {


        public void AutoSizeImageWithWidth()
        {
            gameObject.RecursiveComponent((UGUIImage uguiImage, int depth) =>
            {
                uguiImage.AutoSizeWithWidth();
            }, 1, 99);
        }
    }
}