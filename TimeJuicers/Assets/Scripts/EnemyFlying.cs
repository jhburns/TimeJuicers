using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serial;


public class EnemyFlying : MonoBehaviour, ISerializable
{
    Rigidbody2D rb;

    public float speed; //IM

    public bool isAlive;

    private float storageX; //IM
    private float storageY; //IM

    private float timeLeftInPlay;

    public float maxRange;
    private bool isHeadingUp;

    private bool isMoving; // Prevents the enemy from having jittery movement after killing

    // Enemy starts flying from the top of their cycle if true
    public bool startOnTop; //IM

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
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; //Prevents jittery camera
    }

    /*
     * InitMovement - stores the starting x position for later, and sets up other vars
     */
    private void InitMovement()
    {
        storageX = transform.position.x;
        storageY = transform.position.y;
        isHeadingUp = true;
        isMoving = true;

        if (startOnTop)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + maxRange, transform.position.z);
        }
    }

    /* 
     * Update - Moves enemy based on current direction,
     * Stores enemy after timeout when killed
     */
    void Update()
    {

        if (timeLeftInPlay < 0 && !isAlive)
        {
            Store();
        }
        else if (!isAlive)
        {
            timeLeftInPlay -= Time.deltaTime;
        }
        else if (isMoving)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + GetVelocity(), transform.position.z);
        }
    }

    /*
     * Store - moves enemy to below stage, and stops physics 
     */
    private void Store()
    {
        // Just in case two enemies are on top of each other vertically, also stores to a starting y
        // Symmetric storage around y-axis so that a high enemy isn't stored above the death boundary
        transform.position = new Vector2(storageX, -25f - storageY); 
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
    }

    /*
     * GetVelocity - determines the velocity of this enemy
     * Returns float: the velocity 
     */
    private float GetVelocity()
    {
        float newVelocityY = speed * Time.deltaTime;

        if (transform.position.y + newVelocityY > storageY + maxRange)
        {
            isHeadingUp = false;
        }


        // Enemy can only move up from origin, isHeadingUp only reverses starting location not destination 
        if (transform.position.y - newVelocityY < storageY)
        {
            isHeadingUp = true;
        }

        if (!isHeadingUp)
        {
            newVelocityY *= -1f;
        }

        return newVelocityY;
    }

    /*
     * OnCollisionEnter2D - handles physics collisions
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "bullets")
        {

            Bullet bul = col.gameObject.GetComponent<Bullet>();
            int direction = 1;

            isAlive = false;
            timeLeftInPlay = 0.15f;

            rb.AddForce(new Vector2(3f * direction, 9f), ForceMode2D.Impulse);
            rb.AddTorque(100f * direction, ForceMode2D.Force);
        }

        if (col.gameObject.name == "DeathZone")
        {
            Store();
        }
        else if (col.gameObject.GetComponent<Enemy>() != null ||
                 col.gameObject.GetComponent<EnemyFlying>() != null)
        {
            isAlive = false;

            isMoving = false;
            timeLeftInPlay = 0.15f;
        }
        else
        {
            isMoving = false;
        }

    }

    ////////////////////////////////////////////////////
    // Serial implementation, see Libraries/Serial.cs //
    ////////////////////////////////////////////////////
    public ISerialDataStore GetCurrentState()
    {
        return new SaveFlyingEnemy( isAlive, timeLeftInPlay,
                                    transform.position.x, transform.position.y,
                                    rb.isKinematic, rb.rotation,
                                    isHeadingUp, isMoving
                                  );
    }

    public void SetState(ISerialDataStore state)
    {
        SaveFlyingEnemy past = (SaveFlyingEnemy)state;

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

        isHeadingUp = past.isHeadingUp;
        isMoving = past.isMoving;

    }
}

internal class SaveFlyingEnemy : ISerialDataStore
{

    public bool isAlive { get; private set; }

    public float timeLeftInPlay { get; private set; }

    public float positionX { get; private set; }
    public float positionY { get; private set; }

    public bool isKinematic { get; private set; }
    public float rotation { get; private set; }

    public bool isHeadingUp { get; private set; }
    public bool isMoving { get; private set; }

    public SaveFlyingEnemy( bool alive, float time,
                            float posX, float posY,
                            bool kin, float rot,
                            bool up, bool move
                          )
    {
        isAlive = alive;
        timeLeftInPlay = time;
        positionX = posX;
        positionY = posY;
        isKinematic = kin;
        rotation = rot;
        isHeadingUp = up;
        isMoving = move;
    }

}