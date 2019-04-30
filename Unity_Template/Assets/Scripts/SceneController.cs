using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Serial;

public class SceneController : MonoBehaviour, ISerializable
{
    private bool jumpTriggersRestart;
    public float axisBounds; // likely should be the same as on play, but doesn't have to be, IM

    public string nextSceneName;

    /*
     * Start - is called before the first frame update
     */
    void Start()
    {
        Init();
    }

    /*
     * Init - sets up vars
     */
    private void Init()
    {
        jumpTriggersRestart = false;
    }

    /*
     * Update - is called once per frame,
     * Checks for jump input only if dead
     */
    void Update()
    {
        if (jumpTriggersRestart)
        {
            CheckAndRestart();
        }
    }

    /*
     * CheckAndRestart - reloads scene on trigger inputs
     */
    private void CheckAndRestart()
    {
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.Joystick1Button0) || // A button on xbox 360 controller
            Input.GetKeyDown(KeyCode.Joystick1Button2) || // X button on xbox 360 controller
            Input.GetAxisRaw("Vertical") > axisBounds
           )
        {
            //https://answers.unity.com/questions/1422096/reload-current-scene-with-scene-manager.html
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }

    /*
     * AllowRestart - called by other object, lets jump restart the level
     */
    public void AllowRestart()
    {
        jumpTriggersRestart = true;
    }

    /*
     * NextLevel - loads the next scene, defined in nextSceneName
     */
    public void NextLevel()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    /// Serial Methods, see Serial Namespace 
    public ISerialDataStore GetCurrentState()
    {
        return new SaveSceneController(jumpTriggersRestart);
    }

    public void SetState(ISerialDataStore state)
    {
        SaveSceneController past = (SaveSceneController) state;

        jumpTriggersRestart = past.jumpTriggersRestart;
    }
}

internal class SaveSceneController : ISerialDataStore
{
    public bool jumpTriggersRestart { get; private set; }

    public SaveSceneController(bool jumpable)
    {
        jumpTriggersRestart = jumpable;
    }
}
