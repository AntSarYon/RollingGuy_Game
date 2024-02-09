using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] music;
    public Sound[] sfx;
    public static AudioManager instance;
    public static float music_vol = .5f;
    public static float sfx_vol = .5f;
    Sound actual_music;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        foreach (Sound s in music)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioSO.clips[0];
            s.source.volume = s.audioSO.volume;
            s.source.pitch = s.audioSO.maxPitch;
            s.source.loop = s.audioSO.stopAudioSource;
        }
        foreach (Sound s in sfx)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioSO.clips[0];
            s.source.volume = s.audioSO.volume;
            s.source.pitch = s.audioSO.maxPitch;
            s.source.loop = s.audioSO.stopAudioSource;
        }
    }
    private void Start()
    {
        //PlayMusic("MenuMusic");
    }

    public void PlaySfx(string name)
    {
        Sound s = Array.Find(sfx, sound => sound.audioSO.id == name);
        if (s == null)
        {
            Debug.LogError("No se encontr? el audio!");
            return;
        }
        s.source.Play();
    }
    public void PlayMusic(string name)
    {
        actual_music = Array.Find(music, bgmSounds => bgmSounds.audioSO.id == name);
        if (actual_music == null)
        {
            Debug.LogError("No se encontr? el audio!");
            return;
        }
        actual_music.source.Play();
    }
    public void updateMusic(string newTheme)
    {
        if (actual_music.audioSO.id != newTheme)
        {
            actual_music.source.Stop();
            PlayMusic(newTheme);
            updateMusicVolume(music_vol);
        }
    }
    public void updateMusicVolume(float volume)
    {
        if (actual_music != null)
        {
            music_vol = volume;
            actual_music.source.volume = volume;
        }
    }
    public void updateSfxVolume(float volume)
    {
        sfx_vol = volume;
        foreach (Sound s in sfx)
        {
            s.source.volume = volume;
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            updateMusic("BackgroundMusic");
        }
    }
}