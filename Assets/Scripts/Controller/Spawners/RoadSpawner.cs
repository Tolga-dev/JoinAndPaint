using System;
using System.Collections.Generic;
using GameObjects.Road;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Controller.Spawners
{
    [Serializable]
    public class RoadSpawner
    {
        public GameManager gameManager;
        public GameObject road;
        public GameObject bossRoad;
    
        [Header("Positions")]
        public Vector3 offset;

        [Header("Created")]
        public List<Road> createdRoads = new List<Road>();
        public BossRoad createdBossRoad;

        public int initAmount;
        public int startAmountOfRoad;
        public int spawnAmount;
        
        public void Init(GameManager gameManagerInGame)
        {
            gameManager = gameManagerInGame;
            startAmountOfRoad = initAmount;
            
        }
        
        public void SpawnNormalRoad()
        {
            var created = Object.Instantiate(road, startAmountOfRoad * offset,road.transform.rotation);
            var createdRoad = created.GetComponent<Road>();
            createdRoad.gameManager = gameManager;
            
            createdRoads.Add(createdRoad);
            
            SetNewPos();
        }

        public void SpawnBossObject()
        {
            var created = Object.Instantiate(bossRoad, startAmountOfRoad * offset,road.transform.rotation);
            createdBossRoad = created.GetComponent<BossRoad>();
            createdBossRoad.gameManager = gameManager;

            gameManager.cameraController.SetTarget(createdBossRoad.transform, gameManager.cameraController.winCam);
            SetNewPos();
        }

        public void ResetRoads()
        {
            foreach (var createdRoad in createdRoads)
            {
                Object.Destroy(createdRoad.gameObject);
            }
            createdRoads.Clear();
            
            Object.Destroy(createdBossRoad.gameObject);
            createdBossRoad = null;
            
            startAmountOfRoad = initAmount;
        }
        
        public int GetNumberOfRoad()
        {
            var level = gameManager.gamePropertiesInSave.currenLevel;
            var numberOfRoadsToSpawn = spawnAmount;

            numberOfRoadsToSpawn += level / 10;
            
            return numberOfRoadsToSpawn;
        }
        
        public void SetNewPos()
        {
            startAmountOfRoad++;
        }
    }
}