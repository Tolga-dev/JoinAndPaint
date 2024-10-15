using System;
using System.Collections.Generic;
using Cinemachine;
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
        public Transform spawnPoint;

        [Header("Created")]
        public List<Road> createdRoads = new List<Road>();
        public BossRoad createdBossRoad;
        
        public int startAmountOfRoad = 1;
        public int spawnAmount = 0;

        public void Init(GameManager gameManagerInGame)
        {
            gameManager = gameManagerInGame;
        }
        
        public void SpawnNormalRoad()
        {
            var created = Object.Instantiate(road, spawnAmount * offset,road.transform.rotation);
            createdRoads.Add(created.GetComponent<Road>());
            SetNewPos();
        }

        public void SpawnBossObject()
        {
            var created = Object.Instantiate(bossRoad, spawnPoint.position, bossRoad.transform.rotation);
            createdBossRoad = created.GetComponent<BossRoad>();
            
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
            
            spawnAmount = 0;
        }
        
        public int GetNumberOfRoad()
        {
            var level = gameManager.gamePropertiesInSave.currenLevel;
            var numberOfRoadsToSpawn = startAmountOfRoad;

            numberOfRoadsToSpawn += level / 10;
            
            return numberOfRoadsToSpawn;
        }
        
        public void SetNewPos()
        {
            spawnAmount++;
        }
    }
    
    
    /*// spawn ground prefab
    public GameObject groundPrefab;
    public Vector3 offset;
    public int spawnAmount;
    public Transform playerInitialPosition;
    // spawn boss ground
    public GameObject bossPrefab;

    public List<GameObject> grounds = new List<GameObject>();

    public void SpawnPlayObjects()
    {
         CleanGround();

         SpawnGrounds();

    }
    private void SpawnGrounds()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            grounds.Add(Instantiate(groundPrefab, offset * i, Quaternion.identity));
        }
        grounds.Add(Instantiate(bossPrefab, offset * spawnAmount, Quaternion.identity));
    }

    public void CleanGround()
    {
        foreach (var ground in grounds)
        {
            Destroy(ground);
        }
        grounds.Clear();
    }*/
    
}