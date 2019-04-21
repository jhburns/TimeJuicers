using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float velX; //IM
    private const float velY = 0f;
    private Rigidbody2D rb;
    public bool IsMovingRight { get; set; }

    private float storageX; //IM
    private float storageY; //IM

    private bool isInPlay;
    private float timeLeftInPLay;
    public float maxTime;

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

    public void InPlay()
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
}
