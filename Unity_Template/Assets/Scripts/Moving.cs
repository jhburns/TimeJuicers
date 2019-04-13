using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serial;

public class Moving : MonoBehaviour, ISerializable
{
    public float paddleSpeed;
    public float limitLeft;
    public float limitRight;
    public float paddleYPos;
    private Vector3 playerPos = new Vector3(0, 0, 0);

    /*
     * Update()  
     *      Update is called once per frame
     *      We are getting the xPosition from the player’s keyboard input using the arrow keys (or WASD) keys.
     *      setting our playerPos variable to clamp between a maximum left and right value, which is so the paddle cannot travel further than we want it to (we can update these values at runtime by having them exposed as a public variable in the inspector). We are setting the other 2 values of this vector 3 using the paddle’s Y position (which we can also set in the inspector) and using 0f on the Z axis since we aren’t using any type of depth in this game.
     */
    void Update()
    {
        float xPos = transform.position.x + (Input.GetAxis("Horizontal") * paddleSpeed);
        playerPos = new Vector3(Mathf.Clamp(xPos, limitLeft, limitRight), paddleYPos, 0f);
        transform.position = playerPos;
    }

    /*
     * GetCurrentState
     * Returns: ISerialDataStore of current global variables
     */
    public virtual ISerialDataStore GetCurrentState()
    {
        return new MovingSave(  paddleSpeed, limitLeft,
                                limitRight, paddleYPos,
                                playerPos
                             );
    }

    /*
     * SetState
     * Params:
     *  - ISerialDataStore goalState: what the object should try and change to
     */
    public virtual void SetState(ISerialDataStore goalState)
    {
        // https://stackoverflow.com/questions/16534253/c-sharp-converting-base-class-to-child-class
        MovingSave newState = (MovingSave) goalState;

        paddleSpeed = newState.paddleSpeed;
        limitLeft = newState.limitLeft;
        limitRight = newState.limitRight;
        paddleYPos = newState.paddleYPos;
        // https://docs.unity3d.com/ScriptReference/Vector3.Set.html
        playerPos.Set(newState.playerPos.x, newState.playerPos.y, newState.playerPos.z);

        transform.position = playerPos;
    }
}

internal class MovingSave : ISerialDataStore
{
    public float paddleSpeed { get; }
    public float limitLeft { get; }
    public float limitRight { get; }
    public float paddleYPos { get; }
    public Vector3 playerPos { get; }

    public MovingSave(  float speed, float limitL,
                        float limitR, float paddleY,
                        Vector3 pos
                     )
    {
        paddleSpeed = speed;
        limitLeft = limitL;
        limitRight = limitR;
        paddleYPos = paddleY;
        playerPos = pos;
    }
}