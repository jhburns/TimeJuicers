using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serial;


public class Cloud : MonoBehaviour, ISerializable
{
    public float speed; //IM

    private float leftXBound; //IM  
    private float rightXBound; //IM

    void Update()
    {
        transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);        
        
        if (transform.position.x > rightXBound)
        {
            transform.position = new Vector3(leftXBound, transform.position.y, transform.position.z);
        }
    }

    /*
     * SetBounds - sets both left and right x coordinates for boundary
     * Params:
     *  - float left: left x-axis bound
     *  - float right: right x-axis bound
     */
    public void SetBounds(float left, float right)
    {
        leftXBound = left;
        rightXBound = right;
    }

    ////////////////////////////////////////////////////
    // Serial implementation, see Libraries/Serial.cs //
    ////////////////////////////////////////////////////
    public ISerialDataStore GetCurrentState()
    {
        return new SaveCloud(transform.position.x);
    }

    public void SetState(ISerialDataStore state)
    {
        SaveCloud past = (SaveCloud) state;

        transform.position = new Vector3(past.positionX, transform.position.y, transform.position.z);
    }
}


internal class SaveCloud : ISerialDataStore
{
    public float positionX { get; private set; }

    public SaveCloud(float posX)
    {
        positionX = posX;
    }
}
