using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serial;


public class Bullet : MonoBehaviour, ISerializable
{
    public float velX; //IM
    private const float velY = 0f;
    private Rigidbody2D rb;

    private float storageX; //IM
    private float storageY; //IM

    private bool isInPlay;
    private float timeLeftInPLay;
    public float maxTime; //IM

    public bool IsMovingRight { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; //Prevents jittery camera
        rb.freezeRotation = true;

        StoreStartingPos();

        isInPlay = false;
    }

    /*
     * StoreStartingPos - initializes storage vars
     * So this bullet knows where to return to after timing out
     */
    private void StoreStartingPos()
    {
        storageX = transform.position.x;
        storageY = transform.position.y;
    }

    void Update()
    {
        HasTimeEnded();
    }

    /*
     * InPlay - when the bullet is fired by the player
     * Params:
     *  - bool goRight: direction the bullet is heading in, true when heading right
     */
    public void InPlay(bool goRight, Vector2 location)
    {
        isInPlay = true;
        timeLeftInPLay = maxTime;
        transform.position = location;

        if (goRight)
        {
            rb.velocity = new Vector2(velX, velY);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            rb.velocity = new Vector2(-velX, velY);
            transform.localScale = new Vector3(-1, 1, 1);
        }

        IsMovingRight = goRight;
    }

    /*
     * HasTimeEnded - stores the bullets after a period of time, so it doesn't continue forever
     */
    private void HasTimeEnded()
    {
        if (timeLeftInPLay < 0)
        {
            Store();
        }
        else
        {
            timeLeftInPLay -= Time.deltaTime;
        }
    }

    /*
     * OnCollisionEnter2D - prevents bullet from traveling through enemies, walls
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        Store();
    }

    /*
     * Store - moves bullet bellow stage and stops it from moving
     */
    private void Store()
    {
        transform.position = new Vector2(storageX, storageY);
        rb.velocity = new Vector2(0, 0);
    }

    ////////////////////////////////////////////////////
    // Serial implementation, see Libraries/Serial.cs //
    ////////////////////////////////////////////////////
    public ISerialDataStore GetCurrentState()
    {
        return new SaveBullet(  isInPlay, timeLeftInPLay,
                                rb.velocity.x, rb.velocity.y,
                                transform.position.x, transform.position.y,
                                IsMovingRight
                             );
    }

    public void SetState(ISerialDataStore state)
    {
        SaveBullet past = (SaveBullet) state;

        isInPlay = past.isInPlay;
        timeLeftInPLay = past.timeLeftInPLay;
        rb.velocity = new Vector2(past.velocityX, past.velocityY);
        transform.position = new Vector2(past.positionX, past.positionY);
        IsMovingRight = past.IsMovingRight;

        if (past.IsMovingRight)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}

internal class SaveBullet : ISerialDataStore
{
    public bool isInPlay { get; private set; }
    public float timeLeftInPLay { get; private set; }

    public float velocityX { get; private set;  }
    public float velocityY { get; private set; }

    public float positionX { get; private set; }
    public float positionY { get; private set; }

    public bool IsMovingRight { get; private set; }

    public SaveBullet(  bool play, float time,
                        float velX, float velY,
                        float posX, float posY,
                        bool right
                     )
    {
        isInPlay = play;
        timeLeftInPLay = time;
        velocityX = velX;
        velocityY = velY;
        positionX = posX;
        positionY = posY;
        IsMovingRight = right;
    }

}