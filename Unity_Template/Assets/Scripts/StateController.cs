using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Serial;
using System;

public class StateController : MonoBehaviour
{
    ISerializable[] allSerialObjects;
    // https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.stack-1?view=netframework-4.7.2
    Stack<ISerialDataStore[]> pastStates;

    /*
     * Start - finds serializable objects and initalizes stack  
     */
    void Start()
    {
        FindSerializable();
        InitStack();
    }

    /*
     * FindSerializable - locates any object with 'ISerializable' type,
     * then adds them to array
     */
    void FindSerializable()
    {
        // https://answers.unity.com/questions/863509/how-can-i-find-all-objects-that-have-a-script-that.html
        var serialQuery = FindObjectsOfType<MonoBehaviour>().OfType<ISerializable>();
        allSerialObjects = serialQuery.Cast<ISerializable>().ToArray();
    }

    /*
     * InitStack - creates stack to store global state
     */
    void InitStack()
    {
        pastStates = new Stack<ISerialDataStore[]>();
    }

    /*
     * Update - checks every frame to either store or restore state
     */
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

    /*
     * CollectStates - gets the current state from each serializable object
     */
    ISerialDataStore[] CollectStates()
    {
        ISerialDataStore[] allCurrentStates = new ISerialDataStore[allSerialObjects.Length];

        for (int i = 0; i < allSerialObjects.Length; i++)
        {
            allCurrentStates[i] = allSerialObjects[i].GetCurrentState();
        }

        return allCurrentStates;
    }

    /*
     * RevetState - sends stored state back to object
     */
    void RevetState()
    {
        ISerialDataStore[] lastState = pastStates.Pop();

        for (int i = 0; i < allSerialObjects.Length; i++)
        {
            allSerialObjects[i].SetState(lastState[i]);
        }
    }
}

internal class FixedStack<T>
{
    public int maxSize { get; private set; }
    public int Count { get; private set; }
    private int currentIndex;
    private T[] elements;

    public FixedStack(int max)
    {
        maxSize = max;
        elements = new T[maxSize];
        currentIndex = -1;
        Count = 0;
    }

    public void Push(T element)
    {
        currentIndex = (currentIndex + 1) % maxSize;
        elements[currentIndex] = element;
        Count++;
    }

    public T Pop()
    {
        if (Count < 1)
        {
            throw new EmptyStackException("Cannot Pop, from a empty list.");
        }

        T tempElement = elements[currentIndex];

        currentIndex = (currentIndex - 1) % maxSize;
        Count--;

        return tempElement;
    }
}


internal class EmptyStackException : Exception
{
    public EmptyStackException()
    {

    }

    public EmptyStackException(string message)
        : base(String.Format("FixedArray Stack is Empty: {0}", message))
    {

    }

}