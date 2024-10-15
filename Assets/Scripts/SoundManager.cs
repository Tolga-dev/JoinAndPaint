using System.Collections;
using Save;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource gameMusic;
    
    public GameManager gameManager;
    public GamePropertiesInSave gamePropertiesInSave;
    
    public void PlayASound(AudioClip audioClip)
    {
        if (gameManager.gamePropertiesInSave.isGameSoundOn == false)
            return;

        var tempSoundPlayer = new GameObject("TempSoundPlayer");
        var audioSource = tempSoundPlayer.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = gameManager.gamePropertiesInSave.gameSoundVolume;
        audioSource.PlayOneShot(audioClip);
        Destroy(tempSoundPlayer, audioClip.length);
    }

    public void GameMusic(AudioClip audioClip)
    {
        if (gamePropertiesInSave.isGameSoundOn)
        {
            StartCoroutine(FadeOutMusic(audioClip));
        }
        else
        {
            gameMusic.volume = 0;
        }
    }

    private IEnumerator FadeOutMusic(AudioClip audioClip)
    {
        float duration = gamePropertiesInSave.gameMusicChangeDuration; // Time in seconds to fade out
        float startVolume = gamePropertiesInSave.gameMusicStartVolume;

        while (gameMusic.volume > 0)
        {
            gameMusic.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        StartCoroutine(FadeInMusic(audioClip));
        gameMusic.volume = 0;
    }

    private IEnumerator FadeInMusic(AudioClip audioClip)
    {
        gameMusic.clip = audioClip;
        gameMusic.Play();

        float duration = gamePropertiesInSave.gameMusicChangeDuration; // Time in seconds to fade in
        gameMusic.volume = 0;

        while (gameMusic.volume < gamePropertiesInSave.gameMusicStartVolume)
        {
            gameMusic.volume += Time.deltaTime / duration;
            yield return null;
        }

        gameMusic.volume = gamePropertiesInSave.gameMusicStartVolume;
    }

    public void ButtonClickSound()
    {
        PlayASound(gamePropertiesInSave.buttonClickSound);
    }
    
}