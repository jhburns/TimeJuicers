using System.Collections;
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
    private FixedStack<ISerialDataStore[]> pastStates;
    public int frameCount; // About 60 frames per second so 'frameCount = 3600' means the you can rewind for 1 minute

    public Image RewindIcon;
    public Image FilterImg;

    private float pastTrigger; // Needed to create ghetto KeyUp/KeyDown for trigger buttons

    public bool IsPaused { get; set; }
    public bool RewindInputDisabled { get; set; }
    private bool allowRewindTime; // Used to make sure that the user has to reinput rewind time when it was disabled

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
        DifficultyPersister diff = FindDiff();
        frameCount = diff.MaxFrames;

        pastStates = new FixedStack<ISerialDataStore[]>(frameCount);
    }

    private void InitUI()
    {
        RewindIcon.enabled = false;
        FilterImg.enabled = false;

        IsPaused = false;
        RewindInputDisabled = false;
        allowRewindTime = true;
    }

    public void CatchCreated()
    {
        var serialQuery = FindObjectsOfType<MonoBehaviour>().OfType<ISerializable>();
        allSerialObjects = serialQuery.Cast<ISerializable>().ToArray();
    }

    public DifficultyPersister FindDiff()
    {
        return FindObjectsOfType<DifficultyPersister>()[0]; //There should only be one object in the scene
    }

    /*
     * Update - checks every frame to either store or restore state
     */
    void Update()
    {
        RewindTime();

        StartRewindUI();

        StopRewindUI();
    }

    /*
     * RewindTime - checks wheter player is trying to rewind or saves current game state
     */
    private void RewindTime()
    {
        // https://docs.unity3d.com/ScriptReference/Input.GetKeyDown.html
        if ((Input.GetKey(KeyCode.K) ||
             Input.GetKey(KeyCode.R) ||
             Input.GetKey(KeyCode.JoystickButton3) || // Y button on xbox 360 controller
             Input.GetAxisRaw("LeftTrigger") == 1
            )
            && pastStates.Count > 1 // Check for greater than 1 to prevent initialization issues
            && allowRewindTime)
        {
            RevertState();
        }
        else if (!IsPaused)
        {
            pastStates.Push(CollectStates());
        }
    }

    /*
     * StartRewindUI - displays rewind graphics on press
     */
    private void StartRewindUI()
    {
        // Prevents input when rewinding
        if ((Input.GetKeyDown(KeyCode.K) ||
             Input.GetKeyDown(KeyCode.R) ||
             Input.GetKeyDown(KeyCode.JoystickButton3) || // Y button on xbox 360 controller
             (Input.GetAxisRaw("LeftTrigger") == 1 && pastTrigger != 1)
            )
             && pastStates.Count > 1)
        {
            if (!RewindInputDisabled)
            {
                ToggleBehaviourSerializable(false);
                ToggleRewindUI(true);

                pastTrigger = Input.GetAxisRaw("LeftTrigger");

                IsPaused = false;

                allowRewindTime = true;
            }
            else
            {
                allowRewindTime = false;
            }
        }
    }

    /*
     * StopRewindUI - removes rewind graphics on let go
     */
    private void StopRewindUI()
    {
        if ((Input.GetKeyUp(KeyCode.K) ||
         Input.GetKeyUp(KeyCode.R) ||
         Input.GetKeyUp(KeyCode.JoystickButton3) ||
         (Input.GetAxisRaw("LeftTrigger") == 0 && pastTrigger != 0)
        )
         && pastStates.Count > 1)
        {
            ToggleBehaviourSerializable(true);
            ToggleRewindUI(false);

            pastTrigger = Input.GetAxisRaw("LeftTrigger");
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
     * RevertState - sends stored state back to object
     */
    void RevertState()
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

    /*
     * ToggleRewindUI - turns on/off graphics
     * Params:
     *  - bool turnOn: if true then graphics are enabled
     */
    private void ToggleRewindUI(bool turnOn)
    {
        RewindIcon.enabled = turnOn;
        FilterImg.enabled = turnOn;
    }

    /*
     * GetSavedFrameCount - getter for pastStates count
     * Returns: int of the size of pastStates stack
     */
    public int GetSavedFrameCount()
    {
        return pastStates.Count;
    }

    /*
     * DeleteStates - removes the amount of frames from the bottom of the stack
     * Params:
     *  - int frameCount: number of frames to try and remove, capped at current number of saved frames
     */
    public void DeleteStates(int frameCount)
    {
        pastStates.RemoveBottom(Mathf.Clamp(frameCount, 0, pastStates.Count));
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

    /*
     * DeleteBottom
     * Params:
     *  - int numRemove: a positive number of the number of elements to remove from the bottom of the stack,
     *                   meaning elements pushed first
     * 
     */
    public void RemoveBottom(int numRemove)
    {
        if (numRemove < 0)
        {
            throw new IllegalRemoveStackException("Cannot remove a negative number of elements.");
        }

        if (numRemove > Count)
        {
            throw new IllegalRemoveStackException("The number of elements to remove is less than the current total count.");
        }

        Count -= numRemove;
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

internal class IllegalRemoveStackException : Exception
{
    public IllegalRemoveStackException()
    {

    }

    public IllegalRemoveStackException(string message)
        : base(String.Format("Trying to performn the following operation on the FixedArray Stack is illegal: {0}", message))
    {

    }
}