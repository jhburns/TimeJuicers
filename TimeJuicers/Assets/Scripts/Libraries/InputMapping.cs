using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// for reference: https://wiki.unity3d.com/index.php/Xbox360Controller

namespace InputMapping {

    /* 
     * Purpose of this namespace is to share code for the same buttons across objects
     * Currently cause users can't choose their input scheme,
     * Just returns if any of a number of inputs were selected
     */

    public class UserInput
    {
        private float axisBounds; // inside range [0-1]

        public UserInput()
        {
            axisBounds = 0.5f;
        }

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

        /*
         * JumpDown - when user presses whatever button is 'jump' down
         * Returns: bool true if button is pressed down this frame
         */
        public bool JumpDown()
        {
            return  Input.GetKeyDown(KeyCode.Space) ||
                    Input.GetKeyDown(KeyCode.W) ||
                    Input.GetKeyDown(KeyCode.UpArrow) ||
                    Input.GetKeyDown(KeyCode.Joystick1Button0) || // A button on xbox 360 controller
                    Input.GetKeyDown(KeyCode.Joystick1Button2) || // X button on xbox 360 controller
                    Input.GetAxisRaw("Vertical") > axisBounds;
        }

        /*
         * Jump - when user presses whatever button is 'jump'
         * Returns: bool true if button pressed this frame
         */
        public bool Jump()
        {
            return  Input.GetKey(KeyCode.Space) ||
                    Input.GetKey(KeyCode.W) ||
                    Input.GetKey(KeyCode.UpArrow) ||
                    Input.GetKey(KeyCode.Joystick1Button0) || // A button on xbox 360 controller
                    Input.GetKey(KeyCode.Joystick1Button2) || // X button on xbox 360 controller
                    Input.GetAxisRaw("Vertical") > axisBounds;
        }

        /*
         * RewindDown - when user presses whatever button is 'rewind' down
         * Returns: bool true if button is pressed down this frame
         */
        public bool RewindDown()
        {
            return  Input.GetKey(KeyCode.K) ||
                    Input.GetKey(KeyCode.R) ||
                    Input.GetKey(KeyCode.JoystickButton3) || // Y button on xbox 360 controller
                    Input.GetAxisRaw("LeftTrigger") == 1;
        }

        /*
         * FireDown - when user presses whatever button is 'fire' down
         * Returns: bool true if button is pressed down this frame
         */
        public bool FireDown()
        {
            return  Input.GetKey(KeyCode.F) ||
                    Input.GetKey(KeyCode.J) ||
                    Input.GetKey(KeyCode.JoystickButton1) || // B button on xbox 360 controller
                    Input.GetAxisRaw("RightTrigger") == 1;
        }

        public bool Left()
        {
            return  Input.GetKey(KeyCode.LeftArrow) ||
                    Input.GetKey(KeyCode.A) ||
                    Input.GetAxisRaw("Horizontal") < -axisBounds;
        }

        public bool Right()
        {
            return  Input.GetKey(KeyCode.RightArrow) ||
                    Input.GetKey(KeyCode.D) ||
                    Input.GetAxisRaw("Horizontal") > axisBounds;
        }

        public bool EitherDir()
        {
            return Left() || Right();
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

