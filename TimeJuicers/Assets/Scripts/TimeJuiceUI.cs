using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimeJuiceUI : MonoBehaviour
{
    public Slider timeBar; //IM

    public StateController globalState; 

    public int DeathPenaltyFrames; //IM
    public Color deathBarColor; //IM
    public float fadeRate; //IM

    /*
     * Start - Sets the max value, and penalty frames
     */
    void Start()
    {
        timeBar.maxValue = globalState.frameCount;

        DifficultyPersister diff = FindDiff();

        if (diff != null)
        {
            DeathPenaltyFrames = diff.FramePenalty;
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

    /* 
     * Update - Animates the time bar based on the current number of saved frames
     */
    void Update()
    {
        timeBar.value = globalState.GetSavedFrameCount();
    }

    /*
     * DecreaseBar - Animation for time bar losing time juice
     * Returns: IEnumerator meaning this is an async method
     */
    public IEnumerator DecreaseBar()
    {
        RectTransform removedJuice =  Instantiate(timeBar.fillRect, timeBar.fillRect.transform.position, timeBar.fillRect.transform.rotation, timeBar.fillRect.parent);

        // Because recttransform is deeply cloned, extra vars have to be copied
        removedJuice.anchoredPosition = timeBar.fillRect.anchoredPosition;
        removedJuice.sizeDelta = timeBar.fillRect.sizeDelta;
        removedJuice.anchorMin = timeBar.fillRect.anchorMin;
        removedJuice.anchorMax = timeBar.fillRect.anchorMax;

        // So the new bar is drawn under the normal one
        removedJuice.transform.SetSiblingIndex(0);

        removedJuice.GetComponent<Image>().color = deathBarColor;

        globalState.DeleteStates(DeathPenaltyFrames);

        StartCoroutine(FadeBar(removedJuice, fadeRate));

        yield return 0;
    }

    /*
     * FadeBar - fades out the lost juice bar
     * Params:
     *  - RectTransform bar: the shape to be animated
     *  - float rate: change in opacity every frame
     * Returns: IEnumerator meaning this is an async method
     */
    private IEnumerator FadeBar(RectTransform bar, float rate)
    {
        // http://www.theappguruz.com/blog/how-to-use-coroutines-in-unity

        float i = 1;

        while (i >= 0)
        {
            Color barColor = bar.GetComponent<Image>().color;
            bar.GetComponent<Image>().color = new Color(barColor.r, barColor.g, barColor.b, i);
            i -= rate; //Not based on time due to the game likely being paused
            yield return 0;
        }

        Destroy(bar.gameObject);
    }
}
