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
        public Transform spawnPoint;
        public bool isObjSpawned;
    }
}