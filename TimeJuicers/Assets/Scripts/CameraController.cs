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

    public float maxDistanceY; //IM
    public float maxDistanceX; //IM

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
            float smoothY = Mathf.Lerp(transform.position.y, newPosition.y, 0.05f);
            float smoothX = Mathf.Lerp(transform.position.x, newPosition.x, 0.035f);
            Vector3 smoothPos = new Vector3(smoothX, smoothY, -10);

            smoothPos.y = Mathf.Clamp(smoothPos.y, newPosition.y - maxDistanceY, newPosition.y + maxDistanceY);
            smoothPos.x = Mathf.Clamp(smoothPos.x, newPosition.x - maxDistanceX, newPosition.x + maxDistanceX);

            smoothPos.y = Mathf.Clamp(smoothPos.y, minHeight, float.PositiveInfinity);
 
            transform.position = smoothPos;
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

