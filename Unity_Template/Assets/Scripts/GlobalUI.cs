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

    void Start()
    {
        Init();
    }

    private void Init()
    {
        IsAlive = true;

        filterImg.enabled = false;
        startingAlphaFilter = filterImg.color.a;
        filterImg.color = GetAlphaChange(filterImg, 0f);

        deathText.enabled = false;
        startingAlphaText = deathText.color.a;
        deathText.color = GetAlphaChange(deathText, 0f);
        deathAnimationTrigger = 0f;
    }

    void FixedUpdate()
    {
        if (!IsAlive)
        {
            float nextAlphaFilter = Mathf.Lerp(filterImg.color.a, startingAlphaFilter, 0.08f);
            filterImg.color = GetAlphaChange(filterImg, nextAlphaFilter);

            if (deathAnimationTrigger > 0)
            {
                AnimateDeath();
                deathAnimationTrigger -= Time.deltaTime;
            }
        }
    }

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

            float newX = Mathf.Lerp(timeBar.transform.position.x, 580, 0.2f);
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
        }

        if (deathAnimationTrigger < 7f)
        {
            //Time.timeScale = 0f;
            
        }
    }

    public void OnDeath()
    {
        IsAlive = false;

        filterImg.enabled = true;
        deathText.enabled = true;

        deathAnimationTrigger = 10f;
    }

    public ISerialDataStore GetCurrentState()
    {
        return new SaveUI(  IsAlive, filterImg.enabled, 
                            filterImg.color.a, deathText.enabled,
                            deathText.color.a, deathAnimationTrigger,
                            timeBar.transform.position.x, timeBar.transform.position.y,
                            timeBar.transform.localScale.x
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

    public SaveUI(  bool alive, bool filter,
                    float aFilter, bool deathTxt,
                    float aText, float show,
                    float barX, float barY,
                    float barScale
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
    }
}
