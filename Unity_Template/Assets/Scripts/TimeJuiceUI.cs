using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimeJuiceUI : MonoBehaviour
{
    public Slider timeBar;

    public StateController globalState;

    public int deathPenaltyFrames;
    public Color deathBarColor;
    public float fadeRate;

    void Start()
    {
        timeBar.maxValue = globalState.frameCount;
    }

    void Update()
    {
        timeBar.value = globalState.GetSavedFrameCount();

    }

    public IEnumerator DecreaseBar()
    {
        RectTransform removedJuice =  Instantiate(timeBar.fillRect, timeBar.fillRect.transform.position, timeBar.fillRect.transform.rotation, timeBar.fillRect.parent);

        //https://forum.unity.com/threads/how-to-copy-a-recttransform.497791/
        // Because recttransform is deeply cloned, extra vars have to be set
        removedJuice.anchorMin = timeBar.fillRect.anchorMin;
        removedJuice.anchorMax = timeBar.fillRect.anchorMax;
        removedJuice.anchoredPosition = timeBar.fillRect.anchoredPosition;
        removedJuice.sizeDelta = timeBar.fillRect.sizeDelta;

        // https://answers.unity.com/questions/1138700/how-to-change-position-of-child-object-in-hierarch.html
        // So the new bar is drawn under the normal one
        removedJuice.transform.SetSiblingIndex(0);

        removedJuice.GetComponent<Image>().color = deathBarColor;

        globalState.DeleteStates(deathPenaltyFrames);

        StartCoroutine(FadeBar(removedJuice, fadeRate));

        yield return 0;
    }

    // http://www.theappguruz.com/blog/how-to-use-coroutines-in-unity
    private IEnumerator FadeBar(RectTransform bar, float rate)
    {
        float i = 1;

        while (i >= 0)
        {
            Color barColor = bar.GetComponent<Image>().color;
            bar.GetComponent<Image>().color = new Color(barColor.r, barColor.g, barColor.b, i);
            i -= rate;
            yield return 0;
        }


        Destroy(bar.gameObject);
        
    }
}
