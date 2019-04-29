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

    void Start()
    {
        Init();
    }

    private void Init()
    {
        jumpTriggersRestart = false;
    }

    void Update()
    {
        if (jumpTriggersRestart)
        {
            CheckAndRestart();
        }
    }

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

    public void AllowRestart()
    {
        jumpTriggersRestart = true;
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(nextSceneName);
    }

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
