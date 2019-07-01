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

    public bool IsMovingRight { get; private set; } // Used by other objects to know where the bullet is headed

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; //Prevents jittery camera
        rb.freezeRotation = true;

        StoreStartingPos();
    }

    /*
     * StoreStartingPos - initalizes storage vars and isInPlay
     */
    private void StoreStartingPos()
    {
        storageX = transform.position.x;
        storageY = transform.position.y;
        isInPlay = false;
    }

    void Update()
    {
        HasTimeEnded();
    }

    /*
     * InPlay - 
     * Params:
     *  - bool goRight: direction the bullet is heading in, true when heading right
     */
    public void InPlay(bool goRight)
    {
        isInPlay = true;
        timeLeftInPLay = maxTime;

        if (goRight)
        {
            rb.velocity = new Vector2(velX, velY);
        }
        else
        {
            rb.velocity = new Vector2(-velX, velY);
        }

        IsMovingRight = goRight;
    }

    /*
     * HasTimeEnded - stores the bullets after a period of time, so it doesn't move forever
     */
    private void HasTimeEnded()
    {
        if (timeLeftInPLay < 0)
        {
            Store();
        } else
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

    /// Serial Methods, see Serial Namespace 
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