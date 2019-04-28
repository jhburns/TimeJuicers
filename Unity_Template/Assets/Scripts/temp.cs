using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serial;

public class temp : MonoBehaviour, ISerializable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ISerialDataStore GetCurrentState()
    {
        return new saveTemp(transform.position.x, transform.position.y);
    }

    public void SetState(ISerialDataStore state)
    {
        saveTemp past = (saveTemp)state;
        transform.position = new Vector2(past.positionX, past.positionY);
    }
}

internal class saveTemp: ISerialDataStore
{
    public float positionX { get; private set; }
    public float positionY { get; private set; }
    public saveTemp(float posX, float posY)
    {
        positionX = posX;
        positionY = posY;
    }
}