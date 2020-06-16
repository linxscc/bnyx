namespace Tang.Editor
{
    public class CreateActionState
    {
        public string StateName;
        public string AnimName01;
        public int AnimName01_Start;
        public int AnimName01_End;
        public float AnimName01_Speed;
        public int AnimName01_totalFrame;
        public string AnimName02;
        public int AnimName02_Start;
        public int AnimName02_End;
        public float AnimName02_Speed;
        public int AnimName02_totalFrame;
        public string AnimName03;
        public int AnimName03_Start;
        public int AnimName03_End;
        public float AnimName03_Speed;
        public int AnimName03_totalFrame;
        public string AnimName04;
        public int AnimName04_Start;
        public int AnimName04_End;
        public float AnimName04_Speed;
        public int AnimName04_totalFrame = 1;
        public bool Loop;
    }
    
    public class CreateHurtState
    {
        public string StateName;
        public string AnimName01;
        public int AnimName01_Start;
        public int AnimName01_End;
        public float AnimName01_Speed;
        public int AnimName01_totalFrame;
        public string AnimName02;
        public int AnimName02_Start;
        public int AnimName02_End;
        public float AnimName02_Speed;
        public int AnimName02_totalFrame;
        public string AnimName03;
        public int AnimName03_Start;
        public int AnimName03_End;
        public float AnimName03_Speed;
        public int AnimName03_totalFrame;
        public float ForceValue;
        public bool Loop;
    }

    public class CreateIdleState
    {
        public string StateName;
        public string AnimName;
        public int Start;
        public int End;
        public float Speed;
        public bool Loop;
    }
    
    public class CreateWalkState
    {
        public string StateName;
        public string AnimName;
        public int Start;
        public int End;
        public float Speed;
        public bool Loop;
    }
    
    public class CreateRunState
    {
        public string StateName;
        public string AnimName;
        public int Start;
        public int End;
        public float Speed;
        public bool Loop;
    }
    
    public class CreateDeathState
    {
        public string StateName;
        public string AnimName01;
        public int Start01;
        public int End01;
        public float Speed01;
        public string AnimName02;
        public int Start02;
        public int End02;
        public float Speed02;
        public bool Loop;
    }
}