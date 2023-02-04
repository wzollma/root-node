using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    public static AudioManager instance;

    public AudioSource[] themes;

    [SerializeField] public float FADE_TIME = 4f;
    //[SerializeField] AudioSource walkSound;
    //[SerializeField] AudioSource runSound;
    [SerializeField] AudioSource mainMenuTrack;

    AudioSource curMoveSound;

    float targetVolume;
    int curTheme = -1;

    float lastTimeThemeFadeStart = -700;

    float musicStartVol;

    // Use this for initialization
    void Awake()
    {
        musicStartVol = mainMenuTrack.volume;

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }

        //targetVolume = themes[0].volume;
        //themes[0].volume = 0;


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            mainMenuTrack.volume = mainMenuTrack.volume == 0 ? musicStartVol : 0;
    }

    public void stopMenuTrack()
    {
        mainMenuTrack.Stop();
    }

    //public void playMoveSound(Vector2 move, bool sprint)
    //{
    //    bool moving = !move.Equals(Vector2.zero);
    //    AudioSource prevMoveSound = curMoveSound;

    //    if (!moving)
    //        curMoveSound = null;
    //    else if (sprint)
    //        curMoveSound = runSound;
    //    else
    //        curMoveSound = walkSound;

    //    if (curMoveSound == null && prevMoveSound == null)
    //        return;
    //    if (curMoveSound != null && curMoveSound.Equals(prevMoveSound))
    //        return;

    //    if (prevMoveSound != null)
    //        prevMoveSound.Stop();
    //    if (curMoveSound != null)
    //        curMoveSound.Play();
    //}

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public static bool isPlaying(string soundName)
    {
        Sound s = Array.Find(instance.sounds, sound => sound.name == soundName);
        if (s == null || s.source == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return false;
        }

        return s.source.isPlaying;
    }

    public static float getCurrentPlayingTime(string soundName)
    {
        Sound s = Array.Find(instance.sounds, sound => sound.name == soundName);
        if (s == null || s.source == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return 0;
        }

        return s.source.time;
    }
    //public static void incTheme()
    //{
    //    playTheme((instance.curTheme + 1) % instance.themes.Length, false);
    //}

    //public static void startThemes()
    //{
    //    for (int i = 0; i < instance.themes.Length; i++)
    //    {
    //        instance.themes[i].Play();
    //    }
    //}

    //public static void playTheme(int themeNum, bool force)
    //{
    //    if (Time.time - instance.lastTimeThemeFadeStart < instance.FADE_TIME)
    //    {
    //        return;
    //    }

    //    Math.Clamp(themeNum, 0, instance.themes.Length);

    //    if (themeNum == instance.curTheme)
    //        return;

    //    if (instance.curTheme == -1)
    //        force = true;

    //    if (force)
    //    {
    //        float tVol = instance.targetVolume * (themeNum == 2 ? .9f : 1);

    //        for (int i = 0; i < instance.themes.Length; i++)
    //        {
    //            instance.themes[i].volume = (i == themeNum) ? tVol : 0;
    //        }

    //        instance.curTheme = themeNum;
    //    }
    //    else
    //    {
    //        instance.StartCoroutine(instance.fadeTracks(instance.curTheme, themeNum));
    //    }
    //}

    //IEnumerator fadeTracks(int prevTrackNum, int newTrackNum)
    //{
    //    lastTimeThemeFadeStart = Time.time;
    //    Debug.Log("startFade prev: " + prevTrackNum + " new: " + newTrackNum);
    //    float startTime = Time.time;

    //    AudioSource prevTrack = instance.themes[prevTrackNum];
    //    AudioSource newTrack = instance.themes[newTrackNum];

    //    newTrack.volume = 0;

    //    while (Time.time < startTime + FADE_TIME)
    //    {
    //        yield return null;

    //        prevTrack.volume = targetVolume * (1 - ((Time.time - startTime) / FADE_TIME));
    //        newTrack.volume = targetVolume * (Time.time - startTime) / FADE_TIME;
    //    }

    //    prevTrack.volume = 0;
    //    newTrack.volume = targetVolume;
    //    curTheme = newTrackNum;
    //    Debug.Log("finishFade prev: " + prevTrackNum + " new: " + newTrackNum);
    //}
}
