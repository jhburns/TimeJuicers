using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Direction
{
    public class Direction
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
    }
}
