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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if (IsMovingRight && isInPlay)
        {
            rb.velocity = new Vector2(velX, velY);
        }
        else if (isInPlay)
        {
            rb.velocity = new Vector2(-velX, velY);
        }
    }

    public void InPlay()
    {
        isInPlay = true;
    }

    public void OutOfPlay()
    {
        isInPlay = false;
    }



    void OnCollisionEnter2D(Collision2D col)
    {
        transform.position = new Vector2(storageX, storageY);
    }
}
