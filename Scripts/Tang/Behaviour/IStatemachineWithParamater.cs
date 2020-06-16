using System.Collections.Generic;

namespace Tang
{
    public interface IStatemachineWithParamater
    {
        string Parameter { set; get; }
        List<string> NameList { set; get; }
        string AnimName { set; get; }
    }
}