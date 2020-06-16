namespace AssemblyCSharp
{
    public class Role
    {
        public enum ROLE_STATUS
        {
                NONE = 0,
                IDLE = 1,
                RUNNING = 2 ,
                FLOATING = 3 ,
                ACTION = 4
        }
        public ROLE_STATUS status = 0;

        public string name = "";


        public Role()
        {
        }



    }
}

