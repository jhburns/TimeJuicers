using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Serial;
using System;
using UnityEngine.UI;
using InputMapping;


public class StateController : MonoBehaviour
{
    ISerializable[] allSerialObjects;
    private FixedStack<ISerialDataStore[]> pastStates;
    public int frameCount; // About 60 frames per second so 'frameCount = 3600' means the you can rewind for 1 minute

    public Image RewindIcon;
    public Image FilterImg;

    private float pastTrigger; // Needed to create ghetto KeyUp/KeyDown for trigger buttons

    private bool isPaused;
    private bool rewindDisabled; 
    private bool allowRewindTime; // Used to make sure that the user has to re-input rewind time when it was disabled

    private UserInput input;

    /*
     * Start - finds serializable objects and initializes stack  
     */
    void Start()
    {
        FindSerializable();

        InitStack();
        InitUI();
        InitInput();
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

        if (diff != null)
        {
            frameCount = diff.MaxFrames;
        }

        pastStates = new FixedStack<ISerialDataStore[]>(frameCount);
    }

    /*
     * InitUI - sets up various UI elements 
     */
    private void InitUI()
    {
        RewindIcon.enabled = false;
        FilterImg.enabled = false;

        isPaused = false;
        rewindDisabled = false;
        allowRewindTime = true;
    }

    private void InitInput()
    {
        pastTrigger = 0f;
        input = new UserInput();
    }

    /*
     * CatchCreated - finds all ISerializable objects in the scene, adds them to a global array
     */
    public void CatchCreated()
    {
        var serialQuery = FindObjectsOfType<MonoBehaviour>().OfType<ISerializable>();
        allSerialObjects = serialQuery.Cast<ISerializable>().ToArray();
    }

    /*
    * FindDiff - Returns the first DifficultyPersister in the scene
    * Returns DifficultyPersister: object that stores difficulty between scenes
    */
    public DifficultyPersister FindDiff()
    {
        DifficultyPersister[] diffs = FindObjectsOfType<DifficultyPersister>();

        if (diffs.Length > 0)
        {
            return diffs[0];  //There should only be one object in the scene
        }
        else
        {
            return null;
        }
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
     * RewindTime - checks whether player is trying to rewind or saves current game state
     */
    private void RewindTime()
    {
        // Check for greater than 1 to prevent initialization issues
        if (input.RewindDown() && pastStates.Count > 1 && allowRewindTime)
        {
            RevertState();
            AudioListener.volume = 0;
        }
        else if (!isPaused)
        {
            pastStates.Push(CollectStates());
            AudioListener.volume = 1.0f;
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
            if (!rewindDisabled)
            {
                ToggleBehaviourSerializable(false);
                ToggleRewindUI(true);

                pastTrigger = Input.GetAxisRaw("LeftTrigger");

                isPaused = false;

                allowRewindTime = true;
            }
            else
            {
                allowRewindTime = false;
            }
            AudioListener.volume = 0;
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
     * CollectStates - gets the current state from each serialize object
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
     *  - bool toggle: sets the 'enable' state for all ISerializable objects
     */
    private void ToggleBehaviourSerializable(bool toggle)
    {
        for (int i = 0; i < allSerialObjects.Length; i++)
        {
            ((MonoBehaviour) allSerialObjects[i]).enabled = toggle;
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

    /*
     * GetPaused
     * Returns: bool of whether this thinks the game is paused
     */
    public bool GetPaused()
    {
        return isPaused;
    }

    /*
     * TogglePause - setter for isPaused
     * Params:
     *  - bool pause: tell this to treat the game as paused if true
     */
    public void TogglePause(bool pause)
    {
        isPaused = pause;
    }

    /*
     * GetRewindDisabled
     * Returns: bool if input to allow rewinding is currently disabled
     */
    public bool GetRewindDisabled()
    {
        return rewindDisabled;
    }

    /*
     * ToggleRewindDisabled
     * Params:
     *  - bool rewindDis: tells this to disable rewinding input
     */
    public void ToggleRewindDisabled(bool dis)
    {
        rewindDisabled = dis;
    }
}

internal class FixedStack<T>
{
    public int MaxSize { get; private set; }
    public int Count { get; private set; }
    private int currentIndex;
    private T[] elements;

    public FixedStack(int max)
    {
        MaxSize = max;
        elements = new T[MaxSize];
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
        currentIndex = (currentIndex + 1) % MaxSize;
        elements[currentIndex] = element;

        if (Count < MaxSize)
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
            currentIndex = MaxSize - 1;
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
        : base(String.Format("Trying to perform the following operation on the FixedArray Stack is illegal: {0}", message))
    {

    }
}