using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameObjects.Road
{
    public class Road : MonoBehaviour
    {
        public GameManager gameManager;
        
        [Header("Closest to far")] 
        public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    }
    [Serializable]
    public class SpawnPoint
    {
        public List<Transform> spawnPoint = new List<Transform>();
        public List<bool> isObjSpawned = new List<bool>(); // hate sorry
    }
}