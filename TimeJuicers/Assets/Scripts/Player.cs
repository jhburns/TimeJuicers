using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Serial;
using InputMapping;
using static Direction.Convert;

public class Player : MonoBehaviour, ISerializable
{
    public float jumpHeight; //IM = suppose to be immutable
    public float maxMoveSpeed; //IM
    public float terminalVelocity; // negative number, range < 0
    private bool isExitingRewind; // mutable but tracked, used to transition out of time rewind

    public float acceleration;
    private float velocityX;
    private float velocityY;
    public bool IsMovingRight { get; private set; }

    private bool isEitherDown;
    private bool isLeftDown;
    private bool isRightDown;
    private bool isLeftUp;
    private bool isRightUp;

    private Rigidbody2D rb; // Mutable, only parts of it tracked
    private BoxCollider2D col; //IM

    private bool isGrounded;
    private int jumps;
    private const int maxJumps = 1; //IM
    private int wallJumps;
    private bool canWallJump;
    private const int maxWallJumps = 1; //IM

    public GlobalUI deathHandler; //IM
    public float deathShrinkRatio; // range 0-1 inclusive

    // Irrelevant to rewind, all IM
    public CameraController sceneCamera;
    public WinFlag flag;
    public float maxFlightSpeed;
    private float flightVel;

    private UserInput input;
    public float axisBounds;
    // These vars are used to act as a key-down for stick controls
    private bool leftHorizontalAxisDown;
    private bool rightHorizontalAxisDown;
    private float accerlation;

    void Start()
    {
        InitPhysics();
        InitMovement();
        InitInput();
        ResetDown();
    }

    /*
     * InitPhysics - sets up rigidbody and collider for player
     */
    private void InitPhysics()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 3f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; //Prevents jittery camera

