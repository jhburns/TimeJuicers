using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WinFlag : MonoBehaviour
{
    public SceneController scene;

    /*
     * OnCollisionEnter2D - allows the player to go to the next level
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "player")
        {
            scene.NextLevel();
        }
    }
}
