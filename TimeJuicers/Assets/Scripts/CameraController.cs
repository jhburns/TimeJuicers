using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serial;


// https://www.salusgames.com/2016/12/28/smooth-2d-camera-follow-in-unity3d/
public class CameraController : MonoBehaviour, ISerializable
{
    public Transform target; //IM

    public float minHeight; //IM

    private bool isExactMode; //NOT serialized, only needed for cinematic 

    private void Start()
    {
        transform.position = target.position; // Only player needs to be moved, camera follows on start

        isExactMode = false;
    }

    private void Update()
    {
        Vector3 newPosition = target.position;
        newPosition.z = -10;

        if (isExactMode)
        {
            transform.position = newPosition;
        }
        else
        {
            newPosition.y = Mathf.Clamp(newPosition.y, minHeight, float.PositiveInfinity);
            transform.position = Vector3.Slerp(transform.position, newPosition, 0.05f);
        }
    }

    /*
     * ExactMode - sets the camera to follow the player exactly to their x, y
     * Called by end of level cinematic to prevent player leaving camera behind
     */
    public void ExactMode()
    {
        isExactMode = true;
    }

    /// Serial Methods, see Serial Namespace 
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

