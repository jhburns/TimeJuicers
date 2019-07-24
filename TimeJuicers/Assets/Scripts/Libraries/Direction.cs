namespace Direction
{
    public class Convert
    {
        /*
         * boolToScalar - converts a boolean direction to real value
         * Params:
         *  - bool direction: where the object is currently facing 
         *  Returns: int of 1 for true, and -1 for false
         */
        public static int boolToScalar(bool direction)
        {
            if (direction)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /*
         * ChooseFrom - given two values, determines the correct one
         * Params:
         *  - bool direction: where the object is currently facing
         *  - T first: returned when direction is true
         *  - T second: returned when direction is false
         * Returns: T determined by direction's value
         */
        public static T ChooseFrom<T>(bool direction, T first, T second)
        {
            if (direction)
            {
                return first;
            }
            else
            {
                return second;
            }
        }
    }
}
