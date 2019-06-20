using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Full Name: Jonathan Burns
 * Student ID: 2288851
 * Chapman email: jburns@chapman.edu/
 * Course number and section: 236-02
 * Assignment Number: 5
 */

/*
 * Purpose:
 *  - CloudController: just a data storage object between scenes
 */

public class DifficultyPersister : MonoBehaviour
{
    public int MaxFrames;
    public int FramePenalty;
    public string modeName; // Should be 'normal', 'hard', or 'free' only
}
