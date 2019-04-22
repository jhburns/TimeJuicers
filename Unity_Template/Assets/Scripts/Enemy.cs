using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;

    public bool isMovingRight;
    private bool isGrounded;

    public float speed; //IM

    public bool isAlive;

    // Start is called before the first frame update
    void Start()
    {
        InitRigid();
        InitMovement();

        isAlive = true;
    }

    private void InitRigid()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; //Prevents jittery camera
    }

    private void InitMovement()
    {
        isGrounded = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingRight && isGrounded && isAlive)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        } else if (isGrounded && isAlive)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }
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

            rb.AddForce(new Vector2(10f * direction, 15f), ForceMode2D.Impulse);
        }

        if (col.collider.sharedMaterial != null && col.collider.sharedMaterial.name == "GroundMaterial")
        {
            isGrounded = true;
        } else
        {
            isMovingRight = !isMovingRight;
        }

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Collider2D>().name != "GroundMaterial")
        {
            isMovingRight = !isMovingRight;
        }
    }
}
