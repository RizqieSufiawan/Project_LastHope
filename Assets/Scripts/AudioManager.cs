using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX")]
    public AudioSource sfxSource;

    [Header("Music")]
    public AudioSource musicSource;
    public float musicFadeDuration = 1f;

    private Coroutine musicFadeCoroutine;
    private AudioClip currentMusicClip;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volumeScale);
    }


    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || clip == null) return;
        if (currentMusicClip == clip && musicSource.isPlaying) return; 

        currentMusicClip = clip;
        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        musicFadeCoroutine = StartCoroutine(CrossfadeMusic(clip, loop));
    }

    public void StopMusic()
    {
        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        if (musicSource != null) musicSource.Stop();
        currentMusicClip = null;
    }

    private IEnumerator CrossfadeMusic(AudioClip clip, bool loop)
    {
        float targetVolume = musicSource.volume > 0f ? musicSource.volume : 1f;

        // fade out track lama (kalau ada yang lagi main)
        float timer = 0f;
        float startVolume = musicSource.volume;
        while (musicSource.isPlaying && timer < musicFadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / musicFadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();

        // fade in track baru
        timer = 0f;
        while (timer < musicFadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, timer / musicFadeDuration);
            yield return null;
        }
        musicSource.volume = targetVolume;
    }
}