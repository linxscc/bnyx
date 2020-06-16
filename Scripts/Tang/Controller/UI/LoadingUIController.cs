using FairyGUI;


namespace Tang
{
    public class LoadingUIController : MyMonoBehaviour, UIInterface
    {
        GComponent ui;
        GProgressBar progressBar;
        GGraph back;
        GGraph bar;

        void Awake()
        {
            ui = GetComponent<UIPanel>().ui;
            progressBar = ui.GetChild("ProgressBar").asProgress;
            progressBar.max = 1;
        }

        public void SetProgress(float percent)
        {
            progressBar.value = percent.Range(0, 1);
        }
        

        public void Init()
        {
            throw new System.NotImplementedException();
        }

        public void Show(bool withAnim = true)
        {
            ui.visible = true;
        }

        public void Hide(bool withAnim = true)
        {
            ui.visible = false;
        }

        public bool IsShow()
        {
            throw new System.NotImplementedException();
        }
    }
}