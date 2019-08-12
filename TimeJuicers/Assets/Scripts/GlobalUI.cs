using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serial;
using UnityEngine.UI;


public class GlobalUI : MonoBehaviour, ISerializable
{
    public bool IsAlive { get; private set; }

    public Image filterImg;
    private float startingAlphaFilter; //IM

    public Text deathText;
    private float startingAlphaText;
    private float deathAnimationTrigger;

    public Slider timeBar;
    public float barIncreaseScale; //IM

    public StateController pastStates;

    private bool hasDied; //NOT serialized
    private bool immortal; //NOT serlialized

    public TimeJuiceUI timeBarController;

    public Text rewindPrompt;
    public Text restartPrompt;
    public Text outOfJuiceText;
    private bool fadeInWhichPrompt; // true means display rewind prompt, false restart prompt
    private float startingAlphaPromt;

    public SceneController scene;

    void Start()
    {
        Time.timeScale = 1f; // Prevents the game from being frozen when restarting

        Init();
    }

    /*
     * Init - sets up many of the interface variables
     */
    private void Init()
    {
        IsAlive = true;
        immortal = false;

        filterImg.enabled = false;
        startingAlphaFilter = filterImg.color.a;
        filterImg.color = GetAlphaChange(filterImg, 0f);

        deathText.enabled = false;
        startingAlphaText = deathText.color.a;
        deathText.color = GetAlphaChange(deathText, 0f);
        deathAnimationTrigger = 0f;

        rewindPrompt.enabled = false;
        restartPrompt.enabled = false;
        outOfJuiceText.enabled = false;
        startingAlphaPromt = rewindPrompt.color.a; // Lazy implementation, starting alpha is only dependent on rewind color
        rewindPrompt.color = GetAlphaChange(rewindPrompt, 0f);
        restartPrompt.color = GetAlphaChange(restartPrompt, 0f);
        outOfJuiceText.color = GetAlphaChange(outOfJuiceText, 0f);
    }

    /*
    * Update - Checks every frame if death interface animations should be played
    */
    void Update()
    {
        if (!IsAlive)
        {
            float nextAlphaFilter = Mathf.Lerp(filterImg.color.a, startingAlphaFilter, 0.08f);
            filterImg.color = GetAlphaChange(filterImg, nextAlphaFilter);

            if (deathAnimationTrigger > 0)
            {
                AnimateDeath();

                if (Time.deltaTime > 0)
                {
                    deathAnimationTrigger -= Time.deltaTime;
                }
                else
                {
                    deathAnimationTrigger -= 0.015f; // When time is paused, animations may not be timed perfectly
                }
            }
        }

        CheckResurrected();
    }

    /*
     * AnimateDeath - triggers animations for death interface 
     * TODO: refactor to not just be chained if statements
     */
    private void AnimateDeath()
    {
        if (deathAnimationTrigger < 9.8f)
        {
            float nextAlphaText = Mathf.Lerp(deathText.color.a, startingAlphaText, 0.03f);
            deathText.color = GetAlphaChange(deathText, nextAlphaText);
        }

        if (deathAnimationTrigger < 9.0f)
        {
            float minChange = 2f;

            float newX = Mathf.Lerp(timeBar.transform.position.x, 600, 0.2f);
            float newY = Mathf.Lerp(timeBar.transform.position.y, 347, 0.2f);

            if (Mathf.Abs(580 - newX) < minChange)
            {
                newX = Mathf.Clamp(timeBar.transform.position.x + minChange, timeBar.transform.position.x, 580);
            }

            if (Mathf.Abs(580 - newY) < minChange)
            {
                newX = Mathf.Clamp(timeBar.transform.position.y + minChange, timeBar.transform.position.y, 347);
            }


            timeBar.transform.position = new Vector2(newX, newY);

            float newScale = Mathf.Clamp(timeBar.transform.localScale.x + 0.05f, timeBar.transform.localScale.x, barIncreaseScale);

            timeBar.transform.localScale = new Vector2(newScale, newScale);

            PromptOnDeath();
        }

        if (deathAnimationTrigger < 8.5f)
        {
            PauseGame();
        }

        // After here animation timing is now frame based, not time based
        // Due to the game becoming paused

        if (deathAnimationTrigger < 7.5f)
        {
            scene.AllowRestart();
        }

    }

    /*
     * PauseGame - stops time, and state controller,
     * Starts a coroutine to animate the time juice loss
     */
    private void PauseGame()
    {
        Time.timeScale = 0f;
        pastStates.TogglePause(true);

        if (!hasDied)
        {
            StartCoroutine(timeBarController.DecreaseBar());
            hasDied = true;
            pastStates.ToggleRewindDisabled(false);
        }
    }

    /*
     * PromptOnDeath - determines then displays, the appropriate text prompting the player for input
     */
    private void PromptOnDeath() {
        if (fadeInWhichPrompt)
        {
            float nextAlphaText = Mathf.Lerp(rewindPrompt.color.a, startingAlphaPromt, 0.05f);
            rewindPrompt.color = GetAlphaChange(rewindPrompt, nextAlphaText);
        }
        else
        {
            float nextAlphaText = Mathf.Lerp(restartPrompt.color.a, startingAlphaPromt, 0.05f);
            restartPrompt.color = GetAlphaChange(restartPrompt, nextAlphaText);

            outOfJuiceText.color = GetAlphaChange(outOfJuiceText, nextAlphaText);
        }
    }

