using UnityEditor;

namespace Tang.Editor
{
    [CustomEditor(typeof(ImageWall))]
    public class ImageWallEditor : UnityEditor.Editor
    {
        void OnEnable()
        {
            // foreach (var item in targets)
            // {
            //     ImageWall imageWall = item as ImageWall;

            //     // imageWall.Init();
            //     imageWall.Update();
            // }
        }

        public override void OnInspectorGUI()
        {
            if (MyGUI.Button("重置原始尺寸"))
            {
                foreach (ImageWall imageWall in targets)
                {
                    imageWall.Reset();
                }
            }



            foreach (ImageWall imageWall in targets)
            {
                imageWall.Update();
            }


            DrawDefaultInspector();
        }
    }
}