using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serial;
using UnityEngine.UI;


public class GlobalUI : MonoBehaviour, ISerializable
{
    public bool IsAlive { get; private set; }
    public Image filterImg;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        IsAlive = true;

        filterImg.enabled = false;
    }

    void Update()
    {

    }

    public void OnDeath()
    {
        IsAlive = false;

        filterImg.enabled = true;
    }

    public ISerialDataStore GetCurrentState()
    {
        return new SaveUI(IsAlive, filterImg.enabled);
    }

    public void SetState(ISerialDataStore state)
    {
        SaveUI past = (SaveUI) state;

        IsAlive = past.IsAlive;
        filterImg.enabled = past.filterImgEnabled;
    }
}

internal class SaveUI : ISerialDataStore
{
    public bool IsAlive { get; private set; }
    public bool filterImgEnabled { get; private set; }


    public SaveUI(bool alive, bool filter)
    {
        IsAlive = alive;
        filterImgEnabled = filter;
    }
}
