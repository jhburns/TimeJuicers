using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Serial;

public class StateController : MonoBehaviour
{
    void Start()
    {
        var allSerialObjects = FindObjectsOfType<MonoBehaviour>().OfType<Serializable>();
        foreach (Serializable s in allSerialObjects)
        {
            Debug.Log(s);
        }
    }

    void Update()
    {

    }
}
