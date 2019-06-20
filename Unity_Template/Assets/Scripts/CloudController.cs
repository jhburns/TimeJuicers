using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* 
 * Full Name: Jonathan Burns
 * Student ID: 2288851
 * Chapman email: jburns@chapman.edu/
 * Course number and section: 236-02
 * Assignment Number: 5
 */

/*
 * Purpose:
 *  - CloudController: sets up the bounds on each cloud so they can loop in the scene
 */

public class CloudController : MonoBehaviour
{
    public GameObject leftMarker;
    public GameObject rightMarker;

    /* Start - is called before the first frame update,
    * Sets up each cloud
    */
    void Start()
    {
        SetBoundsAllClouds(GetClouds());
    }

    /*
     * GetClouds - finds all object with Cloud script attached to it
     * Returns CLoud[]: all of the moving clouds in the scene
     */
    private Cloud[] GetClouds()
    {
        return FindObjectsOfType<Cloud>().ToArray<Cloud>();
    }

    /*
     * SetBoundsAllClouds - gives each cloud boundries to not cross
     * Params:
     *  - Cloud[] clouds: a group of cloud objects to have their bounds set
     */
    private void SetBoundsAllClouds(Cloud[] clouds)
    {
        foreach (Cloud c in clouds)
        {
            c.SetBounds(leftMarker.transform.position.x, rightMarker.transform.position.x);
        }
    }
}
