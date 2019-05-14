using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource auds;
    AudioClip re;

    // Start is called bore the first frame update
    /*
     * Start()
     *      When the program start, get the audio source.   
     */
    void Start()
    {
        auds = GetComponent<AudioSource>();
        re = GetComponent<AudioClip>();
    }


    /*
     * PlaySound(AudioClip file)
     * Parameter:
     *      AudioClip file: so that we can put different audioclip to different buttons
     */
    public void PlaySound()
    {
        if (auds != null)
        {
            auds.PlayOneShot(re);
        }
    }
}
