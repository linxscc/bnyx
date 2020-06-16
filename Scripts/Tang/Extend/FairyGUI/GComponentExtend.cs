using FairyGUI;


namespace Tang
{
    public static class GComponentExtend
    {
        public static GComponent GetChildWithPath(this GComponent target, string path)
        {
            string[] splitedName = path.Split('/');
            var parent = target;
            foreach (var name in splitedName)
            {
                parent = parent.GetChild(name).asCom;
            }
            return parent;
        }
    }
}