    /*
     * CheckResurrected - needed to prevent player from rewinding until death display is completed  
     */
    private void CheckResurrected()
    {
        if (IsAlive)
        {
            hasDied = false;
        }
    }

    /*
     * OnDeath - called by player object when they collide with something that causes death,
     * Sets a lot of vars to trigger death
     */
    public void OnDeath()
    {
        if (!immortal)
        {
            IsAlive = false;

            filterImg.enabled = true;
            deathText.enabled = true;
            outOfJuiceText.enabled = true;
            pastStates.ToggleRewindDisabled(true);

            rewindPrompt.enabled = true;
            restartPrompt.enabled = true;
            fadeInWhichPrompt = (pastStates.GetSavedFrameCount() - timeBarController.DeathPenaltyFrames > 0);

            deathAnimationTrigger = 10f;
        }
    }

    /*
     * CannotDie - to prevent death after winning : ),
     * Funny you can only live forever after dying,
     * (For real, is used to prevent the player from re triggering death event)
     */
    public void CannotDie()
    {
        immortal = true;
    }

    ////////////////////////////////////////////////////
    // Serial implementation, see Libraries/Serial.cs //
    ////////////////////////////////////////////////////
    public ISerialDataStore GetCurrentState()
    {
        return new SaveUI(  IsAlive, filterImg.enabled, 
                            filterImg.color.a, deathText.enabled,
                            deathText.color.a, deathAnimationTrigger,
                            timeBar.transform.position.x, timeBar.transform.position.y,
                            timeBar.transform.localScale.x, Time.timeScale,
                            pastStates.GetPaused(), rewindPrompt.enabled,
                            restartPrompt.enabled, outOfJuiceText.enabled,
                            rewindPrompt.color.a, restartPrompt.color.a, 
                            outOfJuiceText.color.a
                         );
    }

    public void SetState(ISerialDataStore state)
    {
        SaveUI past = (SaveUI) state;

        IsAlive = past.IsAlive;
        filterImg.enabled = past.filterImgEnabled;
        filterImg.color = GetAlphaChange(filterImg, past.alphaFilter);
        deathText.enabled = past.deathTextEnabled;
        deathText.color = GetAlphaChange(deathText, past.alphaText);
        deathAnimationTrigger = past.deathAnimationTrigger;

        timeBar.transform.position = new Vector2(past.timeBarPositionX, past.timeBarPositionY);
        timeBar.transform.localScale = new Vector2(past.timeBarScale, past.timeBarScale);

        Time.timeScale = past.timeScale;
        pastStates.TogglePause(past.isPaused);

        rewindPrompt.enabled = past.rewindPrompting;
        restartPrompt.enabled = past.restartPrompting;
        outOfJuiceText.enabled = past.outOfJuicing;

        rewindPrompt.color = GetAlphaChange(rewindPrompt, past.rewindPromptAlpha);
        restartPrompt.color = GetAlphaChange(restartPrompt, past.restartPromptAlpha);
        outOfJuiceText.color = GetAlphaChange(outOfJuiceText, past.outOfJuiceAlpha);
    }

    private Color GetAlphaChange(MaskableGraphic ui, float alpha)
    {
        return new Color(ui.color.r, ui.color.b, ui.color.g, alpha);
    }
}

internal class SaveUI : ISerialDataStore
{
    public bool IsAlive { get; private set; }

    public bool filterImgEnabled { get; private set; }
    public float alphaFilter;

    public bool deathTextEnabled { get; private set; }
    public float alphaText { get; private set; }
    public float deathAnimationTrigger { get; private set; }

    public float timeBarPositionX { get; private set; }
    public float timeBarPositionY { get; private set; }
    public float timeBarScale { get; private set; }

    public float timeScale { get; private set; }

    public bool isPaused { get; private set; }

    public bool rewindPrompting { get; private set; }
    public bool restartPrompting { get; private set; }
    public bool outOfJuicing { get; private set; }

    public float rewindPromptAlpha { get; private set; }
    public float restartPromptAlpha { get; private set; }
    public float outOfJuiceAlpha { get; private set; }


    public SaveUI(  bool alive, bool filter,
                    float aFilter, bool deathTxt,
                    float aText, float show,
                    float barX, float barY,
                    float barScale, float time,
                    bool pause, bool rewindP,
                    bool restartP, bool outJ,
                    float rewindA, float restartA, 
                    float outA
                 )
    {
        IsAlive = alive;
        filterImgEnabled = filter;
        alphaFilter = aFilter;
        deathTextEnabled = deathTxt;
        alphaText = aText;
        deathAnimationTrigger = show;
        timeBarPositionX = barX;
        timeBarPositionY = barY;
        timeBarScale = barScale;
        timeScale = time;
        isPaused = pause;
        rewindPrompting = rewindP;
        restartPrompting = restartP;
        outOfJuicing = outJ;
        rewindPromptAlpha = rewindA;
        restartPromptAlpha = restartA;
        outOfJuiceAlpha = outA;
    }
}
