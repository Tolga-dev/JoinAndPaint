using System.Collections.Generic;
using UnityEngine;

namespace Save
{
    [CreateAssetMenu(fileName = "GamePropertiesInSave", menuName = "GamePropertiesInSave", order = 0)]
    public class GamePropertiesInSave : ScriptableObject
    {
        public bool isGameSoundOn;
        public bool isGameMusicOn;
        
        public AudioClip buttonClickSound;
        
        // in game sounds
        public AudioClip playerHitSound;
        public AudioClip starSound;
        
        // sound
        public float gameSoundVolume;
        public float gameMusicChangeDuration;
        public float gameMusicStartVolume;
        
        // market
        public AudioClip onMenuStateSound;
        public AudioClip onMarketSound;
        public AudioClip purchasedSound;
        public AudioClip failedClickSound;
        public List<AudioClip> gameMusic = new List<AudioClip>();
            // no ads
        public bool isNoAds;
        public string noAdsProductId;
        
        // game properties
        public int totalMoney;
        public string[] winTexts;
        public int currenLevel;
        
        
    }
}