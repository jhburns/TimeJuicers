using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Serial;


public class StateController : MonoBehaviour
{
    Serializable[] allSerialObjects;
    Stack<SerialDataStore[]> pastStates;

    void Start()
    {
        var serialQuery = FindObjectsOfType<MonoBehaviour>().OfType<Serializable>();
        allSerialObjects = serialQuery.Cast<Serializable>().ToArray();

        pastStates = new Stack<SerialDataStore[]>();
    }

    void Update()
    {
        pastStates.Push(collectStates());
    }

    SerialDataStore[] collectStates()
    {
        SerialDataStore[] allCurrentStates = new SerialDataStore[allSerialObjects.Length];

        for (int i = 0; i < allSerialObjects.Length; i++)
        {
            allCurrentStates[i] = allSerialObjects[i].GetCurrentState();
        }

        return allCurrentStates;
    }
}
