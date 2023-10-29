using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer main;

    void Awake()
    {
        main = this;
    }

    [SerializeField]
    public AudioSource transitionMusicSource;
    [SerializeField]
    public AudioSource gameMusicSource;

    [SerializeField]
    public List<AudioClip> musicClips;


    private List<AudioFade> fades = new List<AudioFade>();

    private float gameMusicVolume = 0.5f;
    private float fadeInDuration = 0.5f;

    private AfterFadeCallback afterFadeCallback;


    private IEnumerator InvokeRealtimeCoroutine(UnityAction action, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        if (action != null)
        {
            action();
        }
    }

    void Start()
    {
        FadeInGameMusic();
    }


    private void PlayTransitionMusic()
    {
        transitionMusicSource.Play();
    }

    public void Fade(AudioSource fadeSource, float targetVolume, float duration = 0.5f, float targetPitch = 1.0f)
    {
        AudioFade fade = new AudioFade(duration, targetVolume, fadeSource, targetPitch);
        fades.Add(fade);
    }

    /*public void LevelFadeIn(float invokeTime = 0.5f)
    {
        Invoke("FadeInGameMusic", invokeTime);
    }*/
    public void LevelFadeOut()
    {
        FadeOutGameMusic(delegate
        {
            PlayTransitionMusic();
            StartCoroutine(InvokeRealtimeCoroutine(FadeInGameMusic, 2f));
        }, 0.1f);
    }

    private void FadeOutGameMusic(AfterFadeCallback afterFadeCallback, float duration = 0.5f)
    {
        this.afterFadeCallback = afterFadeCallback;
        Fade(gameMusicSource, 0, duration);
    }

    private void FadeInGameMusic()
    {
        gameMusicSource.clip = musicClips[Random.Range(0, musicClips.Count)];
        Fade(gameMusicSource, gameMusicVolume, fadeInDuration);
    }

    public void Update()
    {
        for (int index = 0; index < fades.Count; index += 1)
        {
            AudioFade fade = fades[index];
            if (fade != null && fade.IsFading)
            {
                fade.Update();
            }
            if (!fade.IsFading)
            {
                fades.Remove(fade);
                if (afterFadeCallback != null)
                {
                    afterFadeCallback();
                    afterFadeCallback = null;
                }
            }
        }
    }

}




public delegate void AfterFadeCallback();

public class AudioFade
{
    public AudioFade(float duration, float target, AudioSource track, float targetPitch)
    {
        if (!track.isPlaying)
        {
            track.Play();
        }
        this.duration = duration;
        IsFading = true;
        timer = 0f;
        originalVolume = track.volume;
        targetVolume = target;
        audioSource = track;

        originalPitch = track.pitch;
        this.targetPitch = targetPitch;
    }
    public bool IsFading { get; private set; }
    private float duration;
    private float timer;
    private float targetVolume;
    private AudioSource audioSource;
    private float originalVolume;

    private float originalPitch, targetPitch;

    public void Update()
    {
        timer += Time.unscaledDeltaTime / duration;
        audioSource.volume = Mathf.Lerp(originalVolume, targetVolume, timer);
        audioSource.pitch = Mathf.Lerp(originalPitch, targetPitch, timer);
        if (timer >= 1)
        {
            audioSource.volume = targetVolume;
            audioSource.pitch = targetPitch;
            IsFading = false;
        }
    }
}