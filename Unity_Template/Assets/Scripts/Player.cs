using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Player : MonoBehaviour
{
    public float jumpHeight;
    public float moveSpeed;
    private float velHorz;
    private const float acceleration = 0.1f;
    private bool movingRight;

    private Rigidbody2D rb;

    private bool grounded;
    private int jumps;
    private const int maxJumps = 1;

    // Start is called before the first frame update
    void Start()
    {
        InitRigid();
        InitPlayer();
    }

    private void InitRigid()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 2f;
        jumps = maxJumps;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; //Prevents jittery camera
    }

    private void InitPlayer()
    {
        velHorz = 0f;
        grounded = false;
    }

    // Update is called once per frame
    void Update()
    {
        Jump();

        InitialVelocitySet();

        MoveDirection();

    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumps > 0)
        {
            rb.velocity = Vector2.zero; // To allow for wall jumping
            rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
            grounded = false;
            jumps--;
        }
    }

    private void InitialVelocitySet()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            velHorz = moveSpeed - 2.0f;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            velHorz = -(moveSpeed - 2.0f);
        }
    }

    private void MoveDirection()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            AccelerateDir(1);
            rb.velocity = new Vector2(velHorz, rb.velocity.y);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            AccelerateDir(-1);
            rb.velocity = new Vector2(velHorz, rb.velocity.y);
        }
        else
        {
            StopMoving();
        }
    }

    private void AccelerateDir(int direction)
    {
        // Turn around checking
        if (velHorz > 0 && direction < 0)
        {
            velHorz = -(moveSpeed - 2.0f);
        }

        if (velHorz < 0 && direction > 0)
        {
            velHorz = moveSpeed - 2.0f;
        }

        //Starting check


        velHorz = Mathf.Clamp(velHorz + acceleration * direction, -moveSpeed, moveSpeed);
    }

    private void StopMoving()
    {
        float accScale = 0.5f; // Air resistence

        if (grounded)
        {
            accScale = 3f; // Grounded friction
        }

        if (velHorz > 0)
        {
            velHorz = RoundToZero(velHorz - acceleration * accScale);
        }
        else
        {
            velHorz = RoundToZero(velHorz + acceleration * accScale);
        }

        rb.velocity = new Vector2(velHorz, rb.velocity.y);
    }

    private float RoundToZero(float num)
    {
        if (velHorz < acceleration && velHorz > -acceleration)
        {
            return 0f;
        }

        return num;

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        grounded = true;
        jumps = maxJumps;
    }
}
