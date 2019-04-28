using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimeJuiceUI : MonoBehaviour
{
    public Slider timeBar;

    public StateController globalState; 

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
        Debug.Log("test");
        yield return 0;
    }
}
