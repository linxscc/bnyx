namespace Tang
{
    public enum ParameterType
    {
        Int,
        Float,
        String
    }

    public class Parameter
    {
        public ParameterType type;
        public string name;
        public object value;
    }

    public enum ConditionMode
    {
        Equal = 1,
        Less = 2,
        Greater = 3,
        If = 4,
        IfNot = 5
    }
    
    public class Condition
    {
        public string parameter;
        public ConditionMode conditionMode;
        public object value;
    }
}