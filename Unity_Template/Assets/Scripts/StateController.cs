﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Serial;
using System;
using UnityEngine.UI;


public class StateController : MonoBehaviour
{
    ISerializable[] allSerialObjects; // Are also of the MonoBehaviour class, so can be cast to
    // https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.stack-1?view=netframework-4.7.2
    FixedStack<ISerialDataStore[]> pastStates;
    public int frameCount; // About 60 frames per second so 'frameCount = 3600' means the you can rewind for 1 minute

    public Image RewindIcon;
    public Image FilterImg;

    private float pastTrigger; // Needed to create ghetto KeyUp/KeyDown for trigger buttons

    /*
     * Start - finds serializable objects and initalizes stack  
     */
    void Start()
    {
        FindSerializable();
        InitStack();

        InitUI();

        pastTrigger = 0f;
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
    private void InitStack()
    {
        pastStates = new FixedStack<ISerialDataStore[]>(frameCount);
    }

    private void InitUI()
    {
        RewindIcon.enabled = false;
        FilterImg.enabled = false;
    }

    public void CatchCreated()
    {
        var serialQuery = FindObjectsOfType<MonoBehaviour>().OfType<ISerializable>();
        allSerialObjects = serialQuery.Cast<ISerializable>().ToArray();
    }

    /*
     * Update - checks every frame to either store or restore state
     */
    void Update()
    {

        // https://docs.unity3d.com/ScriptReference/Input.GetKeyDown.html
        if ((Input.GetKey(KeyCode.K) ||
             Input.GetKey(KeyCode.R) ||
             Input.GetKey(KeyCode.JoystickButton3) || // Y button on xbox 360 controller
             Input.GetAxis("LeftTrigger") == 1
            )
            && pastStates.Count > 1) // Check for greater than 1 to prevent initialization issues
        {
            RevetState();
        }
        else
        {
            pastStates.Push(CollectStates());
        }

        // Prevents input when rewinding
        if ((Input.GetKeyDown(KeyCode.K) ||
             Input.GetKeyDown(KeyCode.R) ||
             Input.GetKeyDown(KeyCode.JoystickButton3) || // Y button on xbox 360 controller
             (Input.GetAxis("LeftTrigger") == 1 && pastTrigger !=1)
            )
             && pastStates.Count > 1)
        {
            ToggleBehaviourSerializable(false);
            ToggleRewindUI(true);

            pastTrigger = Input.GetAxis("LeftTrigger");
        }

        if ((Input.GetKeyUp(KeyCode.K) ||
             Input.GetKeyUp(KeyCode.R) ||
             Input.GetKeyUp(KeyCode.JoystickButton3) ||
             (Input.GetAxis("LeftTrigger") == 0 && pastTrigger != 0)
            )
             && pastStates.Count > 1)
        {
            ToggleBehaviourSerializable(true);
            ToggleRewindUI(false);

            pastTrigger = Input.GetAxis("LeftTrigger");
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

    /*
     * ToggleBehaviourSerializable - allows revert to block other inputs
     * Params:
     *  - bool toggle: sets the 'enable' state for all Serializable objects
     */
    private void ToggleBehaviourSerializable(bool toggle)
    {
        for (int i = 0; i < allSerialObjects.Length; i++)
        {
            ((MonoBehaviour)allSerialObjects[i]).enabled = toggle;
        }
    }

    private void ToggleRewindUI(bool turnOn)
    {
        RewindIcon.enabled = turnOn;
        FilterImg.enabled = turnOn;
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

    /*
     * Push
     * Params:
     *  - T element: the element to be inserted to the front of the array
     */
    public void Push(T element)
    {
        currentIndex = (currentIndex + 1) % maxSize;
        elements[currentIndex] = element;

        if (Count < maxSize)
        {
            Count++;
        }
    }

    /*
     * Pop - remove top element of stack
     * Returns: T which is the element in the top of the stack
     * Throws:
     *  - EmptyStackException because an empty stack can't have an element removed
     */
    public T Pop()
    {
        if (Count < 1)
        {
            throw new EmptyStackException("Cannot Pop, from a empty list.");
        }


        T tempElement = elements[currentIndex];

        if (currentIndex < 1)
        {
            currentIndex = maxSize - 1;
        }
        else
        {
            currentIndex--;
        }

        Count--;

        return tempElement;
    }

    /*
     * Peek
     * Returns: T the top element of the stack
     */
    public T Peek()
    {
        return elements[currentIndex];
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