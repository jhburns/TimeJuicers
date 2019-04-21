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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; //Prevents jittery camera


        StoreStartingPos();
    }

    private void StoreStartingPos()
    {
        storageX = transform.position.x;
        storageY = transform.position.y;
        isInPlay = false;
    }

    // Update is called once per frame
    void Update()
    {

        HasTimeEnded();
    }

    public void InPlay(bool IsMovingRight)
    {
        isInPlay = true;
        timeLeftInPLay = maxTime;

        if (IsMovingRight)
        {
            rb.velocity = new Vector2(velX, velY);
        }
        else
        {
            rb.velocity = new Vector2(-velX, velY);
        }
    }

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

    void OnCollisionEnter2D(Collision2D col)
    {
        Store();
    }

    private void Store()
    {
        transform.position = new Vector2(storageX, storageY);
        rb.velocity = new Vector2(0, 0);
    }

    public ISerialDataStore GetCurrentState()
    {
        return new SaveBullet(  isInPlay, timeLeftInPLay,
                                rb.velocity.x, rb.velocity.y,
                                transform.position.x, transform.position.y
                             );
    }

    public void SetState(ISerialDataStore state)
    {
        SaveBullet past = (SaveBullet) state;

        isInPlay = past.isInPlay;
        timeLeftInPLay = past.timeLeftInPLay;
        rb.velocity = new Vector2(past.velocityX, past.velocityY);
        transform.position = new Vector2(past.positionX, past.positionY);
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

    public SaveBullet(  bool play, float time,
                        float velX, float velY,
                        float posX, float posY
                     )
    {
        isInPlay = play;
        timeLeftInPLay = time;
        velocityX = velX;
        velocityY = velY;
        positionX = posX;
        positionY = posY;
    }

}