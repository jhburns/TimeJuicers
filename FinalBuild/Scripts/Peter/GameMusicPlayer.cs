using UnityEngine;
using System.Collections;

/* 
 * Full Name: Peter Chen
 * Student ID: 2326305
 * Chapman email: haichen@chapman.edu
 * Course number and section: 236-02
 * Assignment Number: 5
 */

/*
 * Purpose:
 *  - GameMusicPlayer: allow background music to keep playing acrossing scenes
 */


//https://answers.unity.com/questions/1260393/make-music-continue-playing-through-scenes.html
public class GameMusicPlayer : MonoBehaviour
{
    private static GameMusicPlayer instance;

    public static GameMusicPlayer Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}

