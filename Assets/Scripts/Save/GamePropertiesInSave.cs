using UnityEngine;

namespace Save
{
    [CreateAssetMenu(fileName = "GamePropertiesInSave", menuName = "GamePropertiesInSave", order = 0)]
    public class GamePropertiesInSave : ScriptableObject
    {
        public bool isGameSoundOn;
        public bool isGameMusicOn;
        
        public AudioClip playerHitSound;
        public AudioClip buttonClickSound;
        
        public float gameSoundVolume;
        public float gameMusicChangeDuration;
        public float gameMusicStartVolume;
        
        
        public AudioClip onMenuStateSound;
    }
}