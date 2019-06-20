﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serial;

/* 
 * Full Name: Jonathan Burns
 * Student ID: 2288851
 * Chapman email: jburns@chapman.edu/
 * Course number and section: 236-02
 * Assignment Number: 5
 */

/*
 * Purpose:
 *  - Cloud: moves each cloud a little to the right
 */

public class Cloud : MonoBehaviour, ISerializable
{
    public float speed; //IM

    private float leftXBound; //IM  
    private float rightXBound; //IM

    /* 
     * Update is called once per frame,
     * Moves the cloud to the right
     */
    void Update()
    {
        transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);        
        
        if (transform.position.x > rightXBound)
        {
            transform.position = new Vector3(leftXBound, transform.position.y, transform.position.z);
        }
    }

    /*
     * SetBounds - sets both left and right x bounds coordinate 
     * Params:
     *  - float left: left x-axis bound
     *  - float right: right x-axis bound
     */
    public void SetBounds(float left, float right)
    {
        leftXBound = left;
        rightXBound = right;
    }

    /// Serial Methods, see Serial Namespace 
    public ISerialDataStore GetCurrentState()
    {
        return new SaveCloud(transform.position.x);
    }

    public void SetState(ISerialDataStore state)
    {
        SaveCloud past = (SaveCloud) state;

        transform.position = new Vector3(past.positionX, transform.position.y, transform.position.z);
    }
}


internal class SaveCloud : ISerialDataStore
{
    public float positionX { get; private set; }

    public SaveCloud(float posX)
    {
        positionX = posX;
    }
}