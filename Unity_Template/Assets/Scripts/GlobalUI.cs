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
    private float deathTextShow;

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
        deathTextShow = 0f;
    }

    void Update()
    {
        if (!IsAlive)
        {
            float nextAlphaFilter = Mathf.Lerp(filterImg.color.a, startingAlphaFilter, 0.08f);
            filterImg.color = GetAlphaChange(filterImg, nextAlphaFilter);

            if (deathTextShow < 0)
            {
                float nextAlphaText = Mathf.Lerp(deathText.color.a, startingAlphaText, 0.03f);
                deathText.color = GetAlphaChange(deathText, nextAlphaText);
            } else
            {
                deathTextShow -= Time.deltaTime;
            }

            Debug.Log(deathTextShow);
        }
    }

    public void OnDeath()
    {
        IsAlive = false;

        filterImg.enabled = true;
        deathText.enabled = true;

        deathTextShow = 0.2f;
    }

    public ISerialDataStore GetCurrentState()
    {
        return new SaveUI(  IsAlive, filterImg.enabled, 
                            filterImg.color.a, deathText.enabled,
                            deathText.color.a, deathTextShow
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
        deathTextShow = past.deathTextShow;
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
    public float deathTextShow { get; private set; }

    public SaveUI(  bool alive, bool filter,
                    float aFilter, bool deathTxt,
                    float aText, float show
                 )
    {
        IsAlive = alive;
        filterImgEnabled = filter;
        alphaFilter = aFilter;
        deathTextEnabled = deathTxt;
        alphaText = aText;
        deathTextShow = show;
    }
}
