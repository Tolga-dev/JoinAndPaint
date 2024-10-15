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
        
        // sound
        public float gameSoundVolume;
        public float gameMusicChangeDuration;
        public float gameMusicStartVolume;
        
        // market
        public AudioClip onMenuStateSound;
        public AudioClip onMarketSound;
        public AudioClip purchasedSound;
        public AudioClip failedClickSound;
            // no ads
        public bool isNoAds;
        public string noAdsProductId;
        
        // game properties
        public int totalMoney;
    }
}