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
    public bool isHeadingUp;

    /* 
     * Start - is called before the first frame update,
     * Initalizes many global variables 
     */
    void Start()
    {
        InitRigid();
        InitMovement();

        isAlive = true;
        timeLeftInPlay = 0f;
    }

    /*
     * InitRigid - starts the physics on the enemy
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
    }

    /* 
     * Update - is called once per frame,
     * Moves enemy based on current direction,
     * Stores enemy after timeout 
     */
    void Update()
    {
        float newPositionY = (speed * Time.deltaTime);

        if (transform.position.y + newPositionY > storageY + maxRange)
        {
            isHeadingUp = false;
        }

        // Enemy can only move up from orgin, range only reverses from height of flight
        if (transform.position.y - newPositionY  < storageY)
        {
            isHeadingUp = true;
        }

        if (!isHeadingUp)
        {
            newPositionY *= -1f;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y + newPositionY, transform.position.z);


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
        // Just in case two enemies are on top of eachother vertically, also stores to a starting y
        // Symmetric storageY around 0 so that a high enemy isn't stored above the death boundry
        transform.position = new Vector2(storageX, -25f - storageY); 
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
    }

    /*
     * OnCollisionEnter2D - handles physics collisions
     * Params:
     *  - Collision2D col: the other object being collided with
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "bullets")
        {
            isAlive = false;

            Bullet bul = col.gameObject.GetComponent<Bullet>();
            int direction = 1;

            rb.AddForce(new Vector2(3f * direction, 9f), ForceMode2D.Impulse);
            rb.AddTorque(100f * direction, ForceMode2D.Force);

            timeLeftInPlay = 0.15f;
        }

        if (col.gameObject.name == "DeathZone")
        {
            Store();
        }



    }

    /// Serial Methods, see Serial Namespace 
    public ISerialDataStore GetCurrentState()
    {
        return new SaveFlyingEnemy( isAlive, timeLeftInPlay,
                                    transform.position.x, transform.position.y,
                                    rb.isKinematic, rb.rotation
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

    public SaveFlyingEnemy( bool alive, float time,
                            float posX, float posY,
                            bool kin, float rot
                          )
    {
        isAlive = alive;
        timeLeftInPlay = time;
        positionX = posX;
        positionY = posY;
        isKinematic = kin;
        rotation = rot;
    }

}