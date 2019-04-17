using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float jumpHeight;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Rigidbody body = getBody();
            body.velocity = new Vector2(body.velocity.y, jumpHeight);
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Rigidbody body = getBody();
            body.velocity = new Vector2(moveSpeed, body.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Rigidbody body = getBody();
            body.velocity = new Vector2(moveSpeed, body.velocity.y);
        }

    }

    Rigidbody getBody()
    {
        return GetComponent<Rigidbody>();
    }


}