        col = GetComponent<BoxCollider2D>();
    }

    /*
     * InitMovement - sets up starting point for player variables 
     */
    private void InitMovement()
    {
        isExitingRewind = false;

        acceleration = 0.1f;
        velocityX = 0f;
        velocityY = 0f;
        IsMovingRight = true;

        isGrounded = false;
        jumps = 0;

        wallJumps = 0;
        canWallJump = false;

        flightVel = 0.1f;

        leftHorizontalAxisDown = true;
        rightHorizontalAxisDown = true;
    }

    private void InitInput()
    {
        input = new UserInput(axisBounds);
    }

    private void ResetDown()
    {
        isEitherDown = true;
        isLeftDown = true;
        isRightDown = true;
        isLeftUp = false;
        isRightUp = false;
    }

    /* 
     * Update - Handles input for player
     */
    void Update()
    {
        /* Disabling death for now, since it can get in the way of development
        if (deathHandler.IsAlive)
        {

        }
        */

        CheckPlatformCollision();

        CheckJump();

        VelocityUpdate();
    }

    /*
     * CheckJump - confirms the player both inputs, and is able to jump
     */

    private void CheckJump()
    {
        if (input.JumpDown() && jumps > 0)
        {
            Jump();
            jumps--;
        }
        else if (input.JumpDown() && wallJumps > 0 && canWallJump)
        {
            Jump();
            wallJumps--;
        }
    }

    /*
     * Jump - player moves like a normal platformer jump
     */
    private void Jump()
    {
        rb.velocity = Vector2.zero; // Prevent wall jumping abuse
        rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
        isGrounded = false;
    }

    /*
     * MoveDirection - moves the player, or stops it with air resistance/friction
     */
    private void VelocityUpdate()
    {
        if (isExitingRewind)
        {
            OnExitRewind();
        }
        else
        {
            velocityY = Mathf.Clamp(rb.velocity.y, terminalVelocity, float.PositiveInfinity);
        }

        if (input.EitherDir())
        {
            OnEitherDown();
            Accelerate(IsMovingRight);

            transform.localScale = new Vector3(boolToScalar(IsMovingRight) * 1, 1, 1);
        }
        else
        {
            StopMoving();
            ResetDown();
        }

        rb.velocity = new Vector2(velocityX, velocityY);
    }

    private void OnExitRewind()
    {
        isExitingRewind = false;
    }

    /*
     * Accelerate - sets velocityX to an increase in the current direction 
     */
    private void Accelerate(bool direction)
    {
        velocityX = Mathf.Clamp(velocityX + acceleration * boolToScalar(direction), -maxMoveSpeed, maxMoveSpeed);
    }

    private void OnEitherDown()
    {
        if (isEitherDown)
        {
            isEitherDown = false;

            CheckFirstInputDown(input.Left(), ref isLeftDown, false);
            CheckFirstInputDown(input.Right(), ref isRightDown, true);
        }
        else
        {
            CheckNextInput(input.Left(), ref isLeftDown, ref isLeftUp, ref isRightUp, false);
            CheckNextInput(input.Right(), ref isRightDown, ref isRightUp, ref isLeftUp, true);
        }
    }

    private void CheckFirstInputDown(bool inputCase, ref bool downVar, bool direction)
    {
        if (inputCase)
        {
            if (IsMovingRight == direction)
            {
                //1Normal Start, Other Up
            }
            else
            {
                //2Turn Around, Other Up
            }

            downVar = false;
            IsMovingRight = direction;
        }
    }

    private void CheckNextInput(bool inputCase, ref bool downVar, ref bool sameUpVar, ref bool otherUpVar, bool direction)
    {
        if (inputCase)
        {
            CheckNextInputChange(ref downVar, ref otherUpVar, direction);
        }
        else
        {
            if (!downVar)
            {
                sameUpVar = true;
            }

            downVar = true;
        }
    }

    private void CheckNextInputChange(ref bool downVar, ref bool otherUpVar, bool direction)
    {
        if (downVar)
        {
            downVar = false;
            IsMovingRight = direction;
            //3Turn Around, when other is already down
        }


        if (otherUpVar)
        {
            if (IsMovingRight != direction)
            {
                IsMovingRight = direction;
                //4Turn around, when other input was let go of
            }

            otherUpVar = false;
        }
    }

    /*
     * InitialMovementSet - Sets starting values for player
     * Params:
     *  - bool direction: true for right, false for left
     */
    private void InitialMovementSet(bool direction, float startingMoveSpeed, float newAcc)
    {
        velocityX = boolToScalar(direction) * startingMoveSpeed;
        acceleration = newAcc;
    }

    /*
     * StopMoving - applies ground friction and air resistance 
     */
    private void StopMoving()
    {
        float accScale = 0.5f; // Air resistance

        if (isGrounded)
        {
            accScale = 3f; // Grounded friction
        }

        bool direction = velocityX > 0;
        velocityX = RoundToZero(velocityX - boolToScalar(direction) * acceleration * accScale);
    }

    /*
     * RoundToZero - prevents player from oscillating back and forth
     * Params:
     *  - float num: the number to be rounded down
     * Returns: float number that is either itself of zero, if near zero
     */
    private float RoundToZero(float num)
    {
        if (velocityX < acceleration * 2 && velocityX > -acceleration * 2) // Needs to be twice to fix an edge case, player was still moving a little
        {
            return 0f;
        }

        return num;
    }

    /*
     * OnCollisionEnter2D - manages many player collision
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        if ((col.gameObject.GetComponent<Enemy>() != null ||
             col.gameObject.GetComponent<EnemyFlying>() != null ||
             col.gameObject.name == "DeathZone"
            ) 
            && deathHandler.IsAlive) // Can only die once, or states will break
        {
            deathHandler.OnDeath();
            AnimateDeath();
         }
    }

    private void CheckPlatformCollision()
    {
        // Vertical velocity check is so that when the player is leaving the ground,
        // It can't gain a double jump
        if (RaycastCollision(-Vector2.up, 0.02f, true) && velocityX < 10f)
        {
            jumps = maxJumps;
            wallJumps = maxWallJumps;
            isGrounded = true;
        }

        if (RaycastCollision(Vector2.left, 0.2f, false) || RaycastCollision(-Vector2.left, 0.2f, false))
        {
            canWallJump = true;
        }
        else
        {
            canWallJump = false;
        }
    }

    /*
     * RaycastCollision - check if a there is a nearby object, based on player side
     * Params:
     *  - Vector2 direction: where to point the ray
     *  - float lengthFromEdge: distance for the ray to extend past the player's own hitbox
     *  - bool isUp: whether the direction is vertical or horizontal
     *  Returns: bool true if the ray hits something
     */
    private bool RaycastCollision(Vector2 direction, float lengthFromEdge, bool isUp)
    {
        // GetMask returns a binary value, 
        // tilde inverts it so this means any layer but Player's
        LayerMask notPlayerLayer =~ LayerMask.GetMask("Player");

        float hitboxMargins = ChooseFrom<float>(isUp, col.size.y / 2, col.size.x / 2);
        float offsetY;
        float offsetX;

        offsetY = ChooseFrom<float>(isUp, 0f, col.size.y / 2);
        offsetX = ChooseFrom<float>(isUp, col.size.x / 2, 0f);

        Vector2 leftRayPos = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);
        Vector2 rightRayPos = new Vector2(transform.position.x - offsetX, transform.position.y - offsetY);

        RaycastHit2D hitLeft = Physics2D.Raycast(leftRayPos, direction, lengthFromEdge + hitboxMargins, notPlayerLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(rightRayPos, direction, lengthFromEdge + hitboxMargins, notPlayerLayer);

        return hitLeft.collider != null || hitRight.collider != null;
    }

    /*
     * AnimateDeath - triggered when player dies, to a some small animations
     */
    public void AnimateDeath()
    {
        rb.freezeRotation = false;
        rb.rotation = 90f; // 90 degrees is on the side
        transform.localScale = new Vector3(deathShrinkRatio, 1, 1);
    }

    /*
     * Fly - animation for player when they beat the level
     */
    public void Fly()
    {
        // Has to set to flag position or else the z value won't change
        // Not sure why, I believe Unity skips only z changes in 2D mode to save resources
        transform.position = new Vector3(flag.transform.position.x, flag.transform.position.y, 20); // Move Behind the flag

        StartCoroutine(AnimateFlight());
    }

    /*
     * AnimateFlight - moves the player upwards at an accelerating rate
     * Returns IEnumerator: meaning this method is async
     */
    private IEnumerator AnimateFlight()
    {
        while (true)
        {
            rb.velocity = new Vector2(rb.velocity.x, flightVel);

            Vector3 currentPos = transform.position;
            currentPos.z = 10; //Makes sure flag is in front of player
            flag.transform.position = currentPos;

            flag.transform.Rotate(flag.transform.rotation.x, flag.transform.rotation.y, flightVel / 10);

            flightVel = Mathf.Clamp(flightVel + 0.7f, 0, maxFlightSpeed);

            yield return null;
        }
    }

    /// Serial Methods, see Serial Namespace 
    public ISerialDataStore GetCurrentState()
    {
        return new SavePlayer(  acceleration, velocityX,
                                velocityY, isEitherDown,
                                isLeftDown, isRightDown,
                                IsMovingRight, isGrounded, 
                                jumps, wallJumps,
                                canWallJump, transform.position.x, 
                                transform.position.y, leftHorizontalAxisDown, 
                                rightHorizontalAxisDown, rb.freezeRotation,
                                rb.rotation, transform.localScale.x
                             );
    }

    public void SetState(ISerialDataStore state)
    {
        SavePlayer past = (SavePlayer) state;

        rb.velocity = Vector2.zero; // Needed because velocity isn't conserved
        acceleration = past.acceleration;
        velocityX = past.velocityX;
        velocityY = past.velocityY;
        isExitingRewind = true;

        isEitherDown = past.isEitherDown;
        isLeftDown = past.isLeftDown;
        isRightDown = past.isRightDown;

        IsMovingRight = past.IsMovingRight;
        isGrounded = past.grounded;
        jumps = past.jumps;
        wallJumps = past.wallJumps;
        canWallJump = past.canWallJump;

        transform.position = new Vector3(past.positionX, past.positionY, transform.position.z);

        rightHorizontalAxisDown = past.rightHorizontalAxisDown;
        leftHorizontalAxisDown = past.leftHorizontalAxisDown;

        rb.freezeRotation = past.freezeRotation;
        rb.rotation = past.rotation;

        transform.localScale = new Vector3(past.localXScale, 1, 1);
    }
}

