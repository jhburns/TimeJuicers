using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serial;


////////////// WIP

public class EnemyFlying : MonoBehaviour, ISerializable
{
    Rigidbody2D rb;

    public bool isAlive;

    private float storageX;

    private float startingY;

    private float timeLeftInPlay;

    public float velocity;

    // Start is called before the first frame update
    void Start()
    {
        InitRigid();
        InitMovement();

        isAlive = true;
        timeLeftInPlay = 0f;
    }

    private void InitRigid()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; //Prevents jittery camera
    }

    private void InitMovement()
    {
        storageX = transform.position.x;
        startingY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeftInPlay < 0 && !isAlive)
        {
            Store();
        }
        else if (!isAlive)
        {
            timeLeftInPlay -= Time.deltaTime;
        } else
        {
            Fly();
        }
    }

    private void Store()
    {
        transform.position = new Vector2(storageX, -25f);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
    }

    private void Fly()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y + velocity);
    }

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

            rb.isKinematic = false;


            rb.AddForce(new Vector2(7f * direction, 12f), ForceMode2D.Impulse);
            rb.AddTorque(50f * direction, ForceMode2D.Force);

            timeLeftInPlay = 0.30f;
        }

        if (col.gameObject.name == "DeathZone")
        {
            Store();
        }

    }

    public ISerialDataStore GetCurrentState()
    {
        return new SaveEnemyFly(isAlive, timeLeftInPlay,
                                transform.position.x, transform.position.y,
                                rb.isKinematic, rb.rotation
                            );
    }

    public void SetState(ISerialDataStore state)
    {
        SaveEnemyFly past = (SaveEnemyFly)state;


        isAlive = past.isAlive;
        timeLeftInPlay = past.timeLeftInPlay;

        transform.position = new Vector2(past.positionX, past.positionY);
        rb.velocity = Vector2.zero;

        rb.isKinematic = past.isKinematic;
        rb.angularVelocity = 0f;
        rb.rotation = past.rotation;
    }
}

internal class SaveEnemyFly : ISerialDataStore
{
    public bool isAlive { get; private set; }

    public float timeLeftInPlay { get; private set; }

    public float positionX { get; private set; }
    public float positionY { get; private set; }

    public bool isKinematic { get; private set; }
    public float rotation { get; private set; }

    public SaveEnemyFly(bool alive, float time,
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