using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CloudController : MonoBehaviour
{
    public GameObject leftMarker;
    public GameObject rightMarker;

    void Start()
    {
        SetBoundsAllClouds(GetClouds());
    }

    private Cloud[] GetClouds()
    {
        return FindObjectsOfType<Cloud>().ToArray<Cloud>();
    }

    private void SetBoundsAllClouds(Cloud[] clouds)
    {
        foreach (Cloud c in clouds)
        {
            c.SetBounds(leftMarker.transform.position.x, rightMarker.transform.position.x);
        }
    }
}
