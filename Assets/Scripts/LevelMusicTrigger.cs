using UnityEngine;

public class LevelMusicTrigger : MonoBehaviour
{
    public AudioClip levelMusic;

    private void Start()
    {
        AudioManager.Instance?.PlayMusic(levelMusic);
    }
}