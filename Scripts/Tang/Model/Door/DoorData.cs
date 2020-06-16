





namespace Tang
{
    public enum DoorState
    {
        Closed = 0,
        Opened = 1
    }

    public class DoorData
    {
        public string style;
        public DoorState state = DoorState.Opened;
        public string key;
    }
}