namespace Tang
{
    public interface IEvent
    {
        string Name { get; }
        object Data { get; }
    }    
}
