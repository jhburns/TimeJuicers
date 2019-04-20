using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serial;

// https://www.salusgames.com/2016/12/28/smooth-2d-camera-follow-in-unity3d/
public class CameraController : MonoBehaviour, ISerializable
{
    public Transform Target; //IM

    public float minHeight; //IM

    private void Update()
    {
        Vector3 newPosition = Target.position;
        newPosition.z = -10;
        newPosition.y = Mathf.Clamp(newPosition.y, minHeight , float.PositiveInfinity);
        transform.position = Vector3.Slerp(transform.position, newPosition, 0.05f);
    }


    public ISerialDataStore GetCurrentState()
    {
        return new SaveCamera(transform.position.x, transform.position.y);
    }

    public void SetState(ISerialDataStore state)
    {
        SaveCamera past = (SaveCamera) state;

        transform.position = new Vector3(past.positionX, past.positionY, -10);
    }

}

internal class SaveCamera : ISerialDataStore
{
    public float positionX  { get; private set; }
    public float positionY { get; private set; }

    public SaveCamera(float posX, float posY)
    {
        positionX = posX;
        positionY = posY;
    }
}

