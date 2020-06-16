namespace Tang
{
    [System.Serializable]
    public class Event : IEvent
    {
        public override string ToString()
        {
            var typeToString = type.ToString();
            var nameToString = name == null ? "null" : name;
            var dataToString = data == null ? "null" : data.ToString();
            return "{" + "type=" + typeToString + ", " + "name=" + nameToString + ", " + "data=" + dataToString + "}";
        }
        
        private EventType type;
        private string name;
        private object data;
        public EventType Type { get { return type; } set { type = value; } }
        public string Name { get { return name; } set { name = value; } }
        public object Data { get { return data; } set { data = value; } }
    }
}
