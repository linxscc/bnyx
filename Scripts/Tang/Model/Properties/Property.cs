namespace Tang
{
    [System.Serializable]
    public class Property
    {
        public string key;
        public string value;
    }

    [System.Serializable]
    public class PropertyString
    {
        public PropertyString(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public string key;
        public string value;
    }

    [System.Serializable]
    public class PropertyFloat
    {
        public PropertyFloat(string key, float value)
        {
            this.key = key;
            this.value = value;
        }

        public string key;
        public float value;
    }
}