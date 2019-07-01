using UnityEngine;
using System.Collections;

// TODO: redo this

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

