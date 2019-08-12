using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class CloudController : MonoBehaviour
{
    public GameObject leftMarker; // Where the clouds teleport to
    public GameObject rightMarker; // Triggers the clouds to loop

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
     * SetBoundsAllClouds - sets boundaries on each cloud to not cross
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
