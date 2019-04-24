﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Serial;

public class Player : MonoBehaviour, ISerializable
{
    public float jumpHeight; //IM = suppose to be immutable
    public float moveSpeed; //IM
    private float velHorz;  // Mutable, but not tracked
    private const float acceleration = 0.1f; //IM
    public bool MovingRight { get; private set; }

    private Rigidbody2D rb; // Mutable, but not tracked

    private bool grounded;
    private int jumps;
    private const int maxJumps = 1; //IM

    private float axisBounds;

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
        rb.gravityScale = 3f;
        jumps = 0;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; //Prevents jittery camera
    }

    private void InitPlayer()
    {
        velHorz = 0f;
        grounded = false;
        MovingRight = true;
        axisBounds = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        // bascially everywhere: https://docs.unity3d.com/ScriptReference/Rigidbody2D.html

        Jump();

        InitialVelocitySet();

        MoveDirection();

        //ffDebug.Log(Input.GetAxis("Horizontal"));
    }

    private void Jump()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || 
             Input.GetKeyDown(KeyCode.W) ||
             Input.GetKeyDown(KeyCode.UpArrow) ||
             Input.GetKeyDown(KeyCode.Joystick1Button0) || // A button on xbox 360 controller
             Input.GetKeyDown(KeyCode.Joystick1Button2) || // X button on xbox 360 controller
             Input.GetAxis("Vertical") > axisBounds
            )
            && jumps > 0)
        {
            rb.velocity = Vector2.zero; // To allow for wall jumping
            rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
            grounded = false;
            jumps--;
        }
    }

    private void InitialVelocitySet()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            velHorz = moveSpeed- 2.0f;
            MovingRight = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            velHorz = -(moveSpeed- 2.0f);
            MovingRight = false;
        }
    }

    private void MoveDirection()
    {
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            AccelerateDir(1);
            rb.velocity = new Vector2(velHorz, rb.velocity.y);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
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
        if (velHorz < acceleration * 2 && velHorz > -acceleration * 2) // Needs to be twice to fix an edge case, player was still moving a little
        {
            return 0f;
        }

        return num;

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Make sure to check if the object has a material first
        if (col.collider.sharedMaterial != null)
        {
            if (col.collider.sharedMaterial.name == "GroundMaterial")
            {
                grounded = true;
                jumps = maxJumps;
            }

            if (col.collider.sharedMaterial.name == "BouncyMaterial") {
                jumps = maxJumps;
                rb.velocity = Vector2.zero; // Prevents unlimited jump height
                rb.AddForce(new Vector2(0, jumpHeight * 0.9f), ForceMode2D.Impulse);
            }
        }
    }

    ///  Serial Methods
    public ISerialDataStore GetCurrentState()
    {
        return new SavePlayer(MovingRight, grounded, jumps, transform.position.x, transform.position.y);
    }

    public void SetState(ISerialDataStore state)
    {
        SavePlayer past = (SavePlayer) state;

        velHorz = 0f;

        MovingRight = past.movingRight;
        grounded = past.grounded;
        jumps = past.jumps;

        transform.position = new Vector3(past.positionX, past.positionY, 0);
        rb.velocity = Vector2.zero; // Needed becasue velocity isn't conserved
    }
}

internal class SavePlayer : ISerialDataStore
{
    public bool movingRight { get; private set; }

    public bool grounded { get; private set; }
    public int jumps { get; private set; }

    public float positionX;
    public float positionY;

    public SavePlayer(bool movingR, bool g,
                        int j, float posX,
                        float posY
                     )
    {
        movingRight = movingR;

        grounded = g;
        jumps = j;

        positionX = posX;
        positionY = posY;
    }
}