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
    public float moveSpeed; //IM
    private float velHorz;  // Mutable, but not tracked
    private float velVert;
    private const float acceleration = 0.1f; //IM
    public bool MovingRight { get; private set; }

    private Rigidbody2D rb; // Mutable, but not tracked
    private BoxCollider2D col; //IM

    private bool isGrounded;
    private int jumps;
    private const int maxJumps = 1; //IM

    public float axisBounds;

    private bool leftHorizontalAxisDown; // These vars are used to act as a key-down for stick controls
    private bool rightHorizontalAxisDown;

    public GlobalUI deathHandler; //IM

    public CameraController sceneCamera;
    public WinFlag flag;
    public float maxFlightSpeed;
    private float flightVel;

    public float deathShrinkRatio; // range 0-1 inclusive

    private UserInput input;

    public float terminalVelocity; // negative number, range < 0 

    void Start()
    {
        InitRigid();
        InitPlayer();
        InitInput();
    }

    /*
     * InitRigid - starts physics on the player
     */
    private void InitRigid()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 3f;
        jumps = 0;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; //Prevents jittery camera

        col = GetComponent<BoxCollider2D>();
    }

    private void InitInput()
    {
        input = new UserInput(axisBounds);
    }

    /*
     * InitPlayer - sets up starting point for player variables 
     */
    private void InitPlayer()
    {
        velHorz = 0f;
        velVert = 0f;
        isGrounded = false;
        MovingRight = true;

        leftHorizontalAxisDown = true;
        rightHorizontalAxisDown = true;

        flightVel = 0.1f;
    }

    /* 
     * Update - Handles input for player
     */
    void Update()
    {
        /* Disabling death for now, since it can get in the way of development
        if (deathHandler.IsAlive)
        {
            Jump();

            InitialVelocitySet();

            MoveDirection();
        }
        */

        CheckPlatformCollision();

        Jump();

        InitialVelocitySet();

        MoveDirection();
    }

    /*
     * Jump - player moves like a normal platformer jump
     */
    private void Jump()
    {
        if (input.JumpDown() && jumps > 0)
        {
            rb.velocity = Vector2.zero; // To allow for wall jumping
            rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
            isGrounded = false;
            jumps--;
        }
    }

    /*
     * InitialVelocitySet - sets initial values to move player left/right
     */
    private void InitialVelocitySet()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.D) 
           )
        {
            SetRightInitialVel();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || 
            Input.GetKeyDown(KeyCode.A)
           )
        {
            SetLeftInitialVel();
        }

        ControllerInitVelSet();
    }

    /*
     * ControllerInitVelSet - moves player left/right based on controller input,
     * Works like ghetto GetKeyDown for axis inputs
     */
    private void ControllerInitVelSet()
    {
        // https://www.reddit.com/r/Unity3D/comments/61hjiy/can_you_get_axis_input_like_getbuttondown/
        if (Input.GetAxisRaw("Horizontal") > axisBounds)
        {
            if (!rightHorizontalAxisDown)
            {
                SetRightInitialVel();
            }

            rightHorizontalAxisDown = true;
        }
        else
        {
            rightHorizontalAxisDown = false;
        }

        if (Input.GetAxisRaw("Horizontal") < -axisBounds)
        {
            if (!leftHorizontalAxisDown)
            {
                SetLeftInitialVel();
            }

            leftHorizontalAxisDown = true;
        }
        else
        {
            leftHorizontalAxisDown = false;
        }
    }

    /*
     * SetRightInitialVel - sets move right
     */
    private void SetRightInitialVel()
    {
        velHorz = moveSpeed - 2.0f;
        MovingRight = true;
    }

    /*
     * SetLeftInitialVel - sets move left 
     */
    private void SetLeftInitialVel()
    {
        velHorz = -(moveSpeed - 2.0f);
        MovingRight = false;
    }

    /*
     * MoveDirection - moves the player, or stops it with air resistance/friction
     */
    private void MoveDirection()
    {
        velVert = Mathf.Clamp(rb.velocity.y, terminalVelocity, float.PositiveInfinity);

        if (Input.GetKey(KeyCode.RightArrow) || 
            Input.GetKey(KeyCode.D) ||
            Input.GetAxisRaw("Horizontal") > axisBounds
           )
        { 
            AccelerateDir(1);
            rb.velocity = new Vector2(velHorz, velVert);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) ||
                 Input.GetKey(KeyCode.A) ||
                 Input.GetAxisRaw("Horizontal") < -axisBounds
                )
        {
            AccelerateDir(-1);
            rb.velocity = new Vector2(velHorz, velVert);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            StopMoving();
        }
    }

    /*
     * AccelerateDir - sets velHorz to an increase in the current direction 
     */
    private void AccelerateDir(int direction)
    {
        velHorz = Mathf.Clamp(velHorz + acceleration * direction, -moveSpeed, moveSpeed);
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

        if (velHorz > 0)
        {
            velHorz = RoundToZero(velHorz - acceleration * accScale);
        }
        else
        {
            velHorz = RoundToZero(velHorz + acceleration * accScale);
        }

        rb.velocity = new Vector2(velHorz, velVert);
    }

    /*
     * RoundToZero - prevents player from oscillating back and forth
     * Params:
     *  - float num: the number to be rounded down
     * Returns: float number that is either itself of zero, if near zero
     */
    private float RoundToZero(float num)
    {
        if (velHorz < acceleration * 2 && velHorz > -acceleration * 2) // Needs to be twice to fix an edge case, player was still moving a little
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
        if (RaycastCollision(-Vector2.up, 0.02f, true) && velVert < 10f)
        {
            jumps = maxJumps;
            isGrounded = true;
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

        if (isUp)
        {
            offsetY = 0f;
            offsetX = col.size.x / 2;
        }
        else
        {
            offsetY = col.size.y / 2;
            offsetX = 0f;
        }

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
        sceneCamera.ExactMode();
        Destroy(flag.GetComponent<BoxCollider2D>());

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
        return new SavePlayer(  MovingRight, isGrounded, 
                                jumps, transform.position.x, 
                                transform.position.y, leftHorizontalAxisDown, 
                                rightHorizontalAxisDown, rb.freezeRotation,
                                rb.rotation, transform.localScale.x
                             );
    }

    public void SetState(ISerialDataStore state)
    {
        SavePlayer past = (SavePlayer) state;

        velHorz = 0f;

        MovingRight = past.movingRight;
        isGrounded = past.grounded;
        jumps = past.jumps;

        transform.position = new Vector3(past.positionX, past.positionY, transform.position.z);
        rb.velocity = Vector2.zero; // Needed because velocity isn't conserved

        rightHorizontalAxisDown = past.rightHorizontalAxisDown;
        leftHorizontalAxisDown = past.leftHorizontalAxisDown;

        rb.freezeRotation = past.freezeRotation;
        rb.rotation = past.rotation;

        transform.localScale = new Vector3(past.localXScale, 1, 1);
    }
}

internal class SavePlayer : ISerialDataStore
{
    public bool movingRight { get; private set; }

    public bool grounded { get; private set; }
    public int jumps { get; private set; }

    public float positionX { get; private set; }
    public float positionY { get; private set; }

    public bool leftHorizontalAxisDown { get; private set; }
    public bool rightHorizontalAxisDown { get; private set; }

    public bool isDead { get; private set; }

    public bool freezeRotation { get; private set; }
    public float rotation { get; private set; }

    public float localXScale { get; private set; }

    public SavePlayer(bool movingR, bool g,
                        int j, float posX,
                        float posY, bool leftDown,
                        bool rightDown, bool fRot,
                        float rot, float lxScale
                     )
    {
        movingRight = movingR;

        grounded = g;
        jumps = j;

        positionX = posX;
        positionY = posY;

        leftHorizontalAxisDown = leftDown;
        rightHorizontalAxisDown = rightHorizontalAxisDown;

        freezeRotation = fRot;
        rotation = rot;

        localXScale = lxScale;
    }
}