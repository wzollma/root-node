using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    enum Tracks {BASE, MANY, MACHINE, BETWEEN }

    public Sound[] sounds;

    public static AudioManager instance;

    [SerializeField] Sound[] themeClips;
    AudioSource[] themes;

    [SerializeField] public float FADE_TIME = 4f;
    [SerializeField] AudioSource mainMenuTrack;
    [SerializeField] int bigThemeEnemyThreshold = 20;
    [SerializeField] float musicStartVolume = .7f;

    AudioSource curMoveSound;

    float targetVolume;
    int curTheme = -1;

    float lastTimeThemeFadeStart = -700;

    float musicStartVol;
    static bool isFading;
    bool isPlayingIndustrial;
    float lastTimeFadeIndustrial;

    // Use this for initialization
    void Awake()
    {
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

        themes = new AudioSource[themeClips.Length];
        int themeCounter = 0;

        foreach (Sound s in themeClips)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.loop = true;

            themes[themeCounter] = s.source;
            s.source.volume = 0;
            
            themes[themeCounter].Play();

            themeCounter++;
        }

        targetVolume = musicStartVolume;//themeClips[0].volume;
        musicStartVol = musicStartVolume;//themeClips[0].volume;//mainMenuTrack.volume;
        //themes[0].volume = 0;

        playTheme((int) Tracks.BETWEEN, true);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.M))
        //    mainMenuTrack.volume = mainMenuTrack.volume == 0 ? musicStartVol : 0;

        //int numEnemies = WaveManager.instance.getNumEnemies();
        float diff = WaveManager.instance.getDiff();
        Tracks trackToUse;
        if (WaveManager.instance.getWaveNum() <= 2/*numEnemies < bigThemeEnemyThreshold / 8*/)
            trackToUse = Tracks.BETWEEN;
        else if (WaveManager.instance.getNumEnemies() < bigThemeEnemyThreshold/*diff < .5f * (WaveManager.instance.waveDiff())*/)
            trackToUse = Tracks.BASE;
        else
            trackToUse = Tracks.MANY;

        bool playIndustrial = WaveManager.instance.hasMachineEnemies();//Time.time % 30 < 15;

        playTheme((int) trackToUse, false);

        fadeTheme((int)Tracks.MACHINE, playIndustrial);

        const string chainsawName = "chainsaw";

        if (WaveManager.instance.hasChainsaw())
        {
            if (!AudioManager.isPlaying(chainsawName))
                AudioManager.instance.Play(chainsawName);
        }
        else
            if (AudioManager.isPlaying(chainsawName))
                AudioManager.instance.Stop(chainsawName);
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

        if (s.source == null)
            return;

        s.source.Play();
    }

    public static void PlayNoOverlap(string name) {
        instance.Play(name);
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

    public static void startThemes()
    {
        for (int i = 0; i < instance.themes.Length; i++)
        {
            instance.themes[i].Play();
        }
    }

    public static void playTheme(int themeNum, bool force)
    {
        if (Time.time - instance.lastTimeThemeFadeStart < instance.FADE_TIME / 2)
        {
            return;
        }

        if (themeNum == instance.curTheme)
            return;

        Math.Clamp(themeNum, 0, instance.themes.Length);

        if (themeNum == instance.curTheme)
            return;

        if (instance.curTheme == -1)
            force = true;

        if (force)
        {
            float tVol = instance.targetVolume;// * (themeNum == 2 ? .9f : 1);

            for (int i = 0; i < instance.themes.Length; i++)
            {
                instance.themes[i].volume = (i == themeNum) ? tVol : 0;
            }

            instance.curTheme = themeNum;
        }
        else if (!isFading)
        {
            Debug.Log("curTheme: " + instance.curTheme);
            instance.StartCoroutine(instance.fadeTracks(instance.curTheme, themeNum));
        }
    }

    public void fadeTheme(int themeNum, bool on) {
        if (!(on != isPlayingIndustrial && Time.time - lastTimeFadeIndustrial > FADE_TIME * 2))
            return;

        lastTimeFadeIndustrial = Time.time;
        isPlayingIndustrial = on;
        StartCoroutine(fadeTrackSingle(themeNum, on));
    }

    IEnumerator fadeTrackSingle(int newTrackNum, bool on)
    {
        //Debug.Log("startFade single: " + newTrackNum);
        float startTime = Time.time;

        AudioSource newTrack = instance.themes[newTrackNum];

        newTrack.volume = on ? 0 : targetVolume;

        while (Time.time < startTime + FADE_TIME)
        {
            yield return null;

            newTrack.volume = on ? targetVolume * (Time.time - startTime) / FADE_TIME : targetVolume * (1 - ((Time.time - startTime) / FADE_TIME));
        }

        newTrack.volume = on ? targetVolume : 0;
        //Debug.Log("finishFade single: " + newTrackNum);
    }

        IEnumerator fadeTracks(int prevTrackNum, int newTrackNum)
        {
            isFading = true;

            lastTimeThemeFadeStart = Time.time;
            //Debug.Log("startFade prev: " + prevTrackNum + " new: " + newTrackNum);
            float startTime = Time.time;

            AudioSource prevTrack = instance.themes[prevTrackNum];
            AudioSource newTrack = instance.themes[newTrackNum];

            newTrack.volume = 0;

            while (Time.time < startTime + FADE_TIME)
            {
                yield return null;

                prevTrack.volume = targetVolume * (1 - ((Time.time - startTime) / FADE_TIME));
                newTrack.volume = targetVolume * (Time.time - startTime) / FADE_TIME;
            }

            prevTrack.volume = 0;
            newTrack.volume = targetVolume;
            curTheme = newTrackNum;
            isFading = false;
            //Debug.Log("finishFade prev: " + prevTrackNum + " new: " + newTrackNum);
        }

    int enumToInt(Tracks theme) {
        return (int) theme;
    }
}