internal class SavePlayer : ISerialDataStore
{
    public float acceleration { get; private set; }
    public float velocityX { get; private set; }
    public float velocityY { get; private set; }

    public bool IsMovingRight { get; private set; }

    public bool isEitherDown { get; private set; }
    public bool isLeftDown { get; private set; }
    public bool isRightDown { get; private set; }

    public bool grounded { get; private set; }
    public int jumps { get; private set; }
    public int wallJumps { get; private set; }
    public bool canWallJump { get; private set; }

    public float positionX { get; private set; }
    public float positionY { get; private set; }

    public bool leftHorizontalAxisDown { get; private set; }
    public bool rightHorizontalAxisDown { get; private set; }

    public bool isDead { get; private set; }

    public bool freezeRotation { get; private set; }
    public float rotation { get; private set; }

    public float localXScale { get; private set; }

    public SavePlayer(  float acc, float velX, 
                        float velY, bool movingR,
                        bool isED, bool isLD,
                        bool isRD, bool g,
                        int j, int wallJ,
                        bool canW, float posX,
                        float posY, bool leftDown,
                        bool rightDown, bool fRot,
                        float rot, float lxScale
                     )
    {
        acceleration = acc;
        velocityX = velX;
        velocityY = velY;

        IsMovingRight = movingR;

        isEitherDown = isED;
        isLeftDown = isLD;
        isRightDown = isRD;

        grounded = g;
        jumps = j;
        wallJumps = wallJ;
        canWallJump = canW;

        positionX = posX;
        positionY = posY;

        leftHorizontalAxisDown = leftDown;
        rightHorizontalAxisDown = rightHorizontalAxisDown;

        freezeRotation = fRot;
        rotation = rot;

        localXScale = lxScale;
    }
}