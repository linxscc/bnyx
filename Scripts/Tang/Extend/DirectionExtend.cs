






namespace Tang
{


    public static class DirectionExtend
    {
        public static Direction FloatToDirection(this Direction direction, float f)
        {
            if (f >= 0)
            {
                return Direction.Right;
            }
            else
            {
                return Direction.Left;
            }
        }

        public static Direction IntToDirection(this Direction direction, int i)
        {
            if (i >= 0)
            {
                return Direction.Right;
            }
            else
            {
                return Direction.Left;
            }
        }

        public static float GetFloat(this Direction direction)
        {
            if (direction == Direction.Right)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public static int GetInt(this Direction direction)
        {
            if (direction == Direction.Right)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public static Direction Reverse(this Direction direction)
        {
            if (direction == Direction.Right)
            {
                return Direction.Left;
            }
            else
            {
                return Direction.Right;
            }
        }
    }
}