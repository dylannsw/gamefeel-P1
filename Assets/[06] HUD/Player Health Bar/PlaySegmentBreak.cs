using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySegmentBreak : MonoBehaviour
{
    [Header("References")]
    public UnityEngine.UI.Image fillBar;
    public Animator[] segmentAnimators;
    public int totalSegments = 10;

    private int lastActiveSegment;
    
    void Start()
    {
    lastActiveSegment = totalSegments; // Start full
    }

}
