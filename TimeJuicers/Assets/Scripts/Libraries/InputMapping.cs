using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace InputMapping
{
    public class UserInput
    {
        private float axisBounds; // inside range [0-1]

        public UserInput(float axis)
        {
            if (axis >= 0 && axis <= 1)
            {
                axisBounds = axis;
            }
            else
            {
                throw new RangeOutOfBoundsException("Range for axis bounds should be 0-1");
            }
        }

        public bool JumpDown()
        {
            return  Input.GetKeyDown(KeyCode.Space) ||
                    Input.GetKeyDown(KeyCode.W) ||
                    Input.GetKeyDown(KeyCode.UpArrow) ||
                    Input.GetKeyDown(KeyCode.Joystick1Button0) || // A button on xbox 360 controller
                    Input.GetKeyDown(KeyCode.Joystick1Button2) || // X button on xbox 360 controller
                    Input.GetAxisRaw("Vertical") > axisBounds;
        }
    }

    internal class RangeOutOfBoundsException : Exception
    {
        public RangeOutOfBoundsException()
        {

        }

        public RangeOutOfBoundsException(string message)
            : base(String.Format("Outside of range bounds: {0}", message))
        {

        }
    }
}

