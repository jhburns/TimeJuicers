using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Full Name: Jonathan Burns
 * Student ID: 2288851
 * Chapman email: jburns@chapman.edu/
 * Course number and section: 236-02
 * Assignment Number: 5
 */

/*
 * Purpose:
 *  - WinFlag: when the player collides, trigger the next scene to load
 */

public class WinFlag : MonoBehaviour
{
    public SceneController scene;

    /*
     * OnCollisionEnter2D - allows the player to go to the next level
     * Params:
     *  - Collision2D col: the other object being collided with 
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "player")
        {
            scene.NextLevel();
        }
    }
}
