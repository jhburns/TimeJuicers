using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serial;


public class Enemy : MonoBehaviour, ISerializable
{
    Rigidbody2D rb;

    public bool isMovingRight;
    private bool isGrounded;

    public float speed; //IM

    public bool isAlive;

    private float storageX;

    private float timeLeftInPlay;

    public float spinRate; //IM

    void Start()
    {
        InitRigid();
        InitMovement();

        isAlive = true;
        timeLeftInPlay = 0f;
    }

    /*
     * InitRigid - starts the physics engine
     */
    private void InitRigid()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; //Prevents jittery camera

        rb.gravityScale = 3f;
    }

    /*
     * InitMovement - stores the starting x position for later, and sets up other vars
     */
    private void InitMovement()
    {
        storageX = transform.position.x;
        isGrounded = false;
    }

    /* 
     * Update - Moves enemy based on current direction,
     * Stores enemy after timeout when killed
     */
    void Update()
    {
        if (isMovingRight && isGrounded && isAlive)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            rb.MoveRotation( -(Time.deltaTime * spinRate) + rb.rotation);
        }
        else if (isGrounded && isAlive)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            rb.MoveRotation((Time.deltaTime * spinRate) + rb.rotation);
        }

        if (timeLeftInPlay < 0 && !isAlive)
        {
            Store();
        }
        else if (!isAlive)
        {
            timeLeftInPlay -= Time.deltaTime;
        }

    }

    /*
     * Store - moves enemy to below stage, and stops physics 
     */
    private void Store()
    {
        transform.position = new Vector2(storageX, -25f);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
    }

    /*
     * OnCollisionEnter2D - can collides with multiple other objects,
     * Importantly turns around when colliding with non-ground blocks
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "bullets")
        {
            isAlive = false;

            Bullet bul = col.gameObject.GetComponent<Bullet>();
            int direction = 1;

            if (!bul.IsMovingRight)
            {
                direction = -1;
            }

            rb.AddForce(new Vector2(3f * direction, 9f), ForceMode2D.Impulse);
            rb.AddTorque(20f * direction, ForceMode2D.Force);

            timeLeftInPlay = 0.30f;
        }

        if (col.gameObject.name == "DeathZone")
        {
            Store();
        }

        if (col.collider.sharedMaterial != null && col.collider.sharedMaterial.name == "GroundMaterial")
        {
            isGrounded = true;
        }
        else
        {
            isMovingRight = !isMovingRight;
        }

    }

    /*
     * OnTriggerEnter2D - collisions, but only used for non-physical objects that the enemy can pass through
     * Turns this around on invisible enemy boundary blocks
     */
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Collider2D>().name != "GroundMaterial")
        {
            isMovingRight = !isMovingRight;
        }
    }

    ////////////////////////////////////////////////////
    // Serial implementation, see Libraries/Serial.cs //
    ////////////////////////////////////////////////////
    public ISerialDataStore GetCurrentState()
    {
        return new SaveEnemy(   isMovingRight, isGrounded,
                                isAlive, timeLeftInPlay,
                                transform.position.x, transform.position.y,
                                rb.isKinematic, rb.rotation
                            );
    }

    public void SetState(ISerialDataStore state)
    {
        SaveEnemy past = (SaveEnemy) state;

        isMovingRight = past.isMovingRight;
        isGrounded = past.isGrounded;
        isAlive = past.isAlive;
        timeLeftInPlay = past.timeLeftInPlay;

        transform.position = new Vector3(past.positionX, past.positionY, 0); // Don't use rigid body to update or it will be jitters
        rb.velocity = Vector2.zero;

        rb.isKinematic = past.isKinematic;
        rb.angularVelocity = 0f;

        // Can't use rb.rotation due to it causing jittery movement
        // Have to convert normal 2d rotation to 3d to use transform.rotation
        // The 2D rotation value is actually along the Z-Axis
        Vector3 currentRot = transform.rotation.eulerAngles;
        Vector3 newRot = new Vector3(currentRot.x, currentRot.y, past.rotation);
        transform.rotation = Quaternion.Euler(newRot);

    }
}

internal class SaveEnemy : ISerialDataStore
{
    public bool isMovingRight { get; private set; }
    public bool isGrounded { get; private set; }

    public bool isAlive { get; private set; }

    public float timeLeftInPlay { get; private set; }

    public float positionX { get; private set; }
    public float positionY { get; private set; }

    public bool isKinematic { get; private set; }
    public float rotation { get; private set; }

    public SaveEnemy(   bool right, bool ground,
                        bool alive, float time,
                        float posX, float posY,
                        bool kin, float rot
                    )
    {
        isMovingRight = right;
        isGrounded = ground;
        isAlive = alive;
        timeLeftInPlay = time;
        positionX = posX;
        positionY = posY;
        isKinematic = kin;
        rotation = rot;
    }

}