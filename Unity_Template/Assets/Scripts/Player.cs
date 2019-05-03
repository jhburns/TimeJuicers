using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Serial;

public class Player : MonoBehaviour, ISerializable
{
    public float jumpHeight; //IM = suppose to be immutable
    public float moveSpeed; //IM
    private float velHorz;  // Mutable, but not tracked
    private const float acceleration = 0.1f; //IM
    public bool MovingRight { get; private set; }

    private Rigidbody2D rb; // Mutable, but not tracked

    private bool grounded;
    private int jumps;
    private const int maxJumps = 1; //IM

    public float axisBounds;

    private bool leftHorizontalAxisDown; // These vars are used to act as a keydown for stick controls
    private bool rightHorizontalAxisDown;

    public GlobalUI deathHandler; //IM

    public CameraController sceneCamera;
    public WinFlag flag;
    public float maxFlightSpeed;
    private float flightVel;

    /* Start - is called before the first frame update
     * Initializes variables
     */
    void Start()
    {
        InitRigid();
        InitPlayer();
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
    }

    /*
     * InitPlayer - sets up starting point for player variables 
     */
    private void InitPlayer()
    {
        velHorz = 0f;
        grounded = false;
        MovingRight = true;

        leftHorizontalAxisDown = true;
        rightHorizontalAxisDown = true;

        flightVel = 0.1f;
    }

    /* Update is called once per frame,
     * Handles input for player
     */
    void Update()
    {
        // bascially everywhere: https://docs.unity3d.com/ScriptReference/Rigidbody2D.html

        if (deathHandler.IsAlive)
        {
            Jump();

            InitialVelocitySet();

            MoveDirection();

        }
    }

    /*
     * Jump - player moves like a normal platformer jump
     */
    private void Jump()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || 
             Input.GetKeyDown(KeyCode.W) ||
             Input.GetKeyDown(KeyCode.UpArrow) ||
             Input.GetKeyDown(KeyCode.Joystick1Button0) || // A button on xbox 360 controller
             Input.GetKeyDown(KeyCode.Joystick1Button2) || // X button on xbox 360 controller
             Input.GetAxisRaw("Vertical") > axisBounds
            )
            && jumps > 0)
        {
            rb.velocity = Vector2.zero; // To allow for wall jumping
            rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
            grounded = false;
            jumps--;
        }
    }

    /*
     * InitialVelocitySet - sets initial values to move move player left/right
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
        if (Input.GetKey(KeyCode.RightArrow) || 
            Input.GetKey(KeyCode.D) ||
            Input.GetAxisRaw("Horizontal") > axisBounds
           )
        { 
            AccelerateDir(1);
            rb.velocity = new Vector2(velHorz, rb.velocity.y);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) ||
                 Input.GetKey(KeyCode.A) ||
                 Input.GetAxisRaw("Horizontal") < -axisBounds
                )
        {
            AccelerateDir(-1);
            rb.velocity = new Vector2(velHorz, rb.velocity.y);
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
        float accScale = 0.5f; // Air resistence

        if (grounded)
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

        rb.velocity = new Vector2(velHorz, rb.velocity.y);
    }

    /*
     * RoundToZero - prevents player from ossilating back and forth
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
     * OnCollisionEnter2D - manages many player collisions
     * Params:
     *  - Collision2D col: the other object being collided with
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        // Make sure to check if the object has a material first
        if (col.collider.sharedMaterial != null)
        {
            if (col.collider.sharedMaterial.name == "GroundMaterial")
            {
                grounded = true;
                jumps = maxJumps;
            }

            if (col.collider.sharedMaterial.name == "BouncyMaterial") {
                jumps = maxJumps;
                rb.velocity = Vector2.zero; // Prevents unlimited jump height
                rb.AddForce(new Vector2(0, jumpHeight * 0.9f), ForceMode2D.Impulse);
            }

        }

        if ((col.gameObject.GetComponent<Enemy>() != null ||
             col.gameObject.name == "DeathZone"
            ) 
            && deathHandler.IsAlive) // Can only die once, or states will break
        {
            deathHandler.OnDeath();
         }
    }

    /*
     * Fly - animation for player when they beat the level
     */
    public void Fly()
    {
        sceneCamera.ExactMode();
        Destroy(flag.GetComponent<BoxCollider2D>());

        // Has to set to flag postion or else the z value won't change
        // Not sure why, I believe Unity skips only z changes in 2D mode to save resources
        transform.position = new Vector3(flag.transform.position.x, flag.transform.position.y, 20); // Move Behind the flag

        StartCoroutine(AnimateFlight());
    }

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
        return new SavePlayer(  MovingRight, grounded, 
                                jumps, transform.position.x, transform.position.y,
                                leftHorizontalAxisDown, rightHorizontalAxisDown
                             );
    }

    public void SetState(ISerialDataStore state)
    {
        SavePlayer past = (SavePlayer) state;

        velHorz = 0f;

        MovingRight = past.movingRight;
        grounded = past.grounded;
        jumps = past.jumps;

        transform.position = new Vector3(past.positionX, past.positionY, transform.position.z);
        rb.velocity = Vector2.zero; // Needed becasue velocity isn't conserved

        rightHorizontalAxisDown = past.rightHorizontalAxisDown;
        leftHorizontalAxisDown = past.leftHorizontalAxisDown;

        if (past.movingRight)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
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

    public SavePlayer(bool movingR, bool g,
                        int j, float posX,
                        float posY, bool leftDown,
                        bool rightDown
                     )
    {
        movingRight = movingR;

        grounded = g;
        jumps = j;

        positionX = posX;
        positionY = posY;

        leftHorizontalAxisDown = leftDown;
        rightHorizontalAxisDown = rightHorizontalAxisDown;
    }
}