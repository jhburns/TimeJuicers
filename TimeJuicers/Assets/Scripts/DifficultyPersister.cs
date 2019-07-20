using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Purpose is to store selected difficulty between level scenes
 */

public class DifficultyPersister : MonoBehaviour
{
    public int MaxFrames;
    public int FramePenalty;
    public string modeName; // Should be "normal", "hard", or "free" values only
}
