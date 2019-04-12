using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Serial;


public class StateController : MonoBehaviour
{
    ISerializable[] allSerialObjects;
    // https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.stack-1?view=netframework-4.7.2
    Stack<ISerialDataStore[]> pastStates;

    void Start()
    {
        FindSerializable();
        InitStack();
    }

    void FindSerializable()
    {
        // https://answers.unity.com/questions/863509/how-can-i-find-all-objects-that-have-a-script-that.html
        var serialQuery = FindObjectsOfType<MonoBehaviour>().OfType<ISerializable>();
        allSerialObjects = serialQuery.Cast<ISerializable>().ToArray();
    }

    void InitStack()
    {
        pastStates = new Stack<ISerialDataStore[]>();
    }

    void Update()
    {

        // https://docs.unity3d.com/ScriptReference/Input.GetKeyDown.html
        if (Input.GetKey(KeyCode.Space) && pastStates.Count > 1) // Check for greater than 1 to prevent initialization issues
        {
            RevetState();
        }
        else
        {
            pastStates.Push(CollectStates());
        }

    }

    ISerialDataStore[] CollectStates()
    {
        ISerialDataStore[] allCurrentStates = new ISerialDataStore[allSerialObjects.Length];

        for (int i = 0; i < allSerialObjects.Length; i++)
        {
            allCurrentStates[i] = allSerialObjects[i].GetCurrentState();
        }

        return allCurrentStates;
    }

    void RevetState()
    {
        ISerialDataStore[] lastState = pastStates.Pop();

        for (int i = 0; i < allSerialObjects.Length; i++)
        {
            allSerialObjects[i].SetState(lastState[i]);
        }
    }
}
