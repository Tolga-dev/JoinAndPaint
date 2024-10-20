using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Save
{
    [CreateAssetMenu(fileName = "GamePropertiesInSave", menuName = "GamePropertiesInSave", order = 0)]
    public class GamePropertiesInSave : ScriptableObject
    {
        public bool isGameSoundOn;
        public bool isGameMusicOn;
        
        public AudioClip buttonClickSound;
        
        // in game sounds
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
        public AudioClip bossFightMusic;
            // no ads
        public bool isNoAds;
        public string noAdsProductId;
        
        // game properties
        public int totalMoney;

        public string[] winText = {
            "You did it! Great job!",
            "You’re a winner!",
            "Hooray! You won!",
            "You’re amazing!",
            "Wow, you’re so good at this!",
            "Fantastic! You made it!",
            "Super work! You won!",
            "You're a champion!",
            "Great effort! You won!",
            "You’re unstoppable!"
        };
        
        public string[] lostText = {
            "Oops, try again!",
            "It’s okay, you can do better next time!",
            "Don’t worry, you’ll win soon!",
            "Keep going, you’re getting better!",
            "Almost there! Try one more time!",
            "Ouch, do more upgrade!",
            "Not this time, but keep trying!",
            "You’re learning! Try again!",
            "It’s just practice! You’ll win next!",
            "That was close! You can do it!"
        };
        
        public int currenLevel;
        public AudioClip onGameWinSound;
        public AudioClip onGameLostSound;
        public List<int> levelRecords = new List<int>();
        
        // updates
        public int updateAmount;
        public int maxHealth = 100;
    }
}