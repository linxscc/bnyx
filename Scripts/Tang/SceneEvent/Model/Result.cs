namespace Tang
{
    public enum ResultType
    {
        AddRole,
        SetObjState,
        GamePass
    }

    public class Result
    {
        public ResultType resultType;
        public object[] parameters;
    }
}