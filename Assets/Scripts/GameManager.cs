using Core;
using Save;
using UnityEngine;
 
public class GameManager : Singleton<GameManager>
{
    // player
    public PlayerManager playerManager;
    public MemberManager memberManager;
    // spawn ground prefab
    public GameObject groundPrefab;
    public Vector3 offset;
    public int spawnAmount;
    public Transform playerInitialPosition;
    // spawn boss ground
    public GameObject bossPrefab;
    
    [Header("Game Save")]
    public GamePropertiesInSave gamePropertiesInSave;

    public int score;

    public void Start()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            Instantiate(groundPrefab, offset * i, Quaternion.identity);
        }
        Instantiate(bossPrefab, offset * spawnAmount, Quaternion.identity);
    }

    private void Update()
    {
        playerManager.UpdatePlayer();
    }

    public void PlayASound(AudioClip audioClip)
    {
        if (gamePropertiesInSave.isGameSoundOn == false)
            return;

        var tempSoundPlayer = new GameObject("TempSoundPlayer");
        var audioSource = tempSoundPlayer.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = gamePropertiesInSave.gameSoundVolume;
        audioSource.PlayOneShot(audioClip);
        Destroy(tempSoundPlayer, audioClip.length);
    }

}
