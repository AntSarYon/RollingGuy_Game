using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioInfo", menuName = "ScriptableObjects/AudioInfoSO", order = 1)]
public class AudioSO : ScriptableObject
{
    public string id;
    public AudioClip[] clips;
    [Range(0f, 1f)]
    public float volume;
    [Range(1, 5)]
    public int frequencyLevel = 2;
    [Range(-3, 3)]
    public float minPitch = 0.5f;
    [Range(-3, 3)]
    public float maxPitch = 3f;
    public bool stopAudioSource;
    [HideInInspector]
    public AudioSource source;
}