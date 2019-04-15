using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serial;

public class Moving2 : MonoBehaviour, ISerializable
{
    public float paddleSpeed;
    public float limitLeft;
    public float limitRight;
    public float paddleXPos;
    private Vector3 playerPos = new Vector3(0, 0, 0);

    /*
     * Update()  
     *      Update is called once per frame
     *      We are getting the xPosition from the player’s keyboard input using the arrow keys (or WASD) keys.
     *      setting our playerPos variable to clamp between a maximum left and right value, which is so the paddle cannot travel further than we want it to (we can update these values at runtime by having them exposed as a public variable in the inspector). We are setting the other 2 values of this vector 3 using the paddle’s Y position (which we can also set in the inspector) and using 0f on the Z axis since we aren’t using any type of depth in this game.
     */
    void Update()
    {
        float yPos = transform.position.y + (Input.GetAxis("Vertical") * paddleSpeed);
        playerPos = new Vector3(paddleXPos, Mathf.Clamp(yPos, limitLeft, limitRight), 0f);
        transform.position = playerPos;
    }

    public virtual ISerialDataStore GetCurrentState()
    {
        return new MovingSave2( paddleSpeed, limitLeft,
                                limitRight, paddleXPos,
                                playerPos
                              );
    }

    public virtual void SetState(ISerialDataStore goalState)
    {
        // https://stackoverflow.com/questions/16534253/c-sharp-converting-base-class-to-child-class
        MovingSave2 newState = (MovingSave2) goalState;

        paddleSpeed = newState.paddleSpeed;
        limitLeft = newState.limitLeft;
        limitRight = newState.limitRight;
        paddleXPos = newState.paddleXPos;
        // https://docs.unity3d.com/ScriptReference/Vector3.Set.html
        playerPos.Set(newState.playerPos.x, newState.playerPos.y, newState.playerPos.z);

        transform.position = playerPos;
    }
}

internal class MovingSave2 : ISerialDataStore
{
    public float paddleSpeed { get; }
    public float limitLeft { get; }
    public float limitRight { get; }
    public float paddleXPos { get; }
    public Vector3 playerPos { get; }

    public MovingSave2( float speed, float limitL,
                        float limitR, float paddleX,
                        Vector3 pos
                      )
    {
        paddleSpeed = speed;
        limitLeft = limitL;
        limitRight = limitR;
        paddleXPos = paddleX;
        playerPos = pos;
    }
}