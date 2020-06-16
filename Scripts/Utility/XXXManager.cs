// 1.需要继承Singleton。
// 2.需要实现非public的构造方法。
// 可继承其他类的单例示例
public class XXXManager : Singleton<XXXManager>
{
    private XXXManager()
    {
        // to do ...
    }

    public void xxxyyyzzz()
    { }

    public static void main(string[] args)
    {
        XXXManager.Instance.xxxyyyzzz();
    }
}

