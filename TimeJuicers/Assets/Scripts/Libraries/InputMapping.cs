using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InputMapping
{
    public class UserInput
    {
        private float axisBounds;

        public UserInput(float axis)
        {
            axisBounds = axis;
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

}

