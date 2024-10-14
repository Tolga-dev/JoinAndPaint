using UnityEngine;

namespace Save
{
    [CreateAssetMenu(fileName = "GamePropertiesInSave", menuName = "GamePropertiesInSave", order = 0)]
    public class GamePropertiesInSave : ScriptableObject
    {
        public bool isGameSoundOn;
        
        public float gameSoundVolume;
        public AudioClip playerHitSound;
    }
}