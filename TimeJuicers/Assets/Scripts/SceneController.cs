using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Serial;
using UnityEngine.UI;
using InputMapping;


public class SceneController : MonoBehaviour, ISerializable
{
    private bool jumpTriggersRestart;
    public float axisBounds; // likely should be the same as on play, but doesn't have to be, IM

    public string nextSceneName;
    private AsyncOperation nextScene; // After the scene is loaded, it is put here
                                      // Be careful not to reference it before it is guranteed to be safe
    private bool mayProcceed;

    public GlobalUI interfaceHandler;

    public Image winFilter;
    public Text winText;
    public Text winTextShadow;
    public Text nextLevelPrompt;
    public float fadeInRate;

    public Player winningPlayer;

    private UserInput input;

    void Start()
    {
        Init();
        InitUI();
    }

    /*
     * Init - sets up vars
     */
    private void Init()
    {
        jumpTriggersRestart = false;
        mayProcceed = false;

        input = new UserInput(axisBounds);
    }

    /*
     * InitUI - disables UI elements on start
     */
    private void InitUI()
    {
        winFilter.enabled = false;
        winText.enabled = false;
        winTextShadow.enabled = false;
        nextLevelPrompt.enabled = false;
    }

    /*
     * Update - Checks for jump input only if dead
     */
    void Update()
    {
        if (jumpTriggersRestart && input.JumpDown())
        {
            DifficultyPersister diff = FindDiff();

            if (diff != null && diff.modeName == "normal")
            {
                diff.MaxFrames += 90; //1.5 seconds
            }

            //https://answers.unity.com/questions/1422096/reload-current-scene-with-scene-manager.html
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
        else if (mayProcceed && CheckJumpDown())
        {
            nextScene.allowSceneActivation = true;
        }

    }

    /*
     * CheckAndRestart - reloads scene on trigger inputs
     * Returns: bool true if the player pressed down space on this frame
     */
    private bool CheckJumpDown()
    {
        return  Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.Joystick1Button0) || // A button on xbox 360 controller
                Input.GetKeyDown(KeyCode.Joystick1Button2) || // X button on xbox 360 controller
                Input.GetAxisRaw("Vertical") > axisBounds;
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
        if (interfaceHandler.IsAlive)
        {
            interfaceHandler.CannotDie();
            StartCoroutine(LoadNextSceneAsync());

            StartCoroutine(FadeInUI(fadeInRate));

            winFilter.enabled = true;
            winText.enabled = true;
            winTextShadow.enabled = true;
            nextLevelPrompt.enabled = true;

            winningPlayer.Fly();
        }
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

    // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadSceneAsync.html

    /*
     * LoadNextSceneAsync - loads the next scene in the background, based on the given nextSceneName,
     * Put the loaded scene trigger into the global nextScene variable
     * Returns IEnumerator: meaning this method is async
     */
    private IEnumerator LoadNextSceneAsync()
    {
        AsyncOperation asyncSceneLoad = SceneManager.LoadSceneAsync(nextSceneName);
        asyncSceneLoad.allowSceneActivation = false;


        // Wait until the asynchronous scene fully loads
        while (!asyncSceneLoad.isDone)
        {

            // https://docs.unity3d.com/ScriptReference/AsyncOperation-allowSceneActivation.html
            if (asyncSceneLoad.progress >= 0.9f)
            {
                nextScene = asyncSceneLoad;
                mayProcceed = true;
                yield break;
            }

            yield return null;
        }
    }

    /*
     * FadeInUI - grandually animates the win UI becoming more opaque
     * Params:
     *  - float rate: the speed at which the animate is performed 
     * Returns: IEnumerator meaning this method is async
     */
    private IEnumerator FadeInUI(float rate)
    {
        float i = 0;

        while (i < 1)
        {
            ChangeOpacityUI(i);
            i += rate * Time.deltaTime; // uses deltaTime unlike FadeBar due to the game never being pausable on win
            yield return 0;
        }
    }

    /*
     * ChangeOpacityUI - sets the opacity for every element
     * Params:
     *  - float alpha: the transparency of the color, range 0-1
     */
    private void ChangeOpacityUI(float alpha)
    {
        winFilter.color = SetOpacity(winFilter, Mathf.Clamp(alpha, 0, 0.5f)); // Filter shouldn't transition to all white
        winText.color = SetOpacity(winText, alpha);
        winTextShadow.color = SetOpacity(winTextShadow, alpha);
        nextLevelPrompt.color = SetOpacity(nextLevelPrompt, alpha);
    }

    /*
     * SetOpacity - only changes the alpha of a given color
     * Params:
     *  - MaskableGraphic UIElement: the object to have its opacity changes
     *  - float alpha: the new transparency value, range 0-1
     * Returns: Color for the object
     */
    private Color SetOpacity(MaskableGraphic UIElement, float alpha)
    {
        return new Color(UIElement.color.r, UIElement.color.g, UIElement.color.b, alpha);
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
